using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
{
    public class TimeZoneService
    {
        private readonly TimeZoneRepository _timeZoneRepository;

        public TimeZoneService(TimeZoneRepository timeZoneRepository)
        {
            _timeZoneRepository = timeZoneRepository;
        }

        public ResultViewModel<TimeZone> TimeZones(int id = default)
        {
            return _timeZoneRepository.TimeZones(id);
        }

        public ResultViewModel<List<TimeZone>> GetTimeZones()
        {
            return _timeZoneRepository.GetTimeZones();
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
