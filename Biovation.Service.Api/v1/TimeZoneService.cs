using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;

namespace Biovation.Service.Api.v1
{
    public class TimeZoneService
    {
        private readonly TimeZoneRepository _timeZoneRepository;

        public TimeZoneService(TimeZoneRepository timeZoneRepository)
        {
            _timeZoneRepository = timeZoneRepository;
        }

        public TimeZone TimeZones(int id = default)
        {
            return _timeZoneRepository.TimeZones(id)?.Data ?? new TimeZone();
        }

        public List<TimeZone> GetTimeZones()
        {
            return _timeZoneRepository.GetTimeZones()?.Data ?? new List<TimeZone>();
        }

        public ResultViewModel ModifyTimeZone(TimeZone timeZone)
        {
            return _timeZoneRepository.ModifyTimeZone(timeZone);
        }

        public ResultViewModel DeleteTimeZone(int id)
        {
            return _timeZoneRepository.DeleteTimeZone(id);
        }
    }
}
