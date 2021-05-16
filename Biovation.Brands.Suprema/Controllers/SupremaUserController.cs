using Biovation.Brands.Suprema.Services;
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

namespace Biovation.Brands.Suprema.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class SupremaUserController : ControllerBase
    {
        private readonly FastSearchService _fastSearchService;

        private readonly UserService _userService;
        private readonly AccessGroupService _accessGroupService;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly DeviceBrands _deviceBrands;

        public SupremaUserController(UserService userService, AccessGroupService accessGroupService, FastSearchService fastSearchService, TaskService taskService, DeviceService deviceService, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskPriorities taskPriorities, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands)
        {
            _userService = userService;
            _accessGroupService = accessGroupService;
            _fastSearchService = fastSearchService;
            _taskService = taskService;
            _deviceService = deviceService;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskPriorities = taskPriorities;
            _taskItemTypes = taskItemTypes;
            _deviceBrands = deviceBrands;
        }

        [HttpGet]
        public Task<List<User>> Users()
        {
            return Task.Run(() =>
            {

                var user = _userService.GetUsers();
                return user;
            }
        );
        }


        [HttpGet]
        [Authorize]
        public User Users(int id)
        {
            var user = _userService.GetUsers(userId: id)?.FirstOrDefault();
            return user;
        }

        [HttpGet]
        [Authorize]
        public List<AccessGroup> GetUserAccessGroups(int userId)
        {
            var accessGroups = _accessGroupService.GetAccessGroups(userId: userId);
            return accessGroups.ToList();
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyUser([FromBody] User user)
        {
            try
            {
                _fastSearchService.Initial();
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> SendUserToDevice(uint code, string userId)
        {
            try
            {
                return new ResultViewModel { Validate = 1, Message = "Sending user queued" };
            }
            catch (Exception e)
            {
                Logger.Log($" --> SendUserToDevice Code: {code}  {e}");
                return new ResultViewModel { Validate = 0, Message = e.Message };
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendUserToAllDevices([FromBody] User user)
        {
            return new ResultViewModel { Id = user.Id, Validate = 1 };
        }
    }
}
