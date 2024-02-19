using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Enums;

namespace WebBanHang.Domain.Model.Account
{
    public class CreateRequest
    {
        [Required]
        public string UserName { get; set; } = "";

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        [Required]
        [EnumDataType(typeof(UserRole))] // Map from enum to equivalent name
        public string Role { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";
        [Required]
        [Phone]
        public string PhoneNo { get; set; } = "";
    }
}
