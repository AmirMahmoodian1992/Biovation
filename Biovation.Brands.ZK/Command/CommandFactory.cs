using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using RestSharp;

namespace Biovation.Brands.ZK.Command
{
    /// <summary>
    /// ایجاد و بازگردانی یک نمونه از اتفاق با توجه به نوع آن
    /// </summary>
    public class CommandFactory
    {
        private readonly LogEvents _logEvents;
        private readonly LogService _logService;
        private readonly UserService _userService;
        private readonly TaskService _taskService;
        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;
        private readonly DeviceService _deviceService;
        private readonly TimeZoneService _timeZoneService;
        private readonly AdminDeviceService _adminDeviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly RestClient _restClient;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;


        public CommandFactory(LogService logService, UserService userService, TaskService taskService, DeviceService deviceService, AdminDeviceService adminDeviceService,
            AccessGroupService accessGroupService, TimeZoneService timeZoneService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes, Dictionary<uint, Device> onlineDevices,
            TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, RestClient restClient)
        {
            _logService = logService;
            _userService = userService;
            _taskService = taskService;
            _deviceService = deviceService;
            _adminDeviceService = adminDeviceService;
            _accessGroupService = accessGroupService;
            _timeZoneService = timeZoneService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
            _onlineDevices = onlineDevices;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _restClient = restClient;
        }

        public ICommand Factory(int eventId, List<object> items)
        {
            return Factory(new DataTransferModel { EventId = eventId, Items = items });
        }

        /// <summary>
        /// <En>Create and return an instance of requested event handler.</En>
        /// <Fa>بر اساس نوع درخواست، یک نمونه از کنترل کننده درخواست می سازد.</Fa>
        /// </summary>
        /// <param name="transferModelData">داده ی دریافتی از کلاینت، بانک، sdk و یا...</param>
        /// <returns></returns>
        public ICommand Factory(DataTransferModel transferModelData)
        {
            switch (transferModelData.EventId)
            {
                #region DatabaseRequests(NoResponces)
                case CommandType.PersonnelEvent:
                    //Change in Personnel
                    throw new NotImplementedException();

                case CommandType.GuestEvent:
                    //Guest request
                    throw new NotImplementedException();

                case CommandType.ServerEventLogForceUpdate:
                    //Force Update from Server_Event_Log request
                    throw new NotImplementedException();

                case CommandType.SendAccessGroupToDevice:
                    //Transfer Access Group request
                    {
                        return new ZkSendAccessGroupToDevice(transferModelData.Items, _onlineDevices, _deviceService, _taskService, _accessGroupService);
                    }

                case CommandType.SendTimeZoneToDevice:
                    //Transfer Time Zone request
                    {
                        return new ZkSendTimeZoneToDevice(transferModelData.Items, _onlineDevices, _timeZoneService, _deviceService, _taskService);
                    }

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    throw new NotImplementedException();

                case CommandType.SendUserToDevice:
                    //Transfer Specific User to Specific Device request
                    {
                        return new ZkSendUserToDevice(transferModelData.Items, _onlineDevices, _logService, _userService, _deviceService, _taskService, _adminDeviceService, _logEvents, _logSubEvents, _matchingTypes);
                    }

                case CommandType.SyncAllUsers:
                    //Sync Update request
                    throw new NotImplementedException();

                case CommandType.SetTime:
                    //Update time in all devices
                    throw new NotImplementedException();

                case CommandType.RetrieveAllLogsOfDevice:
                    //Gets and updates all logs from device
                    {
                        return new ZkRetrieveAllLogsOfDevice(transferModelData.Items, _onlineDevices, _deviceService);
                    }

                case CommandType.RetrieveLogsOfDeviceInPeriod:
                case CommandType.GetLogsOfDeviceInPeriod:
                    {
                        return new ZkRetrieveAllLogsOfDeviceInPeriod(transferModelData.Items, _onlineDevices, _deviceService, _taskService);
                    }

                case CommandType.LockDevice:
                    //Locks the device
                    {
                        return new ZkRetrieveAllLogsOfDevice(transferModelData.Items, _onlineDevices, _deviceService);
                    }

                case CommandType.UnlockDevice:
                    //Unlocks the device
                    {
                        return new ZkRetrieveAllLogsOfDevice(transferModelData.Items, _onlineDevices, _deviceService);
                    }

                case CommandType.EnrollFromTerminal:
                    //Unlocks the device
                    throw new NotImplementedException();

                case CommandType.AddUserToTerminal:
                    //Unlocks the device
                    throw new NotImplementedException();

                case CommandType.DeleteUserFromTerminal:
                    //Delete from Terminal
                    {
                        return new ZkDeleteUserFromTerminal(transferModelData.Items, _onlineDevices, _deviceService, _logService, _logEvents, _logSubEvents, _matchingTypes, _taskService);
                    }
                case CommandType.RetrieveUserFromDevice:
                    //Unlocks the device
                    {
                        return new ZkRetrieveUserFromTerminal(transferModelData.Items, _onlineDevices, _deviceService);
                    }

                case CommandType.RetrieveUsersListFromDevice:
                    //Unlocks the device
                    {
                        return new ZkRetrieveUsersListFromTerminal(transferModelData.Items, _onlineDevices, _deviceService, _taskService);
                    }
                //Get some data like mac,firmware and etc from device
                case CommandType.GetDeviceAdditionalData:
                    {
                        var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new ZkGetAdditionalData(code, _onlineDevices);
                    }
                //backup from device and clear logs 
                case CommandType.ClearLogOfDevice:
                    {
                        return new ZkClearLogOfDevice(transferModelData.Items, _onlineDevices, _taskService, _deviceService);
                    }
                case CommandType.DownloadUserPhotos:
                    return new ZkDownloadUserPhotosFromDevice(transferModelData.Items, _onlineDevices, _deviceService);

                case CommandType.UploadUserPhotos:
                    return new ZkUploadUserPhotosFromDevice(transferModelData.Items, _onlineDevices, _deviceService);

                #region Tools
                case CommandType.UserAdaptation:
                    return new ZkUserAdaptation(transferModelData.Items, _onlineDevices, _deviceService,_taskTypes,_taskService, _taskStatuses, _taskItemTypes,_taskPriorities,_userService,_restClient);
                #endregion

                #endregion

                #region WebClientRequests(WithResponse)
                case CommandType.GetUsersOfDevice:
                    //Gets users of devices
                    throw new NotImplementedException();

                case CommandType.GetOnlineDevices:
                    //Gets online devices
                    throw new NotImplementedException();


                #endregion

                default:
                    return null;
            }
        }
    }
}
