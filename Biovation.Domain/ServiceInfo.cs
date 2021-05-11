using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class ServiceInfo
    {
        [Id]
        public string Uuid { get; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime LastUpTime { get; set; } = DateTime.Now;
    }
}
