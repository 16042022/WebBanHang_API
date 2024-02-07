using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Common
{
    public class JWTTokenIdentity
    {
        public string JWTAccessToken { get; set; } = "";
        public string JWTRefreshToken { get; set; } = "";
    }
}
