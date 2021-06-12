using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Biovation.Repository.Sql.v2.RelayController
{
    public class RelayHubRepository
    {
        private readonly GenericRepository _repository;

        public RelayHubRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public ResultViewModel CreateRelayHubs(RelayHub relayHub)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@IpAddress", SqlDbType.NVarChar) {Value = relayHub.IpAddress},
                new SqlParameter("@Port", SqlDbType.Int) {Value = relayHub.Port},
                new SqlParameter("@Capacity", SqlDbType.Int) {Value = relayHub.Capacity},
                new SqlParameter("@RelayHubModelId", SqlDbType.Int) {Value = relayHub.RelayHubModel.Id},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = relayHub.Description}
            };

            return _repository.ToResultList<ResultViewModel>("InsertRelayHub", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel<PagingResult<RelayHub>> GetRelayHubs(int adminUserId = 0, int id = 0, string ipAddress = null, int port = 0,
            int capacity = 0, int relayHubModelId = default, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
                {
                    new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId },
                    new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                    new SqlParameter("@IpAddress", SqlDbType.NVarChar) {Value = ipAddress},
                    new SqlParameter("@Port", SqlDbType.Int) {Value = port},
                    new SqlParameter("@Capacity", SqlDbType.Int) {Value = capacity},
                    new SqlParameter("@RelayHubModelId", SqlDbType.Int) {Value = relayHubModelId},
                    new SqlParameter("@Description", SqlDbType.NVarChar) {Value = description},
                    new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                    new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize}
                };

            return _repository.ToResultList<PagingResult<RelayHub>>("SelectRelayHub", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel UpdateRelayHubs(RelayHub relayHub)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = relayHub.Id },
                new SqlParameter("@IpAddress", SqlDbType.NVarChar) {Value = relayHub.IpAddress ?? string.Empty},
                new SqlParameter("@Port", SqlDbType.Int) {Value = relayHub.Port},
                new SqlParameter("@Capacity", SqlDbType.Int) {Value = relayHub.Capacity},
                new SqlParameter("@RelayHubModelId", SqlDbType.Int) {Value = relayHub.RelayHubModel.Id},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = relayHub.Description}
            };

            return _repository.ToResultList<ResultViewModel>("InsertRelayHub", parameters).Data.FirstOrDefault();
        }


        public ResultViewModel DeleteRelayHubs(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id }
            };

            return _repository.ToResultList<ResultViewModel>("DeleteRelayHub", parameters).Data.FirstOrDefault();
        }
    }
}
