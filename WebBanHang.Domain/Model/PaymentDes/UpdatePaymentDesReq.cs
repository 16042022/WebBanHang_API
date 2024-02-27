using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Model.PaymentDes
{
    public class UpdatePaymentDesReq
    {
        public string Id { get; set; } = string.Empty;
        public string? DesName { get; set; } = string.Empty;
        public string? DesShortName { get; set; } = string.Empty;
        public string? DesParentId { get; set; } = string.Empty;
        public string? DesLogo { get; set; } = string.Empty;
        public int SortIndex { get; set; }
    }
}
