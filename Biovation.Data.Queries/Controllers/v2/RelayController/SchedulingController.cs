using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2.RelayController
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class SchedulingController : ControllerBase
    {
        private readonly SchedulingRepository _schedulingRepository;

        public SchedulingController(SchedulingRepository schedulingRepository)
        {
            _schedulingRepository = schedulingRepository;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<Scheduling>>> GetRelay(int id = 0,
            TimeSpan startTime = default, TimeSpan endTime = default, string mode = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _schedulingRepository.GetScheduling(id, startTime, endTime, mode, pageNumber, pageSize, nestingDepthLevel));
        }
    }
}