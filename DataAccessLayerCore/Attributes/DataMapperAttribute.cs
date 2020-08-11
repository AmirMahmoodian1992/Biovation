using System;

namespace DataAccessLayerCore.Attributes
{
    /// <summary>
    /// Date Mapper which enable use to write Custom Map
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = true)]
    public class DataMapperAttribute : Attribute
    {
        /// <summary>
        /// Mapper Class which is an implementation of IUnitOfWork
        /// </summary>
        public Type Mapper;
    }
}
