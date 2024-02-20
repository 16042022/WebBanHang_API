using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Enums;

namespace WebBanHang.Domain.Model.Cart
{
    public class AddingProductRequest
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public float Price { get; set; }
        public float Discount { get; set; } = 0;
        public int Stock { get; set; }
        [EnumDataType(typeof(ProductStatus))]
        public string Status { get; set; } = "";
        [EnumDataType(typeof(ProductCategory))]
        public string Category { get; set; } = "";
    }
}
