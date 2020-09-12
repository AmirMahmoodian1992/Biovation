﻿using System;
using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AdminDeviceController : Controller
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly AdminDeviceService _adminDeviceService;

        public AdminDeviceController(AdminDeviceService adminDeviceService)
        {
            _adminDeviceService = adminDeviceService;
        }

        [HttpGet, Route("GetAdminDevicesByPersonId")]
        public List<AdminDeviceGroup> GetAdminDevicesByPersonId(int personId)
        {
            return _adminDeviceService.GetAdminDeviceGroupsByUserId(personId: personId);
        }

        [HttpPost, Route("ModifyAdminDevice")]
        public ResultViewModel ModifyAdminDevice([FromBody] JObject adminDevice)
        {
            return _adminDeviceService.ModifyAdminDevice(adminDevice);
        }
    }
}
