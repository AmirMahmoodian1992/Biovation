﻿using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Api.v2.RelayController;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2.RelayController
{
    public class RelayService
    {
        private readonly RelayRepository _relayRepository;

        public RelayService(RelayRepository relayRepository)
        {
            _relayRepository = relayRepository;
        }

        public async Task<ResultViewModel<PagingResult<Relay>>> GetRelay(int id = 0,
            string name = null, int nodeNumber = 0, int relayHubId = 0, int entranceId = 0, string description = null,
            int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4, List<Scheduling> schedulings = null, string token = default)
        {
            return await _relayRepository.GetRelay(id, name, nodeNumber, relayHubId, entranceId,
                description, pageNumber, pageSize, nestingDepthLevel, schedulings, token);
        }

    }
}
