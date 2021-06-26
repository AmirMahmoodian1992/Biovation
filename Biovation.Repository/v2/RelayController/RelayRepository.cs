﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using Biovation.Domain.RelayModels;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;

namespace Biovation.Repository.Sql.v2.RelayController
{
    public class RelayRepository
    {
        private readonly GenericRepository _repository;

        public RelayRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public ResultViewModel<PagingResult<Relay>> GetRelay(int adminUserId = 0, int id = 0,
           string name = null, int nodeNumber = 0, int relayHubId =0, string description = null, int schedulingId = default, int deviceId = default,
           int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId },
                new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name ?? string.Empty},
                new SqlParameter("@NodeNumber", SqlDbType.Int) {Value = nodeNumber},
                new SqlParameter("@relayHubId", SqlDbType.Int) {Value = relayHubId},
                new SqlParameter("@schedulingId", SqlDbType.Int) {Value = schedulingId},
                new SqlParameter("@deviceId", SqlDbType.Int) {Value = deviceId},
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize}
            };

            return _repository.ToResultList<PagingResult<Relay>>("SelectRelay", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel CreateRelay(Relay relay)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = relay.Name},
                new SqlParameter("@NodeNumber", SqlDbType.Int) {Value = relay.NodeNumber},
                //new SqlParameter("@relayHubId", SqlDbType.Int) {Value = relay.RelayHub.Id},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = relay.Description},
                new SqlParameter("@SchedulingsJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(relay.Schedulings) },
                new SqlParameter("@DevicesJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(relay.Devices) }
            };

            return _repository.ToResultList<ResultViewModel>("CreateRelay", sqlParameter).Data.FirstOrDefault();
        }

        public ResultViewModel UpdateRelay(Relay relay)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.NVarChar) {Value = relay.Id},
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = relay.Name},
                new SqlParameter("@NodeNumber", SqlDbType.Int) {Value = relay.NodeNumber},
                new SqlParameter("@relayHubId", SqlDbType.Int) {Value = relay.RelayHub.Id},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = relay.Description},
                new SqlParameter("@SchedulingsJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(relay.Schedulings) },
                new SqlParameter("@DevicesJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(relay.Devices) },
            };

            return _repository.ToResultList<ResultViewModel>("UpdateRelay", sqlParameter).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteRelay(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id }
            };

            return _repository.ToResultList<ResultViewModel>("DeleteRelay", parameters).Data.FirstOrDefault();
        }
    }
}
