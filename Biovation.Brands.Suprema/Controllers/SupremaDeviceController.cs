using Biovation.Brands.Suprema.Commands;
using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biovation.CommonClasses.Models.ConstantValues;
//using TaskItem = Biovation.CommonClasses.Models.TaskItem;

namespace Biovation.Brands.Suprema.ApiControllers
{
    public class SupremaDeviceController : ApiController
    {
        /*

                private readonly LogService _logServices = new LogService();
        */
        private readonly AccessGroupService _accessGroupServices = new AccessGroupService();
        private readonly DeviceService _deviceService = new DeviceService();
        //private readonly TaskService _taskService = new TaskService();
        private readonly UserService _userService = new UserService();

        [HttpGet]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = BioStarServer.GetOnlineDevices();
            /*
            try
            {
                BioStarServer.DeviceConnectionSemaphore.WaitOne(10000);
            }
            catch (Exception)
            {
                //ignore
            }

            var onlineDeviceModels = new List<DeviceBasicInfo>();

            foreach (var onlineDevice in onlineDevices.Values)
            {
                onlineDeviceModels.Add(onlineDevice.GetDeviceInfo());
            }

            try
            {
                BioStarServer.DeviceConnectionSemaphore.Release();
            }
            catch (Exception)
            {
                //ignore
            }
            */
            return onlineDevices?.Select(onlineDevice => new DeviceBasicInfo(onlineDevice.Value.GetDeviceInfo())).ToList();
        }

        [HttpGet]
        public List<User> Users(int deviceId)
        {
            var userObjects = CommandFactory.Factory(CommandType.GetUsersOfDevice, new List<object> { deviceId })
                .Execute();

            if (userObjects == null)
            {
                return null;
            }

            var users = new List<User>();

            foreach (var userObject in (IEnumerable<User>)userObjects)
            {
                users.Add(userObject);
            }

            return users;
        }

        [HttpPost]
        public ResultViewModel DeleteUserFromDevice(uint code, [FromBody]Newtonsoft.Json.Linq.JArray userId, bool updateServerSideIdentification = false)
        {
            var result = new List<ResultViewModel>();
            _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.SupremaCode);

            _userService.GetUser(123456789, false);

            //var task = new TaskInfo
            //{
            //    CreatedAt = DateTimeOffset.Now,
            //    CreatedBy = creatorUser,

            //    TaskType = TaskTypes.DeleteUsers,
            //    Priority = TaskPriorities.Medium,
            //    DeviceBrand = DeviceBrands.Suprema,
            //    TaskItems = new List<TaskItem>()
            //};


