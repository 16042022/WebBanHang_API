using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class StoredToken : BaseEntity
    {
        public int UserID { get; set; }
        public string? RefreshToken { get; set; } = "";
        public string? JwtID { get; set; } = "";
        public DateTime? ExpiresTime { get; set; }
        public DateTime? IssuedAt { get; set; }
        public bool? IsRevoked { get; set; } = false;
        public bool? IsUsed { get; set; } = false;
        public string? ResetPwdToken { get; set; } = "";
        public DateTime? ResetPwdExpires {  get; set; }
    }
}
