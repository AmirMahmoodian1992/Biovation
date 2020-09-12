using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class TimeZoneController : Controller
    {

        private readonly TimeZoneRepository _timeZoneRepository;

        public TimeZoneController(TimeZoneRepository timeZoneRepository)
        {
            _timeZoneRepository = timeZoneRepository;
        }


        [HttpPut]
        public Task<ResultViewModel>AddTimeZone([FromBody]TimeZone timeZone)
        {
            return Task.Run(() => _timeZoneRepository.AddTimeZone(timeZone));
        }

        [HttpPut]
        public Task<ResultViewModel> ModifyTimeZone([FromBody]TimeZone timeZone)
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
