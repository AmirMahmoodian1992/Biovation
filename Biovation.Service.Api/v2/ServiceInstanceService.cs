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

        public Task<ResultViewModel> AddServiceInstanceWithoutId(string name, string version, string ip, int port, string description, string token = default)
        {
            var serviceInstance = new ServiceInstance()
            {
                Name = name,
                Version = version,
                Ip = ip,
                Port = port,
                Description = description
            };
            return Task.Run(() => _serviceInstanceRepository.AddServiceInstance(serviceInstance, token));
        }
        public Task<ResultViewModel> AddServiceInstance(ServiceInstance serviceInstance, string token = default)
        {
            return Task.Run(() => _serviceInstanceRepository.AddServiceInstance(serviceInstance, token));
        }

        public Task<ResultViewModel> ModifyServiceInstance(ServiceInstance serviceInstance, string token = default)
        {
            return Task.Run(() => _serviceInstanceRepository.ModifyServiceInstance(serviceInstance, token));
        }

        public Task<ResultViewModel<ServiceInstance>> GetServiceInstance(string id, string token = default)
        {
            return Task.Run(() => _serviceInstanceRepository.GetServiceInstance(id, token));
        }

        public Task<ResultViewModel> DeleteServiceInstance(string id, string token = default)
        {
            return Task.Run(() => _serviceInstanceRepository.DeleteServiceInstance(id, token));
        }
    }
}