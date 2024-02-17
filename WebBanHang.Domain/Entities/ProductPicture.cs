using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class ProductPicture : BaseEntity
    {
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
        public string ProductImage { get; set; } = "";
        public string ProductAvatar { get; set; } = "";
        public string Status { get; set; } = "";

    }
}
