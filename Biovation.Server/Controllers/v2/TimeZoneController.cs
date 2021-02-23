using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Server.Attribute;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class TimeZoneController : ControllerBase
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
        public Task<ResultViewModel<TimeZone>> TimeZones([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _timeZoneService.TimeZones(id, token));
        }

        //TODO
        //[HttpPost]
        //public Task<IActionResult> AddTimeZone([FromBody]TimeZone timeZone = default)
        //{
        //    throw null;
        //}


        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteTimeZone([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _timeZoneService.DeleteTimeZone(id, token));
        }

        [HttpPut]
        public Task<ResultViewModel> ModifyTimeZone([FromBody] TimeZone timeZone = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _timeZoneService.ModifyTimeZone(timeZone, token));
        }


        //send to all device when deviceId is null
        [HttpPost]
        [Route("{id}/SendDevice/{deviceId}")]
        public Task<ResultViewModel> SendTimeZoneDevice([FromRoute] int id = default, [FromRoute] int deviceId = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {

                var device = _deviceService.GetDevice(deviceId, token: token)?.Data;
                if (device is null)
                    return new ResultViewModel
                    {
                        Id = id,
                        Code = 404,
                        Message = "The provided device id is wrong",
                        Success = false,
                        Validate = 0
                    };

                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice",
                        Method.GET);
                restRequest.AddParameter("code", device.Code);
                restRequest.AddParameter("timeZoneId", id);
                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                {
                    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                }
                _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                return new ResultViewModel { Validate = 1 };
            });

        }

        [HttpPost]
        [Route("{id}/SendTimeZoneToAllDevices")]
        public Task<List<ResultViewModel>> SendTimeZoneToAllDevices([FromRoute]int id = default)
        {
            return Task.Run(() =>
            {
                var devices = _deviceService.GetDevices()?.Data?.Data;
                if (devices is null)
                    return new List<ResultViewModel> { new ResultViewModel { Id = id, Success = false, Validate = 0, Message = "No device is found", Code = 404 } };

                var result = new List<ResultViewModel>();
                foreach (var device in devices)
                {
                    var restRequest =
                        new RestRequest(
                            $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice",
                            Method.GET);
                    restRequest.AddParameter("code", device.Code);
                    restRequest.AddParameter("timeZoneId", id);
                    if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                    {
                        restRequest.AddHeader("Authorization",
                            HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                    }

                    _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                    result.Add(new ResultViewModel { Validate = 1 });
                }

                return result;
            });

        }
    }
}
