using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;

namespace Biovation.Repository.Sql.v2
{
    public class TaskRepository
    {
        private readonly GenericRepository _repository;

        public TaskRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public List<TaskInfo> GetTasks(int taskId = default, string brandCode = default, int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default, string excludedTaskStatusCodes = default, int taskItemId = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@taskId", taskId),
                 new SqlParameter("@taskItemId", taskItemId),
                new SqlParameter("@brandId", brandCode),
                new SqlParameter("@deviceId", deviceId),
                new SqlParameter("@taskTypeCode", taskTypeCode),
                new SqlParameter("@taskStatusCodes", string.IsNullOrWhiteSpace(taskStatusCodes) ? null : taskStatusCodes),
                new SqlParameter("@excludedTaskStatusCodes", string.IsNullOrWhiteSpace(excludedTaskStatusCodes) ? null : excludedTaskStatusCodes)
            };

            return _repository.ToResultList<TaskInfo>("SelectTasks", parameters, fetchCompositions: true, compositionDepthLevel: 3).Data;
        }


        public ResultViewModel<TaskItem> GetTaskItem(int taskItemId = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", taskItemId)
            };

            return _repository.ToResultList<TaskItem>("SelectTaskItems", parameters, fetchCompositions: true, compositionDepthLevel: 3).FetchFromResultList();
        }

        public ResultViewModel InsertTask(TaskInfo task)
        {
            /*var taskItemsDataTable =JsonConvert.SerializeObject(task.TaskItems?.Select(item => new
                {
                    item.Id,
                    TaskId = task.Id,
                    TaskItemTypeCode = item.TaskItemType?.Code,
                    PriorityCode = item.Priority?.Code,
                    StatusCode = item.Status?.Code,
                    item.DeviceId,
                    item.Data,
                    item.Result,
                    item.OrderIndex,
                    item.IsScheduled,
                    item.DueDate,
                    item.IsParallelRestricted
                }));*/
            var taskItemsData = JsonConvert.SerializeObject(task.TaskItems);


            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@taskTypeCode", task.TaskType?.Code),
                new SqlParameter("@priorityLevelCode", task.Priority?.Code),
                new SqlParameter("@createdBy", task.CreatedBy?.Id),
                new SqlParameter("@createdAt", task.CreatedAt == default ? DateTime.Now : task.CreatedAt.DateTime),
                new SqlParameter("@updatedBy", task.UpdatedBy),
                new SqlParameter("@updatedAt", task.UpdatedAt == default ? (object) null : task.UpdatedAt.DateTime),
                new SqlParameter("@schedulingPattern", task.SchedulingPattern),
                new SqlParameter("@deviceBrandId", task.DeviceBrand.Code),
                new SqlParameter("@dueDate", task.DueDate),
                new SqlParameter("@json", taskItemsData)
            };

            return _repository.ToResultList<ResultViewModel>("InsertTask", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel UpdateTaskStatus(TaskItem taskItem)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", taskItem.Id),
                new SqlParameter("@statusCode", taskItem.Status.Code),
                new SqlParameter("@result", taskItem.Result),
                new SqlParameter("@CurrentIndex", taskItem.CurrentIndex),
                new SqlParameter("@TotalCount", taskItem.TotalCount),
                };

            return _repository.ToResultList<ResultViewModel>("UpdateTaskItemStatus", parameters).Data.FirstOrDefault();
        }
    }
}
