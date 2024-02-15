using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Model;

namespace WebBanHang.Domain.UseCase.Users_Admin
{
    public interface IUserInfor
    {
        public bool IsValidPassword(string inPwd, string dbPwd);
        public bool IsUniqueRefreshToken(string refreshToken);
        public bool IsUniqueVerifyToken(string verifyToken);
        public bool IsUniqueResetToken (string resetToken);
        public string HassPassword(string inPwd);
        public Task<Users> checkLogInInfor(LogInModel logIn);
        public Task<Users> GetUserFromRefreshToken(string refreshToken);
        public Task<Users> ValidationVerifyToken(string verifyToken);
        public Task<Users> GetUserByResetToken (string resetToken);
        // Group of user picture upload & management
    }
}
