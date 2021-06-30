using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class VehicleModel
    {
        [Id]
        public int Id { get; set; }
        [OneToOne]
        public Lookup Manufacturer { get; set; }
        [OneToOne]
        public Lookup Brand { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}