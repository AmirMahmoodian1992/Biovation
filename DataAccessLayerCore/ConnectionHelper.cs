using DataAccessLayerCore.Domain;

namespace DataAccessLayerCore
{
    public static class ConnectionHelper
    {
        public static IConnectionFactory GetConnection(DatabaseConnectionInfo connectionInfo)
        {
            return new DbConnectionFactory(connectionInfo);
        } 
    }
}