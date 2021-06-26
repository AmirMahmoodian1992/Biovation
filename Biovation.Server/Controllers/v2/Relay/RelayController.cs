using Biovation.Domain;
using Biovation.Service.Api.v2.RelayController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2.Relay
{
    [Attribute.Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class RelayController : ControllerBase
    {
        private readonly RelayService _relayService;


        public RelayController(RelayService relayService)
        {
            _relayService = relayService;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public Task<ResultViewModel<PagingResult<Domain.RelayModels.Relay>>> Relay([FromRoute] int id = 0, int adminUserId = 0,
            string name = null, int nodeNumber = 0, int relayHubId = 0, string description = null,
            int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4,  int schedulingId = default , int deviceId = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _relayService.GetRelay(id, adminUserId, name, nodeNumber, relayHubId, description, pageNumber, pageSize,
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

    }
}