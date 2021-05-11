using Biovation.CommonClasses.Extension;
using Biovation.Data.Commands.Sinks;
using Biovation.Domain;
using Biovation.Repository.MessageBus;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class TaskController : ControllerBase
    {
        private readonly TaskApiSink _taskApiSink;
        private readonly TaskRepository _taskRepository;
        private readonly TaskMessageBusRepository _taskMessageBusRepository;

        public TaskController(TaskRepository taskRepository, TaskMessageBusRepository taskMessageBusRepository, TaskApiSink taskApiSink)
        {
            _taskApiSink = taskApiSink;
            _taskRepository = taskRepository;
            _taskMessageBusRepository = taskMessageBusRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> InsertTask([FromBody] TaskInfo task)
        {
            task.CreatedBy ??= HttpContext.GetUser();
            var taskInsertionResult = await _taskRepository.InsertTask(task);
            if (!taskInsertionResult.Success) return taskInsertionResult;
            task.Id = (int)taskInsertionResult.Id;

            //integration
            var taskList = new List<TaskInfo> { task };
            _ = _taskApiSink.TransmitTaskInfo(task).ConfigureAwait(false);
            _ = _taskMessageBusRepository.SendTask(taskList).ConfigureAwait(false);

            return taskInsertionResult;
        }

        [HttpPut]
        [Authorize]
        public async Task<ResultViewModel> UpdateTaskStatus([FromBody] TaskItem taskItem)
        {
            var taskInsertionResult = await _taskRepository.UpdateTaskStatus(taskItem);
            var task = (await _taskRepository.GetTasks(taskItemId: taskItem.Id, excludedTaskStatusCodes: string.Empty))?.Data?.Data?.FirstOrDefault();

            //integration
            var taskList = new List<TaskInfo> { task };
            _ = _taskApiSink.TransmitTaskInfo(task).ConfigureAwait(false);
            _ = _taskMessageBusRepository.SendTask(taskList).ConfigureAwait(false);

            return taskInsertionResult;
        }
    }
}