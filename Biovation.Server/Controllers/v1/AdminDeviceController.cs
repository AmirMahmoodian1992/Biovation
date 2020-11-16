﻿using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Biovation.Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class AdminDeviceController : ControllerBase
    {
        private readonly AdminDeviceService _adminDeviceService;
        private readonly string _kasraAdminToken;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public AdminDeviceController([FromBody] AdminDeviceService adminDeviceService, BiovationConfigurationManager biovationConfigurationManager)
        {
            _adminDeviceService = adminDeviceService;
            _biovationConfigurationManager = biovationConfigurationManager;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;
        }

        [HttpGet, Route("GetAdminDevicesByPersonId")]
        public List<AdminDeviceGroup> GetAdminDevicesByPersonId(int personId)
        {
            return _adminDeviceService.GetAdminDeviceGroupsByUserId(personId: personId, token: _kasraAdminToken);
        }

        [HttpPost, Route("ModifyAdminDevice")]
        public ResultViewModel ModifyAdminDevice([FromBody] JObject adminDevice)
        {
            return _adminDeviceService.ModifyAdminDevice(adminDevice, token: _kasraAdminToken);
        }
    }
}
