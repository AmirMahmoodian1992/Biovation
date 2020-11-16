using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Data.Queries.Controllers.v2
{
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

        public Task<ResultViewModel<TimeZone>> TimeZones(int id = default)
        {
            return Task.Run(() => _timeZoneRepository.GetTimeZone(id));
        }

        [HttpGet]
        [Authorize]

        public Task<ResultViewModel<List<TimeZone>>> GetTimeZones()
        {
            return Task.Run(() => _timeZoneRepository.GetTimeZones());
        }
    }
}
