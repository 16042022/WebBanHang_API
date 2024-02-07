using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Domain.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomAnotation : Attribute
    {
        public string Description { set; get; }
        public CustomAnotation(string name) => Description = name;
    }
}
