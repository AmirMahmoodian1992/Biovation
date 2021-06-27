using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Data.Queries.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class TimeZoneController : ControllerBase
    {
        private readonly TimeZoneRepository _timeZoneRepository;

        public TimeZoneController(TimeZoneRepository timeZoneRepository)
        {
            _timeZoneRepository = timeZoneRepository;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel<TimeZone>> TimeZones([FromRoute] int id = default)
        {
            return Task.Run(() => _timeZoneRepository.GetTimeZone(id));
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<TimeZone>>> GetTimeZones()
        {
            return Task.Run(() => _timeZoneRepository.GetTimeZones());
        }
    }
}
