using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Domain.Common
{
    public class AuthenicationRespone
    {
        public string JWTAccessToken { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        [JsonIgnore]
        public string? JWTRefreshToken { get; set; }
    }
}
