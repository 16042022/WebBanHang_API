using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Common
{
    public class MailPortSetting
    {
        public string Server { get; set; } = ""; // Host = Server name of google
        public int Port { get; set; } = 0; // Cong tiep nhan cuar google ung voi pt SMTP
        public string Password { get; set; } = ""; // App password from google
        public string Mail { get; set; } = ""; // Mail = username mail of google
        public string DisplayName { get; set; } = ""; // Ten hien thi - OPtions
    }
}
