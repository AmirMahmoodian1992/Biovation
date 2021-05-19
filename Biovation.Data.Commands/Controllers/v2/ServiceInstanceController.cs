using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class ServiceInstanceController : ControllerBase
    {
        private readonly ServiceInstanceRepository _serviceInstanceRepository;

        public ServiceInstanceController(ServiceInstanceRepository serviceInstanceRepository)
        {
            _serviceInstanceRepository = serviceInstanceRepository;
        }

        [HttpPost]

        public Task<ResultViewModel> AddServiceInstance([FromBody] ServiceInstance serviceInstance)
        {
            return Task.Run(() => _serviceInstanceRepository.AddServiceInstance(serviceInstance));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyServiceInstance([FromBody] ServiceInstance serviceInstance = default)
        {
            return Task.Run(() => _serviceInstanceRepository.ModifyServiceInstance(serviceInstance));
        }


        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteServiceInstance([FromRoute] string id = default)
        {
            return Task.Run(() => _serviceInstanceRepository.DeleteServiceInstance(id));
        }
    }
}