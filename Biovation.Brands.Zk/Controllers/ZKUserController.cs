using Biovation.Brands.ZK.Devices;
using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses;
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
    public class ZkUserController : ControllerBase
    {
        private readonly AccessGroupService _accessGroupService;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public ZkUserController(AccessGroupService accessGroupService, TaskService taskService, DeviceService deviceService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskManager taskManager, DeviceBrands deviceBrands, Dictionary<uint, Device> onlineDevices)
        {
            _accessGroupService = accessGroupService;
            _taskService = taskService;
            _deviceService = deviceService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskManager = taskManager;
            _deviceBrands = deviceBrands;
            _onlineDevices = onlineDevices;
        }

        [HttpGet]
        [Authorize]
        public async Task<List<ResultViewModel>> SendUserToDevice(uint code, string userId)
        {
            return await Task.Run(() =>
            {
                var listResult = new List<ResultViewModel>();
                try
                {
                    try
                    {
                        var userIds = JsonConvert.DeserializeObject<long[]>(userId);


                        var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                        if (devices != null)
                        {


                            foreach (var id in userIds)
                            {
                                listResult.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                            }

                        }

                        //_taskManager.ProcessQueue();
                    }
                    catch (Exception e)
                    {
                        Logger.Log($" --> SendUserToDevice Code: {code}  {e}");
                        listResult.Add(new ResultViewModel { Message = e.ToString(), Validate = 0 });

                    }
                    return listResult;
                }
                catch (Exception)
                {
                    listResult.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                    return listResult;
                }
            });

        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendUserToAllDevices([FromBody] User user)
        {
            return await Task.Run(() =>
                {
                    return new ResultViewModel { Id = user.Id, Validate = 1 };
                });
        }

        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> DeleteUserFromAllTerminal(int[] userCodes)
        {
            return await Task.Run(() =>
            {
                return userCodes.Select(code=> new ResultViewModel { Id = code, Validate = 1 }).ToList() ;
            });
        }
    }
}