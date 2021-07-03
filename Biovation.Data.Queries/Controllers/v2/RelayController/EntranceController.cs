using Biovation.Domain;
using Biovation.Domain.RelayModels;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2.RelayController
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class EntranceController : ControllerBase
    {
        private readonly EntranceRepository _entranceRepository;

        public EntranceController(EntranceRepository entranceRepository)
        {
            _entranceRepository = entranceRepository;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<Entrance>>> SelectEntrance(int cameraId, int schedulingId, int deviceId = 0, int id = 0, int code = 0,
            string name = null, string description = null, string filterText = default, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _entranceRepository.SelectEntrance(cameraId, schedulingId, deviceId, id, code, name, description, filterText, pageNumber, pageSize, nestingDepthLevel));
        }
    }
}