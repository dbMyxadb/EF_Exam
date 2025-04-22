using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_Exam.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsAdmin { get; set; } = false;
        public decimal? PersonalDiscount { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
