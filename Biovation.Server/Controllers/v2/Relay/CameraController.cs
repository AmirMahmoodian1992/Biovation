using Biovation.Domain;
using Biovation.Service.Api.v2.RelayController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        public CameraController(CameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public Task<ResultViewModel<PagingResult<Camera>>> Camera(long id = default, uint code = default, string name = default, string ip = default, int port = default,
            string brandCode = default, int modelId = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () => await _cameraService.GetCamera(id, code, name, ip, port, brandCode, modelId, pageNumber, pageSize,
                nestingDepthLevel, token));
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddCamera([FromBody]  Camera camera = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _cameraService.CreateCamera(camera, token));
        }

        [HttpPost]
        [Route("Camera")]
        [Authorize]
        public Task<ResultViewModel> UpdateCamera([FromBody]  Camera camera = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _cameraService.UpdateCamera(camera, token));
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        public Task<ResultViewModel> DeleteCamera([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _cameraService.DeleteCamera(id, token));
        }

    }
}