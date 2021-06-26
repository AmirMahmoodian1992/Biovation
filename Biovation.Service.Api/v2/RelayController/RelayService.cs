using Biovation.Domain;
using Biovation.Repository.Api.v2.RelayController;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

namespace Biovation.Service.Api.v2.RelayController
{
    public class RelayService
    {
        private readonly RelayRepository _relayRepository;

        public RelayService(RelayRepository relayRepository)
        {
            _relayRepository = relayRepository;
        }

        public async Task<ResultViewModel<PagingResult<Relay>>> GetRelay(int id = 0, int adminUserId = 0,
            string name = null, int nodeNumber = 0, int relayHubId = 0, string description = null,
            int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4, int schedulingId = default, int deviceId = default, 
            string token = default)
        {
            return await _relayRepository.GetRelay(id, adminUserId, name, nodeNumber, relayHubId,
                description, pageNumber, pageSize, nestingDepthLevel, schedulingId, deviceId, token);
        }

        public async Task<ResultViewModel> CreateRelay(Relay relay, string token = default)
        {
            return await _relayRepository.CreateRelay(relay, token);
        }

        public async Task<ResultViewModel> UpdateRelay(Relay relay, string token = default)
        {
            return await _relayRepository.UpdateRelay(relay, token);
        }

        public async Task<ResultViewModel> DeleteRelay(int id = default, string token = default)
        {
            return await _relayRepository.DeleteRelay(id, token);
        }
    }
}
