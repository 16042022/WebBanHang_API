using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Enums;

namespace WebBanHang.Domain.Model.Product
{
    public class UpdateProductRequest
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public float Price { get; set; }
        public float Discount { get; set; } = 0;
        public int Stock { get; set; }
        public string Status => (Stock > 0) ? "Available" : "Out_of_stock";
    }
}
