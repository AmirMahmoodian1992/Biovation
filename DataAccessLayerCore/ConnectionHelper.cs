using DataAccessLayerCore.Domain;

namespace DataAccessLayerCore
{
    public static class ConnectionHelper
    {
        public static IConnectionFactory GetConnection()
        {
            return new DbConnectionFactory();
        }

        public static IConnectionFactory GetConnection(DatabaseConnectionInfo connectionInfo)
        {
            return new DbConnectionFactory(connectionInfo);
        } 
    }
}