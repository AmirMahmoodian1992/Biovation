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
            int pageSize = default)
        {
            return Task.Run(() => _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes,
                                      excludedTaskStatusCodes, pageNumber, pageSize)?.Data?.Data ?? new List<TaskInfo>());
        }

        public Task<List<TaskInfo>> GetTasks(int taskId = default, string brandCode = default,
            int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
            List<string> excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default)
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
                           excludedTaskStatusCodesString, pageNumber, pageSize)?.Data?.Data ?? new List<TaskInfo>();
            });
        }

        public TaskItem GetTaskItem(int taskItemId = default)
        {
            return _taskRepository.GetTaskItem(taskItemId)?.Data ?? new TaskItem();
        }

        public ResultViewModel InsertTask(TaskInfo task)
        {
            return _taskRepository.InsertTask(task);
        }

        public ResultViewModel UpdateTaskStatus(TaskItem taskItem)
        {
            return _taskRepository.UpdateTaskStatus(taskItem);
        }
    }
}
