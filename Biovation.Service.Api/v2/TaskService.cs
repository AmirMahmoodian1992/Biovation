using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public ResultViewModel<PagingResult<TaskInfo>> GetTasks(int taskId = default, string brandCode = default,
            string instanceId = default, int deviceId = default, string taskTypeCode = default,
            List<string> taskStatusCodes = default, List<string> excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default, int taskItemId = default, string token = default)
        {
            var taskStatusCodesString = string.Empty;
            if (taskStatusCodes != null)
            {
                taskStatusCodesString += '(';
                foreach (var taskStatusCode in taskStatusCodes)
                {
                    taskStatusCodesString += $"{taskStatusCode},";
                }

                taskStatusCodesString = taskStatusCodesString.Trim(',');
                taskStatusCodesString += ')';
            }

            var excludedTaskStatusCodesString = string.Empty;
            if (excludedTaskStatusCodes != null)
            {
                excludedTaskStatusCodesString += '(';
                foreach (var excludedTaskStatusCode in excludedTaskStatusCodes)
                {
                    excludedTaskStatusCodesString += $"{excludedTaskStatusCode},";
                }

                excludedTaskStatusCodesString = excludedTaskStatusCodesString.Trim(',');
                excludedTaskStatusCodesString += ')';
            }

            return _taskRepository.GetTasks(taskId, brandCode, instanceId, deviceId, taskTypeCode, taskStatusCodesString,
                excludedTaskStatusCodesString, pageNumber, pageSize, taskItemId, token);
        }

        public ResultViewModel<TaskItem> GetTaskItem(int taskItemId = default, string token = default)
        {
            return _taskRepository.GetTaskItem(taskItemId, token);
        }

        /* public ResultViewModel InsertTask(TaskInfo task)
         {
             var taskInsertionResult = _taskRepository.InsertTask(task);

             if (taskInsertionResult.Success)
             {
                 //integration
                 Task.Run(() =>
                 {
                     task.Id = (int) taskInsertionResult.Id;
                     var taskList = new List<TaskInfo> { task };

                     var biovationBrokerMessageData = new List<DataChangeMessage<TaskInfo>>
                     {
                         new DataChangeMessage<TaskInfo>
                         {
                             Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                             TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = taskList
                         }
                     };

                     _biovationInternalSource.PushData(biovationBrokerMessageData);
                 });
             }

             return taskInsertionResult;
         }*/

        public ResultViewModel InsertTask(TaskInfo task, string token = default)
        {
            return _taskRepository.InsertTask(task, token);
        }
        public ResultViewModel UpdateTaskStatus(TaskItem taskItem)
        {
            return _taskRepository.UpdateTaskStatus(taskItem);
        }

        public async Task<ResultViewModel> ProcessQueue(Lookup brand, int deviceId = default, string token = default)
        {
            return await _taskRepository.ProcessQueue(brand, deviceId, token);
        }
    }
}
