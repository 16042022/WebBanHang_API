using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public Users User => (Users)HttpContext.Items["User"]!;
    }
}
