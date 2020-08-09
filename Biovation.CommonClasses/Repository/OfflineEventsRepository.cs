using Biovation.CommonClasses.Models;
using DataAccessLayer;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DataAccessLayer.Repositories;

namespace Biovation.CommonClasses.Repository
{
    public class OfflineEventsRepository
    {
        private readonly GenericRepository _repository;

        public OfflineEventsRepository()
        {
            _repository = new GenericRepository();
        }

        /// <summary>
        /// <En>Get the offline events info of the device from database.</En>
        /// <Fa>اطلاعات همه اتفاقات زمان قطع بودن یک دستگاه خاص را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="deviceId">کد دستگاه</param>
        /// <returns></returns>
        public List<OfflineEvent> GetOfflineEvents(int deviceId = 0)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@deviceId", deviceId)
            };

            return _repository.ToResultList<OfflineEvent>("SelectOfflineEvents", parameters).Data;
        }

        public int AddOfflineEvent(OfflineEvent offlineEvent)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@data", offlineEvent.Data),
                new SqlParameter("@deviceCode", offlineEvent.DeviceCode),
                new SqlParameter("@type", offlineEvent.Type)
            };

            return _repository.ToResultList<int>("InsertOfflineEvent", parameters).Data.FirstOrDefault();
        }

        public bool DeleteOfflineEvent(int offlineEventId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@id", offlineEventId)
            };

            return _repository.ToResultList<bool>("DeleteOfflineEvent", parameters).Data.FirstOrDefault();
        }

        public bool DeleteOfflineEvent(OfflineEvent offlineEvent)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@deviceCode", offlineEvent.DeviceCode),
                new SqlParameter("@data", offlineEvent.Data),
                new SqlParameter("@type", offlineEvent.Type)
            };

            return _repository.ToResultList<bool>("DeleteOfflineEventByValues", parameters).Data.FirstOrDefault();
        }
    }
}
