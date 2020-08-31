using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.SQL.v2
{
    public class TimeZoneRepository
    {
        private readonly GenericRepository _repository;

        public TimeZoneRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public ResultViewModel ModifyTimeZone(TimeZone timeZone)
        {


            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", timeZone.Id),
                new SqlParameter("@Name", timeZone.Name)
            };

            var result = _repository.ToResultList<ResultViewModel>("ModifyTimeZone", parameters).Data.FirstOrDefault();

            if (result != null && result.Validate == 0)
            {
                return result;
            }

            foreach (var timeZoneDetail in timeZone.Details)
            {
                parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", timeZoneDetail.Id),
                new SqlParameter("@DayNumber", timeZoneDetail.DayNumber),
                new SqlParameter("@FromTime", timeZoneDetail.FromTime),
                new SqlParameter("@ToTime", timeZoneDetail.ToTime)
            };

                result = _repository.ToResultList<ResultViewModel>("ModifyTimeZoneDetail", parameters).Data.FirstOrDefault();

                if (result != null && result.Validate == 0)
                {
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public List<TimeZone> GetTimeZones()
        {
            return _repository.ToResultList<TimeZone>("SelectTimeZones", fetchCompositions: true).Data;
        }

        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public TimeZone GetTimeZone(int timeZoneId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", timeZoneId)
            };

            return _repository.ToResultList<TimeZone>("SelectTimeZoneByID", parameters, fetchCompositions: true).Data.FirstOrDefault();
        }

        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public ResultViewModel DeleteTimeZone(int timeZoneId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", timeZoneId)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteTimeZoneByID", parameters).Data.FirstOrDefault();
        }
    }
}
