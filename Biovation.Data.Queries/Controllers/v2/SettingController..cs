using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/queries/v2/[controller]")]
    public class SettingController : Controller
    {
        private readonly SettingRepository _settingRepository;


        public SettingController(SettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        [HttpGet]
        [Route("GetSettings")]
        public Task<ResultViewModel<List<Setting>>> GetSettings(string key = default)
        {
            return Task.Run(() => _settingRepository.GetSettings(key));
        }


        [HttpGet]
        [Route("GetSetting")]
        public Task<ResultViewModel<string>> GetSetting(string key)
        {
            return Task.Run(() => _settingRepository.GetSetting(key));
        }

    }
}
