using Biovation.Brands.PW.Command;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Brands.PW.Manager
{
    public class TaskManager
    {
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly ServiceInstance _instanceData;
        private readonly CommandFactory _commandFactory;

        private readonly List<TaskInfo> _tasks = new List<TaskInfo>();
        private bool _processingQueueInProgress;

        public TaskManager(TaskService taskService, CommandFactory commandFactory, TaskStatuses taskStatuses, ServiceInstance instanceData)
        {
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _instanceData = instanceData;
            _commandFactory = commandFactory;
        }

        public async Task ExecuteTask(TaskInfo taskInfo)
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
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.RetrieveLogsOfDeviceInPeriod,
                                        new List<object> { taskItem }).Execute();
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

                if (executeTask is null)
                    return;

                if (taskItem.IsParallelRestricted)
                    await executeTask.ConfigureAwait(false);

                executeTask.Dispose();
            }
        }

        public async Task ProcessQueue(int deviceId = default, CancellationToken cancellationToken = default)
        {
            var allTasks = await _taskService.GetTasks(instanceId: _instanceData?.Id,
                brandCode: _instanceData is null ? DeviceBrands.ProcessingWorldCode : default, deviceId: deviceId,
                excludedTaskStatusCodes: new List<string>
                {
                    TaskStatuses.DoneCode, TaskStatuses.FailedCode, TaskStatuses.RecurringCode,
                    TaskStatuses.ScheduledCode, TaskStatuses.InProgressCode
                });

            lock (_tasks)
            {
                var newTasks = allTasks.ExceptBy(_tasks, task => task.Id).ToList();

                Logger.Log($"_tasks have {_tasks.Count} tasks, adding {newTasks.Count} tasks");
                _tasks.AddRange(newTasks);

                if (_processingQueueInProgress)
                    return;

                _processingQueueInProgress = true;
            }


            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
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

                        Logger.Log($"The task {taskInfo.Id} execution is started");
                        await ExecuteTask(taskInfo);
                        Logger.Log($"The task {taskInfo.Id} is executed");

                        lock (_tasks)
                            if (_tasks.Any(task => task.Id == taskInfo.Id))
                                _tasks.Remove(_tasks.FirstOrDefault(task => task.Id == taskInfo.Id));
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                }
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
