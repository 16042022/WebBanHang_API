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
        private readonly IRepository<User> _userRepository;
        private readonly IAuthenication authenication;
        private readonly JsonConfig config;

        public AuthorizationMiddleware(RequestDelegate requestDelegate, IRepository<User> userRepository, 
            IAuthenication authenication, IOptions<JsonConfig> options)
        {
            _next = requestDelegate;
            _userRepository = userRepository;
            this.authenication = authenication;
            this.config = options.Value;
        }

        public async Task Invoke(HttpContext context) 
        {
            // Retrive the access token
            string token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last()!;
            // Check token
            string userResult = authenication.ValidateJwtToken(token, config);
            if (userResult != null) 
            {
                context.Items["User"] = await _userRepository.GetByName(userResult);
            }
            await _next(context);
        }
    }

    public static class JWTMiddleware
    {
        public static IApplicationBuilder UseJWTMiddleware(this IApplicationBuilder app, IRepository<User> userRepo, IAuthenication authenRepo)
        {
            return app.UseMiddleware<AuthorizationMiddleware>(userRepo, authenRepo);
        }
    } 
}
