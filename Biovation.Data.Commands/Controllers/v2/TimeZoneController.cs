using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]

    public class TimeZoneController : Controller
    {

        private readonly TimeZoneRepository _timeZoneRepository;

        public TimeZoneController(TimeZoneRepository timeZoneRepository)
        {
            _timeZoneRepository = timeZoneRepository;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<TimeZone>> TimeZones(int id = default)
        {
            return Task.Run(() => _timeZoneRepository.GetTimeZone(id));
        }

        [HttpPut]
        [Route("ModifyTimeZone")]
        public Task<ResultViewModel> ModifyTimeZone(TimeZone timeZone)
        {
            return Task.Run(() => _timeZoneRepository.ModifyTimeZone(timeZone));
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteTimeZone(int id)
        {
            return Task.Run(() => _timeZoneRepository.DeleteTimeZone(id));
        }

    }
}
