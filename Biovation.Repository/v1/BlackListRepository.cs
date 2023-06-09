﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;

namespace Biovation.Repository.Sql.v1
{
    public class BlackListRepository
    {
        private readonly GenericRepository _repository;

        public BlackListRepository(GenericRepository repository)
        {
            _repository = repository;
        }
        public ResultViewModel CreateBlackList(BlackList blackList)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Description",blackList.Description),
                new SqlParameter("@DeviceId", blackList.Device.DeviceId),
                new SqlParameter("@UserId", blackList.User.Id),
                new SqlParameter("@StartDate", blackList.StartDate),
                new SqlParameter("@EndDate", blackList.EndDate),
                new SqlParameter("@IsDeleted", blackList.IsDeleted),

            };
            return _repository.ToResultList<ResultViewModel>("InsertBlackList", parameters).Data.FirstOrDefault();
        }
        public List<BlackList> GetBlacklist(int id = default, int userId = default, int deviceId = 0, DateTime? startDate = null, DateTime? endDate = null,bool isDeleted=default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@DeviceId", deviceId),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@IsDeleted", isDeleted),
            };

            return _repository.ToResultList<BlackList>("SelectBlackList", parameters, fetchCompositions: true).Data;
        }
        public List<BlackList> GetActiveBlacklist(int id = default, int userId = default, int deviceId = 0, DateTime? Today = null, bool isDeleted = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@DeviceId", deviceId),
                new SqlParameter("@Today", Today),
                new SqlParameter("@IsDeleted", isDeleted),
            };

            return _repository.ToResultList<BlackList>("SelectActiveBlackList", parameters, fetchCompositions: true).Data;
        }

        public ResultViewModel DeleteBlackList(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", id),
            };
            var result = _repository.ToResultList<ResultViewModel>("DeleteBlackList", parameters).Data.FirstOrDefault();
            return result;
        }

        public ResultViewModel DeleteBlackLists(List<uint> deviceIds)
        {

            var parameters = new List<SqlParameter> { new SqlParameter("@json", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(deviceIds) } };

            return _repository.ToResultList<ResultViewModel>("DeleteBlackLists", parameters).Data.FirstOrDefault();
        }
        public ResultViewModel ChangeBlackList(BlackList blackList)
        {
            var parameters = new List<SqlParameter> 
            {
                new SqlParameter("@Id", blackList.Id),
                new SqlParameter("@Description", blackList.Description),
                new SqlParameter("@DeviceId", blackList.Device.DeviceId),
                new SqlParameter("@UserId",blackList.User.Id),
                new SqlParameter("@StartDate", blackList.StartDate),
                new SqlParameter("@EndDate", blackList.EndDate),
                new SqlParameter("@IsDeleted", blackList.IsDeleted),
            };
            var result = _repository.ToResultList<ResultViewModel>("ChangeBlackList", parameters).Data.FirstOrDefault();
            return result;
        }
    
    }
}