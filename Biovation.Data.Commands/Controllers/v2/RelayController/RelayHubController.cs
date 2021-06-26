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
    public class RelayHubController : ControllerBase
    {
        private readonly RelayHubRepository _relayHubRepository;

        public RelayHubController(RelayHubRepository relayHubRepository)
        {
            _relayHubRepository = relayHubRepository;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddRelayHub([FromBody] RelayHub relayHub = default)
        {
            return Task.Run(() => _relayHubRepository.CreateRelayHubs(relayHub));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> UpdateRelayHub([FromBody] RelayHub relayHub = default)
        {
            return Task.Run(() => _relayHubRepository.UpdateRelayHubs(relayHub));
        }
        [HttpDelete]
        [Authorize]
        public Task<ResultViewModel> DeleteRelayHub(int id = default)
        {
            return Task.Run(() => _relayHubRepository.DeleteRelayHubs(id));
        }

    }
}