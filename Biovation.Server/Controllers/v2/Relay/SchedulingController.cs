using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Biovation.Service.Api.v2.RelayController;

namespace Biovation.Server.Controllers.v2.Relay
{
    [Attribute.Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class SchedulingController : ControllerBase
    {
        private readonly SchedulingService _schedulingService;


        public SchedulingController(SchedulingService schedulingService)
        {
            _schedulingService = schedulingService;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public Task<ResultViewModel<PagingResult<Scheduling>>> Scheduling([FromRoute] int id = 0,
            TimeSpan startTime = default, TimeSpan endTime = default, string mode = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _schedulingService.GetSchedulings(id, startTime, endTime, mode, pageNumber, pageSize,
                nestingDepthLevel, token));
        }

        [HttpPost]
        [Attribute.Authorize]
        public Task<ResultViewModel> AddScheduling([FromBody] Scheduling scheduling)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _schedulingService.CreateScheduling(scheduling, token));
        }

        [HttpPut]
        [Attribute.Authorize]
        public Task<ResultViewModel> ModifyEntrance([FromBody] Scheduling scheduling)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _schedulingService.UpdateScheduling(scheduling, token));
        }

        [HttpDelete]
        [Route("{id}")]
        [Attribute.Authorize]
        public Task<ResultViewModel> DeleteEntrance([FromRoute] int id)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _schedulingService.DeleteScheduling(id, token));
        }

    }
}