using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RestaurantModels;
using Biovation.Repository.Sql.v1.RestaurantRepositories;

namespace Biovation.Service.Sql.v1.RestaurantServices
{
    public class ServeLogService
    {
        private readonly ServeLogRepository _serveLogRepository;

        public ServeLogService(ServeLogRepository serveLogRepository)
        {
            _serveLogRepository = serveLogRepository;
        }

        public Task<List<ServeLog>> GetServeLogs(int serveLogId = default(int))
        {
            return Task.Run(() => _serveLogRepository.GetServeLogs(serveLogId));
        }

        public Task<List<ServeLog>> GetServeLogsByReservationId(int userId = default, int foodId = default, int mealId = default, int deviceId = default)
        {
            return Task.Run(() => _serveLogRepository.GetServeLogsByValues(userId, foodId, mealId, deviceId));
        }
        
        public Task<ResultViewModel> AddServeLogs(List<ServeLog> serveLogs)
        {
            return Task.Run(() => _serveLogRepository.AddServeLogs(serveLogs));
        }

        public Task<ResultViewModel> ModifyServeLog(ServeLog serveLog)
        {
            return Task.Run(() => _serveLogRepository.ModifyServeLog(serveLog));
        }
    }
}
