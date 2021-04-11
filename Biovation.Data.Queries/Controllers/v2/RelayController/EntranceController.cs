using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
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
        public Task<ResultViewModel<PagingResult<Entrance>>> GetEntrance(List<DeviceBasicInfo> devices, List<Scheduling> schedulings, int id = 0,
            string name = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _entranceRepository.GetEntrances(devices,schedulings,id,name,description,pageNumber,pageSize,nestingDepthLevel));
        }
    }
}