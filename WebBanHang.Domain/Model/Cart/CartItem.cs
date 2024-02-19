using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Model.Cart
{
    public class CartItem
    {
        private string Email = "";
        public string UserName
        {
            get { return Email.Split("@")[0]; }
            set
            {
                Email = value;
            }
        }
        public int Quantities { get; set; } = 1;
        public ProductDtos? Product { get; set; }
    }
}
