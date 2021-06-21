using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RelayModels
{
    public class RelayHubModel
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ManufactureCode { get; set; }
        [OneToOne]
        public Lookup Brand { get; set; }
        public string Description { get; set; }
        public int DefaultPortNumber { get; set; }
        public int DefaultCapacity { get; set; }

    }
   
}