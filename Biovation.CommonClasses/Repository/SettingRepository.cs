﻿using Biovation.CommonClasses.Models;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Biovation.CommonClasses.Repository
{
    public class SettingRepository
    {
        private readonly GenericRepository _repository;

        public SettingRepository(GenericRepository repository)
        {
            _repository = repository;
        }

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
