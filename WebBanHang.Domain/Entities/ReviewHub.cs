using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class ReviewHub : BaseEntity
    {
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        public int ReviewDetailId { get; set; }
    }
}
