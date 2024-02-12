using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.UseCase.Users_Admin;
using WebBanHang.Infrastructre.Models;
using WebBanHang.Infrastructre.Security;
using ZstdSharp;

namespace WebBanHang.Infrastructre.User_Admin
{
    public class UserManagement : IUserInfor
    {
        private AppDbContext dbContext;

        public UserManagement(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> checkLogInInfor(LogInModel logIn)
        {
            User? check = await dbContext.user.FirstOrDefaultAsync(x => x.Email == logIn.Email
            && PasswordManagement.IsValidPassword(logIn.Password, x.Password));
            return check ?? throw new InvalidDataException("User is not valid");
        }

        public User FromCustomerInfo(Customer entity)
        {
            User fromCustomer = new User()
            {
                Email = entity.Email,
                UserName = entity.Email.Split("@")[0],
                Status = "Active",
                RoleID = 3,
                CreateAt = DateTime.Now,
                Password = entity.Password,
            };
            return fromCustomer;
        }

        public async Task<Customer> FromUserToCustomer(User entity)
        {
            return await dbContext.Customers.FirstAsync(x => x.UserID == entity.Id);
        }

        public string GenerateRefreshPwdToken()
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserFromRefreshToken(string refreshToken)
        {
            User? check = await dbContext.user.FirstOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == refreshToken));
            if (check == null) throw new InvalidDataException("Invalid refresh token");
            return check;
        }

        public bool IsValidPassword(string inPwd, string dbPwd)
        {
            return PasswordManagement.IsValidPassword(inPwd, dbPwd);
        }
    }
}
