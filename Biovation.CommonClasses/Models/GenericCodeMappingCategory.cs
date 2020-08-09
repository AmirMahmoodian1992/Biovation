using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class GenericCodeMappingCategory
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
