using System;

namespace DataAccessLayerCore.Attributes
{
    /// <summary>
    /// It's Flag which determine default value for a model.
    /// Default value is a property which can stand as the indicator for an instance.
    /// It's a good solution as an alternative for two level Compositions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultAttribute : Attribute
    {

    }
}
