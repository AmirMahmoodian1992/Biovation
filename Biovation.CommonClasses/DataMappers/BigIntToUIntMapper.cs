using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DataAccessLayer;
using DataAccessLayer.Domain;
using DataAccessLayer.Extensions;

namespace Biovation.CommonClasses.DataMappers
{
    class BigIntToUIntMapper : IDataMapper
    {
        public object Map(IDataRecord dataRecord, PropertyInfo property, string columnPrefix = "")
        {
            if (property.PropertyType != typeof(uint))
            {
                throw new DataAccessException(01,
                    "Invalid property type for BigIntToUIntMapper, you have to use uint proprty type.");
            }
            if (dataRecord.HasColumn(columnPrefix + property.Name) && !dataRecord.IsDBNull(dataRecord.GetOrdinal(columnPrefix + property.Name)))
            {
                return Convert.ToUInt32(dataRecord[columnPrefix + property.Name]);
            }
            return null;
        }

        public object Map(DataRow dataRecord, PropertyInfo property, string columnPrefix = "")
        {
            return Convert.ToUInt32(0);
        }
    }
}
