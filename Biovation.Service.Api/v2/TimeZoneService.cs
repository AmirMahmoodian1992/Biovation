using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<ResultViewModel<TimeZone>> TimeZones(int id = default, string token = default)
        {
            return await _timeZoneRepository.TimeZones(id, token);
        }

        public async Task<ResultViewModel<PagingResult<TimeZone>>> GetTimeZones(string token = default)
        {
            return await _timeZoneRepository.GetTimeZones(token);
        }

        public async Task<ResultViewModel> ModifyTimeZone(TimeZone timeZone, string token = default)
        {
            return await _timeZoneRepository.ModifyTimeZone(timeZone, token);
        }

        public async Task<ResultViewModel> DeleteTimeZone(int id, string token = default)
        {
            return await _timeZoneRepository.DeleteTimeZone(id, token);
        }

    }
}
