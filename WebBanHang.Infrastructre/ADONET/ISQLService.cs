using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Infrastructre.ADONET
{
    public interface ISQLService
    {
        MySqlParameter CreateOutputParameter(string name, SqlDbType type);
        MySqlParameter CreateOutputParameter(string name, SqlDbType type, int size);

        (int, string) ExecuteNonQuery(string sqlObjectName,
            params MySqlParameter[] parameters);
        Task<(int, string)> ExecuteNonQueryAsync(string sqlObjectName,
            params MySqlParameter[] parameters);
        (DataTable, string) FillDataTable(string sqlObjectName,
            params MySqlParameter[] parameters);
        Task<(DataTable, string)> FillDataTableAsync(string sqlObjectName,
            params MySqlParameter[] parameters);
    }
}
