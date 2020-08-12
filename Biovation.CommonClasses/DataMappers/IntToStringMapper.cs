using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Extentions;
using System;
using System.Data;
using System.Reflection;

namespace Biovation.CommonClasses.DataMappers
{
    public class IntToStringMapper : IDataMapper
    {
        public object Map(IDataRecord dataRecord, PropertyInfo property, string columnPrefix = "")
        {
            if (property.PropertyType != typeof(string))
            {
                throw new DataAccessException(01,
                    "Invalid property type for IntToLongMapper, you have to use long property type.");
            }
            if (dataRecord.HasColumn(columnPrefix + property.Name) && !dataRecord.IsDBNull(dataRecord.GetOrdinal(columnPrefix + property.Name)))
            {
                return Convert.ToString(dataRecord[columnPrefix + property.Name]);
            }
            return null;
        }
    }
}
