using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
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

        public ResultViewModel<PagingResult<Relay>> GetRelay(List<Scheduling> schedulings, int id = 0,
           string name = null, int nodeNumber = 0, int relayHubId =0, int entranceId = 0, string description = null,
           int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name},
                new SqlParameter("@NodeNumber", SqlDbType.Int) {Value = nodeNumber},
                new SqlParameter("@entranceId", SqlDbType.Int) {Value = entranceId},
                new SqlParameter("@relayHubId", SqlDbType.Int) {Value = relayHubId},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = description},
                new SqlParameter("@SchedulingsJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(schedulings) },
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize}
            };

            return _repository.ToResultList<PagingResult<Relay>>("SelectRelayHubByFilter", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }
    }
}
