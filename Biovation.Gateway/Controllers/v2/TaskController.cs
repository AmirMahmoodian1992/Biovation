﻿using System;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Gateway.Controllers.v2
{
    [Route("biovation/api/[controller]")]
    public class TaskController : Controller
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPatch]
        public Task<IActionResult> TaskExecutionStatus(int taskItemId = default, string taskStatusId = default)
        {
            throw null;
        }
    }
}