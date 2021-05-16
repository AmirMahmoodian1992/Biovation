using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class ZkAccessGroupController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        private readonly TaskService _taskService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;


        public ZkAccessGroupController(DeviceService deviceService, TaskService taskService, TaskTypes taskTypes, TaskPriorities taskPriorities, DeviceBrands deviceBrands, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskManager taskManager)
        {
            _deviceService = deviceService;
            _taskService = taskService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _deviceBrands = deviceBrands;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskManager = taskManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendAccessGroupToAllDevices([FromBody] int accessGroupId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return new ResultViewModel { Validate = 1, Message = "Sending AccessGroupToTerminal queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            return await Task.Run(() =>
            {
                try
                {

                    return new ResultViewModel { Validate = 1, Message = "Sending AccessGroupToTerminal queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }
    }
}