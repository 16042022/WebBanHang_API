using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Infrastructre.ADONET
{
    public interface ISQLScript<T> where T : class
    {
        public string InsertScript(string Table, T obj);
        public string UpdateScript(string Table, T obj);
        public string DeleteScript(string Table, int ID);
        public string GetScript(string Table, T obj, int id);
        public string GetScriptAll(string Tale);
    }
}
