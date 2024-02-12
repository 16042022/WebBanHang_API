using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.DTO;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Domain.UseCase.Others
{
    public interface IUserSerrvice
    {
        Task<AuthenicationRespone> Authenticate(LogInModel model, string ipAddress, JsonConfig config);
        Task<AuthenicationRespone> RefreshToken(string token, string ipAddress);
        Task RevokeToken(string token, string ipAddress);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
    }
}
