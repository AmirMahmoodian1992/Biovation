using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Dashboard;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository
{
    public class AdminDeviceRepository
    {
        private readonly GenericRepository _repository;

        public AdminDeviceRepository(GenericRepository repository)
        {
            _repository = repository;
        }


        public List<AdminDeviceGroup> AddPingTimeStamp(PingModel pingModel, string address)
        {

            var parameters = new List<SqlParameter>// { new SqlParameter("@PersonId", personId) };
            {
                new SqlParameter("@hostAddress", SqlDbType.NVarChar) { Value = pingModel.hostAddress },
                new SqlParameter("@DestinationAddress", SqlDbType.NVarChar) { Value = pingModel.DestinationAddress },
                new SqlParameter("@ttl", SqlDbType.BigInt) { Value = pingModel.ttl },
                new SqlParameter("@roundTripTime", SqlDbType.BigInt) { Value = pingModel.roundTripTime },
                new SqlParameter("@status", SqlDbType.BigInt) { Value = pingModel.status },

            };

            return _repository.ToResultList<AdminDeviceGroup>("SelectAdminDeviceGroupsByUserId", parameters).Data;

        }

        public List<AdminDevice> GetAddPingTimeStamp(int userId)
        {

            var parameters = new List<SqlParameter>// { new SqlParameter("@PersonId", personId) };
            { new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }};

            return _repository.ToResultList<AdminDevice>("SelectAdminDevicesByUserId", parameters).Data;

        }


    }
}
