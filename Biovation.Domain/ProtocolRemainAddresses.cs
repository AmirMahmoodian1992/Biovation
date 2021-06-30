using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class ProtocolRemainAddresses
    {
        [Id]
        public int Id { get; set; }
        [OneToOne]
        public Lookup Protocol { get; set; }
        public string RemainAddress { get; set; }
        public int OrderIndex { get; set; }
    }
}