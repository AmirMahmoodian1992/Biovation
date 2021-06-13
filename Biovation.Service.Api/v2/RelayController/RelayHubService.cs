﻿using Biovation.Domain;
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

        public async Task<ResultViewModel<PagingResult<RelayHub>>> GetRelayHubs(int id = 0, int adminUserId = 0, string ipAddress = null, int port = 0, string name = default,
            int capacity = 0, int relayHubModelId = default, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            return await _relayHubRepository.GetRelayHubs(id, adminUserId, ipAddress, port, name, capacity, relayHubModelId,
                description, pageNumber, pageSize, nestingDepthLevel, token);
        }

        public async Task<ResultViewModel<PagingResult<RelayHubModel>>> GetRelayHubModels(int id = default, string name = default, int manufactureCode = default, int brandCode = default, int defaultPortNumber = default, int defaultCapacity = default, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return await _relayHubRepository.GetRelayHubModels(id, name, manufactureCode, brandCode, defaultPortNumber, defaultCapacity, pageNumber, pageSize
                , nestingDepthLevel);
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
