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

        public TimeZone TimeZones(int id = default, string token = default)
        {
            return _timeZoneRepository.TimeZones(id, token)?.Data ?? new TimeZone();
        }

        public List<TimeZone> GetTimeZones(string token = default)
        {
            return _timeZoneRepository.GetTimeZones()?.Data ?? new List<TimeZone>();
        }

        public ResultViewModel ModifyTimeZone(TimeZone timeZone, string token = default)
        {
            return _timeZoneRepository.ModifyTimeZone(timeZone, token);
        }

        public ResultViewModel DeleteTimeZone(int id, string token = default)
        {
            return _timeZoneRepository.DeleteTimeZone(id, token);
        }
    }
}
