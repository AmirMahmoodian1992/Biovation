using Biovation.Domain;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

namespace Biovation.Data.Queries.Controllers.v2.RelayController
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

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<Camera>>> Camera(long adminUserId = default, long id = default, uint code = default, string name = default, string ip = default, int port = default,
            string brandCode = default, int modelId = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _cameraRepository.GetCamera(adminUserId, id, code, name, ip, port, brandCode, modelId,
                pageNumber, pageSize, nestingDepthLevel));
        }
    }
}