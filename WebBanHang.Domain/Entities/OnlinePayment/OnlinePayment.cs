using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Entities.OnlinePayment
{
    public class OnlinePayment
    {
        public string ID { get; set; } = string.Empty;
        public string PaymentContent { get; set; } = string.Empty;
        public string PaymentCurrency { get; set; } = string.Empty;
        public string PaymentRefId { get; set; } = string.Empty;
        public int? RequiredAmount { get; set; }
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        public DateTime? ExpireDate { get; set; }
        public string? PaymentLanguage { get; set; } = string.Empty;
        public string? MerchantId { get; set; } = string.Empty;
        public string? PaymentDestinationId { get; set; } = string.Empty;
        public int? PaidAmount { get; set; } // Update from TranAmount
        public string? PaymentStatus { get; set; } = string.Empty;
        public string? PaymentLastMessage { get; set; } = string.Empty;
    }
}
