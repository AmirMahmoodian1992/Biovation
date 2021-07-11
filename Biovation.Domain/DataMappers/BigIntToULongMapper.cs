using System;
using System.Data;
using System.Reflection;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Extentions;

namespace Biovation.Domain.DataMappers
{
    public class BigIntToULongMapper : IDataMapper
    {
        public object Map(IDataRecord dataRecord, PropertyInfo property, string columnPrefix = "")
        {
            if (property.PropertyType != typeof(ulong))
            {
                throw new DataAccessException(01,
                    "Invalid property type for BigIntToULongMapper, you have to use ulong property type.");
            }
            if (dataRecord.HasColumn(columnPrefix + property.Name) && !dataRecord.IsDBNull(dataRecord.GetOrdinal(columnPrefix + property.Name)))
            {
                return Convert.ToUInt64(dataRecord[columnPrefix + property.Name]);
            }
            return null;
        }
    }
}
