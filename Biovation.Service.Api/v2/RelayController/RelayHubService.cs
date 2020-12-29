using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Api.v2.RelayController;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2.RelayController
{
    public class RelayHubService
    {
        private readonly RelayHubRepository _relayHubRepository;

        public RelayHubService(RelayHubRepository relayHubRepository)
        {
            _relayHubRepository = relayHubRepository;
        }

        public async Task<ResultViewModel> CreateRelayHub(RelayHub relayHub, string token = default)
        {
            return await _relayHubRepository.CreateRelayHub(relayHub, token);
        }

        public async Task<ResultViewModel<PagingResult<RelayHub>>> GetRelayHubs(int id = 0, string ipAddress = null, int port = 0,
            int capacity = 0, string relayHubModel = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            return await _relayHubRepository.GetRelayHubs(id, ipAddress, port, capacity, relayHubModel,
                description, pageNumber, pageSize, nestingDepthLevel, token);
        }

        public async Task<ResultViewModel> UpdateRelayHub(RelayHub relayHub, string token = default)
        {
            return await _relayHubRepository.UpdateRelayHub(relayHub, token);
        }

        public async Task<ResultViewModel> DeleteRelayHub(int id, string token = default)
        {
            return await _relayHubRepository.DeleteRelayHub(id, token);
        }
    }
}
