
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// ایجاد و بازگردانی یک نمونه از اتفاق با توجه به نوع آن
    /// </summary>
    public class CommandFactory
    {
        //private EventDispatcher _eventDispatcherObj;
        private readonly BioStarServer _supremaServer;
        

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

       //private static ClientConnection Connection { get; set; }

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


        public CommandFactory(BioStarServer supremaServer, LogService logService,
            UserService userService, TaskService taskService, DeviceService deviceService,
            UserCardService userCardService, BlackListService blackListService, AdminDeviceService adminDeviceService,
            AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, TimeZoneService timeZoneService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes)
        {
            _supremaServer = supremaServer;
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
           // _callbacks = callbacks;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
        }

        public  ICommand Factory(int eventId, List<object> items)
        {
            return Factory(new DataTransferModel { EventId = eventId, Items = items });
        }


        public  ICommand Factory(DataTransferModel transferModelData, ClientConnection connection)
        {
            Connection = connection;
            return Factory(transferModelData);
        }

        /// <summary>
        /// <En>Create and return an instance of requested event handler.</En>
        /// <Fa>بر اساس نوع درخواست، یک نمونه از کنترل کننده درخواست می سازد.</Fa>
        /// </summary>
        /// <param name="transferModelData">داده ی دریافتی از کلاینت، بانک، sdk و یا...</param>
        /// <returns></returns>
        public  ICommand Factory(DataTransferModel transferModelData)
        {
            switch (transferModelData.EventId)
            {
                #region DatabaseRequests(NoResponces)
                case CommandType.PersonnelEvent:
                    {
                        //Change in Personnel
                        var userId = Convert.ToInt32(transferModelData.Items.FirstOrDefault());
                        return new SupremaSyncUser(userId, BioStarServer.GetOnlineDevices());
                    }

                case CommandType.GuestEvent:
                    //Guest request
                    throw new NotImplementedException();

                case CommandType.ServerEventLogForceUpdate:
                    //Force Update from Server_Event_Log request
                    return new SupremaSyncAllUsersFromServerEventLog(BioStarServer.GetOnlineDevices());

                case CommandType.SendAccessGroupToDevice:
                    //Transfer Access Group request
                    return new SupremaSyncAccessGroups(BioStarServer.GetOnlineDevices());

                case CommandType.SendTimeZoneToDevice:
                    //Transfer Time Zone request
                    return new SupremaSyncTimeZone(BioStarServer.GetOnlineDevices());

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    {
                        var deviceId = Convert.ToUInt32(transferModelData.Items.FirstOrDefault());
                        return new SupremaSyncUsersOfDevice(deviceId, BioStarServer.GetOnlineDevices());
                    }

                case CommandType.SendUserToDevice:
                    //Transfer Specific User to Specific Device request
                    {
                        var deviceCode = Convert.ToUInt32(transferModelData.Items[0]);
                        var userId = Convert.ToInt32(transferModelData.Items[1]);
                        return new SupremaSyncUserOfDevice(deviceCode, userId, BioStarServer.GetOnlineDevices());
                    }

                case CommandType.SyncAllUsers:
                    //Sync Update request
                    return new SupremaSyncAllUsers(BioStarServer.GetOnlineDevices());

                case CommandType.SetTime:
                    //Update time in all devices
                    var timeToSet = Convert.ToInt32(transferModelData.Items.FirstOrDefault());
                    return new SupremaSetTime(timeToSet, BioStarServer.GetOnlineDevices());

                case CommandType.GetAllLogsOfDevice:
                    //Gets and updates all logs from device
                    {
                        var deviceId = Convert.ToUInt32(transferModelData.Items.FirstOrDefault());
                        return new SupremaGetAllLogsOfDevice(deviceId, BioStarServer.GetOnlineDevices());
                    }

                case CommandType.GetLogsOfDeviceInPeriod:
                    //Gets and updates all log in a period of time from device
                    {
                        var deviceId = Convert.ToUInt32(transferModelData.Items[0]);
                        var startDate = Convert.ToInt64(transferModelData.Items[1]);
                        var endDate = Convert.ToInt64(transferModelData.Items[2]);
                        return new SupremaGetLogsOfDeviceInPeriod(deviceId, startDate, endDate,
                            BioStarServer.GetOnlineDevices());
                    }

                case CommandType.GetLogsOfAllDevicesInPeriod:
                    //Gets and updates all log in a period of time from device
                    return new SupremaGetLogsOfAllDevices(BioStarServer.GetOnlineDevices());

                case CommandType.DeleteUserFromTerminal:
                    {
                        //delete one or multiple users of device
                        var deviceCode = Convert.ToUInt32(transferModelData.Items[0]);
                        var userIds = (uint)Convert.ToInt32(transferModelData.Items[1]);
                        return new SupremaDeleteUserFromTerminal(deviceCode, BioStarServer.GetOnlineDevices(), userIds);
                    }

                case CommandType.RetrieveUserFromDevice:
                    {
                        //gets one or multiple users of device
                        var deviceCode = Convert.ToUInt32(transferModelData.Items[0]);
                        var userIds = (uint)Convert.ToInt32(transferModelData.Items[1]);
                        return new SupremaRetrieveUserFromDevice(deviceCode, BioStarServer.GetOnlineDevices(), userIds);
                    }

                #endregion

                #region WebClientRequests(WithResponse)
                case CommandType.GetUsersOfDevice:
                    //Gets users of devices

                    return new SupremaGetUsersOfDevice(Convert.ToUInt32(transferModelData.Items.FirstOrDefault()), BioStarServer.GetOnlineDevices());

                case CommandType.GetOnlineDevices:
                    //Gets online devices
                    return new SupremaGetOnlineDevices(BioStarServer.GetOnlineDevices(), Connection);




                #endregion

                #region BiominiClientRequests

                case CommandType.DeviceConnectedCallback:
                    //Gets and updates new logs from client
                    return new BiominiDeviceConnectedCallback(Convert.ToUInt32(transferModelData.Items[0]), Convert.ToString(transferModelData.Items[1]), Connection);
                case CommandType.GetAllLogsOfClient:
                    //Gets and updates new logs from client
                    return new BiominiGetAllLogsOfClient(transferModelData.Items, Connection);
                case CommandType.GetNewUserFromClient:
                    //Gets and updates new logs from client
                    return new BiominiGetNewUserFromClient(transferModelData.Items, Connection);
                case CommandType.UserChangedCallback:
                    //Gets and updates new logs from client
                    return new BiominiUserChangedCallback(transferModelData.Items, BioStarServer.GetOnlineDevices());
                case CommandType.ServerSideIdentification:
                    return new BiominiServerSideIdentification(transferModelData.Items, Connection);
                case CommandType.EditFingerTemplate:
                    return new BiominiEditFingerTemplateCallback(transferModelData.Items, Connection);
                case CommandType.GetUser:
                    return new BiominiGetUser(transferModelData.Items, Connection);
                case CommandType.GetServerTime:
                    return new BiominiGetServerTime(Connection);
                #endregion

                default:
                    return null;
            }
        }
    }
}
