using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{

    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class AccessGroupController : Controller
    {
        //private readonly CommunicationManager<ResultViewModel> _communicationManager = new CommunicationManager<ResultViewModel>();
        private readonly AccessGroupService _accessGroupService;
        private readonly RestClient _restClient;

        public AccessGroupController(AccessGroupService accessGroupService)
        {
            _accessGroupService = accessGroupService;
            _restClient =
                new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}");
            //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }

        [HttpGet]
        public Task<ResultViewModel<PagingResult<AccessGroup>>> AccessGroups(long userId = default, int adminUserId = default, int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(async () =>
            {
                return _accessGroupService.GetAccessGroups(userId, adminUserId, userGroupId, id, deviceId,
                    deviceGroupId,
                    pageNumber, pageSize);
            });
        }

        [HttpPost]
        public Task<IActionResult> AddAccessGroup([FromBody]AccessGroup accessGroup)
        {
            throw null;
        }

        [HttpPatch]
        public Task<IActionResult> ModifyAccessGroup(string accessGroup = default, string deviceGroup = default, string userGroup = default, string adminUserIds = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<AccessGroup>> AccessGroup([FromRoute]int id, int nestingDepthLevel = default)
        {
            return Task.Run(async () =>
            {
                return _accessGroupService.GetAccessGroup(id,nestingDepthLevel);
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<IActionResult> DeleteAccessGroups(int id = default)
        {
            throw null;
        }

        [HttpPost]
        [Route("AllUsersToAllDevicesInAccessGroup/{accessGroupId}")]
        public Task<IActionResult> SendAllUsersToAllDevicesInAccessGroup(int accessGroupId = default)
        {
            throw null;
        }


        [HttpPost]
        [Route("AccessGroupToDevice/{accessGroupId}")]
        public Task<IActionResult> SendAccessGroupToDevice(int accessGroupId = default,int deviceId = default)
        {
            throw null;
        }

        
    }
}