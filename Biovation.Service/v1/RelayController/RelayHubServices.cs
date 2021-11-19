using Biovation.Domain;
using Biovation.Repository.Sql.v2.RelayController;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

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

        public Task<ResultViewModel<PagingResult<RelayHub>>> GetRelayHubs(int adminUserId = 0, int id = 0, string ipAddress = default, int port = 0, string name = default,
            int capacity = 0, int relayHubModelId = default, string description = null, string filterText = default, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _relayHubRepository.GetRelayHubs(adminUserId, id, ipAddress, port, name, capacity, relayHubModelId,
                description, filterText, pageNumber, pageSize, nestingDepthLevel));
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
