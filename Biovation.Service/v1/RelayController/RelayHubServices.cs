using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Sql.v2.RelayController;
using System.Threading.Tasks;

namespace Biovation.Service.Sql.v1.RelayController
{
    class RelayHubServices
    {
        private readonly RelayHubRepository _relayHubRepository;

        public RelayHubServices(RelayHubRepository relayHubRepository)
        {
            _relayHubRepository = relayHubRepository;
        }


        public Task<ResultViewModel> CreateRelayHubs(RelayHub relayHub)
        {
            return Task.Run(() => _relayHubRepository.CreateRelayHubs(relayHub));
        }

        public Task<ResultViewModel<PagingResult<RelayHub>>> GetRelayHubs(int adminUserId = 0, int id = 0, string ipAddress = null, int port = 0,
            int capacity = 0, DeviceModel relayHubModel = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _relayHubRepository.GetRelayHubs(adminUserId, id, ipAddress, port, capacity, relayHubModel,
                description, pageNumber, pageSize, nestingDepthLevel));
        }


        public Task<ResultViewModel> UpdateRelayHubs(RelayHub relayHub)
        {
            return Task.Run(() => _relayHubRepository.UpdateRelayHubs(relayHub));
        }

        public Task<ResultViewModel> DeleteRelayHubs(int id)
        {
            return Task.Run(() => _relayHubRepository.DeleteRelayHubs(id));
        }
    }
}
