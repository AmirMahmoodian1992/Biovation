using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class IdentityCard
    {
        [Id]
        public int Id { get; set; }
        public string Number { get; set; }
        public int DataCheck { get; set; }
        public bool IsActive { get; set; }
    }
}
