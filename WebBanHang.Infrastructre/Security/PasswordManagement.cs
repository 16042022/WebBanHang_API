using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.UseCase.Users_Admin;


namespace WebBanHang.Infrastructre.Security
{
    public class PasswordManagement
    {
        public static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        public static bool IsValidPassword (string password, string DbPwd)
        {
            return BCrypt.Net.BCrypt.Verify(password, DbPwd);
        }
    }
}
