using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class ServiceInstanceService
    {
        private readonly ServiceInstanceRepository _serviceInstanceRepository;

        public ServiceInstanceService(ServiceInstanceRepository serviceInstanceRepository)
        {
            _serviceInstanceRepository = serviceInstanceRepository;
        }

        public Task<ResultViewModel> AddServiceInstanceWithoutId(string name, string version, string ip, int port, string description)
        {
            var serviceInstance = new ServiceInstance(null)
            {
                Name = name,
                Version = version,
                IpAddress = ip,
                Port = port,
                Description = description
            };
            return Task.Run(() => _serviceInstanceRepository.AddServiceInstance(serviceInstance));
        }
        public Task<ResultViewModel> AddServiceInstance(ServiceInstance serviceInstance)
        {
            return Task.Run(() => _serviceInstanceRepository.AddServiceInstance(serviceInstance));
        }

        public Task<ResultViewModel> ModifyServiceInstance(ServiceInstance serviceInstance)
        {
            return Task.Run(() => _serviceInstanceRepository.ModifyServiceInstance(serviceInstance));
        }

        public Task<ResultViewModel<List<ServiceInstance>>> GetServiceInstance(string id)
        {
            return Task.Run(() => _serviceInstanceRepository.GetServiceInstance(id));
        }

        public Task<ResultViewModel> DeleteServiceInstance(string id)
        {
            return Task.Run(() => _serviceInstanceRepository.DeleteServiceInstance(id));
        }
    }
}