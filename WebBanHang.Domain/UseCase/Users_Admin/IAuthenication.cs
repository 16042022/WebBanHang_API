using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Domain.UseCase.Users_Admin
{
    public interface IAuthenication
    {
        public Task<JWTTokenIdentity> Authenication(JsonConfig config, LogInModel logIn);
        // Generate Toekn => tra ve client
        public Task<JWTTokenIdentity> GenerateTokenAsync(JsonConfig config, User users);
        // RefreshToken => lay lai accessToken
        public Task<JWTTokenIdentity> RefreshTokenAsync(JWTTokenIdentity identity, JsonConfig config);
        // Generate Reset PWd Token => tra ve string token cho client
        // Di kem cap nhat expires cuar Token
        public string ValidateJwtToken(string Token, JsonConfig config);
    }
}
