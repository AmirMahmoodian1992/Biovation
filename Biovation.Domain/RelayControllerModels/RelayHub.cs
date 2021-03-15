using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RelayControllerModels
{
    public class RelayHub
    {
        [Id]
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int Capacity { get; set; }
        [OneToOne]
        public DeviceModel RelayHubModel { get; set; }
        public string Description { get; set; }
    }
}