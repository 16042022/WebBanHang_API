using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public ProductStatus Status { get; set; } = null!;
        public int CategoryID { get; set; }
        public ProductType ProductType { get; set; } = null!;
        public int PictureID { get; set; }
        [JsonIgnore]
        public ICollection<ProductPicture> Picture { get; set; } = new List<ProductPicture>();
        [JsonIgnore]
        public ICollection<ReviewProduct> ReviewProduct { get; set; } = new List<ReviewProduct>();
    }
}
