﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/queries/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class DeviceController : Controller
    {
        private readonly DeviceRepository _deviceRepository;


        public DeviceController(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }


        [HttpGet]
        public Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> Devices(long adminUserId = 0, int groupId = 0, uint code = 0,
            int brandId = 0, string name = null, int modelId = 0, int typeId = 0, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDevices(adminUserId, groupId, code, brandId, name, modelId, typeId, pageNumber, PageSize));
        }
        [HttpGet]
        [Route("{id?}")]
        public Task<ResultViewModel<DeviceBasicInfo>> Device([FromRoute]long id = 0, int adminUserId = 0)
        {
            return Task.Run(() => _deviceRepository.GetDevice(id, adminUserId));
        }


        [HttpPost]
        public Task<ResultViewModel> AddDevice([FromBody]DeviceBasicInfo device = default)
        {
            return Task.Run(() => _deviceRepository.AddDevice(device));
        }
        [HttpGet]
        [Route("GetDeviceModels/{id}")]
        public Task<PagingResult<DeviceModel>> GetDeviceModels(long id = 0, string brandId = default, string name = default, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDeviceModels(id, brandId, name, pageNumber, PageSize));
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel>  DeleteDevice(uint id)
        {
            return Task.Run(() => _deviceRepository.DeleteDevice(id));
        }


        [HttpDelete]
        [Route("DeleteDevices")]
        public Task<ResultViewModel> DeleteDevices([FromBody]List<uint> ids = default)      
        {
            return Task.Run(() => _deviceRepository.DeleteDevices(ids));
        }
    }
}