using System;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain.RelayModels;

namespace Biovation.Repository.Sql.v2.RelayController
{
    public class RelayHubRepository
    {
        private readonly GenericRepository _repository;
        private readonly RelayRepository _relayRepository;

        public RelayHubRepository(GenericRepository repository, RelayRepository relayRepository)
        {
            _repository = repository;
            _relayRepository = relayRepository;
        }

        public ResultViewModel CreateRelayHubs(RelayHub relayHub)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@IpAddress", SqlDbType.NVarChar) {Value = relayHub.IpAddress},
                new SqlParameter("@Port", SqlDbType.Int) {Value = relayHub.Port},
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = relayHub.Name ?? string.Empty},
                new SqlParameter("@Active", SqlDbType.Bit) {Value = relayHub.Active},
                new SqlParameter("@Capacity", SqlDbType.Int) {Value = relayHub.Capacity},
                new SqlParameter("@RelayHubModelId", SqlDbType.Int) {Value = relayHub.RelayHubModel.Id},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = relayHub.Description ?? string.Empty}
            };

            var insertRelayHubRes =  _repository.ToResultList<ResultViewModel>("InsertRelayHub", parameters).Data.FirstOrDefault();
            if (insertRelayHubRes == null || !insertRelayHubRes.Success || insertRelayHubRes.Id == 0) return insertRelayHubRes;
            foreach (var relay in relayHub.Relays)
            {
                relay.RelayHub = new RelayHub
                {
                    Id = Convert.ToInt32(insertRelayHubRes.Id),
                };
            }
            if (relayHub.Relays.Select(relay => _relayRepository.CreateRelay(relay)).Any(relayResult => !relayResult.Success))
            {
                return new ResultViewModel()
                {
                    Success = false
                };
            }

            return insertRelayHubRes;
        }

        public ResultViewModel<PagingResult<RelayHub>> GetRelayHubs(int adminUserId = 0, int id = 0, string ipAddress = default, int port = 0, string name = default,
            int capacity = 0, int relayHubModelId = default, string description = null, string filterText = default, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
                {
                    new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId },
                    new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                    new SqlParameter("@IpAddress", SqlDbType.NVarChar) {Value = ipAddress},
                    new SqlParameter("@Port", SqlDbType.Int) {Value = port},
                    new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name},
                    new SqlParameter("@Capacity", SqlDbType.Int) {Value = capacity},
                    new SqlParameter("@RelayHubModelId", SqlDbType.Int) {Value = relayHubModelId},
                    new SqlParameter("@Description", SqlDbType.NVarChar) {Value = description},
                    new SqlParameter("@" + nameof(filterText), SqlDbType.NVarChar) {Value = filterText},
                    new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                    new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize}
                };

            return _repository.ToResultList<PagingResult<RelayHub>>("SelectRelayHub", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<RelayHubModel>> GetRelayHubModels(int id = 0, string name = default, int brandId = default, int manufactureCode = 0,
            int defaultPortNumber = 0, int defaultCapacity = default, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name},
                new SqlParameter("@BrandId", SqlDbType.Int) {Value = brandId},
                new SqlParameter("@ManufactureCode", SqlDbType.Int) {Value = manufactureCode},
                new SqlParameter("@DefaultPortNumber", SqlDbType.Int) {Value = defaultPortNumber},
                new SqlParameter("@DefaultCapacity", SqlDbType.Int) {Value = defaultCapacity},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = description},
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize}
            };

            return _repository.ToResultList<PagingResult<RelayHubModel>>("SelectRelayHubModel", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel UpdateRelayHubs(RelayHub relayHub)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = relayHub.Id },
                new SqlParameter("@IpAddress", SqlDbType.NVarChar) {Value = relayHub.IpAddress ?? string.Empty},
                new SqlParameter("@Port", SqlDbType.Int) {Value = relayHub.Port},
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = relayHub.Name},
                new SqlParameter("@Active", SqlDbType.Bit) {Value = relayHub.Active},
                new SqlParameter("@Capacity", SqlDbType.Int) {Value = relayHub.Capacity},
                new SqlParameter("@RelayHubModelId", SqlDbType.Int) {Value = relayHub.RelayHubModel.Id},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = relayHub.Description}
            };

            var insertRelayHubRes =  _repository.ToResultList<ResultViewModel>("UpdateRelayHub", parameters).Data.FirstOrDefault();
            if (insertRelayHubRes == null || !insertRelayHubRes.Success) return insertRelayHubRes;
            if (relayHub.Relays.Select(relay => _relayRepository.UpdateRelay(relay)).Any(relayResult => !relayResult.Success))
            {
                return new ResultViewModel()
                {
                    Success = false
                };
            }
            return insertRelayHubRes;
        }


        public ResultViewModel DeleteRelayHubs(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id }
            };

            return _repository.ToResultList<ResultViewModel>("DeleteRelayHubById", parameters).Data.FirstOrDefault();
        }
    }
}
