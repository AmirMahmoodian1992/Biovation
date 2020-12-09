namespace Biovation.Services.RelayController.Models
{
    public class Relay
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int Id { get; set; }
        public string Brand { get; set; }

        public Relay(string ip, int port, int id, string brand)
        {
            Ip = ip;
            Port = port;
            Id = id;
            Brand = brand;
        }
    }
}
