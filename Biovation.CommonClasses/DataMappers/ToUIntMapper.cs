using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Extentions;

namespace Biovation.CommonClasses.DataMappers
{
    public class ToUIntMapper : IDataMapper
    {
        public object Map(IDataRecord dataRecord, PropertyInfo property, string columnPrefix = "")
        {
            //if (property.PropertyType != typeof(long))
            //{
            //    throw new DataAccessException(01,
            //        "Invalid property type for IntToLongMapper, you have to use long property type.");
            //}
            if (dataRecord.HasColumn(columnPrefix + property.Name) && !dataRecord.IsDBNull(dataRecord.GetOrdinal(columnPrefix + property.Name)))
            {
                return Convert.ToUInt32(dataRecord[columnPrefix + property.Name]);
            }
            return null;
        }
    }
    public class ToInt64Mapper : IDataMapper
    {
        public object Map(IDataRecord dataRecord, PropertyInfo property, string columnPrefix = "")
        {
            //if (property.PropertyType != typeof(long))
            //{
            //    throw new DataAccessException(01,
            //        "Invalid property type for IntToLongMapper, you have to use long property type.");
            //}
            if (dataRecord.HasColumn(columnPrefix + property.Name) && !dataRecord.IsDBNull(dataRecord.GetOrdinal(columnPrefix + property.Name)))
            {
                return Convert.ToInt64(dataRecord[columnPrefix + property.Name]);
            }
            return null;
        }
    }
}
