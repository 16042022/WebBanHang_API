using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Common
{
    public class MailContent
    {
        public string To { get; set; } = "";            // Địa chỉ gửi đến
        public string Subject { get; set; } = "";       // Chủ đề (tiêu đề email)
        public string Body { get; set; } = "";   // Nội dung (hỗ trợ HTML) của email
        // Tam thoi chua gui cac file media, stream
    }
}
