﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;

namespace Biovation.Repository.Sql.v1
{
    public class TaskRepository
    {
        private readonly GenericRepository _repository;

        public TaskRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public List<TaskInfo> GetTasks(int taskId = default, string brandCode = default, int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default, string excludedTaskStatusCodes = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@taskId", taskId),
                new SqlParameter("@brandId", brandCode),
                new SqlParameter("@deviceId", deviceId),
                new SqlParameter("@taskTypeCode", taskTypeCode),
                new SqlParameter("@taskStatusCodes", taskStatusCodes),
                new SqlParameter("@excludedTaskStatusCodes", excludedTaskStatusCodes)
            };

            return _repository.ToResultList<TaskInfo>("SelectTasks", parameters, fetchCompositions: true, compositionDepthLevel: 3).Data;
        }

        public TaskItem GetTaskItem(int taskItemId = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", taskItemId)
            };

            return _repository.ToResultList<TaskItem>("SelectTaskItems", parameters, fetchCompositions: true, compositionDepthLevel: 3).Data.FirstOrDefault();
        }

        public ResultViewModel InsertTask(TaskInfo task)
        {
            var taskItemsDataTable =
                JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(task.TaskItems?.Select(item => new
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
                    item.IsParallelRestricted
                })));

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@taskTypeCode", task.TaskType?.Code),
                new SqlParameter("@priorityLevelCode", task.Priority?.Code),
                new SqlParameter("@createdBy", task.CreatedBy?.Id),
                new SqlParameter("@createdAt", task.CreatedAt),
                new SqlParameter("@deviceBrandId", task.DeviceBrand.Code),
                new SqlParameter("@dueDate", task.DueDate),
                new SqlParameter("@taskItems", taskItemsDataTable)
            };

            return _repository.ToResultList<ResultViewModel>("InsertTask", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel UpdateTaskStatus(TaskItem taskItem)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", taskItem.Id),
                new SqlParameter("@statusCode", taskItem.Status.Code),
                new SqlParameter("@result", taskItem.Result)
            };

            return _repository.ToResultList<ResultViewModel>("UpdateTaskItemStatus", parameters).Data.FirstOrDefault();
        }
    }
}
