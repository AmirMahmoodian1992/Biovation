namespace Biovation.Domain
{
    public class Connection
    {
        public IPv4 Ip { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MacAddress { get; set; }
    }
}