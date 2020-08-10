using System.Data;

namespace DataAccessLayerCore
{
    /// <summary>
    /// Interface for classes which designed for creating database connection
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// The method which create database connection
        /// </summary>
        /// <returns></returns>
        IDbConnection Create();
    }
}
