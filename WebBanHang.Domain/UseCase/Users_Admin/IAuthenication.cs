using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Domain.UseCase.Users_Admin
{
    public interface IAuthenication
    {
        public RefreshToken GenerateRefreshToken(string ipAddress);
        // Generate Toekn => tra ve client
        public string GenerateToken(JsonConfig config, Users users);
        // RefreshToken => lay lai accessToken
        public string ValidateJwtToken(string Token, JsonConfig config);
    }
}
