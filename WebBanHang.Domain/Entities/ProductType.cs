using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class ProductType : BaseEntity
    {
        [Required]
        [MaxLength(45)]
        public string TypeName { get; set; } = "";
    }
}
