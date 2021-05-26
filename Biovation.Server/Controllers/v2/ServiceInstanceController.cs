﻿using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class ServiceInstanceController : ControllerBase
    {
        private readonly ServiceInstanceService _serviceInstanceService;
        //private readonly RestClient _restClient;

        public ServiceInstanceController(ServiceInstanceService serviceInstanceService)
        {
            _serviceInstanceService = serviceInstanceService;
        }

        [HttpPost]
        [Route("WithoutId")]
        [Authorize]
        public Task<ResultViewModel> AddServiceInstance(string name = default, string version = default, string ip = default, int port = default, string description = default)
        {
            return Task.Run(() => _serviceInstanceService.AddServiceInstanceWithoutId(name, version, ip, port, description));
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddServiceInstance([FromBody] ServiceInstance serviceInstance)
        {
            return Task.Run(() => _serviceInstanceService.AddServiceInstance(serviceInstance));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyServiceInstance([FromBody] ServiceInstance serviceInstance)
        {
            return Task.Run(() => _serviceInstanceService.ModifyServiceInstance(serviceInstance));
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<List<ServiceInstance>>> GetServiceInstance([FromRoute] string id = default)
        {
            return Task.Run(() => _serviceInstanceService.GetServiceInstance(id));
        }
        [HttpGet]
        public Task<ResultViewModel<List<ServiceInstance>>> GetServiceInstance()
        {
            return Task.Run(() => _serviceInstanceService.GetServiceInstance());
        }

        [HttpDelete]
        public Task<ResultViewModel> DeleteServiceInstance([FromRoute] string id)
        {
            return Task.Run(() => _serviceInstanceService.DeleteServiceInstance(id));
        }
    }
}