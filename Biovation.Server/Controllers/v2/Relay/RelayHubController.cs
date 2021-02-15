using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2.Relay
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class RelayHubController : ControllerBase
    {
        private readonly RelayHubService _relayHubService;


        public RelayHubController(RelayHubService relayHubService)
        {
            _relayHubService = relayHubService;
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize]
        public Task<ResultViewModel<PagingResult<RelayHub>>> RelayHub([FromRoute] int id = default, string ipAddress = null, int port = 0,
            int capacity = 0, string relayHubModel = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _relayHubService.GetRelayHubs(id, ipAddress, port, capacity, relayHubModel, description, pageNumber, pageSize,
                nestingDepthLevel, token));
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddRelayHub([FromBody] RelayHub relayHub)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _relayHubService.CreateRelayHub(relayHub, token));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyEntrance([FromBody] RelayHub relayHub)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _relayHubService.UpdateRelayHub(relayHub, token));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteEntrance([FromRoute] int id)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _relayHubService.DeleteRelayHub(id, token));
        }

    }
}