            var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());
            foreach (var id in userIds)
            {
                var deleteUser = CommandFactory.Factory(CommandType.DeleteUserFromTerminal,
                    new List<object> { code, id });
                var deleteresult = deleteUser.Execute();
                //result.Add(new ResultViewModel { Id = id, Validate = (ResultViewModel)boolResult ? 1 : 0, Message = "" });
                result.Add((ResultViewModel)deleteresult?? new ResultViewModel { Id = id, Validate =  0, Message = "" });

                //task.TaskItems.Add(new TaskItem
                //{
                //    Status = TaskStatuses.Queued,
                //    TaskItemType = TaskItemTypes.DeleteUserFromTerminal,
                //    Priority = TaskPriorities.Medium,
                //    DueDate = DateTime.Today,
                //    DeviceId = device.DeviceId,
                //    Data = JsonConvert.SerializeObject(new { userId = id }),
                //    IsParallelRestricted = true,
                //    IsScheduled = false,
                //    OrderIndex = 1,

                //});
            }

            //_taskService.InsertTask(task).Wait();
            //BioStarServer.ProcessQueue();

            //var result = new ResultViewModel { Validate = 1, Message = "Removing User queued" };

            if (result.Any(x => x.Validate == 0))
            {
                return new ResultViewModel { Id = userId.Count, Validate = 0, Message = "failed" };
            }

            return new ResultViewModel { Id = userId.Count, Validate = 1, Message = "success" };

            //return result;

        }

        [HttpPost]
        public List<ResultViewModel> RetrieveUserFromDevice(uint code, List<int> userIds)
        {
            //var userIds = JsonConvert.DeserializeObject<int[]>(userId);
            var result = new List<ResultViewModel>();
            foreach (var userIdValue in userIds)
            {
                var retrieveUserFromTerminalCommand = CommandFactory.Factory(CommandType.RetrieveUserFromDevice,
                    new List<object> { code, userIdValue });

                var intResult = (ResultViewModel<User>)retrieveUserFromTerminalCommand.Execute();

                result.Add(new ResultViewModel { Id = userIdValue, Validate = intResult.Validate, Message = intResult.Message });

            }

            return result;
        }

        [HttpGet]
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice(uint code)
        {
            var retrieveUserFromTerminalCommand = CommandFactory.Factory(CommandType.GetUsersOfDevice,
                new List<object> { code });

            var result = (ResultViewModel<List<User>>)retrieveUserFromTerminalCommand.Execute();

            return result;
        }

        [HttpGet]
        public List<SupremaLog> Logs(int deviceId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var startTimeTicks = startTime.Ticks;
                var endTimeTicks = endTime.AddDays(1).Ticks;
                var logObjects = CommandFactory.Factory(CommandType.GetLogsOfDeviceInPeriod, new List<object> { deviceId, startTimeTicks, endTimeTicks }).Execute();

                if (logObjects == null)
                {
                    return null;
                }

                var logs = new List<SupremaLog>();

                foreach (var logObject in (List<object>)logObjects)
                {
                    logs.Add((SupremaLog)logObject);
                }

                return logs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        public Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (fromDate.HasValue && toDate.HasValue)
                    {
                        var startTimeTicks = (fromDate.Value.Ticks);
                        var endTimeTicks = (toDate.Value.AddDays(1).Ticks);
                        //var startTimeTicks = (fromDate.Value.Ticks) / 1000000000;
                        //var endTimeTicks = (toDate.Value.AddDays(1).Ticks) / 1000000000;

                        CommandFactory.Factory(CommandType.GetLogsOfDeviceInPeriod, new List<object> { code, startTimeTicks, endTimeTicks }).Execute();
                    }
                    else
                    {
                        CommandFactory.Factory(CommandType.GetAllLogsOfDevice, new List<object> { code }).Execute();
                    }

                    //if (logObjects == null)
                    //{
                    //    return null;
                    //}

                    //var logs = new List<Log>();

                    //foreach (var logObject in (List<object>)logObjects)
                    //{
                    //    logs.Add((SupremaLog)logObject);
                    //}

                    //logs = ((List<object>) logObjects).Select(logObject => new Log((SupremaLog)logObject)).ToList();

                    //_logServices.AddLog(logs);

                    //return logs;  
                    return new ResultViewModel { Validate = 1 };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = exception.Message };
                }
            });
        }

        [HttpGet]
        public List<AccessGroup> GetDeviceAccessGroups(uint deviceId)
        {
            var accessGroups = _accessGroupServices.GetAccessGroupsOfDevice(deviceId);
            //var result = string.Empty;

            return accessGroups.ToList();
        }

        [HttpGet]
        public Dictionary<int, string> GetTypes()
        {
            var types = new Dictionary<int, string>
            {
                {BSSDK.BS_DEVICE_BIOMINI_CLIENT, "BioMini" },
                {BSSDK.BS_DEVICE_BIOLITE, "BioLite.V1" },
                {BSSDK.BS_DEVICE_BIOSTATION, "BioStation" },
                {BSSDK.BS_DEVICE_BIOSTATION2, "BioStationT2" },
                {BSSDK.BS_DEVICE_FSTATION, "FaceStation.V1" }
            };

            return types;
        }

        //[HttpPost]
        //public SupremaDeviceModel ReportUsersToDatabase(SupremaDeviceModel data)
        //{
        //    var users = CommandFactory.Factory(CommandFactory.GetUsersOfDevice, new List<object> { data.DeviceId })
        //        .Execute();


        //    var result = _userServices.ReportUsersToDatabase(data.nReaderIdn, (List<SupremaUserModel>)users, ConnectionType);

        //    return result ? data : null;
        //}

        [HttpPost]
        public bool SyncDevice(SupremaDeviceModel data)
        {
            //try
            //{
            //    _commonUserService.SyncUsersWithKasra(ConnectionType);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            try
            {
                CommandFactory.Factory(CommandType.ForceUpdateForSpecificDevice, new List<object> { data.DeviceId }).Execute();
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        [HttpGet]
        public int StartRequest(int deviceId)
        {
            //var accessGroups = _accessGroupServices.GetAccessGroupOfDevice(deviceId, ConnectionType);
            //var result = string.Empty;
            var onlineDevices = BioStarServer.GetOnlineDevices();

            if (onlineDevices is null || !onlineDevices.ContainsKey((uint)deviceId))
            {
                return -2569;
            }

            //var onlineDeviceModels = new List<SupremaDeviceModel>();

            //foreach (var onlineDevice in onlineDevices.Values)
            //{
            //    onlineDeviceModels.Add(onlineDevice.GetDeviceInfo());
            //}

            var device = onlineDevices[(uint)deviceId];

            var res = BSSDK.BS_StartRequest(device.GetDeviceInfo().Handle, device.GetDeviceInfo().DeviceTypeId, ConfigurationManager.SupremaDevicesConnectionPort);

            return res;
        }
    }
}
