
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class UserGroupMember
    {
        public int Id { get; set; }
        [Id]
        public long UserId { get; set; }
        public long UserCode { get; set; }
        public int GroupId { get; set; }
        public string UserType { get; set; }
        public string UserTypeTitle { get; set; }
        public string UserName { get; set; }
    }
}
