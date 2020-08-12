using DataAccessLayerCore.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class ServerSideIdentificationCacheModel
    {
        [Id]
        public int Id { get; set; }
        public int AccessGroupId { get; set; }
        public long UserId { get; set; }
        public int UserType { get; set; }
        public long DeviceId { get; set; }
        public long DeviceCode { get; set; }
        [OneToOne]
        public FingerTemplate FingerTemplate { get; set; }
        [OneToOne]
        public FaceTemplate FaceTemplate { get; set; }
        [OneToOne]
        public UserCard UserCard { get; set; }
    }
}
