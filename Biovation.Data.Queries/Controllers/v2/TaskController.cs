using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskRepository _taskRepository;

        public TaskController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet]
        [Authorize]
        [Route("{taskItemId}")]
        public Task<ResultViewModel<TaskItem>> GetTaskItem([FromRoute] int taskItemId = default)
        {
            return Task.Run(() => _taskRepository.GetTaskItem(taskItemId));
        }

        //TODO::QUERY dynamic
        [HttpGet]
        /*[Route("GetTasks")]*/
        [Authorize]
        public async Task<ResultViewModel<PagingResult<TaskInfo>>> GetTasks(int taskId = default, string brandCode = default, int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default, string excludedTaskStatusCodes = default, int pageNumber = default, int pageSize = default, int taskItemId = default)
        {
            return await _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes, excludedTaskStatusCodes, taskItemId, pageNumber, pageSize);
        }
    }
}