﻿using Biovation.Domain;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

namespace Biovation.Data.Commands.Controllers.v2.RelayController
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class SchedulingController : ControllerBase
    {
        private readonly SchedulingRepository _schedulingRepository;

        public SchedulingController(SchedulingRepository schedulingRepository)
        {
            _schedulingRepository = schedulingRepository;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddScheduling([FromBody] Scheduling scheduling = default)
        {
            return Task.Run(() => _schedulingRepository.CreateScheduling(scheduling));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> UpdateScheduling([FromBody] Scheduling scheduling = default)
        {
            return Task.Run(() => _schedulingRepository.UpdateScheduling(scheduling));
        }
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteScheduling([FromRoute]int id = default)
        {
            return Task.Run(() => _schedulingRepository.DeleteScheduling(id));
        }

    }
}