using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Queries.Controllers.v2
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

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel<List<ServiceInstance>>> GetServiceInstance([FromRoute] string id = default)
        {
            return Task.Run(() => _serviceInstanceRepository.GetServiceInstance(id));
        }
    }
}