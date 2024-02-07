using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [CustomAnotation("Insertable")]
        public string UserName { get; set; } = "";
        public string? Avatar { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "";
        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
        [Required]
        public string Status { get; set; } = "";
        [Required]
        public int RoleID { get; set; }
    }
}
