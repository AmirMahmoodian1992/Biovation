using System;
using System.Data;
using System.Reflection;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Extentions;

namespace Biovation.Domain.DataMappers
{
    public class IntToUshortMapper : IDataMapper
    {
        public object Map(IDataRecord dataRecord, PropertyInfo property, string columnPrefix = "")
        {
            if (property.PropertyType != typeof(ushort))
            {
                throw new DataAccessException(01,
                    "Invalid property type for IntToUshortMapper, you have to use ushort property type.");
            }
            if (dataRecord.HasColumn(columnPrefix + property.Name) && !dataRecord.IsDBNull(dataRecord.GetOrdinal(columnPrefix + property.Name)))
            {
                return Convert.ToUInt16(dataRecord[columnPrefix + property.Name]);
            }
            return null;
        }
    }
}
