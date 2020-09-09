using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository;

namespace Biovation.Service.Sql.v1
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public Task<List<TaskInfo>> GetTasks(int taskId = default, string brandCode = default, int deviceId = default, string taskTypeCode = default, List<string> taskStatusCodes = null, List<string> excludedTaskStatusCodes = null)
        {
            var stringifiedTaskCodes = default(string);
            var stringifiedExcludedTaskStatusCodes = default(string);

            if (taskStatusCodes != null)
            {
                stringifiedTaskCodes = " ( ";
                foreach (var taskStatusCode in taskStatusCodes)
                {
                    stringifiedTaskCodes += $"'{taskStatusCode}', ";
                }

                stringifiedTaskCodes = stringifiedTaskCodes.Remove(stringifiedTaskCodes.Length - 2) + " ) ";
            }

            if (excludedTaskStatusCodes != null)
            {
                stringifiedExcludedTaskStatusCodes = " ( ";
                foreach (var excludedTaskStatusCode in excludedTaskStatusCodes)
                {
                    stringifiedExcludedTaskStatusCodes += $"'{excludedTaskStatusCode}', ";
                }

                stringifiedExcludedTaskStatusCodes = stringifiedExcludedTaskStatusCodes.Remove(stringifiedExcludedTaskStatusCodes.Length - 2) + " ) ";
            }

            return Task.Run(() => _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, stringifiedTaskCodes, stringifiedExcludedTaskStatusCodes));
        }

        public Task<TaskItem> GetTaskItem(int taskItemId)
        {
            return Task.Run(() => _taskRepository.GetTaskItem(taskItemId));
        }

        public Task<ResultViewModel> InsertTask(TaskInfo task)
        {
            return Task.Run(() => _taskRepository.InsertTask(task));
        }

        public Task<ResultViewModel> UpdateTaskStatus(TaskItem taskItem)
        {
            return Task.Run(() => _taskRepository.UpdateTaskStatus(taskItem));
        }
    }
}
