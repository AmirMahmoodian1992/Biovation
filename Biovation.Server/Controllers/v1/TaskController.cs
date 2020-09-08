using System;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TaskController : Controller
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPatch]
        [Route("TaskExecutionStatus")]
        public Task<ResultViewModel> TaskExecutionStatus(int taskItemId, string taskStatusId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var taskItem =  _taskService.GetTaskItem(taskItemId).Data;
                    if (taskItem is null)
                        return new ResultViewModel
                            { Validate = 0, Code = taskItemId, Message = "The provided task item id is wrong" };

                    var taskStatus = TaskStatuses.GetTaskStatusByCode(taskStatusId);
                    if (taskStatus is null)
                        return new ResultViewModel
                            { Validate = 0, Code = Convert.ToInt64(taskStatusId), Message = "The provided task status id is wrong" };

                    taskItem.Status = taskStatus;
                    return  _taskService.UpdateTaskStatus(taskItem);
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