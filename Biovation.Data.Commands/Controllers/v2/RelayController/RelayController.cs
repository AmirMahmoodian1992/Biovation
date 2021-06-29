using Biovation.Domain;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

namespace Biovation.Data.Commands.Controllers.v2.RelayController
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class RelayController : ControllerBase
    {
        private readonly RelayRepository _relayRepository;

        public RelayController(RelayRepository relayRepository)
        {
            _relayRepository = relayRepository;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddRelay([FromBody] Relay relay = default)
        {
            return Task.Run(() => _relayRepository.CreateRelay(relay));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> UpdateRelay([FromBody] Relay relay = default)
        {
            return Task.Run(() => _relayRepository.UpdateRelay(relay));
        }
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteRelay([FromRoute]int id = default)
        {
            return Task.Run(() => _relayRepository.DeleteRelay(id));
        }

    }
}