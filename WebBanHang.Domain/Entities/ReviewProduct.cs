using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.Entities
{
    public class ReviewProduct : BaseEntity
    {
        public int UserID { get; set; }
        public Users User { get; set; } = null!;
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
        public string ContentRated { get; set; } = "";
        [Range(0, 10)]
        public int EvaluatedPoint { get; set; }
        public string ContentSeen { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
