using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Biovation.Server.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class AdminDeviceController : Controller
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly AdminDeviceService _adminDeviceService;

        public AdminDeviceController(AdminDeviceService adminDeviceService)
        {
            _adminDeviceService = adminDeviceService;
        }

        //public AdminDeviceController()
        //{
        //    //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        //}

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDevicesByPersonId(int id = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _adminDeviceService.GetAdminDevicesByPersonId(id, pageNumber, pageSize));
        }

        [HttpPost]
        public Task<IActionResult> AddAdminDevice([FromBody]AdminDevice adminDevice = default)
        {
            throw null;
        }

        [HttpPut]
        public Task<ResultViewModel> ModifyAdminDevice([FromBody] JObject adminDevice = default)
        {
            return Task.FromResult(_adminDeviceService.ModifyAdminDevice(adminDevice));
        }
    }
}