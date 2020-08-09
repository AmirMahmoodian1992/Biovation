using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class ServerSideIdentificationCacheModel
    {
        [Id]
        public int Id { get; set; }
        public int AccessGroupId { get; set; }
        public int UserId { get; set; }
        public int UserType { get; set; }
        public int DeviceId { get; set; }
        public int DeviceCode { get; set; }
        [OneToOne]
        public FingerTemplate FingerTemplate { get; set; }
        [OneToOne]
        public FaceTemplate FaceTemplate { get; set; }
        [OneToOne]
        public UserCard UserCard { get; set; }
    }
}
