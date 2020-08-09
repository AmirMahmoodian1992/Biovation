using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class Lookup
    {
        [Id]
        public string Code { get; set; }
        public string Name { get; set; }
        [OneToOne]
        public LookupCategory Category { get; set; }
        public int OrderIndex { get; set; }
        public string Description { get; set; }
    }
}
