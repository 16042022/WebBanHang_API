using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Model.Cart;

namespace WebBanHang.Domain.UseCase.Products
{
    public interface IOrderCaculate
    {
        // Input into order detail
        public Task AddNewOrder (List<CartItem> items);
    }
}
