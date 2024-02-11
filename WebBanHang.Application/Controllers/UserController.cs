using AutoMapper;
using orinAuth = Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebBanHang.Domain;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Enums;
using WebBanHang.Domain.UseCase.Users_Admin;
using customAuth = WebBanHang.Infrastructre.Security;

namespace WebBanHang.Application.Controllers
{
    [orinAuth.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> userRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IAuthenication authenication;
        private JsonConfig config;
        private readonly IUserInfor userInfo;
        private readonly IMapper _mapper;
        public UserController(IRepository<User> userRepo, IAuthenication _authenicateModel
            , IOptions<JsonConfig> _config, IUserInfor userInfo, IRepository<Customer> customerRepo, IMapper mappingSource)
        {
            this.userRepo = userRepo;
            this.authenication = _authenicateModel;
            config = _config.Value;
            this.userInfo = userInfo;
            this.customerRepo = customerRepo;
            _mapper = mappingSource;
        }

        // Cac API su dung cho quan ly tai khoan
        [customAuth.AllowAnonymous]
        [HttpPost("authenicate")]
        public async Task<IActionResult> Authenicate(LogInModel logIn)
        {
            // Input: LogInModel -> email + password
            // Output: + A JWT access token (contain basic user infor)
                     // + A HttpOnly Cookie contain refresh Token
            return Ok();
        }

        [customAuth.AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            /*
             Input: A POST from client with refresh Token inside cookie payload
            Output: + A new JWT access token
                    + A HttpOnly Cookie contain a NEW refresh token (using Refresh Token Rotate)
             */
            return Ok();
        }

        [customAuth.Authorize(UserRole.Employee, UserRole.Customer)]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            /*
             Input: A refresh token inside coookie | request body (mainly: request body if both exists)
            Output: Revoke this token
             */
            return Ok();
        }

        [customAuth.AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> FirstRegister (Customer user)
        {
            /*
             Input: account registation detail (name, mail, password...)
            Output: - the account is registered
                    - a verification email is sent to the email address of the account
            (acc must be verificated before authenication/ authorization
             */
            return Ok();
        }

        [customAuth.AllowAnonymous]
        [HttpPost("verifying-email")]
        public async Task<IActionResult> VerifyingAccount()
        {
            // Input: Request body containing verifying token
            // Output: the account is verified
            return Ok();
        }

        [customAuth.AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordSecure()
        {
            /*
             Input: Request contain acc emial in request body
            Output: a password reset email is sent to the email address of the account
            (The email contains a single use reset token that is valid for one day.)
             */
            return Ok();
        }

        [customAuth.AllowAnonymous]
        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ToValidateResetToken()
        {
            /*
             Input: Request contain reset token (in request body)
            Output: A message is returned to indicate if the token is valid or not.
             */
            return Ok();
        }

        [customAuth.AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword()
        {
            /*
             Input: A POST request contain password, new password & reset token
            Output: - Password is reseted
                    - Reset token is deleted
             */
            return Ok();
        }

        [customAuth.Authorize(UserRole.Admin)]
        [HttpGet("AllAccounts")]
        public async Task<IActionResult> GetAllAccount()
        {
            /*
             Input: GET request
            Output: List of all account in system
             */
            return Ok();
        }

        [customAuth.Authorize(UserRole.Employee)]
        [HttpGet("CustomerAccounts")]
        public async Task<IActionResult> GetCustomerAccount()
        {
            // Output: List of all customer acc in system
            return Ok();
        }

        [customAuth.Authorize(UserRole.Customer)]
        [HttpGet("CustomerID")]
        public async Task<IActionResult> RetriveIndividualCUstomerAcc([FromHeader] int CusID)
        {
            // Retrive only information for specific customer
            return Ok();
        }

        [customAuth.Authorize(UserRole.Admin)]
        [HttpGet("AnyID")]
        public async Task<IActionResult> GetIDInformation([FromHeader] int ID)
        {
            // retrive any acc infor relative to this ID
            return Ok();
        }

        [customAuth.Authorize(UserRole.Employee)]
        [HttpPost("CustomerAcc")]
        public async Task<IActionResult> AddCustomerAcc([FromBody] Customer cus)
        {
            // Input: Customer acc detail
            // Output:  the account is created and automatically verified. (Acc is automatically marked as Customer role)
            return Ok();
        }

        [customAuth.Authorize(UserRole.Admin)]
        [HttpPost("AnyAcc")]
        public async Task<IActionResult> AddAccount([FromBody] Customer cus)
        {
            // Input : A specific infor about the acc
            // Output: the account is created and automatically verified. 
            return Ok();
        }

        [customAuth.Authorize(UserRole.Admin)]
        [HttpPut("AnyAcc")]
        public async Task<IActionResult> EditAccounts([FromBody] User _user)
        {
            // Input: data of specific acc need to update
            // Output: the updated acc in DB
            return Ok();
        }

        [customAuth.Authorize(UserRole.Admin)]
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteAccount([FromQuery] int ID)
        {
            // The specific acc relative to this ID is deleted
            return Ok();
        }
    }
}
