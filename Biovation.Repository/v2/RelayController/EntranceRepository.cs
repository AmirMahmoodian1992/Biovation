using Biovation.Domain;
using Biovation.Domain.RelayModels;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

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
                new SqlParameter(nameof(entrance.Description), SqlDbType.NVarChar) {Value = entrance.Description ?? string.Empty}
            };
            if (!(entrance.Cameras is null))
            {
                parameters.Add(new SqlParameter(nameof(entrance.Cameras) + "Json", SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(entrance.Cameras) });
            }
            if (!(entrance.Schedulings is null))
            {
                parameters.Add(new SqlParameter(nameof(entrance.Schedulings) + "Json", SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(entrance.Schedulings) });
            }
            if (!(entrance.Devices is null))
            {
                parameters.Add(new SqlParameter(nameof(entrance.Devices) + "Json", SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(entrance.Devices) });
            }

            return _repository.ToResultList<ResultViewModel>(MethodBase.GetCurrentMethod().Name, parameters).Data.FirstOrDefault();
        }

        public ResultViewModel<PagingResult<Entrance>> SelectEntrance(int cameraId = 0, int schedulingId = 0, int deviceId = 0, int id = 0, int code = 0,
            string name = null, string description = null, string filterText = default, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 6)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter(nameof(id), SqlDbType.Int) {Value = id },
                new SqlParameter(nameof(code), SqlDbType.Int) {Value = code },
                new SqlParameter(nameof(name), SqlDbType.NVarChar) {Value = name??string.Empty},
                new SqlParameter(nameof(cameraId), SqlDbType.Int) { Value = cameraId },
                new SqlParameter(nameof(schedulingId), SqlDbType.Int) { Value = schedulingId },
                new SqlParameter(nameof(deviceId), SqlDbType.Int) { Value = deviceId },
                new SqlParameter(nameof(filterText), SqlDbType.NVarChar) { Value = filterText },
                new SqlParameter(nameof(pageNumber), SqlDbType.Int) {Value = pageNumber},
                new SqlParameter(nameof(pageSize), SqlDbType.Int) {Value = pageSize}
            };

            return _repository.ToResultList<PagingResult<Entrance>>(MethodBase.GetCurrentMethod().Name, sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel UpdateEntrance(Entrance entrance)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter(nameof(entrance.Id), SqlDbType.Int) {Value = entrance.Id},
                new SqlParameter(nameof(entrance.Name), SqlDbType.NVarChar) {Value = entrance.Name},
                new SqlParameter(nameof(entrance.Code), SqlDbType.BigInt) {Value = entrance.Code},
                new SqlParameter(nameof(entrance.Description), SqlDbType.NVarChar) {Value = entrance.Description ?? string.Empty}
            };
            if (!(entrance.Cameras is null))
            {
                parameters.Add(new SqlParameter(nameof(entrance.Cameras) + "Json", SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(entrance.Cameras) });
            }
            if (!(entrance.Schedulings is null))
            {
                parameters.Add(new SqlParameter(nameof(entrance.Schedulings) + "Json", SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(entrance.Schedulings) });
            }
            if (!(entrance.Devices is null))
            {
                parameters.Add(new SqlParameter(nameof(entrance.Devices) + "Json", SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(entrance.Devices) });
            }

            return _repository.ToResultList<ResultViewModel>(MethodBase.GetCurrentMethod().Name, parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteEntrance(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter(nameof(id), SqlDbType.Int) {Value = id }
            };

            return _repository.ToResultList<ResultViewModel>(MethodBase.GetCurrentMethod().Name, parameters).Data.FirstOrDefault();
        }
    }
}
