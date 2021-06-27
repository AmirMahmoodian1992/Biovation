// ReSharper disable once CheckNamespace
namespace DataAccessLayerCore.Domain
{
    public class DatabaseConnectionInfo
    {
        public string ProviderName { get; set; } = "System.Data.SqlClient";
        public string InitialCatalog { get; set; }
        public string DataSource { get; set; }
        public string WorkstationId { get; set; }
        public string Parameters { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Convert data to a single connection string
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            const string template = "workstation id={0};Data Source={1};Initial Catalog={2};User ID={3};Password={4};{5}";
            return string.Format(template, WorkstationId, DataSource, InitialCatalog, UserId, Password,Parameters);
        }
    }
}
