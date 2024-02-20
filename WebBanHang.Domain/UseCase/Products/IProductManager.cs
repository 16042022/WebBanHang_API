using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Model.Cart;

namespace WebBanHang.Domain.UseCase.Products
{
    public interface IProductManager
    {
        // Add san pham
        public Task AddProduct(AddingProductRequest product);
        // Update san pham
        public Task UpdateProduct(ProductDtos product);
        // Xoa san pham (soft delete)
        public Task DeleteProduct(int ID);
        public Task GetAllProduct();
        // Up anh san pham len cloud => luu chuoi ket noi vao db
    }
}
