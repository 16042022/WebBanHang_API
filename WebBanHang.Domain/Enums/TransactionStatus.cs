using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Enums
{
    public enum TransactionStatus
    {
        Done = 1,
        Not_paid_yet = 2,
        Postponed = 3,
        In_process = 4
    }
}
