using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service;
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
        private readonly DeviceService _deviceService;
        private readonly RestClient _restServer;

        public AccessGroupController(AccessGroupService accessGroupService, DeviceService deviceService)
        {
            _accessGroupService = accessGroupService;
            _deviceService = deviceService;
            _restServer =
                new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}");
            //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }

        [HttpGet]
        public Task<IActionResult> AccessGroups(long userId = default, int adminUserId = default, int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default)
        {
            throw null;
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
            throw null;

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