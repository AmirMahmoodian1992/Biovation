using Biovation.Domain;
using Biovation.Repository.Api.v2.RelayController;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2.RelayController
{
    public class CameraService
    {
        private readonly CameraRepository _cameraRepository;

        public CameraService(CameraRepository cameraRepository)
        {
            _cameraRepository = cameraRepository;
        }

        public async Task<ResultViewModel> CreateCamera(Camera camera, string token = default)
        {
            return await _cameraRepository.CreateCamera(camera, token);
        }

        public async Task<ResultViewModel<PagingResult<Camera>>> GetCamera(long id = default, uint code = default, string name = default, string ip = default, int port = default,
            string brandCode = default, int modelId = default, string filterText = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            return await _cameraRepository.GetCamera(id, code, name, ip, port, brandCode, modelId, filterText, pageNumber, pageSize, nestingDepthLevel, token);
        }

        public async Task<ResultViewModel<PagingResult<CameraModel>>> GetCameraModel(long id = default,
            uint manufactureCode = default, string name = default, string brandCode = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            return await _cameraRepository.GetCameraModel(id, manufactureCode, name, brandCode, pageNumber, pageSize, nestingDepthLevel);
        }

        public async Task<ResultViewModel> UpdateCamera(Camera camera, string token = default)
        {
            return await _cameraRepository.UpdateCamera(camera, token);
        }

        public async Task<ResultViewModel> DeleteCamera(int id, string token = default)
        {
            return await _cameraRepository.DeleteCamera(id, token);
        }
    }
}
