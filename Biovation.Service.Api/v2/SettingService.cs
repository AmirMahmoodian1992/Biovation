using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class SettingService
    {
        private readonly SettingRepository _settingRepository;

        public SettingService(SettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public Task<ResultViewModel<List<Setting>>> GetSettings(string key = default, string token = default)
        {
            return Task.Run(() =>  _settingRepository.GetSettings(key, token));
        }

        public Task<ResultViewModel<string>> GetSetting(string key, string token = default)
        {
            return Task.Run(() => _settingRepository.GetSetting(key, token));
        }
    }
}
