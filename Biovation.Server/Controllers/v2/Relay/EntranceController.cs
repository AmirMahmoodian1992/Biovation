using Biovation.Domain;
using Biovation.Service.Api.v2.RelayController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

namespace Biovation.Server.Controllers.v2.Relay
{
    [Attribute.Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class EntranceController : ControllerBase
    {
        private readonly EntranceService _entranceService;


        public EntranceController(EntranceService entranceService)
        {
            _entranceService = entranceService;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ResultViewModel<PagingResult<Entrance>>> Entrances([FromRoute] int id = default, string name = null, string description = null, int pageNumber = 0,
        int pageSize = 0, int nestingDepthLevel = 4, int cameraId = default, int deviceId = default, int schedulingId = default)
        {
            var token = (string)HttpContext.Items["Token"];
            var result = await _entranceService.GetEntrances(id, name, description, pageNumber, pageSize,
                nestingDepthLevel, cameraId, deviceId, schedulingId, token);
            return result;
        }

        [HttpPost]
        [Attribute.Authorize]
        public Task<ResultViewModel> AddEntrance([FromBody] Entrance entrance)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _entranceService.CreateEntrance(entrance, token));
        }

        [HttpPut]
        [Attribute.Authorize]
        public Task<ResultViewModel> ModifyEntrance([FromBody] Entrance entrance)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _entranceService.UpdateEntrance(entrance, token));
        }

        [HttpDelete]
        [Route("{id}")]
        [Attribute.Authorize]
        public Task<ResultViewModel> DeleteEntrance([FromRoute] int id)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _entranceService.DeleteEntrance(id, token));
        }

    }
}