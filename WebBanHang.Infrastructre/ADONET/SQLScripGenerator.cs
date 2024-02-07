using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Infrastructre.ADONET
{
    public class SQLScripGenerator<T> : ISQLScript<T> where T : class
    {
        private static string dbName = "";
        // Method return name of database
        private static string GetDatabaseName()
        {
            var connectString = Environment.GetEnvironmentVariable("MYSQLCNNSTR_cnnKey")!;
            string[] res = connectString.Split(";");
            if (res.Length > 0)
            {
                for (int i = 0; i < res.Length; i++)
                {
                    if (res[i].Contains("database")) dbName = res[i].Split("=")[1];
                }
            }
            return dbName;
        }

        public string DeleteScript(string Table, int ID)
        {
            string dbName = GetDatabaseName(), noteKey;
            if (Table == "Product") noteKey = "ProductID";
            else noteKey = "ID";
            string SQL = $"DELETE FROM {dbName + "." + Table} WHERE {noteKey} = {ID}";
            return SQL;
        }

        public string GetScript(string Table, T obj, int id)
        {
            string dbName = GetDatabaseName(), noteKey;
            if (Table == "Product") noteKey = "ProductID";
            else noteKey = "ID";
            string SQL = $"SELECT *  FROM {dbName + "." + Table} WHERE {noteKey} = {id}";
            return SQL;
        }

        public string GetScriptAll(string Table)
        {
            string dbName = GetDatabaseName();
            string SQL = $"SELECT *  FROM {dbName + "." + Table}";
            return SQL;
        }

        public string InsertScript(string Table, T obj)
        {
            string dbName = GetDatabaseName();
            string SQL = $"INSERT INTO {dbName + "." + Table} (";
            // Lap qua cac thuoc tinh trong T, chon ra thuoc tinh danh dau la Insertable
            foreach (PropertyInfo item in obj.GetType().GetProperties())
            {
                if (item.Name != "ID" || item.Name != "ProductID")
                {
                    foreach (Attribute filter in item.GetCustomAttributes(false).Cast<Attribute>())
                    {
                        if (filter is CustomAnotation _annotae && _annotae.Description == "Insertable")
                        {
                            SQL += $"{item.Name}, ";
                        }
                    }
                }
            }
            // Xoa dau "," o cuoi, va them ")"
            SQL = SQL.Trim().Replace(",", ")\n");
            // Them cac gia tri can insert vao
            SQL += $"VALUE (";
            foreach (PropertyInfo item in obj.GetType().GetProperties())
            {
                var Value = item.GetValue(obj, null);
                if (Value is not null) { SQL += $"{Value}, "; }
            }
            SQL = SQL.Trim().Replace(",", ");");
            return SQL;
        }

        public string UpdateScript(string Table, T obj)
        {
            string dbName = GetDatabaseName();
            string subSql = $"WHERE ";
            string SQL = $"UPDATE {dbName + "." + Table} \n SET ";
            // Lap qua cac thuoc tinh, chon ra cac thuoc tinh dang co gia tri
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name != "ID" || propertyInfo.Name != "ProductID")
                {
                    var Value = propertyInfo.GetValue(obj, null);
                    if (Value is not null)
                    {
                        SQL += $"{propertyInfo.Name} = {Value}, ";
                    }
                }
                else
                {
                    subSql += $"{propertyInfo.Name} = {propertyInfo.GetValue(obj, null)};";
                }
            }
            SQL = SQL.Trim().Replace(",", "\n");
            SQL += subSql;
            return SQL;
        }
    }
}
