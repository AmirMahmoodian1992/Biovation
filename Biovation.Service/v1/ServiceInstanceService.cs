using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.Sql.v1;
using System.Threading.Tasks;

namespace Biovation.Service.Sql.v1
{
    public class ServiceInstanceService
    {
        private readonly ServiceInstanceRepository _serviceInstanceRepository;

        public ServiceInstanceService(ServiceInstanceRepository serviceInstanceRepository)
        {
            _serviceInstanceRepository = serviceInstanceRepository;
        }

        public Task<ResultViewModel> AddServiceInstance(ServiceInstance serviceInstance)
        {
            return Task.Run(() => _serviceInstanceRepository.AddServiceInstance(serviceInstance));
        }

        public Task<ResultViewModel> ModifyServiceInstance(ServiceInstance serviceInstance)
        {
            return Task.Run(() => _serviceInstanceRepository.ModifyServiceInstance(serviceInstance));
        }

        public Task<ResultViewModel<List<ServiceInstance>>> GetServiceInstance(string id = default)
        {
            return Task.Run(() => _serviceInstanceRepository.GetServiceInstance(id));
        }

        public Task<ResultViewModel> DeleteServiceInstance(string id)
        {
            return Task.Run(() => _serviceInstanceRepository.DeleteServiceInstance(id));
        }
    }
}