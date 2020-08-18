using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class Setting
    {
        [Id]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
