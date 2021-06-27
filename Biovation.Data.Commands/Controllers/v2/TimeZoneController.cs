using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Data.Commands.Controllers.v2
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

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> AddTimeZone([FromBody] TimeZone timeZone)
        {
            return Task.Run(() => _timeZoneRepository.AddTimeZone(timeZone));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyTimeZone([FromBody] TimeZone timeZone)
        {
            return Task.Run(() => _timeZoneRepository.ModifyTimeZone(timeZone));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]

        public Task<ResultViewModel> DeleteTimeZone([FromRoute] int id)
        {
            return Task.Run(() => _timeZoneRepository.DeleteTimeZone(id));
        }

    }
}
