using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class Categories : BaseEntity
    {
        public string CategoryName { get; set; } = "";
    }
}
