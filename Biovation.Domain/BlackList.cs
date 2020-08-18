using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class BlackList
    {
        [Id]
        public int Id { get; set; }
        [OneToOne]
        public User User { get; set; }
        [OneToOne]
        public DeviceBasicInfo Device { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
    }
}