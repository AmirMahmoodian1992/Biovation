using System;

namespace DataAccessLayerCore.Attributes
{
    /// <summary>
    /// Id of a Model. Id property used to merging composited result. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    public class IdAttribute:Attribute
    {

    }
}
