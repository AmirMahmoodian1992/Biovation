using Biovation.Brands.EOS.Commands;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.EOS.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class EosUserController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;
        private readonly AccessGroupService _accessGroupService;

        private readonly TaskTypes _taskTypes;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly CommandFactory _commandFactory;

        public EosUserController(AccessGroupService accessGroupService, CommandFactory commandFactory, TaskService taskService, TaskTypes taskTypes,
            TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, DeviceBrands deviceBrands, DeviceService deviceService)
        {
            _taskTypes = taskTypes;
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _deviceBrands = deviceBrands;
            _taskItemTypes = taskItemTypes;
            _deviceService = deviceService;
            _taskPriorities = taskPriorities;
            _commandFactory = commandFactory;
            _accessGroupService = accessGroupService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> SendUserToDevice(uint code, string userId)
        {
            try
            {
                var device = (await _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode))?.Data?.Data?.FirstOrDefault();
                if (device is null)
                    return new ResultViewModel { Validate = 0, Message = $"Wrong device code is provided : {code}." };



                //foreach (var receivedUserId in userIds)
                //{
                //    _commandFactory.Factory(CommandType.SendUserToDevice, new List<object> {code, receivedUserId})
                //        .Execute();
                //}

                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return new ResultViewModel { Validate = 0, Message = e.Message };
            }
        }


        [HttpPost]
        [Authorize]
        public ResultViewModel SendUserToAllDevices([FromBody] User user)
        {
            var accessGroups = _accessGroupService.GetAccessGroups(user.Id).Result?.Data?.Data;
            if (accessGroups == null || !accessGroups.Any())
            {
                return new ResultViewModel { Id = user.Id, Validate = 0 };
            }
            foreach (var accessGroup in accessGroups)
            {
                foreach (var deviceGroup in accessGroup.DeviceGroup)
                {
                    foreach (var deviceGroupMember in deviceGroup.Devices)
                    {
                        var addUserToTerminalCommand = _commandFactory.Factory(CommandType.SendUserToDevice,
                            new List<object> { deviceGroupMember.Code, user.Code });

                        addUserToTerminalCommand.Execute();
                    }
                }
            }

            return new ResultViewModel { Id = user.Id, Validate = 1 };
        }
    }
}
