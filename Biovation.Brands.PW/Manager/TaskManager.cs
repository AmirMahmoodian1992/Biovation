using Biovation.Brands.PW.Command;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.PW.Manager
{
    public class TaskManager
    {
        private readonly TaskService _taskService;
        private readonly CommandFactory _commandFactory;
        private readonly TaskStatuses _taskStatuses;

        private List<TaskInfo> _tasks = new List<TaskInfo>();
        private bool _processingQueueInProgress;

        public TaskManager(TaskService taskService, CommandFactory commandFactory, TaskStatuses taskStatuses)
        {
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _commandFactory = commandFactory;
        }

        public void ExecuteTask(TaskInfo taskInfo)
        {
            foreach (var taskItem in taskInfo.TaskItems)
            {
                if (taskItem.Status.Code == TaskStatuses.FailedCode || taskItem.Status.Code == TaskStatuses.DoneCode || taskItem.Status.Code == TaskStatuses.InProgressCode)
                    continue;

                Task executeTask = null;
                ResultViewModel result = null;

                switch (taskItem.TaskItemType.Code)
                {
                    case TaskItemTypes.GetLogsCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.RetrieveAllLogsOfDevice,
                                        new List<object> { taskItem }).Execute();

                                    return result;
                                });


                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }

                    case TaskItemTypes.GetLogsInPeriodCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.GetLogsOfDeviceInPeriod,
                                        new List<object> { taskItem }).Execute();

                                    return result;
                                });


                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }
                }

                executeTask?.ContinueWith(task =>
                {
                    if (result is null) return;
                    taskItem.Result = JsonConvert.SerializeObject(result);
                    taskItem.Status = _taskStatuses.GetTaskStatusByCode(result.Code.ToString());

                    _taskService.UpdateTaskStatus(taskItem);
                });

                if (taskItem.IsParallelRestricted)
                    executeTask?.Wait();
            }
        }

        public void ProcessQueue()
        {
            lock (_tasks)
                _tasks = _taskService.GetTasks(brandCode: DeviceBrands.ProcessingWorldCode,
                    excludedTaskStatusCodes: new List<string> { TaskStatuses.DoneCode, TaskStatuses.FailedCode }).Result;

            if (_processingQueueInProgress)
                return;

            _processingQueueInProgress = true;
            while (true)
            {
                TaskInfo taskInfo;
                lock (_tasks)
                {
                    if (_tasks.Count <= 0)
                    {
                        _processingQueueInProgress = false;
                        return;
                    }

                    taskInfo = _tasks.First();
                }

                ExecuteTask(taskInfo);
                lock (_tasks)
                    _tasks.Remove(taskInfo);
            }
        }
    }
}
