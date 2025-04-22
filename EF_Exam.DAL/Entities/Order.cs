using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_Exam.DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }

}
