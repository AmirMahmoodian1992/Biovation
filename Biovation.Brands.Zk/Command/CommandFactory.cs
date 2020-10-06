using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using Biovation.Brands.ZK.Devices;

namespace Biovation.Brands.ZK.Command
{
    /// <summary>
    /// ایجاد و بازگردانی یک نمونه از اتفاق با توجه به نوع آن
    /// </summary>
    public class CommandFactory
    {
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

        private readonly ZkTecoServer _zkServer;

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
        private readonly Dictionary<uint, Device> _onlineDevices;


        public CommandFactory(ZkTecoServer zkServer, LogService logService,
            UserService userService, TaskService taskService, DeviceService deviceService,
            UserCardService userCardService, BlackListService blackListService, AdminDeviceService adminDeviceService,
            AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, TimeZoneService timeZoneService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes, Dictionary<uint, Device> onlineDevices)
        {
            _zkServer = zkServer;
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
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
            _onlineDevices = onlineDevices;
        }

        public  ICommand Factory(int eventId, List<object> items)
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
                        // var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new ZKSendAccessGroupToDevice(transferModelData.Items, _onlineDevices);
                        //var accessGroupId = Convert.ToInt32(transferModelData.Items[1]);

                    }

                case CommandType.SendTimeZoneToDevice:
                    //Transfer Time Zone request
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var timeZoneId = Convert.ToInt32(transferModelData.Items[1]);
                        return new ZKSendTimeZoneToDevice(transferModelData.Items, _onlineDevices);
                    }

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    throw new NotImplementedException();

                case CommandType.SendUserToDevice:
                    //Transfer Specific User to Specific Device request
                    {
                        // var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt32(transferModelData.Items[1]);

                        return new ZKSendUserToDevice(transferModelData.Items, _onlineDevices);
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
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new ZKRetrieveAllLogsOfDevice(transferModelData.Items, _onlineDevices);
                    }

                case CommandType.RetrieveLogsOfDeviceInPeriod:
                case CommandType.GetLogsOfDeviceInPeriod:
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var sdate = (DateTime)(transferModelData.Items[1]);
                        //var edate = (DateTime)(transferModelData.Items[2]);
                        return new ZKRetrieveAllLogsOfDeviceInPeriod(transferModelData.Items, _onlineDevices);
                    }

                case CommandType.LockDevice:
                    //Locks the device
                    {
                        // var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new ZKRetrieveAllLogsOfDevice(transferModelData.Items, _onlineDevices);

                    }


                case CommandType.UnlockDevice:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //return new ZKUnlockDevice(code, _onlineDevices);
                        return new ZKRetrieveAllLogsOfDevice(transferModelData.Items, _onlineDevices);
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
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt64(transferModelData.Items[1]);
                        return new ZKDeleteUserFromTerminal(transferModelData.Items, _onlineDevices);
                    }
                case CommandType.RetrieveUserFromDevice:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt32(transferModelData.Items[1]);
                        return new ZKRetrieveUserFromTerminal(transferModelData.Items, _onlineDevices);
                    }

                case CommandType.RetrieveUsersListFromDevice:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new ZKRetrieveUsersListFromTerminal(transferModelData.Items, _onlineDevices);
                    }
                //Get some data like mac,firmware and etc from device
                case CommandType.GetDeviceAdditionalData:
                    {
                        var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new ZKGetAdditionalData(code, _onlineDevices);
                    }
                //backup from device and clear logs 
                case CommandType.ClearLogOfDevice:
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new ZKClearLogOfDevice(transferModelData.Items, _onlineDevices);
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
