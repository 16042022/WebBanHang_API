using MySql.Data.MySqlClient;

namespace WebBanHang.Infrastructre.ADONET
{
    public class MyConnectionKey
    {
        public static string ConnectionKey => Environment.GetEnvironmentVariable("MYSQLCNNSTR_cnnKey")!;
    }

    public class DBHook : IDisposable
    {
        private MySqlConnection _connection;

        public DBHook()
        {
            _connection = new MySqlConnection(MyConnectionKey.ConnectionKey);
        }

        public MySqlConnection HookConnect => _connection;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool _isDispose)
        {
            if (_isDispose)
            {
                _connection?.Dispose();
            }
        }
    }
}