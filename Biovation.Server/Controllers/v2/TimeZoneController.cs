using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class TimeZoneController : ControllerBase
    {
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
        public async Task<ResultViewModel<TimeZone>> TimeZone([FromRoute] int id = default)
        {
            return await _timeZoneService.TimeZones(id, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        public async Task<ResultViewModel<PagingResult<TimeZone>>> TimeZones()
        {
            return await _timeZoneService.GetTimeZones(HttpContext.Items["Token"] as string);
        }

        //TODO
        //[HttpPost]
        //public Task<IActionResult> AddTimeZone([FromBody]TimeZone timeZone = default)
        //{
        //    throw null;
        //}


        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultViewModel> DeleteTimeZone([FromRoute] int id = default)
        {
            return await _timeZoneService.DeleteTimeZone(id, HttpContext.Items["Token"] as string);
        }

        [HttpPut]
        public async Task<ResultViewModel> ModifyTimeZone([FromBody] TimeZone timeZone = default)
        {
            return await _timeZoneService.ModifyTimeZone(timeZone, HttpContext.Items["Token"] as string);
        }


        //send to all device when deviceId is null
        [HttpPost]
        [Route("{id}/SendDevice/{deviceId}")]
        public async Task<ResultViewModel> SendTimeZoneDevice([FromRoute] int id = default, [FromRoute] int deviceId = default)
        {
            var token = HttpContext.Items["Token"] as string;
            var device = (await _deviceService.GetDevice(deviceId, token))?.Data;
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
            restRequest.AddHeader("Authorization", token!);
            await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
            return new ResultViewModel { Validate = 1 };
        }

        [HttpPost]
        [Route("{id}/SendTimeZoneToAllDevices")]
        public async Task<List<ResultViewModel>> SendTimeZoneToAllDevices([FromRoute] int id = default)
        {
            var devices = (await _deviceService.GetDevices())?.Data?.Data;
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
                restRequest.AddHeader("Authorization", (HttpContext.Items["Token"] as string)!);

                await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                result.Add(new ResultViewModel { Validate = 1 });
            }

            return result;
        }
    }
}
