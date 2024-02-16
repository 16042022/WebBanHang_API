using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Common
{
    public class JsonConfig
    {
        private readonly string SecurityKey = Environment.GetEnvironmentVariable("SecretKey")!;
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public string SecretKey
        {
            get {  return SecurityKey; }
        }
        public int RefreshTokenTTL { get; set; }
    }
}
