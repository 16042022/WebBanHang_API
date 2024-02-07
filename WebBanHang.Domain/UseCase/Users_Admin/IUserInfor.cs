using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Domain.UseCase.Users_Admin
{
    public interface IUserInfor
    {
        public User FromCustomerInfo(Customer entity);
        public bool IsValidPassword(string inPwd, string dbPwd);
        public string GenerateRefreshPwdToken();
        // Group of user picture upload & management
    }
}
