using DataAccessLayerCore.Attributes;
using System;
using System.Threading;

namespace Biovation.Domain
{
    public class ServiceInstance
    {
        private readonly string _id;
        public bool changeId;
        public ServiceInstance(string id)
        {
            changeId = false;
            _id = id ?? Guid.NewGuid().ToString();
            if (!string.Equals(id,_id))
            {
                changeId = true;
            }
            new Timer(HealthCheck, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
        }

        [Id]
        public string Id
        {
            get
            {
                LastUpTime = DateTime.Now;
                return _id;
            }
        }
        public string Name { get; set; }
        public string Version { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public DateTime LastUpTime { get; private set; } = DateTime.Now;
        public string Description { get; set; }
        public bool Health { get; private set; } = true;



        private void HealthCheck(object? state)
        {
            if (DateTime.Now.Subtract(LastUpTime) > new TimeSpan(0, 2, 0))
            {
                Health = false;
            }
            else
            {
                Health = true;
            }
        }
    }
}
