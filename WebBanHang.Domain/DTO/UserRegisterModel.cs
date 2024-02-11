using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebBanHang.Domain.DTO
{
    public class UserRegisterModel
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; } = "";
        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "";
    }
}
