using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class LicensePlate
    {
        [Id]
        public int EntityId { get; set; }
        public string LicensePlateNumber { get; set; }
        public bool IsActive { get; set; }
        //public DateTime StartDate { get; set; }

        public Vehicle Vehicle { get; set; }
        public DateTime StartDate { get; set; }
     

        public DateTime EndDate { get; set; }


        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }
}
