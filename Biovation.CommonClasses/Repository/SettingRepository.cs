using Biovation.CommonClasses.Models;
using DataAccessLayer.Repositories;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Biovation.CommonClasses.Repository
{
    public class SettingRepository
    {
        private readonly GenericRepository _repository = new GenericRepository();

        public List<Setting> GetSettings(string key = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@key", key)
            };

            return _repository.ToResultList<Setting>("SelectSettings", parameters).Data;
        }

        public string GetSetting(string key)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@key", key)
            };

            return _repository.ToResultList<Setting>("SelectSettings", parameters).Data.FirstOrDefault()?.Value;
        }

        public ResultViewModel ModifyFood(string key, string value)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Key", key),
                new SqlParameter("@Value", value)
            };

            return _repository.ToResultList<ResultViewModel>("ModifySetting", parameters).Data.FirstOrDefault();
        }
    }
}
