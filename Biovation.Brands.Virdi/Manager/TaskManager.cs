using Biovation.Brands.Virdi.Command;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq.Extensions;

namespace Biovation.Brands.Virdi.Manager
{
    public class TaskManager
    {
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly CommandFactory _commandFactory;

        private readonly List<TaskInfo> _tasks = new List<TaskInfo>();
        private bool _processingQueueInProgress;

        public TaskManager(TaskService taskService, CommandFactory commandFactory, TaskStatuses taskStatuses)
        {
            _taskService = taskService;
            _commandFactory = commandFactory;
            _taskStatuses = taskStatuses;
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
                                    /*result = (ResultViewModel)CommandFactory.Factory(CommandType.SendUsers,
                                        new List<object> { taskItem.Id, taskItem.DeviceId }).Execute();*/
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.RetrieveAllLogsOfDevice,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });
                                taskItem.ExecutionAt = DateTime.Now;
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
                                    result = (ResultViewModel)_commandFactory.Factory(
                                        CommandType.RetrieveLogsOfDeviceInPeriod,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }

                    case TaskItemTypes.SendUserCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.SendUserToDevice,
                                     new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();

                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }


                    case TaskItemTypes.SendBlackListCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.SendBlackList,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }

                    case TaskItemTypes.UnlockDeviceCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.UnlockDevice,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();

                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }

                    case TaskItemTypes.LockDeviceCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.LockDevice,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }
                    case TaskItemTypes.RetrieveUserFromTerminalCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.RetrieveUserFromDevice,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }
                    case TaskItemTypes.OpenDoorCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.OpenDoor,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }
                    case TaskItemTypes.SendAccessGroupToTerminalCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.SendAccessGroupToDevice,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }
                            break;

                        }

                    case TaskItemTypes.UpgradeDeviceFirmwareCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.UpgradeFirmware,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }

                    case TaskItemTypes.DeleteUserFromTerminalCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.DeleteUserFromTerminal,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);

                            }

                            break;
                        }

                    case TaskItemTypes.EnrollFromTerminalCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.EnrollFromTerminal,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);

                            }

                            break;
                        }

                    case TaskItemTypes.EnrollFaceFromTerminalCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.EnrollFaceFromDevice,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);

                            }

                            break;
                        }

                    case TaskItemTypes.UserAdaptationCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.UserAdaptation,
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
            var allTasks = await _taskService.GetTasks(brandCode: DeviceBrands.VirdiCode, deviceId: deviceId,
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
