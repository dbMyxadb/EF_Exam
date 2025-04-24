using EF_Exam.DAL.Entities;

namespace EF_Exam.DAL
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAll()
        {
            return _context.Orders;
        }

        public void AddOrder(Order Order)
        {
            _context.Orders.Add(Order);
            _context.SaveChanges();
        }
        public Order GetOrderById(int id)
        {
            return _context.Orders.Find(id);
        }
        public List<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }
        public void UpdateOrder(Order Order)
        {
            _context.Orders.Update(Order);
            _context.SaveChanges();
        }
        public void DeleteOrder(int id)
        {
            var Order = _context.Orders.Find(id);
            if (Order != null)
            {
                _context.Orders.Remove(Order);
                _context.SaveChanges();
            }
        }

    }
}
