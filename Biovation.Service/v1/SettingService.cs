using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository;
using Biovation.Repository.Sql.v1;

namespace Biovation.Service.Sql.v1
{
    public class SettingService
    {
        private readonly SettingRepository _settingRepository;

        public SettingService(SettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public Task<List<Setting>> GetSettings(string key = default)
        {
            return Task.Run(() => _settingRepository.GetSettings(key));
        }

        public Task<string> GetSetting(string key)
        {
            return Task.Run(() => _settingRepository.GetSetting(key));
        }

        public Task<T> GetSetting<T>(string key)
        {
            var value = _settingRepository.GetSetting(key);
            return Task.Run(() => value is null ? default : (T)Convert.ChangeType(value, typeof(T)));
        }

        public Task<ResultViewModel> ModifySetting(string key, string value)
        {
            return Task.Run(() => _settingRepository.ModifyFood(key, value));
        }
    }
}
