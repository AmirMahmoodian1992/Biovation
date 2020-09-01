using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("2.0")]
    public class TaskController : Controller
    {

        private readonly TaskRepository _taskRepository;

        public TaskController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }



        [HttpGet]
        public Task<ResultViewModel<PagingResult<TaskInfo>>> GetTasks(int taskId = default, string brandCode = default, int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default, string excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes, excludedTaskStatusCodes, pageNumber, pageSize));
        }

        [HttpGet]
        public Task<ResultViewModel<TaskItem>> GetTaskItem(int taskItemId = default)
        {
            return Task.Run(() => _taskRepository.GetTaskItem(taskItemId));
        }

    }
}