using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;

        public TaskController(TaskService taskService, TaskStatuses taskStatuses)
        {
            _taskService = taskService;
            _taskStatuses = taskStatuses;
        }

        //[HttpGet]
        //public Task<List<TaskInfo>> Tasks(int taskId = default, string brandCode = default,
        //    int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
        //    string excludedTaskStatusCodes = default, int pageNumber = default,
        //    int pageSize = default, int taskItemId = default)
        //{
        //    return _taskService.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes,
        //        excludedTaskStatusCodes, pageNumber, pageSize, taskItemId);
        //}


        [HttpGet]
        [Route("TaskItems")]
        public async Task<ResultViewModel<TaskItem>> TaskItems(int taskItemId = default)
        {
            return await _taskService.GetTaskItem(taskItemId, HttpContext.Items["Token"] as string);
        }


        [HttpPatch]
        public async Task<ResultViewModel> TaskExecutionStatus(int taskItemId = default, string taskStatusId = default)
        {
            try
            {
                var taskItem = (await _taskService.GetTaskItem(taskItemId, HttpContext.Items["Token"] as string)).Data;
                if (taskItem is null)
                    return new ResultViewModel
                    { Validate = 0, Code = taskItemId, Message = "The provided task item id is wrong" };

                var taskStatus = _taskStatuses.GetTaskStatusByCode(taskStatusId);
                if (taskStatus is null)
                    return new ResultViewModel
                    { Validate = 0, Code = Convert.ToInt64(taskStatusId), Message = "The provided task status id is wrong" };

                taskItem.Status = taskStatus;
                return await _taskService.UpdateTaskStatus(taskItem);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel
                { Validate = 0, Code = taskItemId, Message = exception.ToString() };
            }
        }
    }
}