using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Services.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnoymousAtt : Attribute
    {
    }
}
