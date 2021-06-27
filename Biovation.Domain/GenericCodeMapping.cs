using Biovation.Domain.DataMappers;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class GenericCodeMapping
    {
        [Id]
        public int Id { get; set; }
        [OneToOne]
        public GenericCodeMappingCategory Category { get; set; }
        [DataMapper(Mapper = typeof(IntToStringMapper))]
        public string ManufactureCode { get; set; }
        [OneToOne]
        public Lookup GenericValue { get; set; }
        [OneToOne]
        public Lookup Brand { get; set; }
        public string Description { get; set; }
    }
}
