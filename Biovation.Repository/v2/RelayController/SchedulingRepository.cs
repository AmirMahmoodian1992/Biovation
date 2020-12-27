using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v2.RelayController
{
    public class SchedulingRepository
    {
        private readonly GenericRepository _repository;

        public SchedulingRepository(GenericRepository repository)
        {
            _repository = repository;
        }




        public ResultViewModel CreateScheduling(Scheduling scheduling)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Description", SqlDbType.DateTime) {Value = scheduling.StartTime},
                new SqlParameter("@Description", SqlDbType.DateTime) {Value = scheduling.EndTime},
                new SqlParameter("@RelayHubModel", SqlDbType.NVarChar) {Value = scheduling.Mode},
            };
            return _repository.ToResultList<ResultViewModel>("InsertScheduling", parameters).Data.FirstOrDefault();

        }
        public ResultViewModel<PagingResult<Scheduling>> GetScheduling(int id = 0,
            TimeSpan startTime = default, TimeSpan endTime = default, string mode = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                new SqlParameter("@Description", SqlDbType.DateTime) {Value = startTime},
                new SqlParameter("@Description", SqlDbType.DateTime) {Value = endTime},
                new SqlParameter("@RelayHubModel", SqlDbType.NVarChar) {Value = mode},
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},

            };
            return _repository.ToResultList<PagingResult<Scheduling>>($"SelectSchedulingByFilter", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();

        }



        public ResultViewModel UpdateScheduling(Scheduling scheduling)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = scheduling.Id },
                new SqlParameter("@Description", SqlDbType.DateTime) {Value = scheduling.StartTime},
                new SqlParameter("@Description", SqlDbType.DateTime) {Value = scheduling.EndTime},
                new SqlParameter("@RelayHubModel", SqlDbType.NVarChar) {Value = scheduling.Mode},
            };
            return _repository.ToResultList<ResultViewModel>("UpdateScheduling", parameters).Data.FirstOrDefault();

        }

        public ResultViewModel DeleteScheduling(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id }
            };
            return _repository.ToResultList<ResultViewModel>("DeleteScheduling", parameters).Data.FirstOrDefault();

        }

    }
}
