using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DataAccessLayerCore.Domain;

// ReSharper disable once CheckNamespace
namespace DataAccessLayerCore
{
    /// <summary>
    /// Class for creating connection to database.
    /// </summary>
    public class DbConnectionFactory : IConnectionFactory
    {
        //private readonly DbProviderFactory _provider;
        private readonly string _connectionString;

        public DbConnectionFactory()
        {
            _connectionString = "";
        }

        /// <summary>
        /// Constructor which accept connection info and set configurations required for creating connection
        /// </summary>
        /// <param name="connectionInfo"></param>
        public DbConnectionFactory(DatabaseConnectionInfo connectionInfo)
        {
            _connectionString = connectionInfo.GetConnectionString();
        }


        /// <summary>
        /// Create a new connection using specified configuration.
        /// </summary>
        /// <returns></returns>
        public IDbConnection Create()
        {
            //var connection = _provider.CreateConnection();
            var connection = new SqlConnection(_connectionString);
            if (connection == null)
                throw new ConfigurationErrorsException("Failed to create a connection using the specified configurations");
            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }
       
    }

}
