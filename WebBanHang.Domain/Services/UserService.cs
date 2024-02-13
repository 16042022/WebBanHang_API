using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Enums;
using WebBanHang.Domain.Model;
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
        private readonly IRepository<Customer> customerRepo;
        private readonly IMapper _mapper;
        private readonly IEmailSender emailSender;

        public UserService(IAuthenication authenication, IUserInfor infor, 
            IOptions<JsonConfig> config, IRepository<User> repository, IRepository<Customer> customerRepo, 
            IMapper mapper, IEmailSender emailSender)
        {
            this.authenication = authenication;
            this.userInfor = infor;
            this.config = config.Value;
            userRepo = repository;
            this.customerRepo = customerRepo;
            _mapper = mapper;
            this.emailSender = emailSender;
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

        public async Task<IEnumerable<Customer>> GetAllCustomer()
        {
            return await customerRepo.GetAll();
        }

        public async Task Register(UserRegisterModel model, string origin)
        {
            User? check = await userRepo.GetByName(model.Email);
            if (check != null)
            {
                SendAlreadyRegisterEmail(model.Email, origin);
            }
            // Dang ky thong tin
            Customer cus = _mapper.Map<UserRegisterModel, Customer>(model);
            // Chuyen doi sang User
            check = userInfor.FromCustomerInfo(cus);
            var list = await userRepo.GetAll(); int priority = list.Count();
            check.RoleID = priority == 0? (int)UserRole.Admin: check.RoleID;
            check.VerifyToken = GenerateVerifyToken(check);
            // Luu user nay
            await userRepo.Add(check);
            // Gui mail ve dia chi nay
            SendVerificationEmail(check, origin);
        }

        private async void SendAlreadyRegisterEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
            } else
            {
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";
            }
            var html = $@"<h4>Email Already Registered</h4>
                        <p>Your email <strong>{email}</strong> is already registered.</p>
                        {message}";
            MailContent context = new MailContent()
            {
                To = email,
                Subject = "Sign-up Verification API - Email Already Registered",
                Body = html
            };
            await emailSender.SendMail(context);
        }

        private async void SendVerificationEmail(User check, string origin)
        {
            // Tao body message
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                // origin exists if request sent from browser single page app (e.g. Angular or React)
                var verifyUrl = $"{origin}/user/verify-email?=token={check.VerifyToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                        <p><a href = ""{verifyUrl}"">{verifyUrl}</a></p>";
            } else
            {
                // To test through API application test - POSTMAN, ...
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                            <p><code>{check.VerifyToken}</code></p>";
            }
            // Tao MailContext obj
            MailContent context = new MailContent()
            {
                To = check.Email,
                Subject = "Sign-up Verification API - Verify Email",
                Body = message
            };
            await emailSender.SendMail(context);
        }

        private string GenerateVerifyToken(User check)
        {
            string token; bool isExistance;
            do
            {
                token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
                isExistance = userInfor.CheckVerifyToken(token);
            } while (isExistance);
            return token;
        }

        public async Task VerifyEmail(string requestToken)
        {
           await userInfor.ValidationVerifyToken(requestToken);
        }

        public async Task ForgotPasswordProcess(string email)
        {
            /*
             Input: Request contain acc emial in request body
            Output: a password reset email is sent to the email address of the account
            (The email contains a single use reset token that is valid for one day.)
             */
            User? check = await userRepo.GetByName(email) ?? throw new InvalidDataException("Email account is not valid");
            // Generate reset token
            check.ResetPwdToken = GenerateResetPwdToken(check);
            check.ResetPwdExpires = DateTime.Now.AddDays(1);
            // Save this user
            await userRepo.Update(check);
            // Send mail
            SendVerifyResetPassword(check, email);
        }

        private void SendVerifyResetPassword(User check, string email)
        {
            throw new NotImplementedException();
        }

        private string GenerateResetPwdToken(User check)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateReseToken(string requestToken)
        {
            /*
               Input: Request contain reset token (in request body)
               Output: A message is returned to indicate if the token is valid or not.
            */
            User? check = await userInfor.GetUserByResetToken(requestToken) ?? 
                throw new AggregateException("This token is not link to any account");
            // Check if this token is valid period
            if (DateTime.Now > check.ResetPwdExpires) 
            { throw new AggregateException("This token is over actived timing"); }
            return true;
        }

        public Task ResetPasswordProcess(ResetPasswordRequest resetModel)
        {
            throw new NotImplementedException();
        }
    }
}
