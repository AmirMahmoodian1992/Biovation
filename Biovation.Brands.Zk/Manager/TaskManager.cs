using System;
using Biovation.Brands.ZK.Command;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Biovation.Brands.ZK.Manager
{
    public static class TaskManager
    {
        private static readonly TaskService TaskService = new TaskService();
        public static void ExecuteTask(TaskInfo taskInfo)
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.RetrieveAllLogsOfDevice,
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
                                result = (ResultViewModel)CommandFactory.Factory(CommandType.RetrieveLogsOfDeviceInPeriod,
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
                                    result = (ResultViewModel)CommandFactory.Factory(
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.SendUserToDevice,
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.UnlockDevice,
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.LockDevice,
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.RetrieveUserFromDevice,
                                        new List<object> { taskItem.DeviceId, taskItem.Id }).Execute();
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
                                result = (ResultViewModel)CommandFactory.Factory(CommandType.RetrieveUsersListFromDevice,
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.OpenDoor,
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.SendAccessGroupToDevice,
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
                                result = (ResultViewModel)CommandFactory.Factory(CommandType.ClearLogOfDevice,
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.UpgradeFirmware,
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.DeleteUserFromTerminal,
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
                                    result = (ResultViewModel)CommandFactory.Factory(CommandType.EnrollFromTerminal,
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
                                result = (ResultViewModel)CommandFactory.Factory(CommandType.SendTimeZoneToDevice,
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
                    taskItem.Status = TaskStatuses.GetTaskStatusByCode(result.Code.ToString());

                    TaskService.UpdateTaskStatus(taskItem);
                });

                if (taskItem.IsParallelRestricted)
                    executeTask?.Wait();
            }
        }
    }
}
