using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Model.Account;

namespace WebBanHang.Domain.UseCase.Others
{
    public interface IUserSerrvice
    {
        Task<AuthenicationRespone> Authenticate(LogInModel model, string ipAddress, JsonConfig config);
        Task<AuthenicationRespone> RefreshToken(string token, string ipAddress);
        Task RevokeToken(string token, string ipAddress);
        Task<IEnumerable<AccountRespone>> GetAll(bool isCustomer);
        Task<AccountRespone> GetById(int id);
        Task Register(UserRegisterModel model, string origin, bool isEmployee);
        Task VerifyEmail(string requestToken);
        Task ForgotPasswordProcess(string email, string origin);
        Task<Users> ValidateReseToken(string requestToken);
        Task ResetPasswordProcess(ResetPasswordRequest resetModel);
        Task<AccountRespone> CreateAccount(CreateRequest model);
        Task<AccountRespone> UpdateAccount(int ID, EditAccountRequest editAccountRequest);
        Task DeleteAccount(int AccID);
    }
}
