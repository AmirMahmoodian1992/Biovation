using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2.RelayController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2.Relay
{
    [Attribute.Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class CameraController : ControllerBase
    {
        private readonly CameraService _cameraService;
        private readonly Lookups _lookups;
        private readonly RestClient _restClient;
        private readonly SystemInfo _systemInfo;

        //TODO: Complete it
        public CameraController(CameraService cameraService, Lookups lookups, RestClient restClient, SystemInfo systemInfo)
        {
            _cameraService = cameraService;
            _lookups = lookups;
            _restClient = restClient;
            _systemInfo = systemInfo;
        }

        [HttpGet]
        [Route("{id:int}")]
        [Attribute.Authorize]
        public async Task<ResultViewModel<PagingResult<Camera>>> Camera(long id = default, uint code = default, string name = default, string ip = default, int port = default,
            string brandCode = default, int modelId = default, string filterText = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            var token = (string)HttpContext.Items["Token"];
            return await _cameraService.GetCamera(id, code, name, ip, port, brandCode, modelId, filterText, pageNumber, pageSize,
                nestingDepthLevel, token);
        }

        [Route("CameraModel")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResultViewModel<PagingResult<CameraModel>>> CameraModel(long id = default,
            uint manufactureCode = default, string name = default, string brandCode = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            return await _cameraService.GetCameraModel(id, manufactureCode, name, brandCode, pageNumber, pageSize, nestingDepthLevel);
        }

        [HttpGet]
        [Route("CameraBrands")]
        [AllowAnonymous]
        public ResultViewModel<List<Lookup>> CameraBrands()
        {
            return new ResultViewModel<List<Lookup>>
            {
                Data = _lookups.CameraBrand,
                Validate = 1
            };
        }

        [HttpGet]
        [Route("Resolutions")]
        [AllowAnonymous]
        public ResultViewModel<List<Lookup>> Resolutions()
        {
            return new ResultViewModel<List<Lookup>>
            {
                Data = _lookups.Resolution,
                Validate = 1
            };
        }

        [HttpGet]
        [Route("CameraConnectionProtocols")]
        [AllowAnonymous]
        public ResultViewModel<List<Lookup>> CameraConnectionProtocols()
        {
            return new ResultViewModel<List<Lookup>>
            {
                Data = _lookups.CameraProtocol,
                Validate = 1
            };
        }

        [HttpPost]
        [Attribute.Authorize]
        public async Task<ResultViewModel> AddCamera([FromBody] Camera camera = default)
        {
            var token = (string)HttpContext.Items["Token"];
            var result = await _cameraService.CreateCamera(camera, token);
            if (result.Validate != 1) return result;

            _ = Task.Run(async () =>
            {
                try
                {
                    //var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ModifyDevice",
                    var restRequest = new RestRequest("PFK/PFKDevice/ModifyCamera",
                        Method.POST);
                    restRequest.AddJsonBody(camera!);
                    restRequest.AddHeader("Authorization", token!);
                    await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                }
                catch (Exception)
                {
                    //ignore
                }

            }).ConfigureAwait(false);

            return result;
        }

        [HttpPut]
        [Attribute.Authorize]
        public async Task<ResultViewModel> UpdateCamera([FromBody] Camera camera = default)
        {
            var token = (string)HttpContext.Items["Token"];
            var result = await _cameraService.UpdateCamera(camera, token);
            if (result.Validate != 1) return result;

            _ = Task.Run(async () =>
            {
                try
                {
                    //var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ModifyDevice",
                    var restRequest = new RestRequest("PFK/PFKDevice/ModifyCamera",
                        Method.POST);
                    restRequest.AddJsonBody(camera!);
                    restRequest.AddHeader("Authorization", token!);
                    await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                }
                catch (Exception)
                {
                    //ignore
                }
            }).ConfigureAwait(false);

            return result;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Attribute.Authorize]
        public async Task<ResultViewModel> DeleteCamera([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return await _cameraService.DeleteCamera(id, token);
        }

        [HttpGet]
        [Attribute.Authorize]
        [Route("GetOnlineCameras")]
        public List<Camera> GetOnlineCameras()
        {
            var resultList = new List<Camera>();
            var serviceInstances = _systemInfo.Services;

            Parallel.ForEach(serviceInstances, serviceInstance =>
            {
                if (string.Equals(serviceInstance.Name, "Shahab", StringComparison.InvariantCultureIgnoreCase))
                    return;

                var restRequest =
                    new RestRequest($"{serviceInstance.Name}/{serviceInstance.Name}Device/GetOnlineCameras");
                var result = _restClient.ExecuteAsync<List<Camera>>(restRequest).GetAwaiter().GetResult();

                if (result.StatusCode != HttpStatusCode.OK) return;
                lock (resultList)
                    resultList.AddRange(result.Data);
            });

            return resultList;
        }
    }
}