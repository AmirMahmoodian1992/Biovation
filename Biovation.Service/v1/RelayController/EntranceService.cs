using Biovation.Domain;
using Biovation.Domain.RelayModels;
using Biovation.Repository.Sql.v2.RelayController;
using System.Threading.Tasks;

namespace Biovation.Service.Sql.v1.RelayController
{
    class EntranceService
    {
        private readonly EntranceRepository _entranceRepository;

        public EntranceService(EntranceRepository entranceRepository)
        {
            _entranceRepository = entranceRepository;
        }

        public Task<ResultViewModel> InsertEntrance(Entrance entrance)
        {
            return Task.Run(() => _entranceRepository.InsertEntrance(entrance));
        }

        public Task<ResultViewModel<PagingResult<Entrance>>> SelectEntrance(int cameraId, int schedulingId, int deviceId = 0, int id = 0, int code = 0,
            string name = null, string description = null, string filterText = default, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _entranceRepository.SelectEntrance(cameraId, schedulingId, deviceId, id, code, name, description, filterText, pageNumber, pageSize, nestingDepthLevel));
        }

        public Task<ResultViewModel> UpdateEntrance(Entrance entrance)
        {
            return Task.Run(() => _entranceRepository.UpdateEntrance(entrance));
        }

        public Task<ResultViewModel> DeleteEntrance(int id)
        {
            return Task.Run(() => _entranceRepository.DeleteEntrance(id));
        }
    }
}
