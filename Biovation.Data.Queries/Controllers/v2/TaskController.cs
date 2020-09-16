using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class TaskController : Controller
    {

        private readonly TaskRepository _taskRepository;

        public TaskController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }



        [HttpGet]
        [Route("{taskItemId}")]

        public Task<ResultViewModel<TaskItem>> GetTaskItem(int taskItemId = default)
        {
            return Task.Run(() => _taskRepository.GetTaskItem(taskItemId));
        }

        //TODO::QUERY dynamic
        [HttpGet]
        [Route("GetTasks")]
        public ResultViewModel<PagingResult<TaskInfo>> GetTasks(int taskId = default, string brandCode = default, int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default, string excludedTaskStatusCodes = default, int pageNumber = default, int pageSize = default)
        {

            var tasksResult = _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes, excludedTaskStatusCodes);

            var result = new PagingResult<TaskInfo>
            {
                Data = tasksResult,
                Count = tasksResult.Count
            };

            return new ResultViewModel<PagingResult<TaskInfo>>
            {
                Data = result
            };
        }
    }
}