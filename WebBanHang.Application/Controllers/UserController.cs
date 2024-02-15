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
using WebBanHang.Domain.UseCase.Others;
using WebBanHang.Domain.Model;

namespace WebBanHang.Application.Controllers
{
    [orinAuth.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private IUserSerrvice userSerrvice;
        private JsonConfig config;
        public UserController( IOptions<JsonConfig> _config,
            IUserSerrvice serrvice)
        {
            userSerrvice = serrvice;
            config = _config.Value;
        }

        // Cac API su dung cho quan ly tai khoan
        [customAuth.AllowAnonymous]
        [HttpPost("authenicate")]
        public async Task<IActionResult> Authenicate(LogInModel logIn)
        {
            // Input: LogInModel -> email + password
            var respone = await userSerrvice.Authenticate(logIn, IpAdress(), config);
            SetTokenCookie(respone.JWTRefreshToken!);
            // Output: + A JWT access token (contain basic user infor)
                     // + A HttpOnly Cookie contain refresh Token
            return Ok(respone);
        }

        private void SetTokenCookie(string jWTRefreshToken)
        {
            var CookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            Response.Cookies.Append("refreshToken", jWTRefreshToken, CookieOption);
        }

        private string IpAdress()
        {
            if (Request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues value))
                return value!;
            else return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
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
            string? refreshToken;
            if (Request.Cookies.TryGetValue("refreshToken", out refreshToken))
            {
                var identity = await userSerrvice.RefreshToken(refreshToken, IpAdress());
                SetTokenCookie(identity.JWTRefreshToken!);
                return Ok(identity);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody]string token)
        {
            /*
             Input: A refresh token inside coookie | request body (mainly: request body if both exists)
            Output: Revoke this token
             */
            var refreshToken = token ??= Request.Cookies["refreshToken"]!;
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "Token is required" });

            if (!User.OwnedToken(token) && User.RoleID != (int)UserRole.Admin)
                return Unauthorized(new { meesage = "Authorize is required" });

            await userSerrvice.RevokeToken(token, IpAdress());
            return Ok(new { message = "Token is revoked" });
        }

        [customAuth.AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> FirstRegister ([FromBody]UserRegisterModel model, 
            [FromQuery]bool isEmployee)
        {
            /*
             Input: account registation detail (name, mail, password...)
            Output: - the account is registered
                    - a verification email is sent to the email address of the account
            (acc must be verificated before authenication/ authorization
             */
            await userSerrvice.Register(model, Request.Headers["origin"]!, isEmployee);
            return Ok(new { message = "Please check your email for register instructions" });
        }

        [customAuth.AllowAnonymous]
        [HttpPost("verifying-email")]
        public async Task<IActionResult> VerifyingAccount([FromBody] string token)
        {
            // Input: Request body containing verifying token
            // Output: the account is verified
            await userSerrvice.VerifyEmail(token);
            return Ok();
        }

        [customAuth.AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordSecure([FromBody] string email)
        {
            /*
             Input: Request contain acc emial in request body
            Output: a password reset email is sent to the email address of the account
            (The email contains a single use reset token that is valid for one day.)
             */
            await userSerrvice.ForgotPasswordProcess(email, Request.Headers["origin"]!);
            return Ok(new {message = "Please check your email for password reset instructions" });
        }

        [customAuth.AllowAnonymous]
        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ToValidateResetToken([FromBody] string token)
        {
            /*
             Input: Request contain reset token (in request body)
            Output: A message is returned to indicate if the token is valid or not.
             */
            await userSerrvice.ValidateReseToken(token);
            return Ok(new { message = "Token is valid"});
        }

        [customAuth.AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest model)
        {
            /*
             Input: A POST request contain new password, confirm password & reset token
            (Process by FE side)
            Output: - Password is reseted
                    - Reset token is deleted
             */
            await userSerrvice.ResetPasswordProcess(model);
            return Ok(new {message = "Password reset successful, you can now login"});
        }

        [customAuth.Authorize(UserRole.Admin)]
        [HttpGet("AllAccounts")]
        public async Task<IActionResult> GetAllAccount([FromQuery] bool isCustomer)
        {
            /*
             Input: GET request
            Output: List of all account in system
             */
            var listUser = await userSerrvice.GetAll(isCustomer);
            return Ok(listUser);
        }

        [customAuth.Authorize(UserRole.Employee)]
        [HttpGet("CustomerAccounts")]
        public async Task<ActionResult<AccountRespone>> GetCustomerAccount([FromQuery] bool isCustomer)
        {
            // Output: List of all customer acc in system
            var customerList = await userSerrvice.GetAll(isCustomer);
            return Ok(customerList);
        }

        [customAuth.Authorize(UserRole.Admin)]
        [HttpGet("AnyID")]
        public async Task<ActionResult<AccountRespone>> GetIDInformation([FromHeader] int ID)
        {
            // retrive any acc infor relative to this ID
            var result = await userSerrvice.GetById(ID);
            return Ok(result);
        }

        [customAuth.Authorize(UserRole.Admin)]
        [HttpPost("AnyAcc")]
        public async Task<ActionResult<AccountRespone>> AddAccount([FromBody] CreateRequest cus)
        {
            // Input : A specific infor about the acc
            // Output: the account is created and automatically verified. 
            var result = await userSerrvice.CreateAccount(cus);
            return Ok(result);
        }

        [HttpPut("AnyAcc")]
        public async Task<IActionResult> EditAccounts(int ID, [FromBody] EditAccountRequest request)
        {
            // Input: data of specific acc need to update
            // Output: the updated acc in DB (Only Admin can change Role, other exclusive)
            if (User.Id != ID && User.RoleID != (int)UserRole.Admin) return Unauthorized(new { message = "Invalid user identity" });
            if (request == null) return BadRequest(new { message = "None of object to be updated" });
            if (User.RoleID != (int)UserRole.Admin) request.Role = "";

            var result = await userSerrvice.UpdateAccount(ID, request);
            return Ok(result);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteAccount([FromQuery] int ID)
        {
            // The specific acc relative to this ID is deleted
            // only admin can delete any acc, employee & customer can't delete themself
            if (ID != User.Id || User.RoleID != (int)UserRole.Admin)
                return Unauthorized(new { message = "Unauthorize" });
            await userSerrvice.DeleteAccount(ID);
            return Ok();
        }
    }
}
