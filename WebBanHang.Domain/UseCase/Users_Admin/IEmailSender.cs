using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Domain.UseCase.Users_Admin
{
    public interface IEmailSender
    {
        public Task SendMail(MailContent mailContent);
    }
}
