using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int OrderID { get; set; }
        public Order Order { get; set; } = null!;
        public int ProductID { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int Quantities { get; set; }
        public float ProductPrice { get; set; }
    }
}
