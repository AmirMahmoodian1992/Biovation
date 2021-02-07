﻿using Biovation.Brands.ZK.Command;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Manager
{
    public class TaskManager
    {
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly CommandFactory _commandFactory;

        private List<TaskInfo> _tasks = new List<TaskInfo>();
        private bool _processingQueueInProgress;
        public TaskManager(TaskService taskService, TaskStatuses taskStatuses, CommandFactory commandFactory)
        {
            _taskService = taskService;
            _commandFactory = commandFactory;
            _taskStatuses = taskStatuses;
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
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
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
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }

                    case TaskItemTypes.GetServeLogsInPeriodCode:
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
                                        new List<object> { taskItem }).Execute();
                                });

                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }

                            break;
                        }

                    case TaskItemTypes.RetrieveAllUsersFromTerminalCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.RetrieveUsersListFromDevice,
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

                    case TaskItemTypes.ClearLogCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.ClearLogOfDevice,
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


                    case TaskItemTypes.SendTimeZoneToTerminalCode:
                        {
                            try
                            {
                                executeTask = Task.Run(() =>
                                {
                                    result = (ResultViewModel)_commandFactory.Factory(CommandType.SendTimeZoneToDevice,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
                                });
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);

                            }

                            break;
                        }

                        #region MyRegion

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
                    #endregion

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

                executeTask?.Dispose();
            }
        }

        public void ProcessQueue(int deviceId = default)
        {
            var allTasks = _taskService.GetTasks(brandCode: DeviceBrands.ZkTecoCode, deviceId: deviceId,
                excludedTaskStatusCodes: new List<string> { TaskStatuses.DoneCode, TaskStatuses.FailedCode }).Result;

            lock (_tasks)
            {
                var newTasks = allTasks.ExceptBy(_tasks, task => task.Id).ToList();

                Logger.Log($"_tasks have {_tasks.Count} tasks, adding {newTasks.Count} tasks");
                _tasks.AddRange(newTasks);

                if (_processingQueueInProgress)
                    return;

                _processingQueueInProgress = true;
            }


            Task.Run(() =>
            {
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

                    Logger.Log($"The task {taskInfo.Id} execution is started");
                    ExecuteTask(taskInfo);
                    Logger.Log($"The task {taskInfo.Id} is executed");

                    lock (_tasks)
                        if (_tasks.Any(task => task.Id == taskInfo.Id))
                            _tasks.Remove(_tasks.FirstOrDefault(task => task.Id == taskInfo.Id));
                }
            });
        }
    }
}
