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
                JWTRefreshToken = RefreshToken.Token,
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
            // From given token => get user => get the status of refresh token
            User customer = await userInfor.GetUserFromRefreshToken(token);
            Customer relativeCustomer = await userInfor.FromUserToCustomer(customer);
            var refreshToken = customer.RefreshTokens.Single(x => x.Token == token);
            // Check:
            if (refreshToken.IsRevoked)
            {
                // Remove all decendant tokens
                await RemoveChildRefreshToken(refreshToken, customer, 
                    ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                await userRepo.Update(customer);
            }
            if (refreshToken.IsExpired && refreshToken.IsRevoked) throw new InvalidDataException("Token is not valid");
            var rotateToken = RotateRefreshToken(refreshToken, ipAddress);
            customer.RefreshTokens.Add(rotateToken);
            var AccessToken = authenication.GenerateToken(config, customer);
            // Remove old token if any
            removeOldRefreshToken(customer, config);
            // Save the object
            await userRepo.Update(customer);
            AuthenicationRespone respone = new AuthenicationRespone()
            {
                JWTAccessToken = AccessToken,
                JWTRefreshToken = rotateToken.Token,
                FirstName = relativeCustomer.FirstName,
                LastName = relativeCustomer.LastName
            };
            return respone;
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = authenication.GenerateRefreshToken(ipAddress);
            // Revoked the old one
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private async Task RemoveChildRefreshToken(RefreshToken refreshToken, User customer, string ipAddress, string v)
        {
            var childToken = customer.RefreshTokens.FirstOrDefault(x => x.ReplacedByToken == refreshToken.Token);
            if (childToken != null)
            {
                do
                {
                    if (childToken.IsExpired && childToken.IsRevoked)
                    {
                        RevokeRefreshToken(childToken, ipAddress, v);
                        await userRepo.Update(customer);
                    }
                    childToken = customer.RefreshTokens.FirstOrDefault(x => x.ReplacedByToken == childToken.Token);
                    if (childToken == null) break;
                } while (!childToken.IsExpired && !childToken.IsRevoked);
            }
        }

        public async Task RevokeToken(string token, string ipAddress)
        {
            // From given token => get user => get the status of refresh token
            User customer = await userInfor.GetUserFromRefreshToken(token);
            var refreshToken = customer.RefreshTokens.Single(x => x.Token == token);
            if (refreshToken.IsExpired && refreshToken.IsRevoked)
                throw new InvalidDataException("Token is already not actived");
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            await userRepo.Update(customer);
        }

        private void RevokeRefreshToken(RefreshToken refreshToken, string ipAddress,
            string reason = null, string replacedByToken = null)
        {
            refreshToken.Revoked = DateTime.Now;
            refreshToken.ReasonRevoked = reason;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = replacedByToken;
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
