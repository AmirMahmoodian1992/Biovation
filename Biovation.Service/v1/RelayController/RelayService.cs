﻿using Biovation.Domain;
using Biovation.Repository.Sql.v2.RelayController;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

namespace Biovation.Service.Sql.v1.RelayController
{
    class RelayService
    {
        private readonly RelayRepository _relayRepository;

        public RelayService(RelayRepository relayRepository)
        {
            _relayRepository = relayRepository;
        }

        public Task<ResultViewModel<PagingResult<Relay>>> GetRelay(int adminUserId = 0, int id = 0,
            string name = null, int nodeNumber = 0, int relayHubId = 0, int relayTypeId = 0, int cameraId = 0, string description = null, int schedulingId = default, int deviceId = default,
            int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _relayRepository.GetRelay(adminUserId, id, name, nodeNumber, relayHubId,
                relayTypeId, cameraId,  description, schedulingId, deviceId, pageNumber, pageSize, nestingDepthLevel));
        }

    }
}
