using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.UseCase.Others;
using WebBanHang.Domain.UseCase.Users_Admin;

namespace WebBanHang.Domain.Services
{
    public class UserService : IUserSerrvice
    {
        private readonly IAuthenication authenication;
        private readonly IUserInfor userInfor;
        private readonly JsonConfig config;
        private readonly IRepository<User> userRepo; 

        public UserService(IAuthenication authenication, IUserInfor infor, 
            IOptions<JsonConfig> config, IRepository<User> repository)
        {
            this.authenication = authenication;
            this.userInfor = infor;
            this.config = config.Value;
            userRepo = repository;
        }

        // Authenicate: From LogInModel => access token if true, null if invalid

        public async Task<AuthenicationRespone> Authenticate(LogInModel model, string ipAddress, JsonConfig config)
        {
            User check = await userInfor.checkLogInInfor(model);
            Customer relativeCustomer = await userInfor.FromUserToCustomer(check);
            // Generate some token
            var AccessToken = authenication.GenerateToken(config, check);
            var RefreshToken = authenication.GenerateRefreshToken(ipAddress);
            check.RefreshTokens.Add(RefreshToken);
            // Remove old items if any
            removeOldRefreshToken(check, config);
            // return obj
            AuthenicationRespone respone = new AuthenicationRespone()
            {
                JWTAccessToken = AccessToken,
                JWTRefreshToken = RefreshToken,
                FirstName = relativeCustomer.FirstName,
                LastName = relativeCustomer.LastName
            };
            return respone;
        }

        private void removeOldRefreshToken(User check, JsonConfig config)
        {
            // Xoa het cac refresh token cu neu con
            var tokenList = check.RefreshTokens.Where(x =>
            (x.IsRevoked && x.IsExpired) &&
            x.Created.AddDays(config.RefreshTokenTTL) <= DateTime.UtcNow).ToList();
            foreach (var token in tokenList)
            {
                check.RefreshTokens.Remove(token);
            }
        }

        public async Task<AuthenicationRespone> RefreshToken(string token, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public void RevokeToken(string token, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            return userRepo.GetAll();
        }

        public Task<User> GetById(int id)
        {
            return userRepo.GetById(id);
        }
    }
}
