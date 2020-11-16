using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class TaskController : ControllerBase
    {

        private readonly TaskRepository _taskRepository;

        public TaskController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet]
        [Route("{taskItemId}")]
        [Authorize]


        public Task<ResultViewModel<TaskItem>> GetTaskItem([FromRoute] int taskItemId = default)
        {
            return Task.Run(() => _taskRepository.GetTaskItem(taskItemId));
        }

        //TODO::QUERY dynamic
        [HttpGet]
        /*[Route("GetTasks")]*/
        [Authorize]

        public ResultViewModel<PagingResult<TaskInfo>> GetTasks(int taskId = default, string brandCode = default, int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default, string excludedTaskStatusCodes = default, int pageNumber = default, int pageSize = default, int taskItemId = default)
        {

            var tasksResult = _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes, excludedTaskStatusCodes,taskItemId);

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