using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Sql.v2.RelayController;

namespace Biovation.Service.Sql.v1.RelayController
{
    class SchedulingService
    {
        private readonly SchedulingRepository _schedulingRepository;

        public SchedulingService(SchedulingRepository schedulingRepository)
        {
            _schedulingRepository = schedulingRepository;
        }

        public Task<ResultViewModel> CreateScheduling(Scheduling scheduling)
        {
            return Task.Run(() => _schedulingRepository.CreateScheduling(scheduling));
        }

        public Task<ResultViewModel<PagingResult<Scheduling>>> GetScheduling(int id = 0,
            TimeSpan startTime = default, TimeSpan endTime = default, string mode = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() =>
                _schedulingRepository.GetScheduling(id, startTime, endTime, mode, pageNumber, pageSize,
                    nestingDepthLevel));
        }

        public Task<ResultViewModel> UpdateScheduling(Scheduling scheduling)
        {
            return Task.Run(() => _schedulingRepository.UpdateScheduling(scheduling));
        }

        public Task<ResultViewModel> DeleteScheduling(int id)
        {
            return Task.Run(() => _schedulingRepository.DeleteScheduling(id));
        }
    }
}
