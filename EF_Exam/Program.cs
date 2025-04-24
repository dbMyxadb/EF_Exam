
internal class Program
{
    static void Main(string[] args)
    {
        //var context = new AppDbContext();
        MenuService menuService = new MenuService();
        menuService.Run();
        Console.WriteLine("Hello, World!");
    }
}
