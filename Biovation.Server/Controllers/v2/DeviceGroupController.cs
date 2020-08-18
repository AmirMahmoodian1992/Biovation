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
    public class DeviceGroupController : Controller
    {
        private readonly DeviceService _deviceService;
        private readonly DeviceGroupService _deviceGroupService;

        private readonly RestClient _restClient;

        public DeviceGroupController(DeviceService deviceService, DeviceGroupService deviceGroupService)
        {
            _deviceService = deviceService;
            _deviceGroupService = deviceGroupService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetDeviceGroup(int id = default, long userId = default)
        {
            throw null;
        }

        [HttpPost]
        public Task<IActionResult> AddDeviceGroup([FromBody]DeviceGroup deviceGroup = default)
        {
            throw null;
        }


        [HttpPut]
        public Task<IActionResult> ModifyDeviceGroup([FromBody]DeviceGroup deviceGroup = default)
        {
            throw null;
        }



        [HttpDelete]
        [Route("{id}")]
        public Task<IActionResult> DeleteDeviceGroup( int id = default)
        {
            throw null;
        }


        //batch delete
        [HttpPost]
        [Route("DeleteDeviceGroups")]
        public Task<IActionResult> DeleteDeviceGroup([FromBody] int[] ids = default)
        {
            throw null;
        }



        [HttpGet]
        [Route("AccessControlDeviceGroup/{id}")]
        public Task<IActionResult> GetAccessControlDeviceGroup(int id =default)
        {
            throw null;
        }
    }
}
