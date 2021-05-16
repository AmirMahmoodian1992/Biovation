using Biovation.Brands.Virdi.Command;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceBrands = Biovation.Constants.DeviceBrands;

namespace Biovation.Brands.Virdi.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiAccessGroupController : ControllerBase
    {
        private readonly VirdiServer _virdiServer;
        private readonly TaskService _taskService;
        private readonly DeviceBrands _deviceBrands;
        private readonly DeviceService _deviceService;
        private readonly CommandFactory _commandFactory;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public VirdiAccessGroupController(TaskService taskService, DeviceService deviceService, VirdiServer virdiServer, CommandFactory commandFactory, DeviceBrands deviceBrands, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _virdiServer = virdiServer;
            _commandFactory = commandFactory;
            _deviceBrands = deviceBrands;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendAccessGroupToAllDevices([FromBody] int accessGroupId)
        {
            try
            {
                return new ResultViewModel { Validate = 1, Message = "Sending users queued" };
            }
            catch (Exception exception)
            {
                return new ResultViewModel { Validate = 0, Message = exception.ToString() };
            }
        }

        /*   public ResultViewModel SendAccessGroupToAllDevices([FromBody]int accessGroupId)
           {
               var devices = _deviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.VirdiCode);
   
               foreach (var device in devices)
               {
                   var sendAccessGroupCommand = CommandFactory.Factory(CommandType.SendAccessGroupToDevice,
                   new List<object> { device.DeviceId, accessGroupId });
   
                   sendAccessGroupCommand.Execute();
               }
   
               return new ResultViewModel { Validate = 0 };
           }*/

        [HttpGet]
        [Authorize]
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            return new ResultViewModel { Validate = 1 , Message = code.ToString() };
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyAccessGroup(string accessGroup)
        {
            try
            {
                _virdiServer.LoadFingerTemplates().ConfigureAwait(false);
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }
    }
}
