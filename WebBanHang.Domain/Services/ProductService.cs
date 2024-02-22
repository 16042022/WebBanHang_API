using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Enums;
using WebBanHang.Domain.Model.Cart;
using WebBanHang.Domain.Model.Product;
using WebBanHang.Domain.UseCase.Products;

namespace WebBanHang.Domain.Services
{
    public class ProductService : IProductManager
    {
        private IRepository<Product> productRepo;
        private IMapper mapper;

        public ProductService(IRepository<Product> productRepo,
            IMapper _mapper)
        {
            this.productRepo = productRepo;
            mapper = _mapper;
        }

        private async Task<Product> GetByName(string name) 
        {
            return await productRepo.DbSet()
                .FirstOrDefaultAsync(x => x.ProductName.ToLower() == name.ToLower());
        }

        public async Task AddProduct(AddingProductRequest product)
        {
            Product? check = await GetByName(product.Name);
            if (check != null) throw new AggregateException("This product already in system");
            
            // If not
            check = mapper.Map<Product>(product);
            await productRepo.Add(check);
        }

        public async Task DeleteProduct(int ID)
        {
            Product? check = await productRepo.GetById(ID) 
                ?? throw new AggregateException("Product code is not valid");
            check.StatusID = (int)PrdtStatus.Out_of_stock;
            check.Stock = 0;
            check.UpdateAt = DateTime.Now;

            await productRepo.Update(check);
        }

        public async Task<IEnumerable<ProductDtos>> GetAllProduct()
        {
            var listItem = await productRepo.GetAll();
            var result = listItem.Select(x => mapper.Map<ProductDtos>(x));
            return result;
        }

        public async Task UpdateProduct(UpdateProductRequest product)
        {
            Product? check = await GetByName(product.Name) ??
                throw new AggregateException("This product not valid");
            check = mapper.Map<Product>(product);
            await productRepo.Update(check);
        }

        public async Task<ProductDtos> GetProduct(int ID)
        {
            var item = await productRepo.GetById(ID);
            return mapper.Map<ProductDtos>(item);
        }
    }
}
