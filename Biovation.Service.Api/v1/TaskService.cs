using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v1
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public Task<List<TaskInfo>> GetTasks(int taskId = default, string brandCode = default,
            int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
            string excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default, int taskItemId = default, string token = default)
        {
            return Task.Run(() => _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes,
                                      excludedTaskStatusCodes, pageNumber, pageSize, taskItemId, token).Result?.Data?.Data ?? new List<TaskInfo>());
        }

        public Task<List<TaskInfo>> GetTasks(int taskId = default, string brandCode = default,
            int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
            List<string> excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default, int taskItemId = default, string token = default)
        {
            return Task.Run(() =>
            {
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

                return _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes,
                           excludedTaskStatusCodesString, pageNumber, pageSize, taskItemId, token).Result?.Data?.Data ?? new List<TaskInfo>();
            });
        }

        public TaskItem GetTaskItem(int taskItemId = default, string token = default)
        {
            return _taskRepository.GetTaskItem(taskItemId, token).Result?.Data ?? new TaskItem();
        }

        /*public ResultViewModel InsertTask(TaskInfo task)
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
            return _taskRepository.InsertTask(task, token).Result;
        }

        public ResultViewModel UpdateTaskStatus(TaskItem taskItem, string token = default)
        {
            return _taskRepository.UpdateTaskStatus(taskItem).Result;
        }

        public async Task<ResultViewModel> ProcessQueue(Lookup brand, int deviceId = default, string token = default)
        {
            return await _taskRepository.ProcessQueue(brand, deviceId, token);
        }
    }
}
