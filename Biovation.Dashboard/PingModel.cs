using System;

namespace Biovation.Dashboard
{
    public class PingModel
    {
        public string hostAddress { get; set; }
        public string DestinationAddress { get; set; }
        public int ttl { get; set; }
        public long roundTripTime { get; set; }
        public string status { get; set; }

    }
}
