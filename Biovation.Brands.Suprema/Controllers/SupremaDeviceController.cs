using Biovation.Brands.Suprema.Commands;
using Biovation.Brands.Suprema.Devices;
using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Biovation.Brands.Suprema.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class SupremaDeviceController : Controller
    {
        private readonly CommandFactory _commandFactory;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly AccessGroupService _accessGroupServices;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        // private readonly DeviceService _deviceService;
        //private readonly UserService _userService;

        public SupremaDeviceController(AccessGroupService accessGroupServices, Dictionary<uint, Device> onlineDevices, CommandFactory commandFactory, BiovationConfigurationManager biovationConfigurationManager)
        {
            _accessGroupServices = accessGroupServices;
            _onlineDevices = onlineDevices;
            _commandFactory = commandFactory;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        [HttpGet]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            return _onlineDevices?.Select(onlineDevice => new DeviceBasicInfo(onlineDevice.Value.GetDeviceInfo())).ToList();
        }

        [HttpGet]
        public List<User> Users(int deviceId)
        {
            var userObjects = _commandFactory.Factory(CommandType.GetUsersOfDevice, new List<object> { deviceId })
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
            // var deviceBasicInfo = _deviceService.GetDevices(code: code,brandId: DeviceBrands.SupremaCode)?.FirstOrDefault();

            //_userService.GetUsers(123456789, false);

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
                var deleteUser = _commandFactory.Factory(CommandType.DeleteUserFromTerminal,
                    new List<object> { code, id });
                var deleteresult = deleteUser.Execute();
                //result.Add(new ResultViewModel { Id = id, Validate = (ResultViewModel)boolResult ? 1 : 0, Message = "" });
                result.Add((ResultViewModel)deleteresult ?? new ResultViewModel { Id = id, Validate = 0, Message = "" });

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
                var retrieveUserFromTerminalCommand = _commandFactory.Factory(CommandType.RetrieveUserFromDevice,
                    new List<object> { code, userIdValue });

                var intResult = (ResultViewModel<User>)retrieveUserFromTerminalCommand.Execute();

                result.Add(new ResultViewModel { Id = userIdValue, Validate = intResult.Validate, Message = intResult.Message });

            }

            return result;
        }

        [HttpGet]
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice(uint code)
        {
            var retrieveUserFromTerminalCommand = _commandFactory.Factory(CommandType.GetUsersOfDevice,
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
                var logObjects = _commandFactory.Factory(CommandType.GetLogsOfDeviceInPeriod, new List<object> { deviceId, startTimeTicks, endTimeTicks }).Execute();

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

                        _commandFactory.Factory(CommandType.GetLogsOfDeviceInPeriod, new List<object> { code, startTimeTicks, endTimeTicks }).Execute();
                    }
                    else
                    {
                        _commandFactory.Factory(CommandType.GetAllLogsOfDevice, new List<object> { code }).Execute();
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
            var accessGroups = _accessGroupServices.GetAccessGroups(deviceId: (int)deviceId);
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
        //    var users = _commandFactory.Factory(_commandFactory.GetUsersOfDevice, new List<object> { data.DeviceId })
        //        .Execute();


        //    var result = __userServices.ReportUsersToDatabase(data.nReaderIdn, (List<SupremaUserModel>)users, ConnectionType);

        //    return result ? data : null;
        //}

        [HttpPost]
        public bool SyncDevice(SupremaDeviceModel data)
        {
            //try
            //{
            //    _common_userService.SyncUsersWithKasra(ConnectionType);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            try
            {
                _commandFactory.Factory(CommandType.ForceUpdateForSpecificDevice, new List<object> { data.DeviceId }).Execute();
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

            if (_onlineDevices is null || !_onlineDevices.ContainsKey((uint)deviceId))
            {
                return -2569;
            }

            //var onlineDeviceModels = new List<SupremaDeviceModel>();

            //foreach (var onlineDevice in onlineDevices.Values)
            //{
            //    onlineDeviceModels.Add(onlineDevice.GetDeviceInfo());
            //}

            var device = _onlineDevices[(uint)deviceId];

            var res = BSSDK.BS_StartRequest(device.GetDeviceInfo().Handle, device.GetDeviceInfo().DeviceTypeId, _biovationConfigurationManager.SupremaDevicesConnectionPort);

            return res;
        }
    }
}
