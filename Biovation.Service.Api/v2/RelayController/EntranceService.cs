using Biovation.Domain;
using Biovation.Repository.Api.v2.RelayController;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

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
            int pageSize = 0, int nestingDepthLevel = 4, int cameraId = default, int deviceId = default, int schedulingId = default, string filterText = default, string token = default)
        {
            return await _entranceRepository.GetEntrances(id, name, pageNumber, pageSize, nestingDepthLevel, cameraId, deviceId, schedulingId, filterText, token);
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
