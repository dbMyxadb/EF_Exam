using EF_Exam.DAL.Entities;

using EF_Exam;
public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public void AddProduct(string name, decimal price, int stock, decimal? discount)
    {
        var product = new Product
        {
            Name = name,
            Price = price,
            Stock = stock,
            Discount = discount
        };

        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public Product GetProductById(int id)
    {
        return _context.Products.Find(id);
    }

    public List<Product> GetAllProducts()
    {
        return _context.Products.ToList();
    }
}
