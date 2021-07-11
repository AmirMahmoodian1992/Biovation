using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Service.Api.v2.RelayController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Constants;

namespace Biovation.Server.Controllers.v2.Relay
{
    [Attribute.Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class RelayController : ControllerBase
    {
        private readonly RelayService _relayService;
        private readonly Lookups _lookups;


        public RelayController(RelayService relayService, Lookups lookups)
        {
            _relayService = relayService;
            _lookups = lookups;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public Task<ResultViewModel<PagingResult<Domain.RelayModels.Relay>>> Relay([FromRoute] int id = 0, int adminUserId = 0,
            string name = null, int nodeNumber = 0, int relayHubId = 0, int relayTypeId = 0, int cameraId = 0, string description = null,
            int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4,  int schedulingId = default , int deviceId = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _relayService.GetRelay(id, adminUserId, name, nodeNumber, relayHubId, relayTypeId, cameraId, description, pageNumber, pageSize,
                nestingDepthLevel, schedulingId, deviceId, token));
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddRelay([FromBody] Domain.RelayModels.Relay relay = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _relayService.CreateRelay(relay, token));
        }

        [HttpPost]
        [Route("Relay")]
        [Authorize]
        public Task<ResultViewModel> UpdateRelay([FromBody] Domain.RelayModels.Relay relay = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _relayService.UpdateRelay(relay, token));
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        public Task<ResultViewModel> DeleteRelay([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _relayService.DeleteRelay(id, token));
        }

        [HttpGet]
        [Route("Types")]
        public Task<List<Lookup>> RelayTypes()
        {
            return Task.Run(() => _lookups.RelayType);
        }

        [HttpPost]
        [Route("{id:int}/Open")]
        public async Task<ResultViewModel> Open([FromRoute] int id, string messagePriority = "13003")
        {
            var token = (string)HttpContext.Items["Token"];
            return await _relayService.OpenRelay(id, messagePriority, token);
        }

        [HttpPost]
        [Route("{id:int}/Close")]
        public async Task<ResultViewModel> Close([FromRoute] int id, string messagePriority = "13003")
        {
            var token = (string)HttpContext.Items["Token"];
            return await _relayService.CloseRelay(id, messagePriority, token);
        }
    }
}