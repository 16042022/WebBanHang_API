using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Domain.UseCase.Users_Admin
{
    public interface IUserInfor
    {
        public User FromCustomerInfo(Customer entity);
        public bool IsValidPassword(string inPwd, string dbPwd);
        public bool CheckVerifyToken(string verifyToken);
        public Task<User> checkLogInInfor(LogInModel logIn);
        public Task<Customer> FromUserToCustomer (User entity);
        public Task<User> GetUserFromRefreshToken(string refreshToken);
        public Task ValidationVerifyToken(string verifyToken);
        public Task<User> GetUserByResetToken (string resetToken);
        // Group of user picture upload & management
    }
}
