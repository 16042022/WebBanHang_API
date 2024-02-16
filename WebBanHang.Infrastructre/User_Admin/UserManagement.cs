using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Model;
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

        public async Task<Users> checkLogInInfor(LogInModel logIn)
        {
            Users? check = await dbContext.user.FirstOrDefaultAsync(x => x.Email == logIn.Email)?? 
                throw new InvalidDataException("User's email not match any accounts");
            // CHeck password
            bool isOK = PasswordManagement.IsValidPassword(logIn.Password, check.Password);
            if (!isOK) throw new AggregateException("Password isn't not correct");
            return check;
        }

        public async Task<int> GetNumberOfCustomer()
        {
            return await dbContext.user.CountAsync();
        }

        public async Task<Users> GetUserByResetToken(string resetToken)
        {
            Users? check = await dbContext.user.FirstOrDefaultAsync(x => x.ResetPwdToken == resetToken)
                ?? throw new AggregateException("This reset pwd token is not link to any account");
            return check;
        }

        public async Task<Users> GetUserFromRefreshToken(string refreshToken)
        {
            Users? check = await dbContext.user.FirstOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == refreshToken)) 
                ?? throw new AggregateException("This refresh pwd token is not link to any account");
            return check;
        }

        public string HassPassword(string inPwd)
        {
            return PasswordManagement.HashPassword(inPwd);
        }

        public bool IsUniqueRefreshToken(string refreshToken)
        {
            return dbContext.user.Any(x => x.RefreshTokens.Any(x => x.Token == refreshToken));
        }

        public bool IsUniqueResetToken(string resetToken)
        {
            return dbContext.user.Any(x => x.ResetPwdToken == resetToken);
        }

        public bool IsUniqueVerifyToken(string verifyToken)
        {
            return dbContext.user.Any(x => x.VerifyToken == verifyToken);
        }

        public bool IsValidPassword(string inPwd, string dbPwd)
        {
            return PasswordManagement.IsValidPassword(inPwd, dbPwd);
        }

        public async Task<Users> ValidationVerifyToken(string verifyToken)
        {
            Users? check = await dbContext.user.FirstOrDefaultAsync(x => x.VerifyToken == verifyToken);
            if (check == null) throw new AggregateException("This verify pwd token is not link to any account");
            else
            {
                check.VerifyDate = DateTime.Now;
                check.VerifyToken = null;
                dbContext.Update(check); await dbContext.SaveChangesAsync();
                return check;
            }
        }
    }
}
