using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2.RelayController
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class RelayHubController : ControllerBase
    {
        private readonly RelayHubRepository _relayHubRepository;

        public RelayHubController(RelayHubRepository relayHubRepository)
        {
            _relayHubRepository = relayHubRepository;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<RelayHub>>> GetRelayHub(int id = 0, string ipAddress = null, int port = 0,
            int capacity = 0, string relayHubModel = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _relayHubRepository.GetRelayHubs(id, ipAddress, port, capacity, relayHubModel, description,
                pageNumber, pageSize, nestingDepthLevel));
        }
    }
}