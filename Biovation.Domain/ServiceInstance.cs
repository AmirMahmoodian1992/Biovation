using DataAccessLayerCore.Attributes;
using System;
using System.Net;

namespace Biovation.Domain
{
    public class ServiceInstance
    {
        public bool ChangeId;

        public ServiceInstance() {
        }
        public ServiceInstance(string id)
        {
            ChangeId = false;
            Id = id ?? Guid.NewGuid().ToString();
            if (!string.Equals(id,Id))
            {
                ChangeId = true;
            }
        }

        [Id]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Version { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public DateTime LastUpTime { get; set; } 
        public string Description { get; set; }
        }
}
