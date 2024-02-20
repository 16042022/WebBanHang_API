using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Model.Cart;
using WebBanHang.Domain.UseCase.Products;

namespace WebBanHang.Domain.Services
{
    public class ProductService : IProductManager
    {
        private IRepository<Product> productRepo;

        public ProductService(IRepository<Product> productRepo)
        {
            this.productRepo = productRepo;
        }

        public Task AddProduct(AddingProductRequest product)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProduct(int ID)
        {
            throw new NotImplementedException();
        }

        public Task GetAllProduct()
        {
            throw new NotImplementedException();
        }

        public Task UpdateProduct(ProductDtos product)
        {
            throw new NotImplementedException();
        }
    }
}
