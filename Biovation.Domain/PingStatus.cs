namespace Biovation.Domain
{
    public class PingStatus
    {
        public string HostAddress { get; set; }
        public string DestinationAddress { get; set; }
        public int TimeToLive { get; set; }
        public long RoundTripTime { get; set; }
        public string Status { get; set; }
    }
}
