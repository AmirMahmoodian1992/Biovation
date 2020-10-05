using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using Biovation.Domain.DashboardModels;
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


        public ResultViewModel AddPingTimeStamp(HistogramMetrics histogram)
        {

            var parameters = new List<SqlParameter>
            {
                //new SqlParameter("@hostAddress", SqlDbType.NVarChar) { Value =  histogram},
                //new SqlParameter("@DestinationAddress", SqlDbType.NVarChar) { Value = pingModel.DestinationAddress },
                //new SqlParameter("@RoundTripTime", SqlDbType.BigInt) { Value = pingModel.RoundTripTime },
                //new SqlParameter("@Data", SqlDbType.NVarChar) { Value = pingModel.Status }

            };

            return _repository.ToResultList<ResultViewModel>("InsertPing", parameters, connectionInfo: _connectionInfo).Data.FirstOrDefault();

        }
        public ResultViewModel CreatePingTable(string tableName)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@TableName", SqlDbType.NVarChar) { Value = tableName},
                new SqlParameter("@DbName", SqlDbType.NVarChar) { Value = _connectionInfo.InitialCatalog}

            };

            return _repository.ToResultList<ResultViewModel>("CreatePing", parameters,connectionInfo:_connectionInfo).Data.FirstOrDefault();

        }

        public List<AdminDevice> GetAddPingTimeStamp(int userId)
        {
            throw new NotImplementedException();
        }


    }
}
