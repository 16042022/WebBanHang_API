using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Entities.OnlinePayment
{
    public class PaymentSignature
    {
        public string ID { get; set; } = string.Empty;
        public string? PaymentId { get; set; } = string.Empty;
        public string? SignValue { get; set; } = string.Empty;
        public string? SignAlgo { get; set; } = string.Empty;
        public string? SignOwn { get; set; } = string.Empty;
        public DateTime? SignDate { get; set; }
        public bool IsValid { get; set; }
    }
}
