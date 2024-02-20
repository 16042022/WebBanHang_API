using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Model.Cart
{
    public class ProductDtos
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public float Price { get; set; }
        public float Discount { get; set; } = 0;
        public int Stock {  get; set; }
    }
}
