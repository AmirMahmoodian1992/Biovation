﻿
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Brands.Suprema.Devices;
using Biovation.Brands.Suprema.Manager;
using Biovation.Constants;
using Biovation.Service.Api.v2;
using RestSharp;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// ایجاد و بازگردانی یک نمونه از اتفاق با توجه به نوع آن
    /// </summary>
    public class CommandFactory
    {
        //private EventDispatcher _eventDispatcherObj;
        private readonly BioStarServer _bioStarServer;
        

        //private readonly LogEvents _logEvents;
        //private readonly LogService _logService;
        private readonly UserService _userService;
        //private readonly TaskService _taskService;
        //private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly DeviceService _deviceService;
        ///private readonly LogSubEvents _logSubEvents;
       // private readonly MatchingTypes _matchingTypes;
        private readonly TimeZoneService _timeZoneService;
        private readonly UserCardService _userCardService;
        //private readonly BlackListService _blackListService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly AccessGroupService _accessGroupService;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly AdminDeviceService _adminDeviceService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly RestClient _restClient;

        private readonly TaskTypes _taskTypes;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskService _taskService;
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

        //public CommandFactory(BioStarServer supremaServer, LogService logService,
        //    UserService userService, TaskService taskService, DeviceService deviceService,
        //    UserCardService userCardService, BlackListService blackListService, AdminDeviceService adminDeviceService,
        //    AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, TimeZoneService timeZoneService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes, DeviceBrands deviceBrands, TaskStatuses taskStatuses, FingerTemplateService fingerTemplateService, FingerTemplateTypes fingerTemplateTypes, BiometricTemplateManager biometricTemplateManager)
        //{
        public CommandFactory(BioStarServer supremaServer, 
            UserService userService, DeviceService deviceService,
            UserCardService userCardService,  
            AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, TimeZoneService timeZoneService, DeviceBrands deviceBrands, TaskStatuses taskStatuses, FingerTemplateService fingerTemplateService, FingerTemplateTypes fingerTemplateTypes, BiometricTemplateManager biometricTemplateManager, FaceTemplateTypes faceTemplateTypes, Dictionary<uint, Device> onlineDevices, RestClient restClient, TaskTypes taskTypes, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService, AdminDeviceService adminDeviceService)
        {
            _bioStarServer = supremaServer;
            //_logService = logService;
            _userService = userService;
           // _taskService = taskService;
            _deviceService = deviceService;
            _userCardService = userCardService;
           // _blackListService = blackListService;
           // _adminDeviceService = adminDeviceService;
            _accessGroupService = accessGroupService;
            _faceTemplateService = faceTemplateService;
            _timeZoneService = timeZoneService;
           // _callbacks = callbacks;
           // _logEvents = logEvents;
            //_logSubEvents = logSubEvents;
            //_matchingTypes = matchingTypes;
            _deviceBrands = deviceBrands;
            _taskStatuses = taskStatuses;
            _fingerTemplateService = fingerTemplateService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _faceTemplateTypes = faceTemplateTypes;
            _onlineDevices = onlineDevices;
            _restClient = restClient;
            _taskTypes = taskTypes;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskService = taskService;
            _adminDeviceService = adminDeviceService;
        }

        public  ICommand Factory(int eventId, List<object> items)
        {
            return Factory(new DataTransferModel { EventId = eventId, Items = items });
        }


        //public  ICommand Factory(DataTransferModel transferModelData)
        //{
        //   /* Connection = connection;*/
        //    return Factory(transferModelData);
        //}

        /// <summary>
        /// <En>Create and return an instance of requested event handler.</En>
        /// <Fa>بر اساس نوع درخواست، یک نمونه از کنترل کننده درخواست می سازد.</Fa>
        /// </summary>
        /// <param name="transferModelData">داده ی دریافتی از کلاینت، بانک، sdk و یا...</param>
        /// <returns></returns>
        public  ICommand Factory(DataTransferModel transferModelData)
        {
            var taskItem = (TaskItem)transferModelData.Items[0];
            switch (transferModelData.EventId)
            {
                #region DatabaseRequests(NoResponces)
                case CommandType.PersonnelEvent:
                    {
                        //Change in Personnel
                        var userId = Convert.ToInt32(transferModelData.Items.FirstOrDefault());
                        return new SupremaSyncUser(userId, _onlineDevices,_userService,_accessGroupService);
                    }

                case CommandType.GuestEvent:
                    //Guest request
                    throw new NotImplementedException();

                case CommandType.ServerEventLogForceUpdate:
                    //Force Update from Server_Event_Log request
                    return new SupremaSyncAllUsersFromServerEventLog(_onlineDevices,_accessGroupService,_userService);

                case CommandType.SendAccessGroupToDevice:
                    //Transfer Access Group request
                    return new SupremaSyncAccessGroups(_onlineDevices,_accessGroupService,_timeZoneService);

                case CommandType.SendTimeZoneToDevice:
                    //Transfer Time Zone request
                    return new SupremaSyncTimeZone(_onlineDevices,_timeZoneService);

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    {
                        //var deviceId = Convert.ToUInt32(transferModelData.Items.FirstOrDefault());
                        return new SupremaSyncUsersOfDevice(taskItem, _onlineDevices,_accessGroupService,_deviceService,_deviceBrands,_userService);
                    }

                case CommandType.SendUserToDevice:
                    //Transfer Specific User to Specific Device request
                    {
                        return new SupremaSyncUserOfDevice(taskItem, _onlineDevices, _accessGroupService,_userService, _deviceService);
                    }

                case CommandType.SyncAllUsers:
                    //Sync Update request
                    return new SupremaSyncAllUsers(_onlineDevices,_accessGroupService,_userService);

                case CommandType.SetTime:
                    //Update time in all devices
                    //var timeToSet = Convert.ToInt32(transferModelData.Items.FirstOrDefault());
                    return new SupremaSetTime((int)(DateTime.Now.Ticks / 1000000), _onlineDevices);

                case CommandType.GetAllLogsOfDevice:
                    //Gets and updates all logs from device
                    {
                        //var deviceId = Convert.ToUInt32(transferModelData.Items.FirstOrDefault());
                        return new SupremaGetAllLogsOfDevice(taskItem, _onlineDevices,_bioStarServer, _deviceService);
                    }

                case CommandType.GetLogsOfDeviceInPeriod:
                    //Gets and updates all log in a period of time from device
                    {
                        //var deviceId = Convert.ToUInt32(transferModelData.Items[0]);
                        //var startDate = Convert.ToInt64(transferModelData.Items[1]);
                        //var endDate = Convert.ToInt64(transferModelData.Items[2]);
                        return new SupremaGetLogsOfDeviceInPeriod(taskItem, _onlineDevices, _deviceService);
                    }

                case CommandType.GetLogsOfAllDevicesInPeriod:
                    //Gets and updates all log in a period of time from device
                    return new SupremaGetLogsOfAllDevices(_onlineDevices);

                case CommandType.DeleteUserFromTerminal:
                    {
                        //delete one or multiple users of device
                        return new SupremaDeleteUserFromTerminal(taskItem, _onlineDevices, _deviceService);
                    }

                case CommandType.RetrieveUserFromDevice:
                    {
                        //gets one or multiple users of device
                        return new SupremaRetrieveUserFromDevice(taskItem, _deviceService ,_onlineDevices, _userService,_userCardService,_fingerTemplateService,_faceTemplateService,_fingerTemplateTypes,_biometricTemplateManager,_faceTemplateTypes);
                    }

                #endregion

                #region WebClientRequests(WithResponse)
                case CommandType.GetUsersOfDevice:
                    //Gets users of devices

                    return new SupremaGetUsersOfDevice(taskItem, _deviceService, _onlineDevices);

                case CommandType.GetOnlineDevices:
                    //Gets online devices
                    return new SupremaGetOnlineDevices(_onlineDevices);

                #region Tools
                case CommandType.UserAdaptation:
                    return new SupremaUserAdaptation(transferModelData.Items, _onlineDevices, _deviceService, _taskTypes, _taskService, _taskStatuses, _taskItemTypes, _taskPriorities, _userService, _restClient);
                #endregion


                #endregion

                //#region BiominiClientRequests

                //case CommandType.DeviceConnectedCallback:
                //    //Gets and updates new logs from client
                //  //  return new BiominiDeviceConnectedCallback(Convert.ToUInt32(transferModelData.Items[0]), Convert.ToString(transferModelData.Items[1]), Connection);
                //case CommandType.GetAllLogsOfClient:
                //    //Gets and updates new logs from client
                //    //return new BiominiGetAllLogsOfClient(transferModelData.Items, Connection);
                //case CommandType.GetNewUserFromClient:
                //    //Gets and updates new logs from client
                //    //return new BiominiGetNewUserFromClient(transferModelData.Items, Connection);
                //case CommandType.UserChangedCallback:
                //    //Gets and updates new logs from client
                //  //  return new BiominiUserChangedCallback(transferModelData.Items, _onlineDevices);
                //case CommandType.ServerSideIdentification:
                //   // return new BiominiServerSideIdentification(transferModelData.Items, Connection);
                //case CommandType.EditFingerTemplate:
                //  //  return new BiominiEditFingerTemplateCallback(transferModelData.Items, Connection);
                //case CommandType.GetUser:
                //   // return new BiominiGetUser(transferModelData.Items, Connection);
                //case CommandType.GetServerTime:
                //    //return new BiominiGetServerTime(Connection);
                //#endregion

                default:
                    return null;
            }
        }
    }
}
