using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class ProductStatus : BaseEntity
    {
        public string StatusName { get; set; } = "";
        public Product Product { get; set; } = null!;
    }
}
