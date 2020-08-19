using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;

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
        public List<AdminDeviceGroup> GetAdminDeviceGroupsByUserId(int userId)
        {

            var parameters = new List<SqlParameter>// { new SqlParameter("@PersonId", personId) };
            { new SqlParameter("@UserId", SqlDbType.BigInt) { Value = userId }};

            return _repository.ToResultList<AdminDeviceGroup>("SelectAdminDeviceGroupsByUserId", parameters).Data;

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
