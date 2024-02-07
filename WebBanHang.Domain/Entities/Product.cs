using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; } = "";
        public string ProductDescription { get; set; } = ""; // Title
        public float Price { get; set; }
        [Range(0, 100)]
        public float Discount { get; set; }
        public int NumOfViews { get; set; }
        public int Stock { get; set; }
        public int StatusID { get; set; }
        public int CategoryID { get; set; }
        public int PictureID { get; set; }
    }
}
