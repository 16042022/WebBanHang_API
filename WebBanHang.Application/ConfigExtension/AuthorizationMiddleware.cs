using Microsoft.Extensions.Options;
using WebBanHang.Domain;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.UseCase.Users_Admin;

namespace WebBanHang.Application.ConfigExtension
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate requestDelegate)
        {
            _next = requestDelegate;
        }

        public async Task Invoke(HttpContext context, IRepository<Users> _userRepository,
            IAuthenication authenication, IOptions<JsonConfig> config) 
        {
            // Retrive the access token
            string token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last()!;
            // Check token
            string userResult = authenication.ValidateJwtToken(token, config.Value);
            if (userResult != null) 
            {
                context.Items["User"] = await _userRepository.GetByName(userResult);
            }
            await _next(context);
        }
    }

    public static class JWTMiddleware
    {
        public static IApplicationBuilder UseJWTMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthorizationMiddleware>();
        }
    } 
}
