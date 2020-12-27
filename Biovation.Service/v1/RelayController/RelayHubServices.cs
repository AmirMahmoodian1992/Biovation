using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Sql.v2.RelayController;

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

        public Task<ResultViewModel<PagingResult<RelayHub>>> GetRelayHubs(int id = 0, string ipAddress = null, int port = 0,
            int capacity = 0, string relayHubModel = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _relayHubRepository.GetRelayHubs(id, ipAddress, port, capacity, relayHubModel,
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
