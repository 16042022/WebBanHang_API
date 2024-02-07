using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int CustomerID { get; set; }
        public int PaymentID { get; set; }
        public int StatusID { get; set; }
        public float TotalPrice { get; set; }
    }
}
