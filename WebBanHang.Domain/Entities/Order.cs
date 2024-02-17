using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int UserID { get; set; }
        public Users User { get; set; } = null!;
        public int PaymentID { get; set; }
        public Payment Payment { get; set; } = null!;
        public int StatusID { get; set; }
        public OrderStatus Status { get; set; } = null!;
        public float TotalPrice { get; set; }
        [JsonIgnore]
        public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
    }
}
