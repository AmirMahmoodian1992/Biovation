using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Repository.Sql.v1
{
    public class ServiceInstanceRepository
    {
        private readonly GenericRepository _repository;

        public ServiceInstanceRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public Task<ResultViewModel> AddServiceInstance(string name, string version, string ip, int port, string description)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name},
                    new SqlParameter("@Version", SqlDbType.NVarChar) {Value = version},
                    new SqlParameter("@Ip", SqlDbType.NVarChar) {Value = ip},
                    new SqlParameter("@Port", SqlDbType.Int) {Value = port},
                    new SqlParameter("@Description", SqlDbType.NVarChar) {Value = description},
                };

                return _repository.ToResultList<ResultViewModel>("InsertServiceInstance", parameters).Data.FirstOrDefault();
            });
        }

        public Task<ResultViewModel> ModifyServiceInstance(ServiceInstance serviceInstance)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", SqlDbType.NVarChar) {Value = serviceInstance.Id},
                    new SqlParameter("@Name", SqlDbType.NVarChar) {Value = serviceInstance.Name},
                    new SqlParameter("@Version", SqlDbType.NVarChar) {Value = serviceInstance.Version},
                    new SqlParameter("@Ip", SqlDbType.NVarChar) {Value = serviceInstance.Ip},
                    new SqlParameter("@Port", SqlDbType.Int) {Value = serviceInstance.Port},
                    new SqlParameter("@Description", SqlDbType.NVarChar) {Value = serviceInstance.Description},
                };

                return _repository.ToResultList<ResultViewModel>("UpdateServiceInstance", parameters).Data.FirstOrDefault();
            });
        }

        public Task<ResultViewModel<ServiceInstance>> GetServiceInstance(string id)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", SqlDbType.NVarChar) {Value = id},
                };

                return _repository.ToResultList<ResultViewModel<ServiceInstance>>("SelectServiceInstance", parameters).Data.FirstOrDefault();
            });
        }

        public Task<ResultViewModel> DeleteServiceInstance(string id)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", SqlDbType.NVarChar) {Value = id},
                };
                return _repository.ToResultList<ResultViewModel>("DeleteServiceInstance", parameters).Data.FirstOrDefault();
            });
        }
    }
}
