﻿using Biovation.Domain;
using Biovation.Repository.MessageBus;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class TaskController : Controller
    {

        private readonly TaskRepository _taskRepository;
        private readonly TaskMessageBusRepository _taskMessageBusRepository;

        public TaskController(TaskRepository taskRepository, TaskMessageBusRepository taskMessageBusRepository)
        {
            _taskRepository = taskRepository;
            _taskMessageBusRepository = taskMessageBusRepository;
        }

        [HttpPost]
        public Task<ResultViewModel> InsertTask([FromBody]TaskInfo task)
        {
            Task.Run(() =>
            {
                var taskInsertionResult = _taskRepository.InsertTask(task);
                if (!taskInsertionResult.Success) return taskInsertionResult;
                task.Id = (int)taskInsertionResult.Id;
                //integration
                var taskList = new List<TaskInfo> { task };
                _taskMessageBusRepository.SendTask(taskList);

                return taskInsertionResult;
            });

            throw new InvalidOperationException();
        }

        [HttpPut]
        public Task<ResultViewModel> UpdateTaskStatus([FromBody]TaskItem taskItem)
        {
            //return Task.Run(() => _taskRepository.UpdateTaskStatus(taskItem));

            Task.Run(() =>
            {
                var taskInsertionResult = _taskRepository.UpdateTaskStatus(taskItem);
                var task = _taskRepository.GetTasks(taskItemId: taskItem.Id, excludedTaskStatusCodes: string.Empty).FirstOrDefault();
                var taskList = new List<TaskInfo> { task };

                //integration
                _taskMessageBusRepository.SendTask(taskList);
                return taskInsertionResult;
            });

            throw new InvalidOperationException();

        }

    }
}