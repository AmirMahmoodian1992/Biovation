using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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

        public ResultViewModel InsertEntrance(Entrance entrance)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter(nameof(entrance.Name), SqlDbType.NVarChar) {Value = entrance.Name},
                new SqlParameter(nameof(entrance.Code), SqlDbType.Int) {Value = entrance.Code},
                new SqlParameter(nameof(entrance.Cameras), SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(entrance.Cameras) },
                new SqlParameter(nameof(entrance.Schedulings), SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(entrance.Schedulings) },
                new SqlParameter(nameof(entrance.Description), SqlDbType.NVarChar) {Value = entrance.Description}
            };

            return _repository.ToResultList<ResultViewModel>(MethodBase.GetCurrentMethod()?.Name, parameters).Data.FirstOrDefault();
        }

        public ResultViewModel<PagingResult<Entrance>> SelectEntrance(int deviceId, int schedulingId, int id = 0, int code = 0,
            string name = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter(nameof(id), SqlDbType.Int) {Value = id },
                new SqlParameter(nameof(code), SqlDbType.Int) {Value = code },
                new SqlParameter(nameof(name), SqlDbType.NVarChar) {Value = name??string.Empty},
                new SqlParameter(nameof(deviceId), SqlDbType.Int) { Value = deviceId },
                new SqlParameter(nameof(schedulingId), SqlDbType.Int) { Value = schedulingId },
                new SqlParameter(nameof(pageNumber), SqlDbType.Int) {Value = pageNumber},
                new SqlParameter(nameof(pageSize), SqlDbType.Int) {Value = pageSize}
            };

            return _repository.ToResultList<PagingResult<Entrance>>(MethodBase.GetCurrentMethod()?.Name, sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel UpdateEntrance(Entrance entrance)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter(nameof(entrance.Name), SqlDbType.NVarChar) {Value = entrance.Name},
                new SqlParameter(nameof(entrance.Code), SqlDbType.Int) {Value = entrance.Code},
                new SqlParameter(nameof(entrance.Cameras), SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(entrance.Cameras) },
                new SqlParameter(nameof(entrance.Schedulings), SqlDbType.VarChar) { Value = JsonConvert.SerializeObject(entrance.Schedulings) },
                new SqlParameter(nameof(entrance.Description), SqlDbType.NVarChar) {Value = entrance.Description}
            };

            return _repository.ToResultList<ResultViewModel>(MethodBase.GetCurrentMethod()?.Name, parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteEntrance(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter(nameof(id), SqlDbType.Int) {Value = id }
            };

            return _repository.ToResultList<ResultViewModel>(MethodBase.GetCurrentMethod()?.Name, parameters).Data.FirstOrDefault();
        }
    }
}
