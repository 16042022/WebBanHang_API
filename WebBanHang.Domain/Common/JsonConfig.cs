using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Common
{
    public class JsonConfig
    {
        private string SecurityKey = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public string SecretKey
        {
            get {  return SecurityKey; }
            private set { SecurityKey = Environment.GetEnvironmentVariable("SecretKey")!; }
        }
    }
}
