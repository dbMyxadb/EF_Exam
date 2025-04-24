using EF_Exam.DAL.Entities;
using EF_Exam;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public void AddUser(string name, bool isAdmin, decimal? personalDiscount)
    {
        if(personalDiscount < 0 || personalDiscount > 100 || personalDiscount == null)
        { personalDiscount = 0; }
        var user = new User
        {
            Name = name,
            IsAdmin = isAdmin,
            PersonalDiscount = personalDiscount 
        };

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public User GetUserById(int id)
    {
        return _context.Users.Find(id);
    }

    public List<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }
}