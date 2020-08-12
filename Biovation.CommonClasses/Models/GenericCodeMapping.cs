using Biovation.CommonClasses.DataMappers;
using DataAccessLayerCore.Attributes;

namespace Biovation.CommonClasses.Models
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
