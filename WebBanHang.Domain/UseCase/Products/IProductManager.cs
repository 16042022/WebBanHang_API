using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Model.Cart;
using WebBanHang.Domain.Model.Product;

namespace WebBanHang.Domain.UseCase.Products
{
    public interface IProductManager
    {
        // Add san pham
        public Task AddProduct(AddingProductRequest product);
        // Update san pham
        public Task UpdateProduct(UpdateProductRequest product);
        // Xoa san pham (soft delete)
        public Task DeleteProduct(int ID);
        public Task<IEnumerable<ProductDtos>> GetAllProduct();
        public Task<ProductDtos> GetProduct(int ID);
        // Up anh san pham len cloud => luu chuoi ket noi vao db
    }
}
