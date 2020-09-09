using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v2;
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
        private readonly RestClient _restClient;

        public TimeZoneController(TimeZoneService timeZoneService, DeviceService deviceService, RestClient restClient)
        {
            _deviceService = deviceService;
            _timeZoneService = timeZoneService;
            _restClient = restClient;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<TimeZone>> TimeZones(int id = default)
        {
            return Task.Run( () =>  _timeZoneService.TimeZones(id));
        }

        //TODO
        //[HttpPost]
        //public Task<IActionResult> AddTimeZone([FromBody]TimeZone timeZone = default)
        //{
        //    throw null;
        //}


        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteTimeZone(int id = default)
        {
            return Task.Run(() => _timeZoneService.DeleteTimeZone(id));
        }

        [HttpPut]
        public Task<ResultViewModel> ModifyTimeZone([FromBody]TimeZone timeZone = default)
        {
            return Task.Run(() => _timeZoneService.ModifyTimeZone(timeZone));
        }


        //send to all device when deviceId is null
        [HttpPost]
        [Route("TimeZoneToAllDevices")]
        public Task<ResultViewModel> SendTimeZoneToAllDevices(int timeZone = default, int deviceId = default)
        {
            return Task.Run(() =>
            {

                var device = _deviceService.GetDevice(deviceId).Data;
                //var parameters = new List<object> { $"code={device.Code}", $"timeZoneId={timeZoneId}" };
                //_communicationManager.CallRest(
                //    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice", "Get", parameters);
                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice",
                        Method.GET);
                restRequest.AddParameter("code", device.Code);
                restRequest.AddParameter("timeZoneId", timeZone);
                _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                return new ResultViewModel {Validate = 1};
            });
            
        }

    }
}
