using Biovation.Brands.Virdi.Manager;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using RestSharp;
using System;
using System.Collections.Generic;
using UCBioBSPCOMLib;
using UCSAPICOMLib;
using AccessGroupService = Biovation.Service.Api.v1.AccessGroupService;
using AdminDeviceService = Biovation.Service.Api.v1.AdminDeviceService;
using BlackListService = Biovation.Service.Api.v1.BlackListService;
using DeviceService = Biovation.Service.Api.v1.DeviceService;
using FaceTemplateService = Biovation.Service.Api.v1.FaceTemplateService;
using FingerTemplateService = Biovation.Service.Api.v1.FingerTemplateService;
using LogService = Biovation.Service.Api.v1.LogService;
using TaskService = Biovation.Service.Api.v1.TaskService;
using UserCardService = Biovation.Service.Api.v1.UserCardService;
using UserService = Biovation.Service.Api.v1.UserService;

namespace Biovation.Brands.Virdi.Command
{
    /// <summary>
    /// ایجاد و بازگردانی یک نمونه از اتفاق با توجه به نوع آن
    /// </summary>
    public class CommandFactory
    {
        private readonly VirdiServer _virdiServer;
        private readonly UCSAPI _ucsApi;
        private readonly IServerUserData _serverUserData;
        private readonly ITerminalUserData _terminalUserData;
        //private readonly IServerAuthentication _serverAuthentication;
        private readonly IAccessLogData _accessLogData;
        //private readonly ITerminalOption _terminalOption;
        private readonly IAccessControlData _accessControlData;
        //internal readonly ISmartCardLayout SmartCardLayout;

        // UCBioBSP
        //private readonly UCBioBSPClass _ucBioBsp;
        private readonly IFPData _fpData;
        //internal ITemplateInfo TemplateInfo;
        //private readonly IDevice _device;
        //private readonly IExtraction _extraction;
        //private readonly IFastSearch _fastSearch;
        private readonly IMatching _matching;
        //private readonly ITemplateInfo _templateInfo;

        //private readonly ISmartCard _smartCard;

        private readonly LogEvents _logEvents;
        private readonly TaskTypes _taskTypes;
        private readonly RestClient _restClient;
        private readonly LogService _logService;
        private readonly UserService _userService;
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        //private readonly TaskManager _taskManager;
        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;
        private readonly DeviceService _deviceService;
        private readonly TimeZoneService _timeZoneService;
        private readonly UserCardService _userCardService;
        private readonly BlackListService _blackListService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly AdminDeviceService _adminDeviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly VirdiCodeMappings _virdiCodeMappings;
        private readonly IrisTemplateTypes _irisTemplateTypes;
        private readonly IrisTemplateService _rIrisTemplateService;


        public CommandFactory(VirdiServer virdiServer, LogService logService,
            UserService userService, TaskService taskService, DeviceService deviceService,
            UserCardService userCardService, BlackListService blackListService, AdminDeviceService adminDeviceService,
            AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, TimeZoneService timeZoneService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes, UCSAPI ucsApi, FingerTemplateService fingerTemplateService, FaceTemplateTypes faceTemplateTypes, FingerTemplateTypes fingerTemplateTypes, BiometricTemplateManager biometricTemplateManager, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, RestClient restClient, VirdiCodeMappings virdiCodeMappings, IrisTemplateTypes irisTemplateTypes, IrisTemplateService rIrisTemplateService, ITerminalUserData terminalUserData, IAccessControlData accessControlData, IFPData fpData, IServerUserData serverUserData, IAccessLogData accessLogData, IMatching matching)
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
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
            _ucsApi = ucsApi;
            _fingerTemplateService = fingerTemplateService;
            _faceTemplateTypes = faceTemplateTypes;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _restClient = restClient;
            _virdiCodeMappings = virdiCodeMappings;
            _irisTemplateTypes = irisTemplateTypes;
            _rIrisTemplateService = rIrisTemplateService;
            _terminalUserData = terminalUserData;
            _accessControlData = accessControlData;
            _fpData = fpData;
            _serverUserData = serverUserData;
            _accessLogData = accessLogData;
            _matching = matching;
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
                        return new VirdiSendAccessGroupToTerminal(transferModelData.Items, _virdiServer, _taskService, _deviceService, _accessGroupService, _accessControlData);
                    }

