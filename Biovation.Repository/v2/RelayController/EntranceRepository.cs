using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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

        public ResultViewModel<PagingResult<Entrance>> GetEntrances(List<DeviceBasicInfo> devices, List<Scheduling> schedulings, int id = 0,
            string name = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name},
                new SqlParameter("@DevicesJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(devices) },
                new SqlParameter("@SchedulingsJson", SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(schedulings) },
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = description},
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize}
            };

            return _repository.ToResultList<PagingResult<Entrance>>("SelectEntranceByFilter", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
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
