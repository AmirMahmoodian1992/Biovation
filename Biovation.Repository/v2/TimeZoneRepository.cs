using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;

namespace Biovation.Repository.Sql.v2
{
    public class TimeZoneRepository
    {
        private readonly GenericRepository _repository;

        public TimeZoneRepository(GenericRepository repository)
        {
            _repository = repository;
        }


        public ResultViewModel AddTimeZone(TimeZone timeZone)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", timeZone.Id),
                new SqlParameter("@Name", timeZone.Name),
                new SqlParameter("@TimeZoneDetails", JsonConvert.SerializeObject(timeZone.Details))
            };

            return _repository.ToResultList<ResultViewModel>("ModifyTimeZone", parameters).Data.FirstOrDefault();
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
                new SqlParameter("@Name", timeZone.Name),
                new SqlParameter("@TimeZoneDetails", JsonConvert.SerializeObject(timeZone.Details))
            };

            return _repository.ToResultList<ResultViewModel>("ModifyTimeZone", parameters).Data.FirstOrDefault();
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public ResultViewModel<PagingResult<TimeZone>> GetTimeZones(int id = default, int accessGroupId = default, string name = default, int pageNumber = default, int pageSize = default)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@id", SqlDbType.Int) {Value = id },
                new SqlParameter("@accessGroupId", SqlDbType.Int) {Value = accessGroupId },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name},
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},
            };

            return _repository.ToResultList<PagingResult<TimeZone>>("SelectTimeZones", sqlParameters, fetchCompositions: true, compositionDepthLevel: 4).FetchFromResultList();
        }

        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public ResultViewModel<TimeZone> GetTimeZone(int timeZoneId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", timeZoneId)
            };

            return _repository.ToResultList<TimeZone>("SelectTimeZoneByID", parameters, fetchCompositions: true).FetchFromResultList();
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
