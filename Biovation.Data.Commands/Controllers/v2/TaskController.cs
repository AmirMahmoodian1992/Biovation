using Biovation.Domain;
using Biovation.Repository.MessageBus;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class TaskController : ControllerBase
    {
        private readonly TaskRepository _taskRepository;
        private readonly TaskMessageBusRepository _taskMessageBusRepository;

        public TaskController(TaskRepository taskRepository, TaskMessageBusRepository taskMessageBusRepository)
        {
            _taskRepository = taskRepository;
            _taskMessageBusRepository = taskMessageBusRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> InsertTask([FromBody] TaskInfo task)
        {
            task.CreatedBy = HttpContext.GetUser();
            var taskInsertionResult = await _taskRepository.InsertTask(task);
            if (!taskInsertionResult.Success) return taskInsertionResult;
            task.Id = (int)taskInsertionResult.Id;
            //integration
            var taskList = new List<TaskInfo> { task };
            _taskMessageBusRepository.SendTask(taskList).ConfigureAwait(false);

            return taskInsertionResult;
        }

        [HttpPut]
        [Authorize]
        public async Task<ResultViewModel> UpdateTaskStatus([FromBody] TaskItem taskItem)
        {
            //return Task.Run(() => _taskRepository.UpdateTaskStatus(taskItem));

            var taskInsertionResult = await _taskRepository.UpdateTaskStatus(taskItem);
            var task = _taskRepository.GetTasks(taskItemId: taskItem.Id, excludedTaskStatusCodes: string.Empty).FirstOrDefault();
            var taskList = new List<TaskInfo> { task };

            //integration
            _taskMessageBusRepository.SendTask(taskList).ConfigureAwait(false);
            return taskInsertionResult;

        }
    }
}