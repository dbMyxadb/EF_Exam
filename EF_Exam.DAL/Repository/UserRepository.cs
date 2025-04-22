using EF_Exam.DAL;
using EF_Exam.DAL.Entities;

namespace EF_Exam.DAL.Repository
{
    public class UserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public  Task<User> GetUserById(int id)
        {
            return  _context.Users.Find(id);
        }
        public  Task<IEnumerable<User>> GetAllUsers()
        {
            return  _context.Users.ToList();
        }
        public  Task AddUser(User user)
        {
             _context.Users.Add(user);
             _context.SaveChanges();
        }
        public  Task UpdateUser(User user)
        {
            _context.Users.Update(user);
             _context.SaveChanges();
        }
        public  Task DeleteUser(int id)
        {
            var user =  GetUserById(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                 _context.SaveChanges();
            }
        }
    }
}
