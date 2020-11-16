using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class SettingController : ControllerBase
    {
        private readonly SettingRepository _settingRepository;
        
        public SettingController(SettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        [HttpPost]
        [Route("ModifySetting")]
        [Authorize]

        public Task<ResultViewModel> ModifySetting(string value, string key)
        {
            return Task.Run(() => _settingRepository.ModifySetting(value, key));
        }
        }
}
