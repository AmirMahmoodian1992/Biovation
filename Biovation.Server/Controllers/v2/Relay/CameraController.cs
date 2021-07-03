using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2.RelayController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        //TODO: Complete it
        public CameraController(CameraService cameraService, Lookups lookups)
        {
            _cameraService = cameraService;
            _lookups = lookups;
        }

        [HttpGet]
        [Route("{id:int}")]
        [Attribute.Authorize]
        public async Task<ResultViewModel<PagingResult<Camera>>> Camera(long id = default, uint code = default, string name = default, string ip = default, int port = default,
            string brandCode = default, int modelId = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            var token = (string)HttpContext.Items["Token"];
            return await _cameraService.GetCamera(id, code, name, ip, port, brandCode, modelId, pageNumber, pageSize,
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
            return await _cameraService.CreateCamera(camera, token);
        }

        [HttpPut]
        [Attribute.Authorize]
        public async Task<ResultViewModel> UpdateCamera([FromBody] Camera camera = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return await _cameraService.UpdateCamera(camera, token);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Attribute.Authorize]
        public async Task<ResultViewModel> DeleteCamera([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return await _cameraService.DeleteCamera(id, token);
        }

    }
}