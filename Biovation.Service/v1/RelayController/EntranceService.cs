using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Sql.v2.RelayController;

namespace Biovation.Service.Sql.v1.RelayController
{
    class EntranceService
    {
        private readonly EntranceRepository _entranceRepository;

        public EntranceService(EntranceRepository entranceRepository)
        {
            _entranceRepository = entranceRepository;
        }

        public Task<ResultViewModel> CreateEntrance(Entrance entrance)
        {
            return Task.Run(() => _entranceRepository.CreateEntrance(entrance));
        }

        public Task<ResultViewModel<PagingResult<Entrance>>> GetEntrances(List<DeviceBasicInfo> devices, List<Scheduling> schedulings, int id = 0,
            string name = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _entranceRepository.GetEntrances(devices, schedulings, id, name,description, pageNumber, pageSize, nestingDepthLevel));
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
