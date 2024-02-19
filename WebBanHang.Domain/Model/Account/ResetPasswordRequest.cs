using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Model.Account
{
    public class ResetPasswordRequest
    {
        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = ""; // New password
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";
        [Required]
        public string Token { get; set; } = "";
    }
}
