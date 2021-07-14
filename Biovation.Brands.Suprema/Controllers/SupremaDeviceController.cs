using Biovation.Brands.Suprema.Commands;
using Biovation.Brands.Suprema.Devices;
using Biovation.Brands.Suprema.Model;

using Biovation.CommonClasses.Extension;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Biovation.Brands.Suprema.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class DeviceController : ControllerBase
    {
        private readonly CommandFactory _commandFactory;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly AccessGroupService _accessGroupServices;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        private readonly DeviceService _deviceService;
       
        // private readonly DeviceService _deviceService;
        //private readonly UserService _userService;

        public DeviceController(AccessGroupService accessGroupServices, Dictionary<uint, Device> onlineDevices, CommandFactory commandFactory, BiovationConfigurationManager biovationConfigurationManager, DeviceService deviceService)
        {
            _accessGroupServices = accessGroupServices;
            _onlineDevices = onlineDevices;
            _commandFactory = commandFactory;
            _biovationConfigurationManager = biovationConfigurationManager;
            _deviceService = deviceService;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            return _onlineDevices?.Select(onlineDevice => new DeviceBasicInfo(onlineDevice.Value.GetDeviceInfo())).ToList();
        }

        [HttpGet]
        [Authorize]
        public List<User> Users(int deviceId)
        {
            //var creatorUser = HttpContext.GetUser();
            var userObjects = _commandFactory.Factory(CommandType.GetUsersOfDevice, new List<object> { deviceId })
                .Execute();

            if (userObjects == null)
            {
                return null;
            }

            //var users = new List<User>();
            var result = (ResultViewModel<List<User>>)userObjects;

            //foreach (var userObject in (IEnumerable<User>)userObjects)
            //{
            //    users.Add(userObject);
            //}

            // return users;
            return result.Data;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> DeleteUserFromDevice(uint code, [FromBody] List<int> userCodes, bool updateServerSideIdentification = false)
        {
            return await Task.Run(() =>
            {
                try
                {
                    //var creatorUser = HttpContext.GetUser();
                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.SupremaCode)
                        .FirstOrDefault();
                    if (device is null)
                        return new ResultViewModel {Success = false, Message = $"Device {code} does not exists."};

                    //var task = new TaskInfo
                    //{
                    //    CreatedAt = DateTimeOffset.Now,
                    //    CreatedBy = creatorUser,
                    //    TaskType = _taskTypes.DeleteUsers,
                    //    Priority = _taskPriorities.Medium,
                    //    DeviceBrand = _deviceBrands.Suprema,
                    //    TaskItems = new List<TaskItem>()
                    //};

                    //foreach (var userCode in userCodes)
                    //{
                    //    task.TaskItems.Add(new TaskItem
                    //    {
                    //        Status = _taskStatuses.Queued,
                    //        TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                    //        Priority = _taskPriorities.Medium,
                    //        DeviceId = device.DeviceId,
                    //        Data = JsonConvert.SerializeObject(new { userCode }),
                    //        IsParallelRestricted = true,
                    //        IsScheduled = false,
                    //        OrderIndex = 1
                    //    });
                    //}

                    //_taskService.InsertTask(task);
                    //await _taskService.ProcessQueue(_deviceBrands.Suprema, device.DeviceId).ConfigureAwait(false);

                    var result = new ResultViewModel {Validate = 1, Message = "Removing User queued"};
                    return result;
                }
                catch (Exception exception)
                {
                    return new ResultViewModel {Validate = 1, Message = $"Error ,Removing User not queued!{exception}"};
                }
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> RetrieveUserFromDevice(uint code, [FromBody] List<int> userIds)
        {
            return await Task.Run(() => { 
                try
                {
                    return new List<ResultViewModel>
                            {new ResultViewModel {Validate = 1, Message = "Retrieving users queued"}};
                }

                catch (Exception exception)
                {
                    return new List<ResultViewModel>
                                {new ResultViewModel { Validate = 0, Message = exception.ToString() }};
                }
            });
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice([FromBody] TaskInfo task)
        {
            try
            {
                var result = (ResultViewModel<List<User>>)_commandFactory.Factory(
                        CommandType.RetrieveUsersListFromDevice,
                        new List<object>
                            {task.TaskItems?.FirstOrDefault()})
                    .Execute();

                return result;

            }
            catch (Exception exception)
            {
                return new ResultViewModel<List<User>> { Validate = 0, Message = exception.ToString() };
            }

        }




        [HttpGet]
        [Authorize]
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
        [Authorize]
        public async Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return await Task.Run(() => new ResultViewModel {Validate = 1, Message = "Retrieving Log queued"});
        }

        [HttpGet]
        [Authorize]
        public List<AccessGroup> GetDeviceAccessGroups(uint deviceId)
        {
            var accessGroups = _accessGroupServices.GetAccessGroups(deviceId: (int)deviceId);
            //var result = string.Empty;

            return accessGroups.ToList();
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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


        [HttpPost]
        [Authorize]
        public async Task<Dictionary<string, string>> GetAdditionalData([FromBody] TaskInfo task)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = (Dictionary<string, string>)_commandFactory.Factory(
                            CommandType.RetrieveUsersListFromDevice,
                            new List<object>
                                {task.TaskItems?.FirstOrDefault()})
                        .Execute();

                    return result;

                }
                catch (Exception)
                {
                    return new Dictionary<string, string>();
                }
            });
        }
    }
}
