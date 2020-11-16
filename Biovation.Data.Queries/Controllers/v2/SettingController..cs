using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
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

        [HttpGet]
        [Authorize]
        [Route("GetSettings")]
        public Task<ResultViewModel<List<Setting>>> GetSettings(string key = default)
        {
            return Task.Run(() => _settingRepository.GetSettings(key));
        }

        [HttpGet]
        [Authorize]
        [Route("GetSetting")]
        public Task<ResultViewModel<string>> GetSetting(string key)
        {
            return Task.Run(() => _settingRepository.GetSetting(key));
        }

    }
}
