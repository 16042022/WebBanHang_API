using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Model.PaymentDes
{
    public class SetActiveReq
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
