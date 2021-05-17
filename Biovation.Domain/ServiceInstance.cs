using System;
using System.Threading;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class ServiceInstance
    {
        public ServiceInstance()
        {
            new Timer(HealthCheck,null, TimeSpan.Zero,
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
        public string Ip { get; set; }
        public int Port { get; set; }
        public DateTime LastUpTime { get; private set; } = DateTime.Now;
        public string Description { get; set; }
        public bool Health => health;

        private bool health { get; set; } = true;
        private readonly string _id = Guid.NewGuid().ToString(); //UUID

        private void HealthCheck(object? state)
        {
            if (DateTime.Now.Subtract(LastUpTime) > new TimeSpan(0,2,0))
            {
                health = false;
            }
        }
    }
}
