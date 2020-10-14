using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class TimeZoneService
    {
        private readonly TimeZoneRepository _timeZoneRepository;

        public TimeZoneService(TimeZoneRepository timeZoneRepository)
        {
            _timeZoneRepository = timeZoneRepository;
        }

        public ResultViewModel<TimeZone> TimeZones(int id = default, string token = default)
        {
            return _timeZoneRepository.TimeZones(id, token);
        }

        public ResultViewModel<List<TimeZone>> GetTimeZones(string token = default)
        {
            return _timeZoneRepository.GetTimeZones(token);
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
