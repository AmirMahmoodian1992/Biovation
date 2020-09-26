using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v1
{
    public class SettingService
    {
        private readonly SettingRepository _settingRepository;

        public SettingService(SettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public Task<ResultViewModel<List<Setting>>> GetSettings(string key = default)
        {
            return Task.Run(() => _settingRepository.GetSettings(key));
        }

        public Task<ResultViewModel<string>> GetSetting(string key)
        {
            return Task.Run(() => _settingRepository.GetSetting(key));
        }
    }
}
