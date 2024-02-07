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

        public async Task<JWTTokenIdentity> GenerateTokenAsync(JsonConfig config, User users)
        {
            // Generate token:
            var tokenHandle = new JwtSecurityTokenHandler();
            var tkey = Encoding.UTF8.GetBytes(config.SecretKey);
            var TokenDescrp = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, users.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, users.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tkey),
                SecurityAlgorithms.HmacSha256Signature)
            };
            // Put tken into container
            var token = tokenHandle.CreateToken(TokenDescrp);
            var AccesToken = tokenHandle.WriteToken(token);
            var RefresfToken = GetRefreshToken();

            // For the 1st time => Storing into DB
            var refershTknItems = new StoredToken()
            {
                UserID = users.Id,
                RefreshToken = RefresfToken,
                JwtID = token.Id,
                IssuedAt = DateTime.UtcNow,
                ExpiresTime = DateTime.UtcNow.AddHours(1),
                IsRevoked = false,
                IsUsed = false
            };

            if (!await dbContext.stored_token.AnyAsync(x => x.UserID == users.Id)) {
                dbContext.stored_token.Add(refershTknItems);
            } else
            {
                dbContext.stored_token.Update(refershTknItems);
            }
            await dbContext.SaveChangesAsync();

            return new JWTTokenIdentity()
            {
                JWTAccessToken = AccesToken,
                JWTRefreshToken = RefresfToken
            };
        }

        private static string GetRefreshToken()
        {
            var random = new byte[64];
            /*Create and fill in cytography code into orin byte array*/
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(random);
            /**/
            return Convert.ToBase64String(random);
        }

        public async Task<JWTTokenIdentity> RefreshTokenAsync(JWTTokenIdentity identity, JsonConfig config)
        {
            var tokenHandle = new JwtSecurityTokenHandler();
            var tkey = Encoding.UTF8.GetBytes(config.SecretKey);
            var JWTparamenter = new TokenValidationParameters()
            {
                // Tu cap token
                ValidateIssuer = false,
                ValidateAudience = false,
                // Ky len token:
                ValidateIssuerSigningKey = true,
                // Ma hoa:
                IssuerSigningKey = new SymmetricSecurityKey(tkey),
                ValidateLifetime = false
            };
            // Check token
            try
            {
                SecurityToken validateToken;
                // Check format of expired token
                var checkValidate = tokenHandle.ValidateToken(identity.JWTAccessToken, JWTparamenter, out validateToken);
                // Check which algorithm is using:
                if (validateToken is JwtSecurityToken jwtToken)
                {
                    var result = jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result) { return null; }
                }

                // Check accessToken da expired chua: Parse trong danh sach claims tu bien token da validate
                var utcCurrDate = long.Parse(checkValidate.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);
                var expireDate = FromUnixTimeToUtcTime(utcCurrDate);
                if (expireDate > DateTime.UtcNow) { return null; } // Neu van con thoi han

                // Check voi refershToken nay da co ban tuong ung nao trong DB chua
                var checkToken = dbContext.stored_token.FirstOrDefault(x => x.RefreshToken == identity.JWTRefreshToken);
                if (checkToken == null) { return null; }
                if ((bool)checkToken.IsUsed! && (bool)checkToken.IsRevoked!) return null;

                // Check accessToken Id == refreshToken jwtId? => refreshToken co ung voi 1 accessToken ko
                var jti = checkValidate.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;
                if (checkToken.JwtID != jti) { return null; }

                // Da pass cac vong check, cap 1 luot token moi
                checkToken.IsRevoked = true; checkToken.IsUsed = true;
                dbContext.stored_token.Update(checkToken); await dbContext.SaveChangesAsync();
                User ngDung = dbContext.user.FirstOrDefault(x => x.Id == checkToken.UserID)!;
                return await GenerateTokenAsync(config, ngDung);
            } catch (Exception ex)
            {
                throw new AggregateException(ex);
            }
        }

        private static DateTime FromUnixTimeToUtcTime(long utcCurrDate)
        {
            // Tao 1 doi tuong Unix time tc
            DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // Add the current ticks amount, then return the time
            unixTime.AddSeconds(utcCurrDate).ToUniversalTime();
            return unixTime;
        }

        public async Task<JWTTokenIdentity> Authenication(JsonConfig config, LogInModel logIn)
        {
            User check = await dbContext.user.FirstOrDefaultAsync(x => x.UserName == logIn.UserName 
            && PasswordManagement.IsValidPassword(logIn.Password, x.Password)) ?? throw new InvalidDataException("Unknow this user");
            return await GenerateTokenAsync(config, check);
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
                return jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
