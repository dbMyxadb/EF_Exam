using EF_Exam;

public class MenuService
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;
    private readonly ProductService _productService;
    private readonly OrderService _orderService;

    public MenuService(AppDbContext context)
    {
        _context = context;
        _userService = new UserService(context);
        _productService = new ProductService(context);
        _orderService = new OrderService(context);
    }

    public MenuService()
    {
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== MAIN MENU ===");
            Console.WriteLine("1. Add new user");
            Console.WriteLine("2. List all users");
            Console.WriteLine("3. Add new product");
            Console.WriteLine("4. List all products");
            Console.WriteLine("5. Create order");
            Console.WriteLine("6. Delete order");
            Console.WriteLine("7. Check if user ordered a product");
            Console.WriteLine("8. Order history for user");
            Console.WriteLine("9. List products by user");
            Console.WriteLine("10. List users by product");
            Console.WriteLine("11. List all orders detailed");
            Console.WriteLine("12. Users with >3 products");
            Console.WriteLine("13. Order count per product");
            Console.WriteLine("14. Top 5 popular products");
            Console.WriteLine("15. Top 3 customers by spend");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1": AddUser(); break;
                    case "2": ListUsers(); break;
                    case "3": AddProduct(); break;
                    case "4": ListProducts(); break;
                    case "5": CreateOrder(); break;
                    case "6": DeleteOrder(); break;
                    case "7": CheckUserOrderedProduct(); break;
                    case "8": ShowOrderHistory(); break;
                    case "9": ListProductsByUser(); break;
                    case "10": ListUsersByProduct(); break;
                    case "11": ListAllOrdersDetailed(); break;
                    case "12": ShowUsersWithMoreThanThreeProducts(); break;
                    case "13": ShowOrderCountPerProduct(); break;
                    case "14": ShowTop5Products(); break;
                    case "15": ShowTop3Customers(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Invalid selection.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void AddUser()
    {
        Console.Write("Enter user name: ");
        var name = Console.ReadLine();
        Console.Write("Is admin? (y/n): ");
        var isAdmin = Console.ReadLine().ToLower() == "y";
        Console.Write("Enter personal discount (%) or blank: ");
        decimal discInput = Console.Read();
        _userService.AddUser(name, isAdmin,discInput);
        Console.WriteLine("User added.");
    }

    private void ListUsers()
    {
        var users = _userService.GetAllUsers();
        Console.WriteLine("Users:");
        foreach (var u in users)
            Console.WriteLine($"  {u.Id}: {u.Name} (Admin: {u.IsAdmin})");
    }

    private void AddProduct()
    {
        Console.Write("Enter product name: ");
        var name = Console.ReadLine();
        Console.Write("Enter price: ");
        var price = decimal.Parse(Console.ReadLine());
        Console.Write("Enter stock quantity: ");
        var stock = int.Parse(Console.ReadLine());
        Console.Write("Enter discount (%) or blank: ");
        var discInput = Console.ReadLine();
        decimal? discount = string.IsNullOrWhiteSpace(discInput)
            ? (decimal?)null
            : decimal.Parse(discInput);
        _productService.AddProduct(name, price, stock, discount);
        Console.WriteLine("Product added.");
    }

    private void ListProducts()
    {
        var products = _productService.GetAllProducts();
        Console.WriteLine("Products:");
        foreach (var p in products)
            Console.WriteLine($"  {p.Id}: {p.Name} - Price: {p.Price}, Stock: {p.Stock}, Discount: {p.Discount}");
    }

    private void CreateOrder()
    {
        Console.Write("Enter user ID: ");
        var userId = int.Parse(Console.ReadLine());
        var user = _userService.GetUserById(userId)
            ?? throw new Exception("User not found.");

        var items = new Dictionary<int, int>();
        while (true)
        {
            Console.Write("Enter product ID (0 to finish): ");
            var pid = int.Parse(Console.ReadLine());
            if (pid == 0) break;

            var product = _productService.GetProductById(pid)
                ?? throw new Exception("Product not found.");

            Console.Write("Enter quantity: ");
            var qty = int.Parse(Console.ReadLine());
            if (qty <= 0) throw new Exception("Quantity must be positive.");
            if (product.Stock < qty) throw new Exception("Not enough stock.");

            if (items.ContainsKey(pid))
                throw new Exception("Duplicate product in order.");

            items[pid] = qty;
        }

        _orderService.CreateOrder(userId, items);
        Console.WriteLine("Order created.");
    }

    private void DeleteOrder()
    {
        Console.Write("Enter order ID to delete: ");
        var orderId = int.Parse(Console.ReadLine());
        _orderService.DeleteOrder(orderId);
        Console.WriteLine("Order deleted.");
    }

    private void CheckUserOrderedProduct()
    {
        Console.Write("Enter user ID: ");
        var userId = int.Parse(Console.ReadLine());
        Console.Write("Enter product ID: ");
        var prodId = int.Parse(Console.ReadLine());

        var history = _orderService.GetOrderHistory(userId);
        var ordered = history
            .SelectMany(o => o.OrderProducts)
            .Any(op => op.ProductId == prodId);

        Console.WriteLine(ordered
            ? "User has ordered this product."
            : "User has not ordered this product.");
    }

    private void ShowOrderHistory()
    {
        Console.Write("Enter user ID: ");
        var userId = int.Parse(Console.ReadLine());
        var history = _orderService.GetOrderHistory(userId);
        Console.WriteLine($"Order history for user {userId}:");
        foreach (var o in history)
        {
            Console.WriteLine($"  Order {o.Id} on {o.DateCreated}:");
            foreach (var op in o.OrderProducts)
                Console.WriteLine($"    - {op.Product.Name} x{op.Quantity} @ {op.EndPrice}");
        }
    }

    private void ListProductsByUser()
    {
        Console.Write("Enter user ID: ");
        var userId = int.Parse(Console.ReadLine());
        Console.WriteLine($"Products ordered by user {userId}:");
        var products = _orderService.GetProductsByUser(userId);

    }

    private void ListUsersByProduct()
    {
        Console.Write("Enter product ID: ");
        var prodId = int.Parse(Console.ReadLine());
                Console.WriteLine($"Users who ordered product {prodId}:");

         _orderService.GetUsersByProduct(prodId);

    }

    private void ListAllOrdersDetailed()
    {
        Console.WriteLine("All orders detailed:");
        _orderService.GetAllOrdersDetailed();
    }

    private void ShowUsersWithMoreThanThreeProducts()
    {
        Console.WriteLine("Users with more than 3 products ordered:");
        _orderService.GetUsersWithMoreThanThreeProducts();

    }

    private void ShowOrderCountPerProduct()
    {
        Console.WriteLine("Order count per product:");
        _orderService.GetOrderCountPerProduct();

    }

    private void ShowTop5Products()
    {
        var top5 = _orderService.GetTop5Products();
        Console.WriteLine("Top 5 popular products:");
        foreach (var p in top5)
            Console.WriteLine($"  - {p.Name}");
    }

    private void ShowTop3Customers()
    {
        var top3 = _orderService.GetTop3Customers();
        Console.WriteLine("Top 3 customers by total spend:");
        foreach (var c in top3)
            Console.WriteLine($"  - {c.user.Name}: {c.totalSpent}");
    }
}