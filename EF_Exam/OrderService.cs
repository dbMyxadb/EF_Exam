using EF_Exam.DAL.Entities;
using EF_Exam;
using Microsoft.EntityFrameworkCore;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public void CreateOrder(int userId, Dictionary<int, int> items)
    {
        var user = _context.Users.Find(userId);
        if (user == null)
            throw new Exception("User not found");

        // prevent duplicate products
        if (items.Keys.Count != items.Keys.Distinct().Count())
            throw new Exception("Duplicate products in order");

        var order = new Order
        {
            UserId = userId,
            DateCreated = DateTime.UtcNow
        };

        foreach (var kv in items)
        {
            int productId = kv.Key;
            int quantity = kv.Value;

            var product = _context.Products.Find(productId);
            if (product == null)
                throw new Exception("Product not found");

            if (product.Stock < quantity)
                throw new Exception("Not enough stock");

            decimal unitPrice = product.Price;
            if (product.Discount.HasValue)
                unitPrice *= (1 - product.Discount.Value / 100m);
            if (user.PersonalDiscount.HasValue)
                unitPrice *= (1 - user.PersonalDiscount.Value / 100m);

            product.Stock -= quantity;
            _context.SaveChanges();

            order.OrderProducts.Add(new OrderProduct
            {
                ProductId = productId,
                Quantity = quantity,
                EndPrice = unitPrice
            });
        }

        // calculate total price (if you add Order.TotalPrice property)
        // decimal total = order.OrderProducts.Sum(op => op.Quantity * op.EndPrice);

        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    public void DeleteOrder(int orderId)
    {
        var order = _context.Orders.Find(orderId);
        if (order == null)
            throw new Exception("Order not found");

        _context.Orders.Remove(order);
        _context.SaveChanges();
    }

    public List<Order> GetOrderHistory(int userId)
    {
        return _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .ToList();
    }

    public List<Product> GetTop5Products()
    {
        var orders = _context.Orders
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .ToList();

        return orders
            .SelectMany(o => o.OrderProducts)
            .GroupBy(op => op.Product)
            .OrderByDescending(g => g.Sum(op => op.Quantity))
            .Take(5)
            .Select(g => g.Key)
            .ToList();
    }

    public List<(User user, decimal totalSpent)> GetTop3Customers()
    {
        var orders = _context.Orders
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .Include(o => o.User)
            .ToList();

        return orders
            .GroupBy(o => o.User)
            .Select(g => (
                user: g.Key,
                totalSpent: g.Sum(o => o.OrderProducts.Sum(op => op.Quantity * op.EndPrice))
            ))
            .OrderByDescending(x => x.totalSpent)
            .Take(3)
            .ToList();
    }
    public List<Order> GetOrdersByUserId(int userId)
    {
        return _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .ToList();
    }
    public List<Order> GetOrdersByProductId(int productId)
    {
        return _context.Orders
            .Where(o => o.OrderProducts.Any(op => op.ProductId == productId))
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .ToList();
    }
    public List<Order> GetProductsByUser(int userId)
    {
        return _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .ToList();
    }
    public void GetUsersWithMoreThanThreeProducts()
    {
        var users = _context.Users
            .Include(u => u.Orders)
                .ThenInclude(o => o.OrderProducts)
            .Where(u => u.Orders
                .SelectMany(o => o.OrderProducts)
                .Count() > 3)
            .ToList();
        foreach (var user in users)
        {
            Console.WriteLine($"User {user.Name} has ordered more than 3 products.");
        }
    }
    public void GetOrderCountPerProduct()
    {
        var productCounts = _context.Products
            .Select(p => new
            {
                Product = p,
                Count = p.OrderProducts.Count()
            })
            .ToList();
        foreach (var pc in productCounts)
        {
            Console.WriteLine($"Product {pc.Product.Name} has been ordered {pc.Count} times.");
        }
    }
    public void GetAllOrdersDetailed()
    {
        var details = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .Select(o => new
            {
                o.DateCreated,
                UserName = o.User.Name,
                ProductName = o.OrderProducts.Select(op => op.Product.Name).ToList()
            })
            .ToList();
        foreach (var detail in details)
        {
            Console.WriteLine($"Order on {detail.DateCreated}: {detail.UserName} bought {string.Join(", ", detail.ProductName)}");
        }
    }
    public void GetUsersByProduct(int productId)
    {
        var users = _context.Users
            .Include(u => u.Orders)
                .ThenInclude(o => o.OrderProducts)
            .Where(u => u.Orders
                .Any(o => o.OrderProducts
                    .Any(op => op.ProductId == productId)))
            .ToList();
        foreach (var user in users)
        {
            Console.WriteLine($"User {user.Name} has ordered product {productId}.");
        }
    }
}