﻿using System;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class TaskController : Controller
    {
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;

        public TaskController(TaskService taskService, TaskStatuses taskStatuses)
        {
            _taskService = taskService;
            _taskStatuses = taskStatuses;
        }

        [HttpPatch]
        public Task<ResultViewModel> TaskExecutionStatus(int taskItemId = default, string taskStatusId = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    var taskItem = _taskService.GetTaskItem(taskItemId).Data;
                    if (taskItem is null)
                        return new ResultViewModel
                            { Validate = 0, Code = taskItemId, Message = "The provided task item id is wrong" };

                    var taskStatus = _taskStatuses.GetTaskStatusByCode(taskStatusId);
                    if (taskStatus is null)
                        return new ResultViewModel
                            { Validate = 0, Code = Convert.ToInt64(taskStatusId), Message = "The provided task status id is wrong" };

                    taskItem.Status = taskStatus;
                    return _taskService.UpdateTaskStatus(taskItem);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel
                        { Validate = 0, Code = taskItemId, Message = exception.ToString() };
                }
            });
        }
    }
}