using System.Collections.Generic;
using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class TimeZone
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        [OneToMany]
        public List<TimeZoneDetail> Details { get; set; }
    }
}
