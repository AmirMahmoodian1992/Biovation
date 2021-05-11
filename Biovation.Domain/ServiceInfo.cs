using System;
using System.Threading;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class ServiceInstance
    {
        private Timer _timer;
        public ServiceInstance()
        {
            _timer = new Timer(HealthCheck,null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
        }

        [Id]
        public string Id { get; } = Guid.NewGuid().ToString(); //UUID
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime LastUpTime { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public bool Health => hHealth;

        private bool hHealth { get; set; } = true;

        private void HealthCheck(object? state)
        {
            if (DateTime.Now.Subtract(LastUpTime) > new TimeSpan(0,2,0))
            {
                hHealth = false;
            }
        }
    }
}
