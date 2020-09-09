using Biovation.Domain;
using Biovation.Repository.API.v2;

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
            int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
            string excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default)
        {
            return _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes,
                excludedTaskStatusCodes, pageNumber, pageSize);
        }

        public ResultViewModel<TaskItem> GetTaskItem(int taskItemId = default)
        {
            return _taskRepository.GetTaskItem(taskItemId);
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
