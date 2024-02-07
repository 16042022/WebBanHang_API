using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.UseCase.Users_Admin;
using WebBanHang.Infrastructre.Security;

namespace WebBanHang.Infrastructre.User_Admin
{
    public class UserManagement : IUserInfor
    {
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

        public string GenerateRefreshPwdToken()
        {
            throw new NotImplementedException();
        }

        public bool IsValidPassword(string inPwd, string dbPwd)
        {
            return PasswordManagement.IsValidPassword(inPwd, dbPwd);
        }
    }
}
