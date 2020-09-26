using DataAccessLayerCore.Attributes;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Extentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace DataAccessLayerCore.Repositories
{
    /// <summary>
    /// A Generic Repository which is responsible for calling SqlCommand and mapping it's result.
    /// </summary>
    public class GenericRepository
    {

        private IConnectionFactory _connectionFactory;

        public GenericRepository() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionFactory">IConnectionFactory which is responsible for creating connection to database</param>
        public GenericRepository(DatabaseConnectionInfo connectionInfo)
        {
            _connectionFactory = new DbConnectionFactory(connectionInfo);
        }

        /// <summary>
        /// Execute specified ADO.Net SqlCommand and map it's result to Result<List<T>>.
        /// </summary>
        /// <typeparam name="T">The coresponding object type for mapping</typeparam>
        /// <param name="commandText">SP name or SQL query</param>
        /// <param name="parameters">List of parameters</param>
        /// <param name="commandType">Type of command(CommandType.StoredProcedure by default))</param>
        /// <param name="timeOut">Amount of millisecond which DAL waits for response(3000 by default)</param>
        /// <param name="fetchCompositions">Fetch nesting object in true case.(false by default)</param>
        /// <param name="compositionDepthLevel">Determine how much depth should be considered in fetching compositions</param>
        /// <returns>Mapped result to Result<List<T>></returns>
        public Result<List<T>> ToResultList<T>(string commandText, List<SqlParameter> parameters = null,
            CommandType commandType = CommandType.StoredProcedure, int timeOut = 3000, bool fetchCompositions = false,
            int compositionDepthLevel = 1, DatabaseConnectionInfo connectionInfo = null)
        {
            try
            {
                Result<List<T>> result;
                //if (_connectionFactory is null)
                //    _connectionFactory = ConnectionHelper.GetConnection(connectionName.ToUpper());
                _connectionFactory = connectionInfo is null ? _connectionFactory : ConnectionHelper.GetConnection(connectionInfo);

                using (var tmpContext = new DbContext(_connectionFactory))
                {
                    using (var tmpCommand = tmpContext.CreateCommand())
                    {
                        tmpCommand.CommandTimeout = timeOut;
                        tmpCommand.CommandText = commandText;

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                tmpCommand.Parameters.Add(param);
                            }
                        }
                        tmpCommand.CommandType = commandType;

                        using (var reader = tmpCommand.ExecuteReader())
                        {
                            result = fetchCompositions ? MapToResultListCompositionEnabled<T>(reader, compositionDepthLevel) : MapToResultList<T>(reader);
                            while (reader.NextResult()) { }
                        }
                    }
                }

                if (result != null) return result;

                result = new Result<List<T>> { Data = new List<T>() };
                return result;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(500, "Internal Database Error", $"({ex.Class}:{ex.State}): {ex.Message}");
            }
        }

        /// <summary>
        /// Map result with composition enabled. 
        /// </summary>
        /// <typeparam name="T">The coresponding object type for mapping</typeparam>
        /// <param name="reader">Reader of command result.</param>
        /// <param name="maxDepthLevel">Determine how much depth should be considered in fetching compositions.</param>
        /// <returns></returns>
        private Result<List<T>> MapToResultListCompositionEnabled<T>(IDataReader reader, int maxDepthLevel)
        {

            Result<List<T>> result = null;
            var entities = new List<T>();
            var oneToManyProperties = GetPropertiesByAttribute(typeof(T), typeof(OneToManyAttribute));
            var oneToOneProperties = GetPropertiesByAttribute(typeof(T), typeof(OneToOneAttribute));

            if (!oneToManyProperties.Any() && !oneToOneProperties.Any())
            {
                throw new DataAccessException(105, "No relation has been found for this class, Add some or disable fetchCompositions!");
            }
            while (reader.Read())
            {
                if (result == null)
                {
                    result = new Result<List<T>>();
                    Map(reader, typeof(Result<List<T>>), result, depth: 0, maxDepth: 0, columnPrefix: "e_");
                    result.Data = entities;
                }

                Map(reader, typeof(List<T>), result.Data, maxDepth: maxDepthLevel);
            }
            return result;
        }

        /// <summary>
        /// Map result without considering it's nesting objects.
        /// </summary>
        /// <typeparam name="T">The coresponding object type for mapping</typeparam>
        /// <param name="reader">Reader of command result.</param>
        /// <returnsResult<List<T>>></returns>
        private Result<List<T>> MapToResultList<T>(IDataReader reader)
        {
            Result<List<T>> result = null;
            var entities = new List<T>();
            while (reader.Read())
            {
                if (result == null)
                {
                    result = new Result<List<T>>();
                    Map(reader, typeof(Result<List<T>>), result, depth: 0, maxDepth: 0, columnPrefix: "e_");
                    result.Data = entities;
                }

                if (typeof(T) == typeof(string))
                    entities.Add((T)reader[0]);
                else
                {
                    if (typeof(T).IsPrimitive)
                    {
                        try
                        {
                            var value = Convert.ChangeType(reader[0], typeof(T));
                            entities.Add((T)value);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }

                    else
                    {
                        var entity = (T)Activator.CreateInstance(typeof(T));
                        entities.Add(entity);
                        Map(reader, typeof(T), entity, 0, 0);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Map a single record to an specified type.
        /// </summary>
        /// <param name="record">Result record</param>
        /// <param name="type">The Type which result should be casted to it</param>
        /// <param name="maxDepth">>Determine how much depth should be considered in fetching compositions.</param>
        /// <param name="columnPrefix">Column prefix in cases which properties needs prefix to be found in result</param>
        /// <param name="instance">Real non-null instance of object which the method will fill it</param>
        /// <param name="depth">The current depth of recursion</param>
        /// <returns></returns>
        private void Map(IDataRecord record, Type type, object instance, int depth = 0, int maxDepth = 1, string columnPrefix = "")
        {
            try
            {
                if (instance == null) throw new ArgumentNullException(nameof(instance));
                var objT = instance;

                if (type.IsPrimitive)
                {
                    instance = Convert.ChangeType(record[0], type);
                }

                else if (type.IsGenericType && depth < maxDepth && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                {
                    var idProperty = GetPropertiesByAttribute(type.GenericTypeArguments.Single(), typeof(IdAttribute)).FirstOrDefault();
                    if (idProperty == null)
                    {
                        throw new DataAccessException(104, $"[Id] attribute does not specified for any property of {type.GenericTypeArguments.Single().FullName}!");
                    }
                    object id;
                    if (record.HasColumn(columnPrefix + idProperty.Name))
                    {
                        if (!record.IsDBNull(record.GetOrdinal(columnPrefix + idProperty.Name)))
                        {
                            id = record[columnPrefix + idProperty.Name];
                        }
                        else
                        {
                            instance = null;
                            return;
                        }
                    }
                    else
                    {
                        // throw new DataAccessException(106, $"{columnPrefix}{idProperty.Name} column in dataset not specified!");
                        return;
                    }

                    var listobjT = (IList)instance;
                    object entity = null;
                    var genericType = type.GenericTypeArguments.Single();
                    if (listobjT.Count > 0)
                    {

                        for (var i = 0; i < listobjT.Count; i++)
                        {
                            var item = Convert.ChangeType(listobjT[i], genericType);
                            if (!string.Equals(idProperty.GetValue(item).ToString(), id.ToString())) continue;
                            entity = item;
                            break;
                        }
                    }
                    if (entity == null)
                    {
                        entity = Activator.CreateInstance(genericType);
                        listobjT.Add(entity);
                    }

                    Map(record, genericType, entity, depth, maxDepth, columnPrefix);

                }
                else
                {
                    foreach (var property in type.GetProperties())
                    {
                        var propertyName = columnPrefix + property.Name;
                        var propertyType = property.PropertyType;
                        if (Attribute.IsDefined(property, typeof(DataMapperAttribute)))
                        {
                            var dataMapperType = (DataMapperAttribute)property
                                .GetCustomAttributes(typeof(DataMapperAttribute), true).FirstOrDefault();
                            if (dataMapperType?.Mapper?.GetInterface("IDataMapper") != null)
                            {
                                var dataMapper = (IDataMapper)Activator.CreateInstance(dataMapperType.Mapper);
                                property.SetValue(objT, dataMapper.Map(record, property, columnPrefix));
                            }
                            else
                            {
                                throw new DataAccessException(05,
                                    "Invalid or null mapper has defined for DataMapperAttribute");
                            }
                        }
                        else if (depth < maxDepth && Attribute.IsDefined(property, typeof(OneToOneAttribute)))
                        {
                            var propertyIdOfNest =
                                GetPropertiesByAttribute(property.PropertyType, typeof(IdAttribute))
                                    .FirstOrDefault();
                            if (propertyIdOfNest == null)
                                throw new DataAccessException(05,
                                    $"[Id] attribute does not specified for any property of {property.PropertyType.FullName}!");
                            var nestedIdPropertyColName = columnPrefix + property.Name + "_" + propertyIdOfNest.Name;
                            if (record.HasColumn(nestedIdPropertyColName) && !record.IsDBNull(record.GetOrdinal(nestedIdPropertyColName)))
                            {
                                if (property.GetValue(objT) == null)
                                {
                                    property.SetValue(objT, Activator.CreateInstance(propertyType));
                                }

                                Map(record, propertyType, property.GetValue(objT), depth + 1, maxDepth,
                                    columnPrefix + property.Name + "_");
                            }
                        }
                        else if (depth < maxDepth && Attribute.IsDefined(property, typeof(OneToManyAttribute)))
                        {

                            if (property.GetValue(objT) == null)
                            {
                                property.SetValue(objT, Activator.CreateInstance(propertyType));
                            }

                            Map(record, propertyType, property.GetValue(objT), depth + 1, maxDepth,
                                columnPrefix + property.Name + "_");

                        }
                        else
                        {
                            if (!record.HasColumn(propertyName)) continue;
                            //var innerPropertyType = record[propertyName].GetType();
                            property.SetValue(objT,
                                record.IsDBNull(record.GetOrdinal(propertyName)) ? null : Convert.ChangeType(record[propertyName], propertyType), null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        /// <summary>
        /// Get Properties of an special type using attribute. 
        /// </summary>
        /// <param name="entity">The Type which it's properties going to be searched.</param>
        /// <param name="attribute">The attribute which properties should looked for it</param>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetPropertiesByAttribute(Type entity, Type attribute)
        {
            var props = entity.GetProperties().Where(
                prop => Attribute.IsDefined(prop, attribute));
            return props;
        }

        /// <summary>
        /// Get all of attributeless properties in a special type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GetAttributelessProperties(Type type)
        {
            return type.GetProperties().Where(
                prop => (!Attribute.IsDefined(prop, typeof(OneToOneAttribute)) && !Attribute.IsDefined(prop, typeof(OneToManyAttribute))));
        }

    }
}
