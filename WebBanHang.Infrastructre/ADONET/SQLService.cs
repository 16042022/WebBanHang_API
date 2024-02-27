using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;

namespace WebBanHang.Infrastructre.ADONET
{
    public class SQLService : ISQLService
    {
        public MySqlParameter CreateOutputParameter(string name, SqlDbType type)
        {
            MySqlParameter parameter = new MySqlParameter(name, type);
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }

        public MySqlParameter CreateOutputParameter(string name, SqlDbType type, int size)
        {
            MySqlParameter parameter = new MySqlParameter(name, (MySqlDbType)type, size);
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }

        public (int, string) ExecuteNonQuery(string sqlObjectName, 
            params MySqlParameter[] parameters)
        {
            int res = -1;
            string message = string.Empty;
            try
            {
                using (DBHook hook = new DBHook())
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlObjectName, hook.HookConnect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 240;
                        cmd.Parameters.AddRange(parameters);
                        hook.HookConnect.Open();
                        res = cmd.ExecuteNonQuery();
                        hook.HookConnect.Close();
                    }
                }
            } catch (Exception ex)
            {
                message =ex.Message;
                throw new AggregateException(ex);
            }
            return (res, message);
        }

        public async Task<(int, string)> ExecuteNonQueryAsync(string sqlObjectName, 
            params MySqlParameter[] parameters)
        {
            int res = -1;
            string message = string.Empty;
            try
            {
                using (DBHook hook = new DBHook())
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlObjectName, hook.HookConnect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 240;
                        cmd.Parameters.AddRange(parameters);
                        await hook.HookConnect.OpenAsync();
                        res = cmd.ExecuteNonQuery();
                        await hook.HookConnect.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                throw new AggregateException(ex);
            }
            return (res, message);
        }

        public (DataTable, string) FillDataTable(string sqlObjectName, 
            params MySqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            string message = string.Empty;
            try
            {
                using (DBHook hook = new DBHook())
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlObjectName, hook.HookConnect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 240;
                        cmd.Parameters.AddRange(parameters);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        hook.HookConnect.Open();
                        adapter.Fill(dt);
                        hook.HookConnect.Close();
                    }
                }
            } catch (Exception ex)
            {
                message = ex.Message;
                throw new AggregateException(ex);
            }
            return (dt, message);
        }

        public async Task<(DataTable, string)> FillDataTableAsync(string sqlObjectName, 
            params MySqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            string message = string.Empty;
            try
            {
                using (DBHook hook = new DBHook())
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlObjectName, hook.HookConnect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 240;
                        cmd.Parameters.AddRange(parameters);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        await hook.HookConnect.OpenAsync();
                        adapter.Fill(dt);
                        await hook.HookConnect.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                throw new AggregateException(ex);
            }
            return (dt, message);
        }
    }
}
