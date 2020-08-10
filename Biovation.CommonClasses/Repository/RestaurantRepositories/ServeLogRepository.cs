using Biovation.CommonClasses.Models;
using DataAccessLayerCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.CommonClasses.Models.DataTransferObjects;
using Biovation.CommonClasses.Models.RestaurantModels;
using Newtonsoft.Json;

namespace Biovation.CommonClasses.Repository.RestaurantRepositories
{
    public class ServeLogRepository
    {
        private readonly GenericRepository _repository = new GenericRepository();

        public List<ServeLog> GetServeLogs(int serveLogId = default(int))
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@serveLogId", serveLogId)
            };

            return _repository.ToResultList<ServeLog>("SelectServeLogsById", parameters, fetchCompositions: true, compositionDepthLevel: 3).Data;
        }

        public List<ServeLog> GetServeLogsByValues(int userId = default, int foodId = default, int mealId = default, int deviceId = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@foodId", foodId),
                new SqlParameter("@mealId", mealId),
                new SqlParameter("@deviceId", deviceId)
            };

            return _repository.ToResultList<ServeLog>("SelectServeLogsByValues", parameters).Data;
        }

        public ResultViewModel AddServeLogs(List<ServeLog> serveLogs)
        {
            var serveLogDataObjects = serveLogs?.Select(sl => new ServeLogDTO
            {
                Id = sl.Id,
                UserId = sl.User.Id,
                FoodId = sl.Food.Id,
                MealId = sl.Meal.Id,
                DeviceId = sl.Device.DeviceId,
                StatusId = sl.Status.Id,
                Count = sl.Count,
                TimeStamp = sl.TimeStamp,
                IsSynced = true
            });

            var serializedObj = JsonConvert.SerializeObject(serveLogDataObjects);
            var serveLogsDataTable = JsonConvert.DeserializeObject<DataTable>(serializedObj);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@serveLogTable", serveLogsDataTable)
            };

            return _repository.ToResultList<ResultViewModel>("InsertServeLogsBatch", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel ModifyServeLog(ServeLog serveLog)
        {
            throw new NotImplementedException();
        }
    }
}
