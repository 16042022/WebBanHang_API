using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.UseCase.Users_Admin;
using WebBanHang.Infrastructre.Models;

namespace WebBanHang.Infrastructre.Security
{
    public class AuthenicationProvider : IAuthenication
    {
        private readonly AppDbContext dbContext;

        public AuthenicationProvider(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            RefreshToken token = new RefreshToken()
            {
                CreatedByIp = ipAddress,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                Token = getUniqueToken()
            };

            return token;

            string getUniqueToken()
            {
                string token; bool isUnique;
                // token is a cryptographically strong random sequence of values
                // Check
                do
                {
                    token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
                    isUnique = dbContext.user.Any(op => op.RefreshTokens.Any(utk => utk.Token == token));
                } while (isUnique);
                return token;
            }
        }

        public string GenerateToken(JsonConfig config, Users users)
        {
            // Generate token:
            var tokenHandle = new JwtSecurityTokenHandler();
            var tkey = Encoding.UTF8.GetBytes(config.SecretKey);
            var TokenDescrp = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserName", users.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, users.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tkey),
                SecurityAlgorithms.HmacSha256Signature)
            };
            // Put tken into container
            var token = tokenHandle.CreateToken(TokenDescrp);
            return tokenHandle.WriteToken(token);
        }

        public string ValidateJwtToken(string Token, JsonConfig config)
        {
            if (string.IsNullOrWhiteSpace(Token)) return null;
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandle = new JwtSecurityTokenHandler();
            SecurityToken checkResult;
            try
            {
                tokenHandle.ValidateToken(Token, validationParameters, out checkResult);
                var jwtToken = (JwtSecurityToken)checkResult;
                return jwtToken.Claims.First(x => x.Type == "UserName").Value.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
