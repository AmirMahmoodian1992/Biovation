using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Api.v2.RelayController;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2.RelayController
{
    public class EntranceService
    {
        private readonly EntranceRepository _entranceRepository;

        public EntranceService(EntranceRepository entranceRepository)
        {
            _entranceRepository = entranceRepository;
        }

        public async Task<ResultViewModel> CreateEntrance(Entrance entrance, string token = default)
        {
            return await _entranceRepository.CreateEntrance(entrance, token);
        }

        public async Task<ResultViewModel<PagingResult<Entrance>>> GetEntrances(int id = 0, string name = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4, List<DeviceBasicInfo> devices = null, List<Scheduling> schedulings = null, string token = default)
        {
            return await _entranceRepository.GetEntrances(id, name, description, pageNumber, pageSize, nestingDepthLevel, devices, schedulings, token);
        }

        public async Task<ResultViewModel> UpdateEntrance(Entrance entrance, string token = default)
        {
            return await _entranceRepository.UpdateEntrance(entrance, token);
        }

        public async Task<ResultViewModel> DeleteEntrance(int id, string token = default)
        {
            return await _entranceRepository.DeleteEntrance(id, token);
        }
    }
}
