using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities.OnlinePayment
{
    public class PaymentTransaction
    {
        public string ID { get; set; } = string.Empty;
        public string? TranMessage { get; set; } = string.Empty;
        public string? TranPayload { get; set; } = string.Empty;
        public string? TranStatus { get; set; } = string.Empty;
        public decimal? TranAmount { get; set; }
        public DateTime? TranDate { get; set; }
        public string? OnlinePaymentId { get; set; } = string.Empty;
        public string? TranRefId { get; set; } = string.Empty;
    }
}
