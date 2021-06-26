using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class Vehicle
    {
        [Id]
        public int Id { get; set; }
        public Color Color { get; set; }
        public VehicleModel Model { get; set; }
        [OneToOne]
        public LicensePlate Plate { get; set; }
    }
}