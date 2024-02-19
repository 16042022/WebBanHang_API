using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebBanHang.Domain;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Model.Cart;

namespace WebBanHang.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private IRepository<Product> productRepo;
        private ICollection<CartItem> cartItems;
        private IMapper _mapper;
        private const string CARTKEY = "cart";
        private ISession session;
        public ShoppingCartController(IRepository<Product> repo, 
            IMapper mapper) 
        {
            _mapper = mapper;
            cartItems = new List<CartItem>();
            productRepo = repo;
            session = HttpContext.Session;
        }

        // Tra ve list cart
        [HttpGet("getListCart")]
        public IActionResult GetListCart() 
        {
            var result = session.GetString(CARTKEY);
            if (result != null)
            {
                return Ok(JsonSerializer.Deserialize<List<CartItem>>(result));
            }
            return Ok(cartItems);
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
            var cart = new CartItem()
            {
                UserName = userIdentity!, Product = model
            };
            // Save into session
            cartItems.Add(cart); // For further search task
            SaveToSession(cart);
            return Ok(cart);
        }

        private void SaveToSession(CartItem cart)
        {
            var jsonObj = JsonSerializer.Serialize(cart);
            session.SetString(CARTKEY, jsonObj);
        }

        [HttpPut("updateCart")]
        public IActionResult UpdateCart([FromQuery] int ID, [FromQuery] int quantity) 
        {
            // Tim trong Session product t/ung
            CartItem cartItem = FindElement(ID);
            if (cartItem == null) return BadRequest(new { message = "Not found" });
            // Cap nhat
            UpdateIntoCarts(cartItem, quantity);
            return Ok();    
        }

        private void UpdateIntoCarts(CartItem cartItem, int quantity)
        {
            cartItems.Remove(cartItem);
            cartItem.Quantities = quantity;
            SaveToSession(cartItem); cartItems.Add(cartItem);
        }

        private CartItem FindElement(int iD)
        {
            return cartItems.Where(x => x.Product!.Id == iD).Single();
        }

        [HttpDelete("deleteCart/{productID}")]
        public IActionResult RemoveCart([FromQuery] int ID)
        {
            return Ok();
        }

        [HttpGet("checkOut")]
        public IActionResult CheckOut()
        {
            // From list cart => order detail
            return Ok();
        }
    }
}
