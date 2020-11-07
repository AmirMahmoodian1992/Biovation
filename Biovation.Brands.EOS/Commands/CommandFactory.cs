using Biovation.Brands.Eos.Commands;
using Biovation.Brands.EOS.Devices;
using Biovation.Brands.EOS.Manager;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System.Collections.Generic;

namespace Biovation.Brands.EOS.Commands
{
    /// <summary>
    /// ایجاد و بازگردانی یک نمونه از اتفاق با توجه به نوع آن
    /// </summary>
    public class CommandFactory
    {
        //private EventDispatcher _eventDispatcherObj;
        private readonly EosServer _eosServer;


        //private readonly LogEvents _logEvents;
        //private readonly LogService _logService;
        private readonly UserService _userService;
        private readonly TaskService _taskService;
        //private readonly TaskManager _taskManager;
        //private readonly DeviceBrands _deviceBrands;
        //private readonly TaskStatuses _taskStatuses;
        private readonly DeviceService _deviceService;
        ///private readonly LogSubEvents _logSubEvents;
       // private readonly MatchingTypes _matchingTypes;
        //private readonly TimeZoneService _timeZoneService;
        private readonly UserCardService _userCardService;
        //private readonly BlackListService _blackListService;
        //private readonly FaceTemplateTypes _faceTemplateTypes;
        //private readonly AccessGroupService _accessGroupService;
        //private readonly FaceTemplateService _faceTemplateService;
        private readonly AdminDeviceService _adminDeviceService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly FingerTemplateService _fingerTemplateService;
        //private readonly BiometricTemplateManager _biometricTemplateManager;
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
        public CommandFactory(EosServer eosServer,
            UserService userService, DeviceService deviceService,
            UserCardService userCardService,
            AccessGroupService accessGroupService, TimeZoneService timeZoneService, DeviceBrands deviceBrands, TaskStatuses taskStatuses, FingerTemplateService fingerTemplateService, FingerTemplateTypes fingerTemplateTypes, BiometricTemplateManager biometricTemplateManager, FaceTemplateTypes faceTemplateTypes, Dictionary<uint, Device> onlineDevices, AdminDeviceService adminDeviceService, TaskService taskService)
        {
            _eosServer = eosServer;
            //_logService = logService;
            _userService = userService;
            _taskService = taskService;
            _deviceService = deviceService;
            _userCardService = userCardService;
            // _blackListService = blackListService;
            _adminDeviceService = adminDeviceService;
            //_accessGroupService = accessGroupService;
            //_faceTemplateService = faceTemplateService;
            //_timeZoneService = timeZoneService;
            // _callbacks = callbacks;
            // _logEvents = logEvents;
            //_logSubEvents = logSubEvents;
            //_matchingTypes = matchingTypes;
            //_deviceBrands = deviceBrands;
            //_taskStatuses = taskStatuses;
            _fingerTemplateService = fingerTemplateService;
            _fingerTemplateTypes = fingerTemplateTypes;
            //_biometricTemplateManager = biometricTemplateManager;
            //_faceTemplateTypes = faceTemplateTypes;
            _onlineDevices = onlineDevices;
        }

        public ICommand Factory(int eventId, List<object> items)
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
        public ICommand Factory(DataTransferModel transferModelData)
        {
            //int.TryParse(transferModelData.Items[0].ToString(), out var taskItemId);
            //var taskItem = _taskService.GetTaskItem(taskItemId)?.Data;
            var taskItem = (TaskItem)transferModelData.Items[0];

            switch (transferModelData.EventId)
            {
                #region DatabaseRequests(NoResponces)

                #endregion

                #region WebClientRequests(WithResponse)

                case CommandType.DeleteUserFromTerminal:
                    {
                        //var deviceCode = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userIds = (uint)Convert.ToInt32(transferModelData.Items[1]);
                        return new EosDeleteUserFromTerminal(taskItem, _onlineDevices, _userService, _deviceService);
                    }
                case CommandType.SendUserToDevice:
                    {

                        //var deviceCode = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userIds = (uint)Convert.ToInt32(transferModelData.Items[1]);

                        return new EosSendUserToDevice(taskItem, _onlineDevices, _userService, _deviceService, _adminDeviceService);
                    }
                case CommandType.RetrieveUserFromDevice:
                    {

                        //var deviceCode = Convert.ToUInt32(transferModelData.Items[0]);
                        //var userIds = (uint)Convert.ToInt32(transferModelData.Items[1]);

                        return new EosRetrieveUserFromDevice(taskItem, _onlineDevices, _deviceService, _userService,_userCardService,_fingerTemplateService,_fingerTemplateTypes);
                    }
                case CommandType.RetrieveUsersListFromDevice:
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        return new EosGetUsersOfDevice(taskItem, _onlineDevices, _deviceService);
                    }
                case CommandType.RetrieveLogsOfDeviceInPeriod:
                    {
                        return new EosGetLogsOfDeviceInPeriod(taskItem,_onlineDevices,_deviceService);
                    }

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
