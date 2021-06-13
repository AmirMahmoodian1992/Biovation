using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
                new SqlParameter("@StartTime", SqlDbType.BigInt) {Value = scheduling.StartTime.Ticks},
                new SqlParameter("@EndTime", SqlDbType.BigInt) {Value = scheduling.EndTime.Ticks},
                new SqlParameter("@Mode", SqlDbType.Int) {Value = scheduling.Mode}
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
                new SqlParameter("@StartTime", SqlDbType.BigInt) {Value = startTime.Ticks},
                new SqlParameter("@EndTime", SqlDbType.BigInt) {Value = endTime.Ticks},
                new SqlParameter("@Mode", SqlDbType.Int) {Value = mode},
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize}

            };

            return _repository.ToResultList<PagingResult<Scheduling>>("SelectScheduling", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel UpdateScheduling(Scheduling scheduling)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = scheduling.Id },
                new SqlParameter("@StartTime", SqlDbType.BigInt) {Value = scheduling.StartTime.Ticks},
                new SqlParameter("@EndTime", SqlDbType.BigInt) {Value = scheduling.EndTime.Ticks},
                new SqlParameter("@Mode", SqlDbType.Int) {Value = scheduling.Mode}
            };

            return _repository.ToResultList<ResultViewModel>("UpdateScheduling", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteScheduling(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };

            return _repository.ToResultList<ResultViewModel>("DeleteScheduling", parameters).Data.FirstOrDefault();
        }
    }
}
