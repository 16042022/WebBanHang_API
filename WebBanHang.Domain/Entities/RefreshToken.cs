using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    [Owned]
    public class RefreshToken
    {
        [Key]
        [JsonIgnore]
        public int ID { get; set; }
        public string Token { get; set; } = "";
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public string CreatedByIp { get; set; } = "";
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; } = "";
        public string ReplacedByToken { get; set; } = "";
        public string ReasonRevoked { get; set; } = "";
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
    }
}
