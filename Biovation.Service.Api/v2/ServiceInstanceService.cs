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

        public Task<ResultViewModel> AddServiceInstance(string name, string version, string ip, int port, string description)
        {
            return Task.Run(() => _serviceInstanceRepository.AddServiceInstance(name, version, ip, port, description));
        }

        public Task<ResultViewModel> ModifyServiceInstance(ServiceInstance serviceInstance)
        {
            return Task.Run(() => _serviceInstanceRepository.ModifyServiceInstance(serviceInstance));
        }

        public Task<ResultViewModel<ServiceInstance>> GetServiceInstance(string id)
        {
            return Task.Run(() => _serviceInstanceRepository.GetServiceInstance(id));
        }

        public Task<ResultViewModel> DeleteServiceInstance(string id)
        {
            return Task.Run(() => _serviceInstanceRepository.DeleteServiceInstance(id));
        }
    }
}