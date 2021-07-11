using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2.RelayController
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class CameraController : ControllerBase
    {
        private readonly CameraRepository _cameraRepository;

        public CameraController(CameraRepository cameraRepository)
        {
            _cameraRepository = cameraRepository;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddCamera([FromBody] Camera camera = default)
        {
            return Task.Run(() => _cameraRepository.CreateCamera(camera));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> UpdateCamera([FromBody] Camera camera = default)
        {
            return Task.Run(() => _cameraRepository.UpdateCamera(camera));
        }
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteCamera([FromRoute]int id = default)
        {
            return Task.Run(() => _cameraRepository.DeleteCamera(id));
        }
    }
}