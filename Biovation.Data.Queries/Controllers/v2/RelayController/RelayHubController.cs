using Biovation.Domain;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

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
        public Task<ResultViewModel<PagingResult<RelayHub>>> GetRelayHub(int adminUserId = 0, int id = 0, string ipAddress = default, int port = 0, string name = default,
            int capacity = 0,int relayHubModelId = default, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _relayHubRepository.GetRelayHubs(adminUserId,id, ipAddress, port, name, capacity, relayHubModelId, description,
                pageNumber, pageSize, nestingDepthLevel));
        }

        [HttpGet]
        [Authorize]
        [Route("RelayHubModel")]
        public Task<ResultViewModel<PagingResult<RelayHubModel>>> GetRelayHubModel(int id = 0, string name = default,
            int manufactureCode = 0, int brandId = default, int defaultPortNumber = default, int defaultCapacity = default, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _relayHubRepository.GetRelayHubModels(id, name, brandId, manufactureCode,defaultPortNumber, defaultCapacity, description,
                pageNumber, pageSize, nestingDepthLevel));
        }
    }
}