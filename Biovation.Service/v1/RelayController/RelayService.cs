using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Sql.v2.RelayController;

namespace Biovation.Service.Sql.v1.RelayController
{
    class RelayService
    {
        private readonly RelayRepository _relayRepository;

        public RelayService(RelayRepository relayRepository)
        {
            _relayRepository = relayRepository;
        }

        public Task<ResultViewModel<PagingResult<Relay>>> GetRelay(List<Scheduling> schedulings, int id = 0,
            string name = null, int nodeNumber = 0, int relayHubId = 0, int entranceId = 0, string description = null,
            int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _relayRepository.GetRelay(schedulings, id, name, nodeNumber, relayHubId, entranceId,
                description, pageNumber, pageSize, nestingDepthLevel));
        }

    }
}
