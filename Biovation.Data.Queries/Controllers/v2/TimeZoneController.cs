using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

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

        [HttpGet]
        public Task<ResultViewModel<List<TimeZone>>> GetTimeZones()
        {
            return Task.Run(() => _timeZoneRepository.GetTimeZones());
        }
    }
}
