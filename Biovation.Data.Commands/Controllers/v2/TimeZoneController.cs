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

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> AddTimeZone([FromBody] TimeZone timeZone)
        {
            return await Task.Run(() => _timeZoneRepository.AddTimeZone(timeZone));
        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public async Task<ResultViewModel> ModifyTimeZone([FromBody] TimeZone timeZone)
        {
            return await Task.Run(() => _timeZoneRepository.ModifyTimeZone(timeZone));
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<ResultViewModel> DeleteTimeZone([FromRoute] int id)
        {
            return await Task.Run(() => _timeZoneRepository.DeleteTimeZone(id));
        }
    }
}
