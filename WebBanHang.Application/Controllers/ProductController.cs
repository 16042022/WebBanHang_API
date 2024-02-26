using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MR.AspNetCore.Pagination;
using WebBanHang.Domain.Model.Cart;
using WebBanHang.Domain.Model.Product;
using WebBanHang.Domain.UseCase.Products;
using cusAuth = WebBanHang.Infrastructre.Security;

namespace WebBanHang.Application.Controllers
{
    [cusAuth.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private IPaginationService _pagerService;
        private IProductManager product;
        public ProductController(IPaginationService service, IProductManager manager) 
        {
            _pagerService = service;
            product = manager;
        }

        [cusAuth.AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDtos>> Get(int ID) 
        {
            var result = await product.GetProduct(ID);
            return Ok(result);
        }

        [cusAuth.AllowAnonymous]
        [HttpGet("getAllList")]
        public async Task<ActionResult<OffsetPaginationResult<ProductDtos>>> GetAll() 
        {
            var result = await product.GetAllProduct();
            var outData =  _pagerService.OffsetPaginate(result.ToList(), 15);
            return Ok(outData);
        }

        [cusAuth.Authorize(Domain.Enums.UserRole.Admin 
            | Domain.Enums.UserRole.Employee)]
        [HttpPost("adding_product")]
        public async Task<IActionResult> AddingProduct (AddingProductRequest request)
        {
            if (request == null) return BadRequest("Not found item");
            await product.AddProduct(request);
            return Ok(new {message = "Add product is success"});
        }

        [cusAuth.Authorize(Domain.Enums.UserRole.Admin
            | Domain.Enums.UserRole.Employee)]
        [HttpPut("update_product")]
        public async Task<IActionResult> UpdateProduct (UpdateProductRequest request)
        {
            if (request == null) return BadRequest("Not found item");
            await product.UpdateProduct(request);
            return Ok(new { message = "Update product is success" });
        }

        [cusAuth.Authorize(Domain.Enums.UserRole.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int ID)
        {
            await product.DeleteProduct(ID);
            return Ok(new {message = "Delete product sucessful"});
        }
    }
}
