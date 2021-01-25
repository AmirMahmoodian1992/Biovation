using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class AdminDevice
    {
        [Id]
        public int Id { get; set; }
        public long UserId { get; set; }
        public int DeviceId { get; set; }
    }
}
