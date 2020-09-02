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
    public class TimeZoneController : Controller
    {
        //private readonly CommunicationManager<List<ResultViewModel>> _communicationManager = new CommunicationManager<List<ResultViewModel>>();
        private readonly TimeZoneService _timeZoneService;
        private readonly DeviceService _deviceService;
        private readonly RestClient _restServer;

        public TimeZoneController(TimeZoneService timeZoneService, DeviceService deviceService)
        {
            _deviceService = deviceService;
            _timeZoneService = timeZoneService;
            _restServer =
                (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}");
            //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<Domain.TimeZone>> TimeZones(int id = default)
        {
            return Task.Run(async () => { return _timeZoneService.TimeZones(id); });
        }

        [HttpPost]
        public Task<IActionResult> AddTimeZone([FromBody]TimeZone timeZone = default)
        {
            throw null;
        }


        [HttpDelete]
        [Route("{id}")]
        public Task<IActionResult> DeleteTimeZone(int id = default)
        {
            throw null;
        }

        [HttpPut]
        public Task<IActionResult> ModifyTimeZone([FromBody]TimeZone timeZone = default)
        {
            throw null;
        }


        //send to all device when deviceId is null
        [HttpPost]
        [Route("TimeZoneToAllDevices")]
        public Task<IActionResult> SendTimeZoneToAllDevices(int timeZone = default, int deviceId = default)
        {
            throw null;
        }

    }
}
