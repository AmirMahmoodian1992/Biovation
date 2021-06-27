using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class AccessGroup
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }

        //public int TimeZoneId
        //{
        //    get { return TimeZone.Id; }
        //    set { TimeZone = _timeZoneService.GetTimeZoneById(value); }
        //}
        [OneToOne]
        public TimeZone TimeZone { get; set; }
        [OneToMany]
        public List<User> AdminUserId { get; set; }
        [OneToMany]
        public List<UserGroup> UserGroup { get; set; }
        [OneToMany]
        public List<DeviceGroup> DeviceGroup { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
    }
}
