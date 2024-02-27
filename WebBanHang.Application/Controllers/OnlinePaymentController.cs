using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MR.AspNetCore.Pagination;
using WebBanHang.Domain.Entities.OnlinePayment;
using WebBanHang.Domain.Model.Payment;

namespace WebBanHang.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlinePaymentController : ControllerBase
    {
        private IPaginationService paginationService;
        public OnlinePaymentController (IPaginationService paginationService)
        {
            this.paginationService = paginationService;
        }

        [HttpPost("createPayment")]
        public IActionResult CreatePayment([FromBody] CreatePaymentReq req)
        {
            return Ok();
        }

        [HttpGet("listPayment")]
        public ActionResult<OffsetPaginationResult<OnlinePayment>> GetListPayment()
        {
            return Ok();
        }

        [HttpGet("{ID")]
        public ActionResult<OnlinePayment> GetPayment(int id)
        {
            return Ok();
        }
    }
}
