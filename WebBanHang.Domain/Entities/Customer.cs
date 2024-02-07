using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class Customer : BaseEntity
    {
        [Required]
        [MaxLength(10)]
        public string FirstName { get; set; } = "";
        [Required]
        [DataType(DataType.Text)]
        public string LastName { get; set; } = "";
        [Required]
        [DataType (DataType.EmailAddress)]
        public string Email { get; set; } = "";
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNo { get; set; } = "";
        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
