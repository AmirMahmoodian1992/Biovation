using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    public class SettingController : ControllerBase
    {
        private readonly SettingRepository _settingRepository;


        public SettingController(SettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        [HttpGet]
        [Route("GetSettings")]
        [Authorize]

        public Task<ResultViewModel<List<Setting>>> GetSettings(string key = default)
        {
            return Task.Run(() => _settingRepository.GetSettings(key));
        }


        [HttpGet]
        [Route("GetSetting")]
        [Authorize]

        public Task<ResultViewModel<string>> GetSetting(string key)
        {
            return Task.Run(() => _settingRepository.GetSetting(key));
        }

    }
}
