using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.UseCase.Users_Admin;

namespace WebBanHang.Infrastructre.User_Admin
{
    public class EmailSender : IEmailSender
    {
        private readonly MailPortSetting _setting;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<MailPortSetting> setting, ILogger<EmailSender> logger)
        {
            _setting = setting.Value;
            _logger = logger;
        }
        public async Task SendMail(MailContent mailContent)
        {
            // Tao Mail Message obj
            MailMessage _message = new MailMessage(
                from: _setting.Mail,
                to: mailContent.To,
                subject: mailContent.Subject,
                body: mailContent.Body
            );
            var mimeMessege = (MimeMessage)_message;
            // Ket noi bang SMTP of MIME library
            using var _client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                _client.Connect(_setting.Server, _setting.Port, SecureSocketOptions.Auto);
                _client.Authenticate(_setting.Mail, _setting.Password);
                await _client.SendAsync(mimeMessege);
                //_client.Disconnect(true);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                if (!Directory.Exists("mailsave"))
                {
                    Directory.CreateDirectory("mailsave");
                }
                var emailsavefile = string.Format(@"mailsave/{0}.eml", Guid.NewGuid());
                await mimeMessege.WriteToAsync(emailsavefile);
                _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                _logger.LogError(ex.Message);
            }
        }
    }
}
