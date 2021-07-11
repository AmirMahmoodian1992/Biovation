using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class GadgetModel
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ManufactureCode { get; set; }
        [OneToOne]
        public Lookup Brand { get; set; }
        public string Description { get; set; }
    }
}