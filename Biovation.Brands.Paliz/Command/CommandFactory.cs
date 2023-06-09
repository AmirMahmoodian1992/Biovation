﻿using Biovation.Constants;
using System;
using System.Collections.Generic;
using Biovation.Service.Api.v2;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using Biovation.Brands.Paliz.Manager;

namespace Biovation.Brands.Paliz.Command
{
    public class CommandFactory
    {
        private readonly PalizServer _palizServer;
        private readonly PalizCodeMappings _palizCodeMappings;
        private readonly LogEvents _logEvents;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly UserService _userService;
        private readonly UserCardService _userCardService;
        private readonly LogService _logService;
        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;
        private readonly TimeZoneService _timeZoneService;

        public CommandFactory(PalizServer palizServer, LogEvents logEvents
            , TaskService taskService, PalizCodeMappings palizCodeMappings
            , DeviceService deviceService, FingerTemplateService fingerTemplateService
            , BiometricTemplateManager biometricTemplateManager, FaceTemplateService faceTemplateService
            , FaceTemplateTypes faceTemplateTypes, UserService userService, FingerTemplateTypes fingerTemplateTypes
            , UserCardService userCardService, LogService logService, LogSubEvents logSubEvents, MatchingTypes matchingTypes
            , TimeZoneService timeZoneService)
        {
            _palizCodeMappings = palizCodeMappings;
            _palizServer = palizServer;
            _logEvents = logEvents;
            _taskService = taskService;
            _deviceService = deviceService;
            _fingerTemplateService = fingerTemplateService;
            _biometricTemplateManager = biometricTemplateManager;
            _faceTemplateService = faceTemplateService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _faceTemplateTypes = faceTemplateTypes;
            _userService = userService;
            _userCardService = userCardService;
            _logService = logService;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
            _timeZoneService = timeZoneService;
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
                case CommandType.GetAllLogsOfDevice:
                    // Gets all the device access(traffic) logs.
                    {
                        return new PalizGetAllTrafficLogs(transferModelData.Items, _palizServer, _taskService, _deviceService, _logEvents, _palizCodeMappings);
                    }
                case CommandType.GetLogsOfDeviceInPeriod:
                    // Gets all the device access(traffic) logs in a period of time.
                    {
                        return new PalizGetTrafficLogsInPeriod(transferModelData.Items, _palizServer, _taskService, _deviceService, _logEvents, _palizCodeMappings);
                    }
                case CommandType.RetrieveUserFromDevice:
                    // Gets a specific user info from device.
                    {
                        return new PalizGetUserFromTerminal(transferModelData.Items, _palizServer, _taskService, _deviceService, _userService, _biometricTemplateManager,
                            _fingerTemplateTypes, _fingerTemplateService, _faceTemplateService, _faceTemplateTypes, _userCardService);
                    }
                case CommandType.RetrieveUsersListFromDevice:
                    // Gets a specific user info from device.
                    {
                        return new PalizGetAllUsersFromTerminal(transferModelData.Items, _palizServer, _deviceService, _userService, _biometricTemplateManager,
                            _fingerTemplateTypes, _fingerTemplateService, _faceTemplateService, _faceTemplateTypes, _userCardService);
                    }
                case CommandType.DeleteUserFromTerminal:
                    // Deletes the user with a specified ID.
                    {
                        return new PalizDeleteUserFromTerminal(transferModelData.Items, _palizServer, _taskService, _deviceService, _logService, _logEvents, _logSubEvents,
                            _matchingTypes);
                    }
                case CommandType.SendTimeZoneToDevice:
                    // Sends the time zone to the device
                    return new PalizSendTimeZoneToTerminal(transferModelData.Items, _taskService, _palizServer, _timeZoneService, _deviceService);
                case CommandType.SendUserToDevice:
                    // Gets a specific user info from device.
                    {
                        return new PalizSendUserToDevice(transferModelData.Items, _palizServer, _taskService, _deviceService, _userService, _biometricTemplateManager,
                            _fingerTemplateTypes, _fingerTemplateService, _logService, _faceTemplateService, _faceTemplateTypes, _userCardService, _palizCodeMappings);
                    }
                case CommandType.PersonnelEvent:
                    //Change in Personnel
                    throw new NotImplementedException();

                case CommandType.GuestEvent:
                    //Guest request
                    throw new NotImplementedException();

                case CommandType.ServerEventLogForceUpdate:
                    //Force Update from Server_Event_Log request
                    throw new NotImplementedException();

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    throw new NotImplementedException();
                //Get some data like mac,firmware and etc from device
                case CommandType.GetDeviceAdditionalData:
                    {
                        return new PalizGetAdditionalData(transferModelData.Items, _palizServer, _deviceService);
                    }
                case CommandType.SyncAllUsers:
                    //Sync Update request
                    throw new NotImplementedException();

                case CommandType.SetTime:
                    //Update time in all devices
                    //var timeToSet = Convert.ToInt32(transferModelData.Items.FirstOrDefault());
                    throw new NotImplementedException();

                case CommandType.SetDeviceDateTime:
                    {
                        return new PalizSetDeviceDateTime(transferModelData.Items, _palizServer, _taskService, _deviceService);
                    }

                case CommandType.EnrollFromTerminal:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //return new PalizEnrollFromTerminal(transferModelData.Items, PalizServer.GetOnlineDevices());
                        throw new NotImplementedException();
                    }
                case CommandType.GetOnlineDevices:
                    //Gets online devices
                    throw new NotImplementedException();

                default:
                    return null;
            }
        }
    }
}
