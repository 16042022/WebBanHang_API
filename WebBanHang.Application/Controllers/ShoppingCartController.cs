using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using WebBanHang.Domain;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Model.Cart;
using WebBanHang.Domain.UseCase.Products;

namespace WebBanHang.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private IRepository<Product> productRepo;
        private IMapper _mapper;
        private IOrderCaculate orderCaculate;
        private const string CARTKEY = "cart";
        private ISession session;
        public ShoppingCartController(IRepository<Product> repo, 
            IMapper mapper, IOrderCaculate caculate) 
        {
            _mapper = mapper;
            productRepo = repo;
            session = HttpContext.Session;
            orderCaculate = caculate;
        }

        private List<CartItem> GetCartItems()
        {
            var result = session.GetString(CARTKEY);
            if (result != null)
            {
                return JsonSerializer.Deserialize<List<CartItem>>(result)!;
            }
            return new List<CartItem>();
        }

        // Tra ve list cart
        [HttpGet("getListCart")]
        public IActionResult GetListCart() 
        {
            return Ok(GetCartItems());
        }

        // Them hang vao gio hang
        [HttpPost("addcart/{productID}")]
        public async Task<IActionResult> AddToCart([FromQuery] int ID)
        {
            // Return Product from ProductID
            Product items = await productRepo.GetById(ID);
            if (items == null) { return BadRequest(new {message = "Not found"}); } 
            // Add into cart session (list of cart)
            var model = _mapper.Map<ProductDtos>(items);
            var userIdentity = session.GetString("userName");
            // Save into session
            var listItem = GetCartItems();
            var cartItem = listItem.Find(x => x.Product!.Id == ID);
            if (cartItem != null)
            {
                if (cartItem.Quantities > cartItem.Product!.Stock) return BadRequest(new { message = "Out of stock product" });
                else cartItem.Quantities++;
            }
            else listItem.Add(new CartItem() { UserName = userIdentity!, Product = model });
            SaveToSession(listItem);
            return Ok();
        }

        private void SaveToSession(List<CartItem> ls)
        {
            var jsonObj = JsonSerializer.Serialize(ls);
            session.SetString(CARTKEY, jsonObj);
        }

        [HttpPut("updateCart")]
        public IActionResult UpdateCart([FromQuery] int ID, [FromQuery] int quantity) 
        {
            var listItem = GetCartItems();
            var elem = listItem.Find(x => x.Product!.Id == ID);
            if (elem != null) 
            {
                if (quantity > 0 && quantity <= elem.Product!.Stock)
                    elem.Quantities = quantity; 
                else return BadRequest(new { message = "Out of stock product" });
            }
            SaveToSession(listItem);
            return Ok(new {message = "Update shopping cart succesfful"});    
        }

        [HttpDelete("deleteCart/{productID}")]
        public IActionResult RemoveCart([FromQuery] int ID)
        {
            // input: ProductID
            var items = GetCartItems();
            var itemToDel = items.Find(x => x.Product!.Id == ID);
            if (itemToDel != null)
            {
                items.Remove(itemToDel);
                return Ok(new { message = "Remove cart items done" });
            }
            return BadRequest(new { message = "Invalid product code" });
        }

        [HttpGet("checkOut")]
        public async Task<IActionResult> CheckOut()
        {
            // From list cart => order detail
            var finalList = GetCartItems();
            await orderCaculate.AddNewOrder(finalList);
            return Ok(new {message = "Order has done"});
        }
    }
}
