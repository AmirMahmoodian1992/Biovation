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

namespace Biovation.Brands.Virdi.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiBlackListController : ControllerBase
    {
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public VirdiBlackListController(TaskService taskService, DeviceService deviceService, DeviceBrands deviceBrands, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _deviceBrands = deviceBrands;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
        }

        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> SendBlackLisDevice(List<BlackList> blackLists)
        {
            var resultList = new List<ResultViewModel>();

            //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
            resultList.Add(new ResultViewModel { Message = "Sending BlackList queued", Validate = 1 });

            return resultList;

        }
    }
}
