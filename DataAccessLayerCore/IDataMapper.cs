using System.Data;
using System.Reflection;

namespace DataAccessLayerCore
{
    /// <summary>
    /// An interface for mapping complex data types
    /// </summary>
    public interface IDataMapper
    {
        /// <summary>
        /// Map data from IDataReader to desired object.
        /// </summary>
        /// <param name="dataReader">data source</param>
        /// <param name="property">property wich need mapper</param>
        /// <returns>Mapped object</returns>
        object Map(IDataRecord dataRecord, PropertyInfo property,string columnPrefix = "");
    }
}
