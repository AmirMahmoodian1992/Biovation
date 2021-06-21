using Biovation.Domain;
using Biovation.Repository.Api.v2.RelayController;
using System;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

namespace Biovation.Service.Api.v2.RelayController
{
    public class SchedulingService
    {
        private readonly SchedulingRepository _schedulingRepository;

        public SchedulingService(SchedulingRepository schedulingRepository)
        {
            _schedulingRepository = schedulingRepository;
        }

        public async Task<ResultViewModel> CreateScheduling(Scheduling scheduling, string token = default)
        {
            return await _schedulingRepository.CreateScheduling(scheduling, token);
        }

        public async Task<ResultViewModel<PagingResult<Scheduling>>> GetSchedulings(int id = 0,
            TimeSpan startTime = default, TimeSpan endTime = default, string mode = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            return await _schedulingRepository.GetScheduling(id, startTime, endTime, mode, pageNumber, pageSize,
                    nestingDepthLevel, token);
        }

        public async Task<ResultViewModel> UpdateScheduling(Scheduling scheduling, string token = default)
        {
            return await _schedulingRepository.UpdateScheduling(scheduling, token);
        }

        public async Task<ResultViewModel> DeleteScheduling(int id, string token = default)
        {
            return await _schedulingRepository.DeleteScheduling(id, token);
        }
    }
}
