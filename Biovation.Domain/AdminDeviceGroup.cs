
namespace Biovation.Domain
{
    public class AdminDeviceGroup
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public long GroupDeviceId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
    }
}
