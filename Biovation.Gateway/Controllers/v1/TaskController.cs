using System;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Gateway.Controllers.v1
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
        [Route("TaskExecutionStatus")]
        public Task<ResultViewModel> TaskExecutionStatus(int taskItemId, string taskStatusId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var taskItem = await _taskService.GetTaskItem(taskItemId);
                    if (taskItem is null)
                        return new ResultViewModel
                            { Validate = 0, Code = taskItemId, Message = "The provided task item id is wrong" };

                    var taskStatus = TaskStatuses.GetTaskStatusByCode(taskStatusId);
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
            });
        }
    }
}