using System;
using System.Data;
using System.Reflection;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Extentions;

namespace Biovation.Domain.DataMappers
{
    public class IntToLongMapper : IDataMapper
    {
        public object Map(IDataRecord dataRecord, PropertyInfo property, string columnPrefix = "")
        {
            if (property.PropertyType != typeof(long))
            {
                throw new DataAccessException(01,
                    "Invalid property type for IntToLongMapper, you have to use long property type.");
            }
            if (dataRecord.HasColumn(columnPrefix + property.Name) && !dataRecord.IsDBNull(dataRecord.GetOrdinal(columnPrefix + property.Name)))
            {
                return Convert.ToInt64(dataRecord[columnPrefix + property.Name]);
            }
            return null;
        }
    }
}
