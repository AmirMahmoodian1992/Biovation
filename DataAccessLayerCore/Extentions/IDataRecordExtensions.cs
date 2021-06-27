using System;
using System.Data;

// ReSharper disable once CheckNamespace
namespace DataAccessLayerCore.Extentions
{
    /// <summary>
    /// Some required extenstions on IDataRecord
    /// </summary>
    public static class IDataRecordExtensions
    {

        /// <summary>
        /// Check whether the record has any propery with the name columnName or not.
        /// </summary>
        /// <param name="dr">Our lovely DateRecord</param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (var i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Map a record to a specific type which it's type determined by generic type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <returns></returns>
        public static T MapType<T>(this IDataRecord record) where T : class, new()
        {
            var objT = Activator.CreateInstance<T>();
            foreach (var property in typeof(T).GetProperties())
            {
                if (record.HasColumn(property.Name) && !record.IsDBNull(record.GetOrdinal(property.Name)))
                    property.SetValue(objT, record[property.Name]);
            }
            return objT;
        }
    }

}
