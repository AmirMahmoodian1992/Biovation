using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Biovation.Repository.v2
{
    public class AdminDeviceRepository
    {
        private readonly GenericRepository _repository;

        public AdminDeviceRepository(GenericRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک دستگاه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="userId">کد دستگاه</param>
        /// <returns></returns>
        public ResultViewModel<PagingResult<AdminDeviceGroup>> GetAdminDeviceGroupsByUserId(int userId, int pageNumber = 0, int PageSize = 0, int nestingDepthLevel = 4)
        {

            var parameters = new List<SqlParameter>// { new SqlParameter("@PersonId", personId) };
            {
              new SqlParameter("@UserId", SqlDbType.BigInt) { Value = userId },
              new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber },
              new SqlParameter("@PageSize", SqlDbType.Int) { Value = PageSize }
            };

            return _repository.ToResultList<PagingResult<AdminDeviceGroup>>("SelectAdminDeviceGroupsByUserId", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public List<AdminDevice> GetAdminDevicesByUserId(int userId)
        {

            var parameters = new List<SqlParameter>// { new SqlParameter("@PersonId", personId) };
            { new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }};

            return _repository.ToResultList<AdminDevice>("SelectAdminDevicesByUserId", parameters).Data;

        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک دستگاه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="adminDevice">کد دستگاه</param>
        /// <returns></returns>
        public ResultViewModel ModifyAdminDevice(string adminDevice)
        {

            var parameters = new List<SqlParameter>// { new SqlParameter("@PersonId", personId) };
            { new SqlParameter("@StrXml", SqlDbType.NVarChar) { Value = adminDevice }};

            var result = _repository.ToResultList<ResultViewModel>("ModifyAdminDevice", parameters).Data.FirstOrDefault();

            return result;

        }


    }
}
