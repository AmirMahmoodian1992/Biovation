using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Repositories;

namespace Biovation.Dashboard.Repository
{
    public class PingRepository
    {
        private readonly GenericRepository _repository;
        private readonly DatabaseConnectionInfo _connectionInfo;

        public PingRepository(GenericRepository repository, DatabaseConnectionInfo connectionInfo)
        {
            _repository = repository;
            _connectionInfo = connectionInfo;
        }


        public ResultViewModel AddPingTimeStamp(PingStatus pingModel)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@hostAddress", SqlDbType.NVarChar) { Value = pingModel.HostAddress },
                new SqlParameter("@DestinationAddress", SqlDbType.NVarChar) { Value = pingModel.DestinationAddress },
                new SqlParameter("@ttl", SqlDbType.Int) { Value = pingModel.TimeToLive },
                new SqlParameter("@roundTripTime", SqlDbType.BigInt) { Value = pingModel.RoundTripTime },
                new SqlParameter("@status", SqlDbType.NVarChar) { Value = pingModel.Status },

            };

            return _repository.ToResultList<ResultViewModel>("InsertPing", parameters,connectionInfo:_connectionInfo).Data.FirstOrDefault();

        }

        public List<AdminDevice> GetAddPingTimeStamp(int userId)
        {
            throw new NotImplementedException();
        }


    }
}
