using System;
using System.Collections.Generic;
using System.Text;

namespace Biovation.Domain.MassTransit
{
    public class MassTransitHostConfig
    {
        public string Type { get; set; }
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
