using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Threading.Tasks;

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

        public async Task<ResultViewModel<PagingResult<TimeZone>>> GetTimeZones(int id = default, int accessGroupId = default, string name = default, int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _timeZoneRepository.GetTimeZones(id, accessGroupId, name, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel> AddTimeZone(TimeZone timeZone, string token = default)
        {
            return await _timeZoneRepository.AddTimeZone(timeZone, token);
        }

        public async Task<ResultViewModel> ModifyTimeZone(int id, TimeZone timeZone, string token = default)
        {
            return await _timeZoneRepository.ModifyTimeZone(id, timeZone, token);
        }

        public async Task<ResultViewModel> DeleteTimeZone(int id, string token = default)
        {
            return await _timeZoneRepository.DeleteTimeZone(id, token);
        }

    }
}
