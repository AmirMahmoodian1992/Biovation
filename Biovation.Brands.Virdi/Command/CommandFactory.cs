using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using System;
using System.Collections.Generic;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Virdi.Command
{
    /// <summary>
    /// ایجاد و بازگردانی یک نمونه از اتفاق با توجه به نوع آن
    /// </summary>
    public class CommandFactory
    {
        private readonly VirdiServer _virdiServer;
        private readonly Callbacks _callbacks;

        private readonly LogEvents _logEvents;
        private readonly LogService _logService;
        private readonly UserService _userService;
        private readonly TaskService _taskService;
        //private readonly TaskManager _taskManager;
        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes; 
        private readonly DeviceService _deviceService;
        private readonly TimeZoneService _timeZoneService;
        private readonly UserCardService _userCardService;
        private readonly BlackListService _blackListService;
        private readonly AdminDeviceService _adminDeviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly FaceTemplateService _faceTemplateService;


        public CommandFactory(VirdiServer virdiServer, LogService logService,
            UserService userService, TaskService taskService, DeviceService deviceService,
            UserCardService userCardService, BlackListService blackListService, AdminDeviceService adminDeviceService,
            AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, TimeZoneService timeZoneService, Callbacks callbacks, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes)
        {
            _virdiServer = virdiServer;
            _logService = logService;
            _userService = userService;
            _taskService = taskService;
            _deviceService = deviceService;
            _userCardService = userCardService;
            _blackListService = blackListService;
            _adminDeviceService = adminDeviceService;
            _accessGroupService = accessGroupService;
            _faceTemplateService = faceTemplateService;
            _timeZoneService = timeZoneService;
            _callbacks = callbacks;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
        }
        //private EventDispatcher _eventDispatcherObj;

        // <summary>
        // <En>Create and return an instance of requested event handler.</En>
        // <Fa>بر اساس نوع درخواست، یک نمونه از کنترل کننده درخواست می سازد.</Fa>
        // </summary>
        // <param name="transferModelData">داده ی دریافتی از کلاینت، بانک، sdk و یا...</param>
        // <returns></returns>
        //public static ICommand Factory(DataTransferModel transferModelData)
        //{
        //    return Factory(transferModelData.EventId);
        //}

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
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var accessGroupId = Convert.ToInt32(transferModelData.Items[1]);
                        return new VirdiSendAccessGroupToTerminal(transferModelData.Items, _virdiServer, _callbacks, _taskService, _deviceService, _accessGroupService);
                    }

                case CommandType.SendTimeZoneToDevice:
                    //Transfer Time Zone request
                    {
                        var code = Convert.ToUInt32(transferModelData.Items[0]);
                        var timeZoneId = Convert.ToInt32(transferModelData.Items[1]);
                        return new VirdiSendTimeZoneToTerminal(code, timeZoneId, _virdiServer, _callbacks, _timeZoneService);
                    }

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    throw new NotImplementedException();

                case CommandType.SendUserToDevice:
                    //Transfer Specific User to Specific Device request
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt32(transferModelData.Items[1]);
                        return new VirdiSendUserToDevice(transferModelData.Items, _virdiServer, _callbacks, _logService, _userService, _taskService, _deviceService, _userCardService, _blackListService, _adminDeviceService, _accessGroupService, _faceTemplateService, _logEvents, _logSubEvents, _matchingTypes);
                    }

                case CommandType.SendBlackList:
                    //Transfer Specific User to Specific Device request
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt32(transferModelData.Items[1]);

                        return new VirdiSendBlackList(transferModelData.Items, _virdiServer, _taskService, _userService, _deviceService, _blackListService);
                    }

                case CommandType.SyncAllUsers:
                    //Sync Update request
                    throw new NotImplementedException();

                case CommandType.SetTime:
                    //Update time in all devices
                    //var timeToSet = Convert.ToInt32(transferModelData.Items.FirstOrDefault());
                    throw new NotImplementedException();

                case CommandType.RetrieveAllLogsOfDevice:
                    //Gets and updates all logs from device
                    {

                        return new VirdiRetrieveAllLogsOfDevice(transferModelData.Items, _virdiServer, _callbacks, _deviceService);

                    }

                case CommandType.RetrieveLogsOfDeviceInPeriod:
                    //Gets and updates all log in a period of time from device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var startDate = Convert.ToDateTime(transferModelData.Items[1]);
                        //var endDate = Convert.ToDateTime(transferModelData.Items[2]);
                        return new VirdiRetrieveAllLogsOfDeviceInPeriod(transferModelData.Items, _virdiServer, _callbacks, _taskService, _deviceService);
                    }

                case CommandType.LockDevice:
                    //Locks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new VirdiLockDevice(transferModelData.Items, _virdiServer, _deviceService);
                    }

                case CommandType.UnlockDevice:
                    //Unlocks the device
                    {
                        return new VirdiUnlockDevice(transferModelData.Items, _virdiServer, _deviceService);
                    }

                case CommandType.EnrollFromTerminal:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //return new VirdiEnrollFromTerminal(transferModelData.Items, VirdiServer.GetOnlineDevices());
                        throw new NotImplementedException();
                    }

                case CommandType.EnrollFaceFromDevice:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new VirdiEnrollFaceFromTerminal(transferModelData.Items, _callbacks, _virdiServer, _deviceService);
                    }

                //case CommandType.AddUserToTerminal:
                //    //Unlocks the device
                //    {
                //        var deviceId = Convert.ToUInt32(transferModelData.Items[0]);
                //        var userId = Convert.ToInt32(transferModelData.Items[1]);
                //        return new VirdiAddUserToTerminal(deviceId, userId, VirdiServer.GetOnlineDevices());
                //    }

                case CommandType.DeleteUserFromTerminal:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt32(transferModelData.Items[1]);
                        return new VirdiDeleteUserFromTerminal(transferModelData.Items, _virdiServer, _callbacks, _taskService, _logService, _deviceService, _logEvents, _logSubEvents, _matchingTypes);
                    }

                case CommandType.RetrieveUserFromDevice:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt32(transferModelData.Items[1]);
                        return new VirdiRetrieveUserFromTerminal(transferModelData.Items, _virdiServer, _callbacks, _taskService, _deviceService);
                    }

                case CommandType.RetrieveUsersListFromDevice:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new VirdiRetrieveUsersListFromTerminal(transferModelData.Items, _virdiServer, _callbacks, _deviceService);
                    }
                case CommandType.OpenDoor:
                    //Unlocks the device
                    {
                        return new VirdiOpenDoor(transferModelData.Items, _virdiServer, _callbacks, _deviceService);
                    }
                case CommandType.UpgradeFirmware:
                    //Unlocks the device
                    {
                        //var deviceCode = Convert.ToInt32(transferModelData.Items[0]);
                        //var filePath = Convert.ToString(transferModelData.Items[1]);
                        return new VirdiUpgradeDeviceFirmware(transferModelData.Items, _virdiServer, _callbacks, _taskService, _deviceService);
                    }
                case CommandType.GetDeviceAdditionalData:
                    //Unlocks the device
                {
                    return new VirdiUpgradeDeviceFirmware(transferModelData.Items, _virdiServer, _callbacks, _taskService, _deviceService);
                        //return new VirdiGetAdditionalData(transferModelData.Items, _virdiServer, _callbacks, _taskService, _deviceService);
                    }
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
