using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RelayModels;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2.RelayController
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

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddEntrance([FromBody] Entrance entrance = default)
        {
            return Task.Run(() => _entranceRepository.InsertEntrance(entrance));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> UpdateEntrance([FromBody] Entrance entrance = default)
        {
            return Task.Run(() => _entranceRepository.UpdateEntrance(entrance));
        }
        [HttpDelete]
        [Authorize]
        public Task<ResultViewModel> DeleteEntrance(int id = default)
        {
            return Task.Run(() => _entranceRepository.DeleteEntrance(id));
        }

    }
}