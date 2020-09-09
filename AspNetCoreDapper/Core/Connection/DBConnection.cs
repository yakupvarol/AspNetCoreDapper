using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AspNetCoreDapper.Dapper
{
    public class DBConnection : IDBConnection
    {
        private readonly IDbConnection _connection;
        private readonly IConfiguration configuration;
        
        public DBConnection(IConfiguration Configuration)
        {
            configuration = Configuration;
            _connection = new SqlConnection(configuration.GetSection("ConnectionStrings")["Default"]);
        }

        public IDbConnection DB
        {
            get
            {
                if (_connection.State == ConnectionState.Broken)
                    _connection.Close();

                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                return _connection;
            }
        }

        private bool _disposed = false;

        ~DBConnection() =>
            Dispose();

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

                if (_connection != null)
                    _connection.Dispose();

                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
