using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Service.Api.v2;
using Biovation.Brands.EOS.Commands;

namespace Biovation.Brands.Eos.Manager
{
    public class TaskManager
    {
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly CommandFactory _commandFactory;

        private List<TaskInfo> _tasks = new List<TaskInfo>();
        private bool _processingQueueInProgress;

        public TaskManager(TaskService taskService, CommandFactory commandFactory, TaskStatuses taskStatuses)
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
                }

                executeTask?.ContinueWith(task =>
                {
                    if (result is null) return;
                    taskItem.Result = JsonConvert.SerializeObject(result);
                    taskItem.Status = _taskStatuses.GetTaskStatusByCode(result.Code.ToString());

                    _taskService.UpdateTaskStatus(taskItem);
                });
            }
        }

        public void ProcessQueue()
        {
            lock (_tasks)
                _tasks = _taskService.GetTasks(brandCode: DeviceBrands.EosCode,
                    excludedTaskStatusCodes: new List<string> { _taskStatuses.Done.Code, _taskStatuses.Failed.Code })?.Data?.Data ?? new List<TaskInfo>();

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