                case CommandType.SendTimeZoneToDevice:
                    //Transfer Time Zone request
                    {
                        var code = Convert.ToUInt32(transferModelData.Items[0]);
                        var timeZoneId = Convert.ToInt32(transferModelData.Items[1]);
                        return new VirdiSendTimeZoneToTerminal(code, timeZoneId, _virdiServer, _timeZoneService, _accessControlData);
                    }

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    throw new NotImplementedException();

                case CommandType.SendUserToDevice:
                    //Transfer Specific User to Specific Device request
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt32(transferModelData.Items[1]);
                        return new VirdiSendUserToDevice(transferModelData.Items, _virdiServer, _logService, _userService, _taskService, _deviceService, _userCardService, _blackListService, _adminDeviceService, _accessGroupService, _faceTemplateService, _logEvents, _logSubEvents, _matchingTypes, _virdiCodeMappings, _rIrisTemplateService, _serverUserData);
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

                        return new VirdiRetrieveAllLogsOfDevice(transferModelData.Items, _virdiServer, _deviceService, _accessLogData);

                    }

                case CommandType.RetrieveLogsOfDeviceInPeriod:
                    //Gets and updates all log in a period of time from device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var startDate = Convert.ToDateTime(transferModelData.Items[1]);
                        //var endDate = Convert.ToDateTime(transferModelData.Items[2]);
                        return new VirdiRetrieveAllLogsOfDeviceInPeriod(transferModelData.Items, _virdiServer, _taskService, _deviceService, _accessLogData);
                    }

                case CommandType.LockDevice:
                    //Locks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new VirdiLockDevice(transferModelData.Items, _virdiServer, _deviceService, _ucsApi);
                    }

                case CommandType.UnlockDevice:
                    //Unlocks the device
                    {
                        return new VirdiUnlockDevice(transferModelData.Items, _virdiServer, _deviceService, _ucsApi);
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
                        return new VirdiEnrollFaceFromTerminal(transferModelData.Items, _virdiServer, _deviceService, _terminalUserData);
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
                        return new VirdiDeleteUserFromTerminal(transferModelData.Items, _virdiServer, _taskService, _logService, _deviceService, _logEvents, _logSubEvents, _matchingTypes, _terminalUserData);
                    }

                case CommandType.RetrieveUserFromDevice:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userId = Convert.ToInt32(transferModelData.Items[1]);
                        return new VirdiRetrieveUserFromTerminal(transferModelData.Items, _virdiServer, _ucsApi, _taskService, _userService, _deviceService, _userCardService, _faceTemplateTypes, _faceTemplateService, _fingerTemplateTypes, _fingerTemplateService, _biometricTemplateManager, _rIrisTemplateService, _irisTemplateTypes, _fpData, _terminalUserData, _matching, _accessGroupService);
                    }

                case CommandType.RetrieveUsersListFromDevice:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new VirdiRetrieveUsersListFromTerminal(transferModelData.Items, _virdiServer, _ucsApi, _deviceService, _terminalUserData);
                    }
                case CommandType.OpenDoor:
                    //Unlocks the device
                    {
                        return new VirdiOpenDoor(transferModelData.Items, _virdiServer, _deviceService, _ucsApi);
                    }
                case CommandType.UpgradeFirmware:
                    //Unlocks the device
                    {
                        //var deviceCode = Convert.ToInt32(transferModelData.Items[0]);
                        //var filePath = Convert.ToString(transferModelData.Items[1]);
                        return new VirdiUpgradeDeviceFirmware(transferModelData.Items, _virdiServer, _taskService, _deviceService, _ucsApi);
                    }
                case CommandType.GetDeviceAdditionalData:
                    {
                        return new VirdiGetAdditionalData(transferModelData.Items, _ucsApi, _virdiServer, _deviceService,
                            _terminalUserData, _accessLogData);
                    }

                #region Tools
                case CommandType.UserAdaptation:
                    return new VirdiUserAdaptation(transferModelData.Items, _virdiServer.GetOnlineDevices(), _deviceService, _taskTypes, _taskService, _taskStatuses, _taskItemTypes, _taskPriorities, _userService, _restClient, _faceTemplateTypes, _fingerTemplateTypes, _biometricTemplateManager, _ucsApi, _terminalUserData);
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
