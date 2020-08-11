
namespace Biovation.CommonClasses.Models
{
    public class AccessGroupUser
    {
        public int Id { get; set; }

        public UserGroup UserGroup { get; set; }

        public AccessGroup AccsessGroup { get; set; }
        public int UserGroupId { get; set; }
        public int AccsessGroupId { get; set; }

    }
}
