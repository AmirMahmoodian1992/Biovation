using DataAccessLayerCore.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class UserCard
    {
        [Id]
        public int Id { get; set; }
        public long UserId { get; set; }
        public string CardNum { get; set; }
        public int DataCheck { get; set; }
        public bool IsActive { get; set; }
    }
}
