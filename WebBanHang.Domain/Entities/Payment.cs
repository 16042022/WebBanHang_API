using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string PaymentMethod { get; set; } = "";
        public Order? Order { get; set; }
    }
}
