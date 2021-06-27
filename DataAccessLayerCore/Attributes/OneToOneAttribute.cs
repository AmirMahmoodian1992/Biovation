using System;
using DataAccessLayerCore.Domain;

namespace DataAccessLayerCore.Attributes
{
    /// <summary>
    /// A flag for the properties which need One-To-Many Relation with other models.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    public class OneToOneAttribute:Attribute
    {
        /// <summary>
        /// Fetch type which can be addressed by FetchType enum.
        /// </summary>
        public FetchType FetchType;
    }
}
