using System.Collections.Generic;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;

namespace Biovation.CommonClasses.Service
{
    public class TimeZoneService
    {
        public List<TimeZone> GetAllTimeZones()
        {
            var timeZoneRepository = new TimeZoneRepository();
            return timeZoneRepository.GetTimeZones();
        }

        public TimeZone GetTimeZoneById(int timeZoneId)
        {
            var timeZoneRepository = new TimeZoneRepository();
            return timeZoneRepository.GetTimeZone(timeZoneId);
        }

        public ResultViewModel ModifyTimeZoneById(TimeZone timeZone)
        {
            var timeZoneRepository = new TimeZoneRepository();
            return timeZoneRepository.ModifyTimeZone(timeZone);
        }

        public ResultViewModel DeleteTimeZoneById(int timeZoneId)
        {
            var timeZoneRepository = new TimeZoneRepository();
            return timeZoneRepository.DeleteTimeZone(timeZoneId);
        }
    }
}
