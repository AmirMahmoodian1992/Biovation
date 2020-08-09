using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Biovation.WebService.APIControllers
{
    public class TaskController : ApiController
    {
        private readonly TaskService _taskService = new TaskService();

        [HttpPatch]
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
