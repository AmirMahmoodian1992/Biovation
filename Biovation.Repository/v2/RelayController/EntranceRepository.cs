﻿using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain.RelayModels;

namespace Biovation.Repository.Sql.v2.RelayController
{
    public class EntranceRepository
    {
        private readonly GenericRepository _repository;

        public EntranceRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public ResultViewModel CreateEntrance(Entrance entrance)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = entrance.Name},
                new SqlParameter("@DevicesJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(entrance.Devices) },
                new SqlParameter("@SchedulingJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(entrance.Schedulings) },
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = entrance.Description}
            };

            return _repository.ToResultList<ResultViewModel>("InsertEntrance", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel<PagingResult<Entrance>> GetEntrances(int deviceId, int schedulingId, int id = 0,
            string name = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name??string.Empty},
                new SqlParameter("@DeviceId", SqlDbType.Int) { Value = deviceId },
                new SqlParameter("@SchedulingId", SqlDbType.Int) { Value = schedulingId },
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize}
            };

            return _repository.ToResultList<PagingResult<Entrance>>("SelectEntrance", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel UpdateEntrance(Entrance entrance)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = entrance.Id },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = entrance.Name},
                new SqlParameter("@DevicesJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(entrance.Devices) },
                new SqlParameter("@SchedulingJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(entrance.Schedulings) },
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = entrance.Description}
            };

            return _repository.ToResultList<ResultViewModel>("UpdateEntrance", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteEntrance(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id }
            };

            return _repository.ToResultList<ResultViewModel>("DeleteEntrance", parameters).Data.FirstOrDefault();
        }
    }
}
