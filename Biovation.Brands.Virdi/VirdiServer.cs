using Biovation.Brands.Virdi.Manager;
using Biovation.Brands.Virdi.Service;
using Biovation.Brands.Virdi.UniComAPI;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Kasra.MessageBus.Domain.Enumerators;
using Kasra.MessageBus.Domain.Interfaces;
using Kasra.MessageBus.Infrastructure;
using Kasra.MessageBus.Managers.Sinks.EventBus;
using Kasra.MessageBus.Managers.Sinks.Internal;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UCBioBSPCOMLib;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi
{
    public class VirdiServer : IDisposable
    {
        #region CTOR

        private readonly UserService _commonUserService;
        private readonly DeviceService _commonDeviceService;
        private readonly UserCardService _commonUserCardService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly VirdiLogService _virdiLogService;
        private readonly VirdiCodeMappings _virdiCodeMappings;
        private readonly LogService _logService;
        private readonly LogEvents _logEvents;
        private readonly TaskStatuses _taskStatuses;
        private readonly DeviceBrands _deviceBrands;
        private readonly BlackListService _blackListService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly RestClient _monitoringRestClient;//= new RestClient(_configurationManager.LogMonitoringApiUrl);
        public static bool ModifyUserData = false;
        public static bool GetUserTaskFinished = true;
        public static bool GetLogTaskFinished = true;
        public static bool UploadFirmwareFileTaskFinished = true;
        public static bool UpgradeFirmwareTaskFinished = true;
        public static List<User> RetrieveUsers = new List<User>();
        public static Dictionary<int, List<Log>> RetrievedLogs = new Dictionary<int, List<Log>>();
        private readonly TaskService _taskService;
        private readonly AccessGroupService _accessGroupService;
        private readonly ILogger<VirdiServer> _logger;
        private Timer _fixDaylightSavingTimer;

        //Todo: Find better solution
        //private readonly Dictionary<int, TaskInfo> _tasks = new Dictionary<int, TaskInfo>();

        private BiovationConfigurationManager BiovationConfiguration { get; }

        public int GetAccessLogType;
        public DateTime AccessLogPeriodFromDateTime;
        public DateTime AccessLogPeriodToDateTime;

        private int _logCount;

        //public static readonly Semaphore GetLogSemaphore = new Semaphore(1, 1);
        //public static readonly Semaphore SetLogPeriodSemaphore = new Semaphore(1, 1);

        // UCSAPI
        private readonly UCSAPICOMLib.UCSAPI _ucsApi;
        //private readonly IServerUserData _serverUserData;
        //private readonly ITerminalUserData _terminalUserData;
        private readonly IServerAuthentication _serverAuthentication;
        private readonly IAccessLogData _accessLogData;
        private readonly ITerminalOption _terminalOption;

        //private readonly IAccessControlData _accessControlData;
        //internal readonly ISmartCardLayout SmartCardLayout;

        // UCBioBSP
        private readonly UCBioBSPClass _ucBioBsp;

        private readonly IFPData _fpData;
        //internal ITemplateInfo TemplateInfo;
        //private readonly IDevice _device;
        //private readonly IExtraction _extraction;
        //private readonly IFastSearch _fastSearch;
        private readonly IMatching _matching;
        //private readonly ITemplateInfo _templateInfo;

        //private readonly ISmartCard _smartCard;

        internal readonly object LoadFingerTemplateLock = new object();


        private static Dictionary<uint, DeviceBasicInfo> _onlineDevices;
        //private readonly Dictionary<int, string> _deviceTypes = new Dictionary<int, string>();
        private readonly Dictionary<long, IFastSearch> _fastSearchOfDevices = new Dictionary<long, IFastSearch>();
        private readonly Dictionary<long, IFPData> _fingerPrintDataOfDevices = new Dictionary<long, IFPData>();
        private readonly Dictionary<long, UCBioBSPClass> _bioBspClasses = new Dictionary<long, UCBioBSPClass>();

        //private readonly Dictionary<int, IFastSearch> _fastSearchOfUsers = new Dictionary<int, IFastSearch>();
        //private readonly Dictionary<int, IFPData> _fingerPrintDataOfUsers = new Dictionary<int, IFPData>();
        //private readonly Dictionary<int, UCBioBSPClass> _bioBspClassesOfUsers = new Dictionary<int, UCBioBSPClass>();

        ///// <summary>
        ///// نمونه ی ساخته شده از سرور
        ///// </summary>
        //private static Callbacks _callbacksObj;

        ///// <summary>
        ///// <En>Make or return the unique instance of Biostar Server.</En>
        ///// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        ///// </summary>
        ///// <returns></returns>
        //public static Callbacks FactoryCallbacks(UCSAPICOMLib.UCSAPI ucsapi, Dictionary<uint, DeviceBasicInfo> onlineDevices)
        //{
        //    lock (Object)
        //    {
        //        return _callbacksObj ?? (_callbacksObj = new Callbacks(ucsapi, onlineDevices));
        //    }
        //}

        //public static Callbacks GetInstance()
        //{
        //    lock (Object)
        //    {
        //        return _callbacksObj;
        //    }
        //}



        //integration


        private const string BiovationTopicName = "BiovationTaskStatusUpdateEvent";

        private ISource<DataChangeMessage<ConnectionStatus>> _deviceConnectionStateInternalSource;
        private const string DeviceConnectionStateTopicName = "BiovationDeviceConnectionStateEvent";

        public void DeleteUserFromDeviceFastSearch(uint deviceCode, int userCode)
        {
            lock (_fastSearchOfDevices)
            {
                try
                {
                    lock (_fastSearchOfDevices)
                        if (_fastSearchOfDevices.ContainsKey((int)deviceCode))
                            _fastSearchOfDevices[(int)deviceCode].RemoveUser(userCode);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
            }
        }

        public async Task AddUserToDeviceFastSearch(uint deviceCode, int userCode)
        {
            await Task.Run(() =>
            {
                lock (_fastSearchOfDevices)
                {
                    try
                    {
                        var user = _commonUserService.GetUsers(code: userCode).FirstOrDefault();
                        if (user != null)
                        {
                            var userFingerTemplates = user.FingerTemplates.Where(fingerTemplate =>
                                fingerTemplate.FingerTemplateType.Code == FingerTemplateTypes.V400Code).ToList();
                            if (_fastSearchOfDevices.ContainsKey((int)deviceCode))
                            {
                                var fpData = _fingerPrintDataOfDevices[(int)deviceCode];
                                var fastSearch = _fastSearchOfDevices[(int)deviceCode];
                                fpData.ClearFPData();

                                for (var i = 0; i < userFingerTemplates.Count - 1; i += 2)
                                {
                                    fpData.Import(0, userFingerTemplates[i].FingerIndex.OrderIndex, 2, 400, 400,
                                        userFingerTemplates[i].Template, userFingerTemplates[i + 1].Template);
                                }

                                var firTemplate = fpData.TextFIR;
                                fastSearch?.AddFIR(firTemplate, userCode);
                                fpData.ClearFPData();
                            }

                            else
                                LoadFingerTemplates().Wait();
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                }
            });
        }

        public VirdiServer(UCSAPICOMLib.UCSAPI ucsapi, UserService commonUserService, DeviceService commonDeviceService
            , UserCardService commonUserCardService, FingerTemplateService fingerTemplateService
            , LogService logService, BlackListService blackListService, FaceTemplateService faceTemplateService, TaskService taskService
            , AccessGroupService accessGroupService, BiovationConfigurationManager biovationConfiguration, VirdiLogService virdiLogService
            , FingerTemplateTypes fingerTemplateTypes, VirdiCodeMappings virdiCodeMappings, DeviceBrands deviceBrands
            , LogEvents logEvents, FaceTemplateTypes faceTemplateTypes, ILogger<VirdiServer> logger, TaskStatuses taskStatuses, Dictionary<uint, DeviceBasicInfo> onlineDevices, IServerAuthentication serverAuthentication, IAccessLogData accessLogData, ITerminalOption terminalOption, UCBioBSPClass ucBioBsp, IFPData fpData, IMatching matching)
        {
            _commonUserService = commonUserService;
            _commonDeviceService = commonDeviceService;
            _commonUserCardService = commonUserCardService;
            _fingerTemplateService = fingerTemplateService;
            _logService = logService;
            _blackListService = blackListService;
            _faceTemplateService = faceTemplateService;
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _accessGroupService = accessGroupService;
            BiovationConfiguration = biovationConfiguration;
            _virdiLogService = virdiLogService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _virdiCodeMappings = virdiCodeMappings;
            _onlineDevices = onlineDevices;
            _serverAuthentication = serverAuthentication;
            _accessLogData = accessLogData;
            _terminalOption = terminalOption;
            _ucBioBsp = ucBioBsp;
            _fpData = fpData;
            _matching = matching;

            _deviceBrands = deviceBrands;
            _logEvents = logEvents;
            _faceTemplateTypes = faceTemplateTypes;
            _monitoringRestClient = (RestClient)new RestClient(biovationConfiguration.LogMonitoringApiUrl).UseSerializer(() => new RestRequestJsonSerializer());

            _logger = logger;
            _ucsApi = ucsapi;

            //_ucBioApi = new UCBioAPI();
            //_ucBioApiExport = new UCBioAPI.Export(_ucBioApi);

            //ServerUserData = UcsApi.ServerUserData as IServerUserData;
            //TerminalUserData = UcsApi.TerminalUserData as ITerminalUserData;
            //AccessLogData = UcsApi.AccessLogData as IAccessLogData;
            //ServerAuthentication = UcsApi.ServerAuthentication as IServerAuthentication;
            //TerminalOption = UcsApi.TerminalOption as ITerminalOption;
            //SmartCardLayout = UcsApi.SmartCardLayout as ISmartCardLayout;
            //AccessControlData = UcsApi.AccessControlData as IAccessControlData;

            //// create UCBioBSP Instance
            //_ucBioBsp = new UCBioBSPClass();
            //FpData = _ucBioBsp.FPData as IFPData;
            //Device = _ucBioBsp.Device as IDevice;
            //Extraction = _ucBioBsp.Extraction as IExtraction;
            ////_fastSearch = _ucBioBsp.FastSearch as IFastSearch;
            //_matching = _ucBioBsp.Matching as IMatching;
            ////_smartCard = _ucBioBsp.SmartCard as ISmartCard;

            LoadFingerTemplates().Wait();

            //var deviceModels = _commonDeviceService.GetDeviceModelsByBrandCode(DeviceBrands.VirdiCode);

            //foreach (var deviceModel in deviceModels)
            //{
            //    _deviceTypes.Add(deviceModel.ManufactureCode, deviceModel.Name);
            //}

            // create event handle
            _ucsApi.EventTerminalConnected += TerminalConnectedCallback;
            _ucsApi.EventTerminalDisconnected += TerminalDisconnectedCallback;
            _ucsApi.EventGetTerminalTime += GetTerminalTimeCallback;
            //ucsAPI.EventAddUser += new AddUserEventHandler(ucsAPI_EventAddUser);
            _ucsApi.EventAntipassback += GetAntiPassBack;
            _ucsApi.EventAuthTypeWithUniqueID += AuthTypeWithUniqueId;
            _ucsApi.EventAuthTypeWithUserID += AuthTypeWithUserId;
            //ucsAPI.EventControlPeripheralDevice += new ControlPeripheralDeviceEventHandler(ucsAPI_EventControlPeripheralDevice);
            //ucsAPI.EventDeleteAllUser += new DeleteAllUserEventHandler(ucsAPI_EventDeleteAllUser);
            //ucsAPI.EventDeleteUser += new DeleteUserEventHandler(ucsAPI_EventDeleteUser);
            //ucsAPI.EventFingerImageData += new FingerImageDataEventHandler(ucsAPI_EventFingerImageData);
            _ucsApi.EventFirmwareUpgraded += FirmwareUpgraded;
            _ucsApi.EventFirmwareUpgrading += FirmwareUpgrading;
            _ucsApi.EventFirmwareVersion += FirmwareVersionCallback;
            _ucsApi.EventGetAccessLog += GetAccessLogCallback;
            _ucsApi.EventGetAccessLogCount += GetAccessLogCount;
            //ucsAPI.EventGetTAFunction += new GetTAFunctionEventHandler(ucsAPI_EventGetTAFunction);
            //ucsAPI.EventGetUserCount += new GetUserCountEventHandler(ucsAPI_EventGetUserCount);
            //UcsApi.EventGetUserData += GetUserDataCallback;
            //UcsApi.EventGetUserInfoList += GetUserListCallback;
            //ucsAPI.EventOpenDoor += new OpenDoorEventHandler(ucsAPI_EventOpenDoor);
            _ucsApi.EventPictureLog += PictureLog;
            _ucsApi.EventRealTimeAccessLog += RealTimeAccessLogCallback;
            //ucsAPI.EventSetAccessControlData += new SetAccessControlDataEventHandler(ucsAPI_EventSetAccessControlData);
            //ucsAPI.EventSetTAFunction += new SetTAFunctionEventHandler(ucsAPI_EventSetTAFunction);
            //ucsAPI.EventSetTATime += new SetTATimeEventHandler(ucsAPI_EventSetTATime);
            _ucsApi.EventTerminalStatus += TerminalStatus;

            _ucsApi.EventVerifyCard += VerifyCardCallback;
            _ucsApi.EventVerifyFinger1to1 += VerifyFinger1To1Callback;
            _ucsApi.EventVerifyFinger1toN += VerifyFinger1ToNCallback;
            _ucsApi.EventVerifyPassword += VerifyPassword;
            _ucsApi.EventVerifyFace1to1 += VerifyFace1To1;
            _ucsApi.EventVerifyFace1toN += VerifyFace1ToN;

            //ucsAPI.EventPrivateMessage += new PrivateMessageEventHandler(ucsAPI_EventPrivateMessage);
            //ucsAPI.EventPublicMessage += new PublicMessageEventHandler(ucsAPI_EventPublicMessage);
            //ucsAPI.EventUserFileUpgrading += new UserFileUpgradingEventHandler(ucsAPI_EventUserFileUpgrading);
            //ucsAPI.EventUserFileUpgraded += new UserFileUpgradedEventHandler(ucsAPI_EventUserFileUpgraded);
            //ucsAPI.EventEmergency += new EmergencyEventHandler(ucsAPI_EventEmergency);
            //ucsAPI.EventSetEmergency += new SetEmergencyEventHandler(ucsAPI_EventSetEmergency);
            //
            //ucsAPI.EventTerminalControl += new TerminalControlEventHandler(ucsAPI_EventTerminalControl);
            _ucsApi.EventRegistFace += RegisterFace;
            //ucsAPI.EventACUStatus += new ACUStatusEventHandler(ucsAPI_EventACUStatus);
            //ucsAPI.EventGetOptionFromACU += new GetOptionFromACUEventHandler(ucsAPI_EventGetOptionFromACU);
            _ucsApi.EventGetTerminalOption += GetTerminalOptionCallback;
            //ucsAPI.EventSetOptionToACU += new SetOptionToACUEventHandler(ucsAPI_EventSetOptionToACU);
            //ucsAPI.EventGetLockScheduleFromACU += new GetLockScheduleFromACUEventHandler(ucsAPI_EventGetLockScheduleFromACU);
            //ucsAPI.EventSetLockScheduleToACU += new SetLockScheduleToACUEventHandler(ucsAPI_EventSetLockScheduleToACU);
            //ucsAPI.EventAlarmFromACU += new AlarmFromACUEventHandler(ucsAPI_EventAlarmFromACU);
            //ucsAPI.EventSetSirenToTerminal += new SetSirenToTerminalEventHandler(ucsAPI_EventSetSirenToTerminal);
            //ucsAPI.EventGetSirenFromTerminal += new GetSirenFromTerminalEventHandler(ucsAPI_EventGetSirenFromTerminal);
            //ucsAPI.EventSetSmartCardLayout += new SetSmartCardLayoutEventHandler(ucsAPI_EventSetSmartCardLayout);
            //ucsAPI.EventGetFpMinutiaeFromTerminal += new GetFpMinutiaeFromTerminalEventHandler(ucsAPI_EventGetFpMinutiaeFromTerminal);
            //
            //ucsAPI.EventGetVoipInfoFromTerminal += new GetVoipInfoFromTerminalEventHandler(ucsAPI_EventGetVoipInfoFromTerminal);
            //ucsAPI.EventSetVoipInfoToTerminal += new SetVoipInfoToTerminalEventHandler(ucsAPI_EventSetVoipInfoToTerminal);
            //ucsAPI.EventSetPGMOutputToACU += new SetPGMOutputToACUEventHandler(ucsAPI_EventSetPGMOutputToACU);
            //
            _ucBioBsp.OnCaptureEvent += ucBioBSP_OnCaptureEvent;
            _ucBioBsp.OnEnrollEvent += ucBioBSP_OnEnrollEvent;
        }

        public void StartServer()
        {
            //integration 
            var kafkaServerAddress = BiovationConfiguration.KafkaServerAddress;
            var biovationInternalSource = InternalSourceBuilder.Start().SetPriorityLevel(PriorityLevel.Medium)
                .Build<DataChangeMessage<TaskInfo>>();

            var biovationKafkaTarget = KafkaTargetBuilder.Start().SetBootstrapServer(kafkaServerAddress).SetTopicName(BiovationTopicName)
                .BuildTarget<DataChangeMessage<TaskInfo>>();

            var biovationTaskConnectorNode = new ConnectorNode<DataChangeMessage<TaskInfo>>(biovationInternalSource, biovationKafkaTarget);
            biovationTaskConnectorNode.StartProcess();

            //DeviceStatus integration 
            _deviceConnectionStateInternalSource = InternalSourceBuilder.Start().SetPriorityLevel(PriorityLevel.Medium)
                .Build<DataChangeMessage<ConnectionStatus>>();

            var deviceConnectionStateKafkaTarget = KafkaTargetBuilder.Start().SetBootstrapServer(kafkaServerAddress).SetTopicName(DeviceConnectionStateTopicName)
                .BuildTarget<DataChangeMessage<ConnectionStatus>>();

            var deviceConnectionStateConnectorNode = new ConnectorNode<DataChangeMessage<ConnectionStatus>>(_deviceConnectionStateInternalSource, deviceConnectionStateKafkaTarget);
            deviceConnectionStateConnectorNode.StartProcess();

            _ucsApi.ServerStart(150, BiovationConfiguration.VirdiDevicesConnectionPort);

            Logger.Log(_ucsApi.ErrorCode != 0
                    ? $"Error on starting service.\n   +ErrorCode:{_ucsApi.ErrorCode} {_ucsApi.EventError}"
                    : $"Service started on port: {BiovationConfiguration.VirdiDevicesConnectionPort}"
                , logType: _ucsApi.ErrorCode != 0 ? LogType.Error : LogType.Information);


            try
            {
                //var daylightSaving = DateTime.Now.DayOfYear <= 81 || DateTime.Now.DayOfYear > 265 ? new DateTime(DateTime.Now.Year, 3, 22, 0, 2, 0) : new DateTime(DateTime.Now.Year, 9, 22, 0, 2, 0);
                //var dueTime = (daylightSaving.Ticks - DateTime.Now.Ticks) / 10000;
                var dueTime =
                    ((DateTime.Now.DayOfYear <= 81 || DateTime.Now.DayOfYear > 265
                        ? new DateTime(DateTime.Now.Year, 3, 22, 0, 0, 10)
                        : new DateTime(DateTime.Now.Year, 9, 22, 0, 0, 10)) - DateTime.Now).TotalMilliseconds;
                _fixDaylightSavingTimer = new Timer(FixDaylightSavingTimer_Elapsed, null, (long)dueTime, (long)TimeSpan.FromHours(24).TotalMilliseconds);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }
        }

        public void StopServer()
        {
            _ucsApi.ServerStop();
        }

        public Dictionary<uint, DeviceBasicInfo> GetOnlineDevices()
        {
            return _onlineDevices;
        }

        private void FixDaylightSavingTimer_Elapsed(object state)
        {
            StopServer();
            Thread.Sleep(3000);
            StartServer();
        }


        //    public void GetUserDataCallback(int clientId, int terminalId)
        //    {
        //        //if (clientId != TaskItemId)
        //        //    return;

        //        try
        //        {
        //            lock (UcsApi)
        //            {
        //                lock (TerminalUserData)
        //                {
        //                    var isoEncoding = Encoding.GetEncoding(28591);
        //                    var windowsEncoding = Encoding.GetEncoding(1256);


        //                    var deviceUserName = TerminalUserData.UserName;
        //                    var replacements = new Dictionary<string, string> { { "˜", "\u0098" }, { "Ž", "\u008e" } };
        //                    var userName = replacements.Aggregate(deviceUserName, (current, replacement) => current.Replace(replacement.Key, replacement.Value));

        //                    userName = string.IsNullOrEmpty(userName) ? null : windowsEncoding.GetString(isoEncoding.GetBytes(userName)).Trim();

        //                    var indexOfSpace = userName?.IndexOf(' ') ?? 0;
        //                    var firstName = indexOfSpace > 0 ? userName?.Substring(0, indexOfSpace).Trim() : null;
        //                    var surName = indexOfSpace > 0 ? userName?.Substring(indexOfSpace, userName.Length - indexOfSpace).Trim() : userName;

        //                    byte[] picture = null;

        //                    try
        //                    {
        //                        if (TerminalUserData.PictureDataLength > 0)
        //                            picture = TerminalUserData.PictureData as byte[];
        //                    }
        //                    catch (Exception exception)
        //                    {
        //                        Logger.Log(exception);
        //                    }

        //                    var user = new User
        //                    {
        //                        Code = TerminalUserData.UserID,
        //                        AdminLevel = TerminalUserData.IsAdmin,
        //                        StartDate = TerminalUserData.StartAccessDate == "0000-00-00"
        //                               ? DateTime.Parse("1970/01/01")
        //                               : DateTime.Parse(TerminalUserData.StartAccessDate),
        //                        EndDate = TerminalUserData.EndAccessDate == "0000-00-00"
        //                               ? DateTime.Parse("2050/01/01")
        //                               : DateTime.Parse(TerminalUserData.EndAccessDate),
        //                        AuthMode = TerminalUserData.AuthType,
        //                        Password = TerminalUserData.Password,
        //                        UserName = userName,
        //                        FirstName = firstName,
        //                        SurName = surName,
        //                        IsActive = true,
        //                        ImageBytes = picture
        //                    };
        //                    //user.Id = _userService.GetUsers(code: TerminalUserData.UserID, withPicture: false)?.FirstOrDefault()?.Id == null
        //                    //   ? 0
        //                    //   : _userService.GetUsers(code: TerminalUserData.UserID, withPicture: false).FirstOrDefault().Id;

        //                    //if (RetrieveUsers.All(retrievedUser => retrievedUser.Id != user.Id))
        //                    //{
        //                    //    RetrieveUsers.Add(user);
        //                    //}

        //                    //if (ModifyUserData)
        //                    //{
        //                    var existUser = _commonUserService.GetUsers(code: TerminalUserData.UserID).FirstOrDefault();
        //                    if (existUser != null)
        //                    {
        //                        user = new User
        //                        {
        //                            Id = existUser.Id,
        //                            Code = existUser.Code,
        //                            AdminLevel = TerminalUserData.IsAdmin,
        //                            StartDate = TerminalUserData.StartAccessDate == "0000-00-00"
        //                                ? existUser.StartDate
        //                                : DateTime.Parse(TerminalUserData.StartAccessDate),
        //                            EndDate = TerminalUserData.EndAccessDate == "0000-00-00"
        //                                ? existUser.EndDate
        //                                : DateTime.Parse(TerminalUserData.EndAccessDate),
        //                            AuthMode = TerminalUserData.AuthType,
        //                            Password = TerminalUserData.Password,
        //                            UserName = string.IsNullOrEmpty(userName) ? existUser.UserName : userName,
        //                            FirstName = firstName ?? existUser.FirstName,
        //                            SurName = string.Equals(surName, userName) ? existUser.SurName ?? surName : surName,
        //                            IsActive = existUser.IsActive,
        //                            ImageBytes = picture
        //                        };
        //                    }

        //                    var userInsertionResult = _commonUserService.ModifyUser(user);

        //                    Logger.Log("<--User is Modified");
        //                    user.Id = userInsertionResult.Id;

        //                    //Card
        //                    try
        //                    {
        //                        Logger.Log($"   +TotalCardCount:{TerminalUserData.CardNumber}");
        //                        if (TerminalUserData.CardNumber > 0)
        //                            for (var i = 0; i < TerminalUserData.CardNumber; i++)
        //                            {
        //                                var card = new UserCard
        //                                {
        //                                    CardNum = TerminalUserData.RFID[i],
        //                                    IsActive = true,
        //                                    UserId = user.Id
        //                                };
        //                                _commonUserCardService.ModifyUserCard(card);
        //                            }
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Logger.Log(e);
        //                    }

        //                    //Finger
        //                    try
        //                    {
        //                        var nFpDataCount = TerminalUserData.TotalFingerCount;
        //                        Logger.Log($"   +TotalFingerCount:{nFpDataCount}");

        //                        if (user.FingerTemplates is null)
        //                            user.FingerTemplates = new List<FingerTemplate>();

        //                        for (var i = 0; i < nFpDataCount; i++)
        //                        {
        //                            var sameTemplateExists = false;
        //                            var fingerIndex = TerminalUserData.FingerID[i];
        //                            if (existUser != null)
        //                            {
        //                                // if (existUser.FingerTemplates.Exists(fp =>
        //                                //fp.FingerIndex.Code == BiometricTemplateManager.GetFingerIndex(fingerIndex).Code && fp.FingerTemplateType == FingerTemplateTypes.V400))
        //                                lock (FpData)
        //                                {
        //                                    var firstSampleCheckSum = 0;
        //                                    var secondSampleCheckSum = 0;

        //                                    var firstTemplateSample = TerminalUserData.FPSampleData[fingerIndex, 0] as byte[];
        //                                    byte[] secondTemplateSample = null;
        //                                    try
        //                                    {
        //                                        secondTemplateSample = TerminalUserData.FPSampleData[fingerIndex, 1] as byte[];
        //                                    }
        //                                    catch (Exception exception)
        //                                    {
        //                                        Logger.Log(exception);
        //                                    }

        //                                    if (firstTemplateSample != null) firstSampleCheckSum = firstTemplateSample.Sum(x => x);
        //                                    if (secondTemplateSample != null) secondSampleCheckSum = secondTemplateSample.Sum(x => x);

        //                                    if (FpData == null) continue;
        //                                    FpData.ClearFPData();
        //                                    FpData.Import(1, TerminalUserData.CurrentIndex, 2, 400, 400, firstTemplateSample, secondTemplateSample);

        //                                    var deviceTemplate = FpData.TextFIR;
        //                                    var fingerTemplates = _fingerTemplateService.FingerTemplates(userId: (int)(existUser.Id)).Where(ft => ft.FingerTemplateType.Code == FingerTemplateTypes.V400Code).ToList();

        //                                    if (fingerTemplates.Exists(ft => ft.CheckSum == firstSampleCheckSum) || fingerTemplates.Exists(ft => ft.CheckSum == secondSampleCheckSum))
        //                                        continue;

        //                                    for (var j = 0; j < fingerTemplates.Count - 1; j += 2)
        //                                    {
        //                                        if (FpData == null) continue;
        //                                        FpData.ClearFPData();
        //                                        FpData.Import(1, fingerTemplates[j].FingerIndex.OrderIndex, 2, 400, 400,
        //                                            fingerTemplates[j].Template, fingerTemplates[j + 1].Template);
        //                                        var firTemplate = FpData.TextFIR;
        //                                        lock (_matching)
        //                                        {
        //                                            _matching.VerifyMatch(deviceTemplate, firTemplate);
        //                                            if (_matching.MatchingResult == 0) continue;
        //                                        }

        //                                        sameTemplateExists = true;
        //                                        break;
        //                                    }

        //                                    if (sameTemplateExists) continue;

        //                                    user.FingerTemplates.Add(new FingerTemplate
        //                                    {
        //                                        FingerIndex = _biometricTemplateManager.GetFingerIndex(TerminalUserData.FingerID[i]),
        //                                        Index = _fingerTemplateService.FingerTemplates(userId: (int)(existUser.Id))?.Count(ft => ft.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(TerminalUserData.FingerID[i]).Code) ?? 0 + 1,
        //                                        TemplateIndex = 0,
        //                                        Size = TerminalUserData.FPSampleDataLength[fingerIndex, 0],
        //                                        Template = firstTemplateSample,
        //                                        CheckSum = firstSampleCheckSum,
        //                                        UserId = user.Id,
        //                                        FingerTemplateType = _fingerTemplateTypes.V400
        //                                    });

        //                                    if (secondTemplateSample != null)
        //                                    {
        //                                        user.FingerTemplates.Add(new FingerTemplate
        //                                        {
        //                                            FingerIndex = _biometricTemplateManager.GetFingerIndex(TerminalUserData.FingerID[i]),
        //                                            Index = _fingerTemplateService.FingerTemplates(userId: (int)(existUser.Id))?.Count(ft => ft.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(TerminalUserData.FingerID[i]).Code) ?? 0 + 1,
        //                                            TemplateIndex = 1,
        //                                            Size = TerminalUserData.FPSampleDataLength[fingerIndex, 1],
        //                                            Template = secondTemplateSample,
        //                                            CheckSum = secondSampleCheckSum,
        //                                            UserId = user.Id,
        //                                            FingerTemplateType = _fingerTemplateTypes.V400
        //                                        });
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                var firstSampleCheckSum = 0;
        //                                var secondSampleCheckSum = 0;

        //                                var firstTemplateSample = TerminalUserData.FPSampleData[fingerIndex, 0] as byte[];
        //                                byte[] secondTemplateSample = null;
        //                                try
        //                                {
        //                                    secondTemplateSample = TerminalUserData.FPSampleData[fingerIndex, 1] as byte[];
        //                                }
        //                                catch (Exception exception)
        //                                {
        //                                    Logger.Log(exception);
        //                                }

        //                                if (firstTemplateSample != null) firstSampleCheckSum = firstTemplateSample.Sum(x => x);
        //                                if (secondTemplateSample != null) secondSampleCheckSum = secondTemplateSample.Sum(x => x);

        //                                user.FingerTemplates.Add(new FingerTemplate
        //                                {
        //                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(TerminalUserData.FingerID[i]),
        //                                    Index = fingerIndex,
        //                                    TemplateIndex = 0,
        //                                    Size = TerminalUserData.FPSampleDataLength[fingerIndex, 0],
        //                                    Template = firstTemplateSample,
        //                                    CheckSum = firstSampleCheckSum,
        //                                    UserId = user.Id,
        //                                    FingerTemplateType = _fingerTemplateTypes.V400
        //                                });

        //                                if (secondTemplateSample != null)
        //                                {
        //                                    user.FingerTemplates.Add(new FingerTemplate
        //                                    {
        //                                        FingerIndex = _biometricTemplateManager.GetFingerIndex(TerminalUserData.FingerID[i]),
        //                                        Index = fingerIndex,
        //                                        TemplateIndex = 1,
        //                                        Size = TerminalUserData.FPSampleDataLength[fingerIndex, 1],
        //                                        Template = secondTemplateSample,
        //                                        CheckSum = secondSampleCheckSum,
        //                                        UserId = user.Id,
        //                                        FingerTemplateType = _fingerTemplateTypes.V400
        //                                    });
        //                                }
        //                            }
        //                        }

        //                        if (user.FingerTemplates.Any())
        //                            foreach (var fingerTemplate in user.FingerTemplates)
        //                            {
        //                                _fingerTemplateService.ModifyFingerTemplate(fingerTemplate);
        //                            }
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Logger.Log(e);
        //                    }

        //                    //Face
        //                    try
        //                    {
        //                        var faceCount = TerminalUserData.FaceNumber;
        //                        Logger.Log($"   +TotalFaceCount:{faceCount}");

        //                        if (faceCount > 0)
        //                        {
        //                            if (user.FaceTemplates is null)
        //                                user.FaceTemplates = new List<FaceTemplate>();

        //                            var userFaces = _faceTemplateService.FaceTemplates(userId: TerminalUserData.UserID);
        //                            //existUser.FaceTemplates = new List<FaceTemplate>();

        //                            if (existUser != null)
        //                                existUser.FaceTemplates = (userFaces.Any() ? userFaces : new List<FaceTemplate>());

        //                            var faceData = (byte[])TerminalUserData.FaceData;
        //                            var faceTemplate = new FaceTemplate
        //                            {
        //                                Index = faceCount,
        //                                FaceTemplateType = _faceTemplateTypes.VFACE,
        //                                UserId = user.Id,
        //                                Template = faceData,
        //                                CheckSum = faceData.Sum(x => x),
        //                                Size = faceData.Length
        //                            };
        //                            if (existUser != null)
        //                            {
        //                                if (!existUser.FaceTemplates.Exists(fp => fp.FaceTemplateType.Code == FaceTemplateTypes.VFACECode))
        //                                    user.FaceTemplates.Add(faceTemplate);
        //                            }
        //                            else
        //                                user.FaceTemplates.Add(faceTemplate);

        //                            if (user.FaceTemplates.Any())
        //                                foreach (var faceTemplates in user.FaceTemplates)
        //                                {
        //                                    _faceTemplateService.ModifyFaceTemplate(faceTemplates);
        //                                }
        //                        }
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Logger.Log(e);
        //                    }
        //                    //}

        //                    //if (user.FingerTemplates != null && user.FingerTemplates.Count > 0)
        //                    //{
        //                    //    Task.Run(() =>
        //                    //    {
        //                    //        try
        //                    //        {
        //                    //            lock (LoadFingerTemplateLock)
        //                    //            {
        //                    //                var accessGroupsOfUser = _accessGroupService.GetAccessGroups(userId: user.Id);
        //                    //                if (accessGroupsOfUser is null || accessGroupsOfUser.Count == 0)
        //                    //                {
        //                    //                    var devices =
        //                    //                        _commonDeviceService.GetDevices(brandId: DeviceBrands.VirdiCode);

        //                    //                    foreach (var device in devices)
        //                    //                    {
        //                    //                        AddUserToDeviceFastSearch(device.Code, (int)user.Code).ConfigureAwait(false);
        //                    //                    }
        //                    //                }

        //                    //                else
        //                    //                {
        //                    //                    foreach (var accessGroup in accessGroupsOfUser)
        //                    //                    {
        //                    //                        foreach (var deviceGroup in accessGroup.DeviceGroup)
        //                    //                        {
        //                    //                            foreach (var device in deviceGroup.Devices)
        //                    //                            {
        //                    //                                AddUserToDeviceFastSearch(device.Code, (int)user.Code).ConfigureAwait(false);
        //                    //                            }
        //                    //                        }
        //                    //                    }
        //                    //                }
        //                    //            }
        //                    //        }
        //                    //        catch (Exception exception)
        //                    //        {
        //                    //            Logger.Log(exception);
        //                    //        }
        //                    //    });
        //                    //}

        //                    Logger.Log($@"<--EventGetUserData
        //+ClientID:{clientId}
        //+TerminalID:{terminalId}
        //+ErrorCode:{UcsApi.ErrorCode}
        //+UserID:{TerminalUserData.UserID}
        //+Admin:{TerminalUserData.IsAdmin}
        //+Admin:{TerminalUserData.FaceNumber}
        //+AccessGroup:{TerminalUserData.AccessGroup}
        //+AccessDateType:{TerminalUserData.AccessDateType}
        //+AccessDate:{TerminalUserData.StartAccessDate}~{TerminalUserData.EndAccessDate}
        //+AuthType:{TerminalUserData.AuthType}
        //+Password:{TerminalUserData.Password}
        //+Progress:{TerminalUserData.CurrentIndex}/{TerminalUserData.TotalNumber}", logType: LogType.Information);
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Log($"--> Error On GetUserDataCallback Error: {e.Message} ");
        //        }

        //        //_ucsApi.EventGetUserData -= GetUserDataCallback;
        //    }

        private async void GetAccessLogCount(int clientId, int terminalId, int logCount)
        {
            if (logCount > 0)
            {
                Logger.Log($"Retrieving {logCount} logs from device {terminalId}, with type {GetAccessLogType}");
                _logCount = logCount;
                if (GetAccessLogType == (int)VirdiDeviceLogType.Period)
                {
                    _accessLogData.SetPeriod(AccessLogPeriodFromDateTime.Year, AccessLogPeriodFromDateTime.Month, AccessLogPeriodFromDateTime.Day, AccessLogPeriodToDateTime.Year, AccessLogPeriodToDateTime.Month, AccessLogPeriodToDateTime.Day);
                    await Task.Run(() =>
                    {
                        //try
                        //{
                        //    GetLogSemaphore.WaitOne(120000);
                        //}
                        //catch (Exception exception)
                        //{
                        //    Logger.Log(exception);
                        //}

                        _accessLogData.GetAccessLogFromTerminal(clientId, terminalId, (int)VirdiDeviceLogType.Period);
                    });

                    //try
                    //{
                    //    SetLogPeriodSemaphore.Release();
                    //}
                    //catch (Exception exception)
                    //{
                    //    Logger.Log(exception);
                    //}
                    AccessLogPeriodFromDateTime = new DateTime();
                    AccessLogPeriodToDateTime = new DateTime();
                }
                else if (GetAccessLogType == (int)VirdiDeviceLogType.New)
                {
                    await Task.Run(() =>
                    {
                        //try
                        //{
                        //    GetLogSemaphore.WaitOne(120000);
                        //}
                        //catch (Exception exception)
                        //{
                        //    Logger.Log(exception);
                        //}

                        _accessLogData.GetAccessLogFromTerminal(clientId, terminalId, (int)VirdiDeviceLogType.New);
                    });
                }
                else
                {
                    await Task.Run(() =>
                    {
                        //try
                        //{
                        //    GetLogSemaphore.WaitOne(120000);
                        //}
                        //catch (Exception exception)
                        //{
                        //    Logger.Log(exception);
                        //}

                        _accessLogData.GetAccessLogFromTerminal(clientId, terminalId, (int)VirdiDeviceLogType.All);
                    });
                }
            }

            else if (AccessLogPeriodFromDateTime != default && AccessLogPeriodToDateTime != default)
            {
                //try
                //{
                //    SetLogPeriodSemaphore.Release();
                //}
                //catch (Exception exception)
                //{
                //    Logger.Log(exception);
                //}

                AccessLogPeriodFromDateTime = new DateTime();
                AccessLogPeriodToDateTime = new DateTime();
            }
        }

        #endregion

        public async Task LoadFingerTemplates()
        {
            await Task.Run(() =>
            {
                lock (LoadFingerTemplateLock)
                {
                    Logger.Log("Fast search initialization started.");
                    var devices = _commonDeviceService.GetDevices(brandId: DeviceBrands.VirdiCode);
                    var accessGroups = _accessGroupService.GetAccessGroups();
                    var devicesWithAccessGroup = accessGroups.SelectMany(accessGroup =>
                        accessGroup.DeviceGroup.SelectMany(deviceGroup => deviceGroup.Devices)).ToList();
                    var deviceWithoutAccessGroup =
                        devices.ExceptBy(devicesWithAccessGroup, device => device.DeviceId).ToList();

                    var tasks = new List<Task>
                    {
                        Task.Run(() => Parallel.ForEach(accessGroups, accessGroup =>
                        {
                            try
                            {
                                var cachedList =
                                    _accessGroupService.GetServerSideIdentificationCacheOfAccessGroup(accessGroup.Id,
                                        DeviceBrands.VirdiCode);

                                var identificationObjectsOfDevices = cachedList
                                    .GroupBy(cachedObject => cachedObject.DeviceCode)
                                    .Select(group => new
                                    {
                                        DeviceCode = group.Key,
                                        IdentificationModels = group.Select(identificationModel =>
                                            new ServerSideIdentificationCacheModel
                                            {
                                                Id = identificationModel.Id,
                                                DeviceId = identificationModel.DeviceId,
                                                UserId = identificationModel.UserId,
                                                UserType = identificationModel.UserType,
                                                AccessGroupId = identificationModel.AccessGroupId,
                                                FingerTemplate = identificationModel.FingerTemplate
                                            })
                                    });

                                foreach (var cachedIdsForDevice in identificationObjectsOfDevices)
                                {
                                    var ucBioBsp = new UCBioBSPClass();

                                    if (!(ucBioBsp.FPData is IFPData fpData) ||
                                        !(ucBioBsp.FastSearch is IFastSearch fastSearch))
                                        continue;

                                    var identificationObjectsOfUsers = cachedIdsForDevice.IdentificationModels
                                        .GroupBy(cachedObject => cachedObject.UserId).Select(group => new
                                        {
                                            UserId = group.Key,
                                            IdentificationModels = group.Select(identificationModel =>
                                                new ServerSideIdentificationCacheModel
                                                {
                                                    Id = identificationModel.Id,
                                                    DeviceId = identificationModel.DeviceId,
                                                    UserId = identificationModel.UserId,
                                                    UserType = identificationModel.UserType,
                                                    AccessGroupId = identificationModel.AccessGroupId,
                                                    FingerTemplate = identificationModel.FingerTemplate
                                                }).ToList()
                                        });

                                    foreach (var identificationObjectsOfUser in identificationObjectsOfUsers)
                                    {
                                        if (identificationObjectsOfUser.UserId > int.MaxValue)
                                            continue;

                                        var templatesOfUser = identificationObjectsOfUser.IdentificationModels.ToList();
                                        fpData.ClearFPData();
                                        foreach (var templateOfUser in templatesOfUser)
                                        {
                                            var minIndexTemplate = templatesOfUser.Where(x =>
                                                    x.UserId == templateOfUser.UserId &&
                                                    x.FingerTemplate.FingerIndex.Code ==
                                                    templateOfUser.FingerTemplate.FingerIndex.Code &&
                                                    x.FingerTemplate.Index == templateOfUser.FingerTemplate.Index)
                                                .MinBy(x => x.FingerTemplate.TemplateIndex).FirstOrDefault();
                                            if (templateOfUser.FingerTemplate.Id != minIndexTemplate?.FingerTemplate.Id)
                                                continue;

                                            var secondTemplateSample = templatesOfUser.FirstOrDefault(x =>
                                                    x.FingerTemplate.FingerIndex.Code ==
                                                    templateOfUser.FingerTemplate.FingerIndex.Code &&
                                                    x.FingerTemplate.Index == templateOfUser.FingerTemplate.Index &&
                                                    x.FingerTemplate.TemplateIndex ==
                                                    templateOfUser.FingerTemplate.TemplateIndex + 1)?.FingerTemplate
                                                .Template;
                                            fpData.Import(0, templateOfUser.FingerTemplate.FingerIndex.OrderIndex, 2,
                                                400, 400,
                                                templateOfUser.FingerTemplate.Template,
                                                secondTemplateSample);
                                        }

                                        var firTemplate = fpData.TextFIR;
                                        fastSearch.AddFIR(firTemplate,
                                            Convert.ToInt32(identificationObjectsOfUser.UserId));
                                        fpData.ClearFPData();
                                    }

                                    lock (_bioBspClasses)
                                    {
                                        if (_bioBspClasses.ContainsKey(cachedIdsForDevice.DeviceCode))
                                        {
                                            _bioBspClasses.Remove(cachedIdsForDevice.DeviceCode);
                                            _fastSearchOfDevices.Remove(cachedIdsForDevice.DeviceCode);
                                            _fingerPrintDataOfDevices.Remove(cachedIdsForDevice.DeviceCode);
                                        }

                                        _bioBspClasses.Add(cachedIdsForDevice.DeviceCode, ucBioBsp);
                                        _fastSearchOfDevices.Add(cachedIdsForDevice.DeviceCode, fastSearch);
                                        _fingerPrintDataOfDevices.Add(cachedIdsForDevice.DeviceCode, fpData);
                                    }
                                }

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        })),

                        Task.Run(() =>
                        {
                            //var fingerTemplatesCount =
                            //    _fingerTemplateService.GetAllFingerTemplatesCountByFingerTemplateType(FingerTemplateTypes
                            //        .V400);

                            var userCount = _commonUserService.GetUsersCount();

                            const int groupSize = 250;
                            var loopUpperBound = userCount / groupSize;
                            loopUpperBound = loopUpperBound == 0 ? 1 : loopUpperBound;
                            loopUpperBound = userCount % groupSize <= 0 ? loopUpperBound : loopUpperBound + 1;

                            var fingerTemplates = new List<FingerTemplate>();

                            Parallel.For(0, loopUpperBound, index =>
                            {
                                var tempTemplates =
                                    _fingerTemplateService.FingerTemplates(
                                        fingerTemplateType: FingerTemplateTypes.V400Code, pageNumber: index,
                                        pageSize: groupSize);

                                lock (fingerTemplates)
                                    fingerTemplates.AddRange(tempTemplates);
                            });

                            var fingerTemplatesOfUsers = fingerTemplates.GroupBy(template => template.UserId).Select(
                                group => new
                                {
                                    UserId = Convert.ToInt32(group.Key),
                                    FingerTemplates = group.Select(fingerTemplate => new FingerTemplate
                                    {
                                        Id = fingerTemplate.Id,
                                        UserId = fingerTemplate.UserId,
                                        FingerTemplateType = fingerTemplate.FingerTemplateType,
                                        Size = fingerTemplate.Size,
                                        Template = fingerTemplate.Template,
                                        Index = fingerTemplate.Index,
                                        CheckSum = fingerTemplate.CheckSum,
                                        EnrollQuality = fingerTemplate.EnrollQuality,
                                        Duress = fingerTemplate.Duress,
                                        FingerIndex = fingerTemplate.FingerIndex,
                                        SecurityLevel = fingerTemplate.SecurityLevel,
                                        TemplateIndex = fingerTemplate.TemplateIndex
                                    })
                                });

                            Task.Run(() =>
                            {
                                var ucBioBsp = new UCBioBSPClass();

                                if (!(ucBioBsp.FPData is IFPData fpData) ||
                                    !(ucBioBsp.FastSearch is IFastSearch fastSearch))
                                    return;

                                //fastSearch.UseGroupMatch = 1;
                                //fastSearch.MaxSearchTime = 2000;
                                //fastSearch.MatchMethod = 0;

                                foreach (var fingerTemplateOfUser in fingerTemplatesOfUsers)
                                {
                                    fpData.ClearFPData();

                                    var userFingerTemplates = fingerTemplateOfUser.FingerTemplates.ToList();
                                    if (userFingerTemplates.Count < 2)
                                        continue;

                                    for (var j = 0; j < userFingerTemplates.Count - 1; j += 2)
                                    {
                                        fpData.Import(0, userFingerTemplates[j].FingerIndex.OrderIndex, 2, 400, 400,
                                            userFingerTemplates[j].Template, userFingerTemplates[j + 1].Template);
                                    }

                                    var firTemplate = fpData.TextFIR;
                                    fastSearch.AddFIR(firTemplate, fingerTemplateOfUser.UserId);
                                    fpData.ClearFPData();
                                }

                                lock (_bioBspClasses)
                                {
                                    if (_bioBspClasses.ContainsKey(0))
                                    {
                                        _bioBspClasses.Remove(0);
                                        _fastSearchOfDevices.Remove(0);
                                        _fingerPrintDataOfDevices.Remove(0);
                                    }

                                    _bioBspClasses.Add(0, ucBioBsp);
                                    _fastSearchOfDevices.Add(0, fastSearch);
                                    _fingerPrintDataOfDevices.Add(0, fpData);
                                }
                            });

                            Parallel.ForEach(deviceWithoutAccessGroup, device =>
                            {
                                var ucBioBsp = new UCBioBSPClass();

                                if (!(ucBioBsp.FPData is IFPData fpData) ||
                                    !(ucBioBsp.FastSearch is IFastSearch fastSearch))
                                    return;

                                //fastSearch.UseGroupMatch = 1;
                                //fastSearch.MaxSearchTime = 2000;
                                //fastSearch.MatchMethod = 0;

                                foreach (var fingerTemplateOfUser in fingerTemplatesOfUsers)
                                {
                                    fpData.ClearFPData();

                                    var userFingerTemplates = fingerTemplateOfUser.FingerTemplates.ToList();
                                    if (userFingerTemplates.Count < 2)
                                        continue;

                                    for (var j = 0; j < userFingerTemplates.Count - 1; j += 2)
                                    {
                                        fpData.Import(0, userFingerTemplates[j].FingerIndex.OrderIndex, 2, 400, 400,
                                            userFingerTemplates[j].Template, userFingerTemplates[j + 1].Template);
                                    }

                                    var firTemplate = fpData.TextFIR;
                                    fastSearch.AddFIR(firTemplate, fingerTemplateOfUser.UserId);
                                    fpData.ClearFPData();
                                }

                                lock (_bioBspClasses)
                                {
                                    if (_bioBspClasses.ContainsKey((int) device.Code))
                                    {
                                        _bioBspClasses.Remove((int) device.Code);
                                        _fastSearchOfDevices.Remove((int) device.Code);
                                        _fingerPrintDataOfDevices.Remove((int) device.Code);
                                    }

                                    _bioBspClasses.Add((int) device.Code, ucBioBsp);
                                    _fastSearchOfDevices.Add((int) device.Code, fastSearch);
                                    _fingerPrintDataOfDevices.Add((int) device.Code, fpData);
                                }
                            });
                        })
                    };

                    Task.WaitAll(tasks.ToArray());
                    Logger.Log("Fast search initialization completed.");
                }
            });

            //Task.Run(() =>
            //{
            //    var fingerTemplatesCount = _fingerTemplateService.GetAllFingerTemplatesCountByFingerTemplateType(FingerTemplateTypes.V400);

            //    const int groupSize = 500;
            //    var loopUpperBound = fingerTemplatesCount / groupSize;
            //    loopUpperBound = loopUpperBound == 0 ? 1 : loopUpperBound;
            //    loopUpperBound = fingerTemplatesCount % groupSize <= 0 ? loopUpperBound : loopUpperBound + 1;

            //    var tasks = new List<Task>();
            //    for (var i = 0; i < loopUpperBound; i++)
            //    {
            //        var index = i;
            //        tasks.Add(Task.Run(() =>
            //        {
            //            var fingerTemplates =
            //                _fingerTemplateService.GetAllFingerTemplatesByFingerTemplateType(
            //                    FingerTemplateTypes.V400, index * groupSize, groupSize);

            //            var fingerTemplatesOfUsers = fingerTemplates.GroupBy(template => template.UserId).Select(
            //                group => new
            //                {
            //                    UserId = Convert.ToInt32(group.Key),
            //                    FingerTemplates = group.Select(fingerTemplate => new FingerTemplate
            //                    {
            //                        Id = fingerTemplate.Id,
            //                        UserId = fingerTemplate.UserId,
            //                        FingerTemplateType = fingerTemplate.FingerTemplateType,
            //                        Size = fingerTemplate.Size,
            //                        Template = fingerTemplate.Template,
            //                        Index = fingerTemplate.Index,
            //                        CheckSum = fingerTemplate.CheckSum,
            //                        EnrollQuality = fingerTemplate.EnrollQuality,
            //                        Duress = fingerTemplate.Duress,
            //                        FingerIndex = fingerTemplate.FingerIndex,
            //                        SecurityLevel = fingerTemplate.SecurityLevel,
            //                        TemplateIndex = fingerTemplate.TemplateIndex
            //                    })
            //                });

            //            var ucBioBsp = new UCBioBSPClass();
            //            var fpData = ucBioBsp.FPData as IFPData;
            //            var fastSearch = ucBioBsp.FastSearch as IFastSearch;

            //            if (fpData is null || fastSearch is null)
            //                return;

            //            //var x = fastSearch.UseGroupMatch;
            //            fastSearch.MaxSearchTime = 2000;
            //            //var y = fastSearch.MaxSearchTime;
            //            fastSearch.MatchMethod = 0;
            //            //var t = fastSearch.MatchMethod;

            //            foreach (var fingerTemplateOfUser in fingerTemplatesOfUsers)
            //            {
            //                fpData.ClearFPData();

            //                var userFingerTemplates = fingerTemplateOfUser.FingerTemplates.ToList();
            //                if (userFingerTemplates.Count < 2)
            //                    continue;

            //                for (var j = 0; j < userFingerTemplates.Count - 1; j += 2)
            //                {
            //                    fpData.Import(0, userFingerTemplates[j].FingerIndex.OrderIndex, 2, 400, 400,
            //                        userFingerTemplates[j].Template, userFingerTemplates[j + 1].Template);
            //                }

            //                var firTemplate = fpData.TextFIR;
            //                fastSearch.AddFIR(firTemplate, fingerTemplateOfUser.UserId);
            //                fpData.ClearFPData();
            //            }

            //            //for (var i = 0; i < fingerTemplates.Count - 1; i += 2)
            //            //{
            //            //    if (FPData == null) continue;
            //            //    FPData.Import(1, fingerTemplates[i].FingerIndex.OrderIndex, 2, 400, 400,
            //            //        fingerTemplates[i].Template, fingerTemplates[i + 1].Template);
            //            //    var firTemplate = FPData.TextFIR;
            //            //    _fastSearch?.AddFIR(firTemplate, (int)fingerTemplates[i].UserId);
            //            //    FPData.ClearFPData();
            //            //}
            //            lock (_bioBspClassesOfUsers)
            //            {
            //                if (_bioBspClassesOfUsers.ContainsKey(index))
            //                {
            //                    _bioBspClassesOfUsers.Remove(index);
            //                    _fastSearchOfDevices.Remove(index);
            //                    _fingerPrintDataOfDevices.Remove(index);
            //                }



            //                _bioBspClassesOfUsers.Add(index, ucBioBsp);
            //                _fastSearchOfUsers.Add(index, fastSearch);
            //                _fingerPrintDataOfUsers.Add(index, fpData);

            //                //var x = fastSearch?.FpCount;
            //            }
            //        }));
            //    }
            //    Task.WaitAll(tasks.ToArray());
            //});
            //});
        }

        private void ucBioBSP_OnEnrollEvent(int eventId)
        {
            Logger.Log($"<--Bio Enroll : EventID({eventId})");
            Logger.Log($"   +EventID:{eventId}");
        }

        private void ucBioBSP_OnCaptureEvent(int quality)
        {
            Logger.Log($"<--Bio Capture : Quality({quality})");
            Logger.Log($"   +Quality:{quality}");
        }

        private void TerminalConnectedCallback(int terminalId, string terminalIp)
        {
            var existDevice = _commonDeviceService.GetDevices(code: (uint)terminalId, brandId: DeviceBrands.VirdiCode).FirstOrDefault();

            Task.Run(async () =>
                {

                    var connectionStatus = new ConnectionStatus
                    {
                        DeviceId = existDevice?.DeviceId ?? 0,
                        IsConnected = true
                    };

                    try
                    {
                        var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                        restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                        await _monitoringRestClient.ExecuteAsync<ResultViewModel>(restRequest);
                        //integration
                        var connectionStatusList = new List<ConnectionStatus> { connectionStatus };
                        var biovationBrokerMessageData = new List<DataChangeMessage<ConnectionStatus>>
                        {
                            new DataChangeMessage<ConnectionStatus>
                            {
                                Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                                TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = connectionStatusList
                            }
                        };

                        _deviceConnectionStateInternalSource.PushData(biovationBrokerMessageData);

                        await _logService.AddLog(new Log
                        {
                            DeviceId = existDevice?.DeviceId ?? 0,
                            LogDateTime = DateTime.Now,
                            EventLog = _logEvents.Connect
                        });
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                });

            DeviceBasicInfo device;
            if (existDevice != null)
            {
                device = new DeviceBasicInfo
                {
                    Code = (uint)terminalId,
                    DeviceId = existDevice.DeviceId,
                    Name = existDevice.Name,
                    Brand = existDevice.Brand,
                    Model = existDevice.Model,
                    IpAddress = terminalIp,
                    Port = BiovationConfiguration.VirdiDevicesConnectionPort,
                    MacAddress = existDevice.MacAddress,
                    RegisterDate = existDevice.RegisterDate,
                    TimeSync = existDevice.TimeSync,
                    Active = existDevice.Active,
                    DeviceTypeId = existDevice.DeviceTypeId
                };

                if (existDevice.Code != (uint)terminalId || !string.Equals(existDevice.IpAddress, terminalIp, StringComparison.InvariantCultureIgnoreCase) || existDevice.Port != BiovationConfiguration.VirdiDevicesConnectionPort)
                    _commonDeviceService.ModifyDevice(device);
            }
            else
            {
                device = new DeviceBasicInfo
                {
                    Code = (uint)terminalId,
                    Brand = _deviceBrands.Virdi,
                    Model = new DeviceModel { Id = 1001 },
                    IpAddress = terminalIp,
                    Port = BiovationConfiguration.VirdiDevicesConnectionPort,
                    MacAddress = "",
                    RegisterDate = DateTime.Now,
                    TimeSync = true,
                    Active = true
                };

                device.Name = terminalId + "[" + device.IpAddress + "]";
                var result = _commonDeviceService.ModifyDevice(device);
                if (result.Validate == 1)
                    device.DeviceId = (int)result.Id;
            }

            //Task.Run(() =>
            //{
            //    var data = JsonConvert.SerializeObject(device);
            //    try
            //    {
            //        _communicationManager.CallRest("/api/Biovation/DeviceOperationDetail/SaveOnAccessRole", "Post", null,
            //            data);
            //    }
            //    catch (Exception)
            //    {
            //        //ignore
            //    }
            //});

            if (!_onlineDevices.ContainsKey((uint)terminalId))
                _onlineDevices.Add((uint)terminalId, device);

            Logger.Log($"Connected device: {{ ID: {terminalId} , IP: {terminalIp} }}", logType: LogType.Information);
            Logger.Log($"Retrieving new log: {{ ID: {terminalId} , IP: {terminalIp} }}", logType: LogType.Information);

            Task.Run(() =>
            {
                lock (LoadFingerTemplateLock)
                {
                    if (_bioBspClasses.ContainsKey((int)device.Code))
                        return;

                    var fingerTemplatesCount =
                        _fingerTemplateService.GetFingerTemplatesCountByFingerTemplateType(_fingerTemplateTypes.V400);

                    const int groupSize = 800;
                    var loopUpperBound = fingerTemplatesCount / groupSize;
                    loopUpperBound = loopUpperBound == 0 ? 1 : loopUpperBound;
                    loopUpperBound = fingerTemplatesCount % groupSize <= 0 ? loopUpperBound : loopUpperBound + 1;

                    var fingerTemplates = new List<FingerTemplate>();

                    Parallel.For(0, loopUpperBound, index =>
                    {
                        var tempTemplates =
                            _fingerTemplateService.FingerTemplates(fingerTemplateType: FingerTemplateTypes.V400Code, pageNumber: index, pageSize: groupSize);

                        lock (fingerTemplates)
                            fingerTemplates.AddRange(tempTemplates);
                    });

                    var fingerTemplatesOfUsers = fingerTemplates.GroupBy(template => template.UserId).Select(
                        group => new
                        {
                            UserId = Convert.ToInt32(group.Key),
                            FingerTemplates = group.Select(fingerTemplate => new FingerTemplate
                            {
                                Id = fingerTemplate.Id,
                                UserId = fingerTemplate.UserId,
                                FingerTemplateType = fingerTemplate.FingerTemplateType,
                                Size = fingerTemplate.Size,
                                Template = fingerTemplate.Template,
                                Index = fingerTemplate.Index,
                                CheckSum = fingerTemplate.CheckSum,
                                EnrollQuality = fingerTemplate.EnrollQuality,
                                Duress = fingerTemplate.Duress,
                                FingerIndex = fingerTemplate.FingerIndex,
                                SecurityLevel = fingerTemplate.SecurityLevel,
                                TemplateIndex = fingerTemplate.TemplateIndex
                            })
                        });

                    var ucBioBsp = new UCBioBSPClass();

                    if (!(ucBioBsp.FPData is IFPData fpData) || !(ucBioBsp.FastSearch is IFastSearch fastSearch))
                        return;

                    //fastSearch.UseGroupMatch = 1;
                    //fastSearch.MaxSearchTime = 2000;
                    //fastSearch.MatchMethod = 0;

                    foreach (var fingerTemplateOfUser in fingerTemplatesOfUsers)
                    {
                        fpData.ClearFPData();

                        var userFingerTemplates = fingerTemplateOfUser.FingerTemplates.ToList();
                        if (userFingerTemplates.Count < 2)
                            continue;

                        for (var j = 0; j < userFingerTemplates.Count - 1; j += 2)
                        {
                            fpData.Import(0, userFingerTemplates[j].FingerIndex.OrderIndex, 2, 400, 400,
                                userFingerTemplates[j].Template, userFingerTemplates[j + 1].Template);
                        }

                        var firTemplate = fpData.TextFIR;
                        fastSearch.AddFIR(firTemplate, fingerTemplateOfUser.UserId);
                        fpData.ClearFPData();
                    }

                    lock (_bioBspClasses)
                    {
                        if (_bioBspClasses.ContainsKey((int)device.Code))
                        {
                            _bioBspClasses.Remove((int)device.Code);
                            _fastSearchOfDevices.Remove((int)device.Code);
                            _fingerPrintDataOfDevices.Remove((int)device.Code);
                        }

                        _bioBspClasses.Add((int)device.Code, ucBioBsp);
                        _fastSearchOfDevices.Add((int)device.Code, fastSearch);
                        _fingerPrintDataOfDevices.Add((int)device.Code, fpData);
                    }
                }
            });


            GetAccessLogType = (int)VirdiDeviceLogType.New;
            _accessLogData.GetAccessLogCountFromTerminal(0, terminalId, (int)VirdiDeviceLogType.New);

            _taskService.ProcessQueue(_deviceBrands.Virdi, device.DeviceId).ConfigureAwait(false);

            //AccessLogData.GetAccessLogFromTerminal(0, terminalId, (int)VirdiDeviceLogType.New);
            //});
        }

        private async void RealTimeAccessLogCallback(int terminalId)
        {
            Logger.Log($@"<--EventRealTimeAccessLog
    +TerminalID:{terminalId}
    +ErrorCode:{_ucsApi.ErrorCode}
    +UserID:{_accessLogData.UserID}
    +DataTime:{_accessLogData.DateTime}
    +AuthMode:{_accessLogData.AuthMode}
    +AuthType:{_accessLogData.AuthType}
    +IsAuthorized:{_accessLogData.IsAuthorized}
    +Device:{_accessLogData.DeviceID}
    +Result:{_accessLogData.AuthResult}
    +RFID:{_accessLogData.RFID}
    +PictureDataLength:{_accessLogData.PictureDataLength}
    +Progress:{_accessLogData.CurrentIndex}/{_accessLogData.TotalNumber}");

            byte[] picture = null;
            try
            {
                if (_accessLogData.PictureDataLength > 0)
                    picture = _accessLogData.PictureData as byte[];
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            DateTime logDateTime;

            try
            {
                logDateTime = DateTime.Parse(_accessLogData.DateTime);
            }
            catch (Exception)
            {
                try
                {
                    logDateTime = DateTime.Parse(_accessLogData.DateTime.Replace("0000-00-00", DateTime.Today.ToString("yyyy-MM-dd")));
                }
                catch (Exception)
                {
                    Logger.Log("Error in parsing the log date, the value is replaced with \"Now\".", logType: LogType.Warning);
                    logDateTime = DateTime.Now;
                }
            }

            //var deviceBasicInfo = _commonDeviceService.GetDeviceBasicInfoWithCode((uint) terminalId,DeviceBrands.VirdiCode);
            //var authMode = _commonDeviceService.GetBioAuthModeWithDeviceId(deviceBasicInfo.DeviceId, AccessLogData.AuthMode);

            var log = new Log
            {
                DeviceCode = (uint)terminalId,
                EventLog = _accessLogData.AuthResult == 0 ? _logEvents.Authorized : _logEvents.UnAuthorized,
                UserId = _accessLogData.UserID,
                LogDateTime = logDateTime,
                AuthType = _accessLogData.AuthType,
                AuthResult = _accessLogData.AuthResult,
                //MatchingType = AccessLogData.AuthType,
                //MatchingType = authMode?.BioCode,
                MatchingType = _virdiCodeMappings.GetMatchingTypeGenericLookup(_accessLogData.AuthType),
                SubEvent = _virdiCodeMappings.GetLogSubEventGenericLookup(_accessLogData.AuthMode),
                TnaEvent = 0,
                PicByte = picture
            };

            await _virdiLogService.AddLog(log);
        }

        private void TerminalStatus(int clientId, int terminalId, int terminalStatus, int doorStatus, int coverStatus)
        {
            Logger.Log($@"<--EventTerminal Status
               +ClientID:{clientId}
               +TerminalID:{terminalId}
               +Terminal Status:{terminalStatus}
               +Door Status:{doorStatus}
               +Cover Status:{coverStatus}", logType: LogType.Verbose);
        }

        public void VerifyCardCallback(int terminalId, int authMode, int antiPassBackLevel, string textRfid)
        {
            Task.Run(() =>
            {
                var txtEventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var user = _commonUserCardService.FindUserByCardNumber(textRfid);
                var isAuthorized = 0;

                if (user != null)
                {



                    if (user.IsActive)
                    {

                        isAuthorized = 1;

                        Logger.Log($@"<--EventVerifyCard
    +TerminalID:{terminalId}
    +ErrorCode:{_ucsApi.ErrorCode}
    +UserID:{user.Id}
    +DataTime:{txtEventTime}
    +AuthMode:{authMode}
    +AuthType:{_accessLogData.AuthType}
    +IsAuthorized:{isAuthorized}
    +RFID:{textRfid}
    +PictureDataLength:{_accessLogData.PictureDataLength}
    +Progress:{_accessLogData.CurrentIndex}/{_accessLogData.TotalNumber}", logType: LogType.Information);
                    }
                }

                else
                {
                    Logger.Log($@"<--EventVerifyCard
    +TerminalID:{terminalId}
    +ErrorCode:{_ucsApi.ErrorCode}
    +DataTime:{txtEventTime}
    +AuthMode:{authMode}
    +AuthType:{_accessLogData.AuthType}
    +IsAuthorized:{isAuthorized}
    +Result:-1
    +RFID:{textRfid}
    +PictureDataLength:{_accessLogData.PictureDataLength}
    +Progress:{_accessLogData.CurrentIndex}/{_accessLogData.TotalNumber}", logType: LogType.Information);
                }

                var isVisitor = user != null && user.IsAdmin ? 0 : 1;
                var hasAccess = user != null && (user.StartDate < DateTime.Now && user.EndDate > DateTime.Now || user.StartDate == user.EndDate || user.StartDate == default || user.EndDate == default) && user.IsActive ? 1 : 0;
                var blacklist = _blackListService.GetBlacklist(userId: (int)(user?.Id ?? -1), deviceId: terminalId, startDate: DateTime.Today, endDate: DateTime.Now).Result;
                if (blacklist != null && blacklist.Count > 0) hasAccess = 0;
                isAuthorized = hasAccess;
                var authErrorCode = hasAccess == 0 ? user != null ? (int)VirdiError.Permission : (int)VirdiError.InvalidUser : (int)VirdiError.None;

                _serverAuthentication.SetAuthType(0, 0, 0, 0, 1, 0);
                _serverAuthentication.SendAuthResultToTerminal(terminalId, (int)(user?.Code ?? -1), hasAccess, isVisitor,
                   isAuthorized, txtEventTime, authErrorCode);
            });
        }

        private void VerifyFinger1ToNCallback(int terminalId, int authMode, int inputIdLength, int securityLevel,
            int antiPassBackLevel, object fingerData)
        {
            Task.Run(() =>
            {
                // Always compare the fingerprints contained in the file(*.UFS) is read and the current fingerprint.

                Logger.Log("<--EventVerifyFinger1toN");

                const int isFinger = 1;
                int authErrorCode;
                int isAuthorized;
                var txtEventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                _fpData.Import(1, 1, 2, 400, 400, fingerData);

                var szCapturedFir = _fpData.TextFIR;

                UCBioBSPClass ucBioBsp;
                IFastSearch fastSearch;
                IFPData fpData;

                if (_bioBspClasses.ContainsKey(terminalId))
                {
                    ucBioBsp = _bioBspClasses[terminalId];
                    //fastSearch = _fastSearchOfAccessGroups[terminalId];
                    fastSearch = _bioBspClasses[terminalId].FastSearch as IFastSearch;
                    fpData = _bioBspClasses[terminalId].FPData as IFPData;
                }
                else if (_bioBspClasses.ContainsKey(0))
                {
                    ucBioBsp = _bioBspClasses[0];
                    fastSearch = _bioBspClasses[0].FastSearch as IFastSearch;
                    fpData = _bioBspClasses[0].FPData as IFPData;
                }
                else
                {
                    isAuthorized = 0;
                    authErrorCode = 770; //Matching failed
                    _serverAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                    _serverAuthentication.SendAuthResultToTerminal(terminalId, -1, 0, 1, isAuthorized, txtEventTime,
                        authErrorCode);
                    return;
                }
                /*else /*if (_bioBspClasses.Count == 1)
                {
                    var identifySuccess = false;
                    var lockObject = new object();

                    Parallel.For(0, _bioBspClassesOfUsers.Count, index =>
                    {
                        var innerUcBioBsp = _bioBspClassesOfUsers[index];
                        //fastSearch = _fastSearchOfAccessGroups[0];
                        var innerFastSearch = _bioBspClassesOfUsers[index].FastSearch as IFastSearch;
                        fpData = _bioBspClassesOfUsers[index].FPData as IFPData;

                        if (fpData is null || innerFastSearch is null)
                        {
                            return;
                        }

                        //string szCapturedFir_;
                        //lock (fpData)
                        //{
                        //    fpData.Import(1, 1, 2, 400, 400, fingerData);
                        //    szCapturedFir_ = fpData.TextFIR;
                        //}

                        //var fastSearch = _fastSearchOfAccessGroups.ContainsKey(terminalId) ? _fastSearchOfAccessGroups[terminalId] : _fastSearchOfAccessGroups[0];

                        int innerUserId;
                        lock (innerFastSearch)
                        {
                            innerFastSearch.IdentifyUser(szCapturedFir, securityLevel);
                        }

                        //innerFastSearch.UseGroupMatch = 0;

                        if (innerUcBioBsp.ErrorCode == 0)
                        {
                            // Authentication successful
                            lock (lockObject)
                            {
                                isAuthorized = 1;
                                authErrorCode = 0;
                                dynamic matchedFpInfo = innerFastSearch.MatchedFpInfo;
                                innerUserId = matchedFpInfo.UserId;

                                if (!identifySuccess)
                                {

                                    identifySuccess = true;

                                    ServerAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                                    ServerAuthentication.SendAuthResultToTerminal(terminalId, innerUserId, 1, 0,
                                        isAuthorized,
                                        txtEventTime, authErrorCode);

                                    Logger.Log($@"   +ErrorCode:{UcsApi.ErrorCode}
    +TerminalID:{terminalId}
    +UserID:{userId}
    +DateTime:{txtEventTime}
    +AuthMode:{authMode}
    +InputID Length:{inputIDLength}
    +Security Level:{securityLevel}
    +AntiPassBack Level:{antiPassBackLevel}
    +FingerData:{fingerData}", logType: LogType.Information);
                                }
                            }
                        }
                    });

                    //                for (var i = 0; i < _bioBspClassesOfUsers.Count; i++)
                    //                {
                    //                    var index = i;

                    //                    tasks.Add(Task.Run(() =>
                    //                    {
                    //                        var innerUcBioBsp = _bioBspClassesOfUsers[index];
                    //                        //fastSearch = _fastSearchOfAccessGroups[0];
                    //                        var innerFastSearch = _bioBspClassesOfUsers[index].FastSearch as IFastSearch;
                    //                        fpData = _bioBspClassesOfUsers[index].FPData as IFPData;

                    //                        if (fpData is null || innerFastSearch is null)
                    //                        {
                    //                            return;
                    //                        }

                    //                        //string szCapturedFir_;
                    //                        //lock (fpData)
                    //                        //{
                    //                        //    fpData.Import(1, 1, 2, 400, 400, fingerData);
                    //                        //    szCapturedFir_ = fpData.TextFIR;
                    //                        //}

                    //                        //var fastSearch = _fastSearchOfAccessGroups.ContainsKey(terminalId) ? _fastSearchOfAccessGroups[terminalId] : _fastSearchOfAccessGroups[0];

                    //                        int innerUserId;
                    //                        lock (innerFastSearch)
                    //                        {
                    //                            innerFastSearch.IdentifyUser(szCapturedFir, securityLevel);
                    //                        }

                    //                        //innerFastSearch.UseGroupMatch = 0;

                    //                        if (innerUcBioBsp.ErrorCode == 0)
                    //                        {
                    //                            // Authentication successful
                    //                            lock (lockObject)
                    //                            {
                    //                                isAuthorized = 1;
                    //                                authErrorCode = 0;
                    //                                dynamic matchedFpInfo = innerFastSearch.MatchedFpInfo;
                    //                                innerUserId = matchedFpInfo.UserId;

                    //                                if (!identifySuccess)
                    //                                {

                    //                                    identifySuccess = true;

                    //                                    ServerAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                    //                                    ServerAuthentication.SendAuthResultToTerminal(terminalId, innerUserId, 1, 0,
                    //                                        isAuthorized,
                    //                                        txtEventTime, authErrorCode);

                    //                                    Logger.Log($@"   +ErrorCode:{UcsApi.ErrorCode}
                    //+TerminalID:{terminalId}
                    //+UserID:{userId}
                    //+DateTime:{txtEventTime}
                    //+AuthMode:{authMode}
                    //+InputID Length:{inputIDLength}
                    //+Security Level:{securityLevel}
                    //+AntiPassBack Level:{antiPassBackLevel}
                    //+FingerData:{fingerData}", logType: LogType.Information);
                    //                                }
                    //                            }
                    //                        }
                    //                        //else
                    //                        //{
                    //                        //Authentication failed
                    //                        //isAuthorized = 0;
                    //                        //authErrorCode = 770; //Matching failed

                    //                        //this.serverAuthentication.SetAuthType(IsAndOperation, IsFinger, IsFPCard, IsPassword, IsCard, IsCardID);
                    //                        //ServerAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                    //                        //ServerAuthentication.SendAuthResultToTerminal(terminalId, 0, 1, 0, isAuthorized, txtEventTime,
                    //                        //    authErrorCode);

                    //                        //                            Logger.Log($@"   +ErrorCode:{UcsApi.ErrorCode}
                    //                        //+TerminalID:{terminalId}
                    //                        //+AuthMode:{authMode}
                    //                        //+InputID Length:{inputIDLength}
                    //                        //+Security Level:{securityLevel}
                    //                        //+AntiPassBack Level:{antiPassBackLevel}
                    //                        //+FingerData:{fingerData}", logType: LogType.Information);


                    //                        //}
                    //                    }));
                    //                }

                    //                Task.WaitAll(tasks.ToArray());


                    //                if (isAuthorized == 1)
                    //                {
                    //                    ServerAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                    //                    ServerAuthentication.SendAuthResultToTerminal(terminalId, userId, 1, 0, isAuthorized,
                    //                        txtEventTime, authErrorCode);

                    //                    Logger.Log($@"   +ErrorCode:{UcsApi.ErrorCode}
                    //+TerminalID:{terminalId}
                    //+UserID:{userId}
                    //+DateTime:{txtEventTime}
                    //+AuthMode:{authMode}
                    //+InputID Length:{inputIDLength}
                    //+Security Level:{securityLevel}
                    //+AntiPassBack Level:{antiPassBackLevel}
                    //+FingerData:{fingerData}", logType: LogType.Information);

                    //                    return;
                    //                }

                    if (identifySuccess)
                        return;

                    isAuthorized = 0;
                    authErrorCode = 770; //Matching failed
                    ServerAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                    ServerAuthentication.SendAuthResultToTerminal(terminalId, 0, 1, 0, isAuthorized, txtEventTime,
                        authErrorCode);
                    return;
                }*/
                //            else
                //            {
                //                isAuthorized = 0;
                //                authErrorCode = 770; //Matching failed

                //                Logger.Log($@"   +ErrorCode:{UcsApi.ErrorCode}
                //+TerminalID:{terminalId}
                //+AuthMode:{authMode}
                //+InputID Length:{inputIDLength}
                //+Security Level:{securityLevel}
                //+AntiPassBack Level:{antiPassBackLevel}
                //+FingerData:{fingerData}", logType: LogType.Information);


                //                //this.serverAuthentication.SetAuthType(IsAndOperation, IsFinger, IsFPCard, IsPassword, IsCard, IsCardID);
                //                ServerAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                //                ServerAuthentication.SendAuthResultToTerminal(terminalId, 0, 1, 0, isAuthorized, txtEventTime,
                //                    authErrorCode);

                //                return;
                //            }


                if (fpData is null || fastSearch is null /*|| authErrorCode == 770*/)
                {
                    isAuthorized = 0;
                    authErrorCode = 770; //Matching failed
                    _serverAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                    _serverAuthentication.SendAuthResultToTerminal(terminalId, -1, 0, 1, isAuthorized, txtEventTime,
                        authErrorCode);
                    return;
                }


                //var fastSearch = _fastSearchOfAccessGroups.ContainsKey(terminalId) ? _fastSearchOfAccessGroups[terminalId] : _fastSearchOfAccessGroups[0];


                fastSearch.IdentifyUser(szCapturedFir, securityLevel);

                if (ucBioBsp.ErrorCode == 0)
                {
                    // Authentication successful
                    dynamic matchedFpInfo = fastSearch.MatchedFpInfo;
                    int userId = matchedFpInfo.UserId;

                    var user = _commonUserService.GetUsers(code: userId)?.FirstOrDefault();

                    if (user is null)
                    {
                        //Authentication failed
                        isAuthorized = 0;
                        authErrorCode = 770; //Matching failed

                        //this.serverAuthentication.SetAuthType(IsAndOperation, IsFinger, IsFPCard, IsPassword, IsCard, IsCardID);
                        _serverAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                        _serverAuthentication.SendAuthResultToTerminal(terminalId, -1, 0, 1, isAuthorized, txtEventTime,
                            authErrorCode);

                        Logger.Log($@"   +ErrorCode:{_ucsApi.ErrorCode}
    +TerminalID:{terminalId}
    +AuthMode:{authMode}
    +InputID Length:{inputIdLength}
    +Security Level:{securityLevel}
    +AntiPassBack Level:{antiPassBackLevel}
    +FingerData:{fingerData}", logType: LogType.Information);

                        return;
                    }

                    var isVisitor = user.IsAdmin ? 0 : 1;
                    var blacklist = _blackListService.GetBlacklist(userId: (int)user.Id, deviceId: terminalId, startDate: DateTime.Today, endDate: DateTime.Now).Result;
                    var hasAccess = (user.StartDate < DateTime.Now && user.EndDate > DateTime.Now || user.StartDate == user.EndDate || user.StartDate == default || user.EndDate == default) && user.IsActive ? blacklist != null && blacklist.Count > 0 ? 1 : 0 : 0;

                    isAuthorized = hasAccess;
                    authErrorCode = hasAccess == 0 ? (int)VirdiError.Permission : (int)VirdiError.None;

                    _serverAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                    _serverAuthentication.SendAuthResultToTerminal(terminalId, userId, hasAccess, isVisitor, isAuthorized,
                        txtEventTime, authErrorCode);

                    Logger.Log($@"   +ErrorCode:{_ucsApi.ErrorCode}
    +TerminalID:{terminalId}
    +UserID:{matchedFpInfo.UserId}
    +DateTime:{txtEventTime}
    +AuthMode:{authMode}
    +InputID Length:{inputIdLength}
    +Security Level:{securityLevel}
    +AntiPassBack Level:{antiPassBackLevel}
    +FingerData:{fingerData}", logType: LogType.Information);


                }
                else
                {
                    //Authentication failed
                    isAuthorized = 0;
                    authErrorCode = 770; //Matching failed

                    //this.serverAuthentication.SetAuthType(IsAndOperation, IsFinger, IsFPCard, IsPassword, IsCard, IsCardID);
                    _serverAuthentication.SetAuthType(0, isFinger, 0, 0, 0, 0);
                    _serverAuthentication.SendAuthResultToTerminal(terminalId, -1, 0, 1, isAuthorized, txtEventTime,
                        authErrorCode);

                    Logger.Log($@"   +ErrorCode:{_ucsApi.ErrorCode}
    +TerminalID:{terminalId}
    +AuthMode:{authMode}
    +InputID Length:{inputIdLength}
    +Security Level:{securityLevel}
    +AntiPassBack Level:{antiPassBackLevel}
    +FingerData:{fingerData}", logType: LogType.Information);
                }

                //var log = new Log
                //{
                //    DeviceCode = (uint)terminalId,
                //    EventId = AccessLogData.IsAuthorized == 1 ? Event.ATHORIZED : Event.UNATHORIZED,
                //    UserId = userId,
                //    LogDateTime = DateTime.Now,
                //    MatchingType = 1,
                //    SubEvent = VirdiCodeMappings.GetLogSubEventGenericLookup(AccessLogData.AuthMode),
                //    TnaEvent = 0
                //};

                //_virdiLogService.AddLog(log);
            });
        }

        private void VerifyFinger1To1Callback(int terminalId, int userId, int authMode, int securityLevel, int antiPassBackLevel, object fingerData)
        {
            Task.Run(() =>
            {
                // Always compared with the finger immediately before the terminal input.
                Logger.Log("<--EventVerifyFinger1to1");

                _ucBioBsp.SecurityLevelForVerify = securityLevel;

                int isAuthorized;
                int authErrorCode;
                var szCapturedFir = _fpData.TextFIR;
                var txtEventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var user = _commonUserService.GetUsers(code: userId).FirstOrDefault();
                if (user is null)
                {
                    isAuthorized = 0;
                    authErrorCode = (int)VirdiError.InvalidUser;
                    Logger.Log($"   +User {userId} not found.");
                    _serverAuthentication.SendAuthResultToTerminal(terminalId, userId, 0, 1, isAuthorized, txtEventTime, authErrorCode);
                    return;
                }

                var virdiFingerTemplates =
                    user.FingerTemplates.Where(ft => ft.FingerTemplateType.Code == FingerTemplateTypes.V400Code).ToList();

                for (var i = 0; i < virdiFingerTemplates.Count - 1; i += 2)
                {
                    if (_fpData == null) continue;
                    _fpData.ClearFPData();
                    _fpData.Import(1, virdiFingerTemplates[i].FingerIndex.OrderIndex, 2, 400, 400, virdiFingerTemplates[i].Template, virdiFingerTemplates[i + 1].Template);
                    var firTemplate = _fpData.TextFIR;

                    //dynamic storedFir = _fastSearch.FpInfo[userID];
                    //_matching.VerifyMatch(szTextEnrolledFIR, szCapturedFIR);
                    _matching.VerifyMatch(szCapturedFir, firTemplate);
                    //_matching.VerifyMatch(szCapturedFir, storedFir.TextFIR);

                    if (_matching.MatchingResult == 0) continue;

                    Logger.Log("   +Matching Success.");
                    Logger.Log($@"   +ErrorCode:{_ucsApi.ErrorCode}
    +TerminalID:{terminalId}
    +UserID:{userId}
    +AuthMode:{authMode}
    +Security Level:{securityLevel}
    +AntiPassBack Level:{antiPassBackLevel}
    +FingerData:{fingerData}", logType: LogType.Information);

                    var isVisitor = user.IsAdmin ? 0 : 1;
                    var blacklist = _blackListService.GetBlacklist(userId: (int)user.Id, deviceId: terminalId, startDate: DateTime.Today, endDate: DateTime.Now).Result;

                    var hasAccess =
                        (user.StartDate < DateTime.Now && user.EndDate > DateTime.Now ||
                         user.StartDate == user.EndDate || user.StartDate == default || user.EndDate == default) &&
                        user.IsActive
                            ? 1
                            : 0;

                    if (blacklist != null && blacklist.Count > 0) hasAccess = 0;
                    isAuthorized = hasAccess;
                    authErrorCode = hasAccess == 0 ? (int)VirdiError.Permission : (int)VirdiError.None;

                    _serverAuthentication.SendAuthResultToTerminal(terminalId, userId, hasAccess, isVisitor,
                        isAuthorized, txtEventTime, authErrorCode);
                    return;

                    //szTextEnrolledFIR = szCapturedFIR;
                }

                isAuthorized = 0;
                authErrorCode = 770;
                Logger.Log("   +Matching Fail.");
                _serverAuthentication.SendAuthResultToTerminal(terminalId, userId, 0, 1, isAuthorized, txtEventTime, authErrorCode);
            });
        }

        private async void GetAccessLogCallback(int clientId, int terminalId)
        {
            if (string.Equals(_accessLogData.DateTime, "0000-00-00 00:00:00", StringComparison.InvariantCultureIgnoreCase))
                return;

            //Task<List<TaskInfo>> taskAwaiter = null;
            //if (!_tasks.ContainsKey(clientId))
            //    taskAwaiter = _taskService.GetTasks(excludedTaskStatusCodes: string.Empty, taskItemId: clientId);

            byte[] picture = null;
            try
            {
                if (_accessLogData.PictureDataLength > 0)
                    picture = _accessLogData.PictureData as byte[];
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            var currentIndex = _accessLogData.CurrentIndex;
            var totalCount = _accessLogData.TotalNumber;

            var log = new Log
            {
                DeviceCode = (uint)terminalId,
                EventLog = _accessLogData.AuthResult == 0 ? _logEvents.Authorized : _logEvents.UnAuthorized,
                UserId = _accessLogData.UserID,
                LogDateTime = DateTime.Parse(_accessLogData.DateTime),
                //MatchingType = AccessLogData.AuthType,
                //MatchingType = authMode.BioCode,
                MatchingType = _virdiCodeMappings.GetMatchingTypeGenericLookup(_accessLogData.AuthType),
                AuthType = _accessLogData.AuthType,
                SubEvent = _virdiCodeMappings.GetLogSubEventGenericLookup(_accessLogData.AuthMode),
                PicByte = picture,
                TnaEvent = 0
            };

            _logger.LogDebug($@"<--EventGetAccessLog
    +ClientID:{clientId}
    +TerminalID:{terminalId}
    +ErrorCode:{_ucsApi.ErrorCode}
    +UserID:{log.UserId}
    +DateTime:{log.LogDateTime}
    +AuthMode:{log.SubEvent.Code}
    +AuthType:{log.AuthType}
    +IsAuthorized:{log.EventLog.Code == LogEvents.AuthorizedCode}
    +Result:{log.EventLog.Code == LogEvents.AuthorizedCode}
    +RFID:{_accessLogData.RFID}
    +PictureDataLength:{log.PicByte?.Length}
    +Progress:{currentIndex}/{totalCount}/Total:{_logCount}");

            //Task.Run(async () =>
            //{
            if (!RetrievedLogs.ContainsKey(clientId))
                RetrievedLogs[clientId] = new List<Log>();

            RetrievedLogs[clientId].Add(log);

            if (currentIndex == totalCount)
            {
                Logger.Log($"Retrieving logs from device: {terminalId} is finished. {currentIndex} logs retrieved and total {totalCount} and {currentIndex}/{totalCount} in progress");

                await Task.Run(async () =>
                {
                    var logInsertion = await _virdiLogService.AddLog(RetrievedLogs[clientId]);

                    var taskItem = _taskService.GetTaskItem(clientId);
                    taskItem.Status = logInsertion is null || !logInsertion.Success ? _taskStatuses.Failed : _taskStatuses.Done;
                    taskItem.Result = logInsertion is null || !logInsertion.Success ? JsonConvert.SerializeObject(logInsertion) : taskItem.Result;
                    _taskService.UpdateTaskStatus(taskItem);
                }).ContinueWith(_ => { RetrievedLogs.Remove(clientId); });
            }

            //var task = _tasks.ContainsKey(clientId) ? _tasks[clientId] : (taskAwaiter != null ? (await taskAwaiter).FirstOrDefault() : null);

            //if (task != null)
            //{
            //    _tasks.Add(clientId, task);
            //    task.TotalCount = totalCount;
            //    task.CurrentIndex = currentIndex;
            //    var taskList = new List<TaskInfo> { task };

            //    var biovationBrokerMessageData = new List<DataChangeMessage<TaskInfo>>
            //    {
            //        new DataChangeMessage<TaskInfo>
            //        {
            //            Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
            //            TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = taskList
            //        }
            //    };

            //    _biovationInternalSource.PushData(biovationBrokerMessageData);
            //}
            //});
        }

        private void TerminalDisconnectedCallback(int terminalId)
        {
            if (terminalId == 0)
            {
                return;
            }

            _onlineDevices.Remove((uint)terminalId);

            Task.Run(async () =>
            {
                var device = _commonDeviceService.GetDevices(code: (uint)terminalId, brandId: DeviceBrands.VirdiCode)[0];
                var connectionStatus = new ConnectionStatus
                {
                    DeviceId = device.DeviceId,
                    IsConnected = false
                };

                //var data = JsonConvert.SerializeObject(connectionStatus);
                //data = "jsoninput=" + data;

                try
                {
                    //_communicationManager.CallRest("/api/Biovation/DeviceConnectionState/DeviceConnectionState", "SignalR",
                    //                 new List<object> { data });

                    // var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                    // restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                    // await _monitoringRestClient.ExecuteAsync<ResultViewModel>(restRequest);
                    //integration

                    var connectionStatusList = new List<ConnectionStatus> { connectionStatus };
                    var biovationBrokerMessageData = new List<DataChangeMessage<ConnectionStatus>>
                    {
                        new DataChangeMessage<ConnectionStatus>
                        {
                            Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                            TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = connectionStatusList
                        }
                    };

                    _deviceConnectionStateInternalSource.PushData(biovationBrokerMessageData);

                    await _logService.AddLog(new Log
                    {
                        DeviceId = device.DeviceId,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.Disconnect
                    });

                }
                catch (Exception)
                {
                    //ignore
                }
            });


            Logger.Log("Disconnected device: { ID:" + terminalId + " }", logType: LogType.Information);

            //WinApi.SendPrivateMessageToTerminal(0, (uint)TerminalID, 0, ref message);
        }

        private void FirmwareVersionCallback(int clientId, int terminalId, string version)
        {
            if (_onlineDevices.ContainsKey((uint)terminalId))
            {
                _onlineDevices[(uint)terminalId].FirmwareVersion = version;
            }

            var deviceModels = _commonDeviceService.GetDeviceModels(brandId: DeviceBrands.VirdiCode);
            var acModelNumCharacterIndex = version.IndexOf("AC", StringComparison.Ordinal);

            if (acModelNumCharacterIndex > 0)
            {
                var deviceModel = deviceModels.FirstOrDefault(model => string.Equals(model.Name, version.Substring(acModelNumCharacterIndex, 6)));
                if (deviceModel != null)
                {
                    var device = _commonDeviceService.GetDevices(code: (uint)terminalId, brandId: DeviceBrands.VirdiCode)[0];
                    device.ModelId = deviceModel.Id;
                    device.Model = deviceModel;
                    _commonDeviceService.ModifyDevice(device);
                }
            }

            else if (version.ToLowerInvariant().Contains("ubio"))
            {
                var deviceModel = deviceModels.FirstOrDefault(model => model.Name.Contains("U-Bio"));
                if (deviceModel != null)
                {
                    var device = _commonDeviceService.GetDevices(code: (uint)terminalId, brandId: DeviceBrands.VirdiCode)[0];
                    device.ModelId = deviceModel.Id;
                    device.Model = deviceModel;
                    _commonDeviceService.ModifyDevice(device);
                }
            }

            //TerminalOption.GetOptionFromTerminal(clientId, terminalId);

            Logger.Log("Device firmware version: { ClientID:" + clientId + ", DeviceID:" + terminalId + ", Version:" + version + " }", logType: LogType.Information);
        }

        private void GetTerminalTimeCallback(int terminalId)
        {
            Logger.Log($"Device time: {{ DeviceID: {terminalId} Time: {DateTime.Now} }}");

            //UcsApi.SetTerminalTime((short) DateTime.Now.Year, (byte) DateTime.Now.Month, (byte) DateTime.Now.Day, (byte) DateTime.Now.Hour, (byte) DateTime.Now.Minute, (byte) DateTime.Now.Second);
            _ucsApi.SetTerminalTimezone(terminalId, "Iran Daylight Time");
        }

        //private void GetTerminalOptionCallback(int clientId, int terminalId)
        //{
        //    var info = new TerminalInfo();
        //    Thread.Sleep(500);
        //    WinApi.GetTerminalInfo((uint)terminalId, ref info);
        //    var existDevice = _commonDeviceService.GetDevices(code:(uint)terminalId, brandId:DeviceBrands.VirdiCode)[0];
        //    DeviceBasicInfo device;
        //    if (existDevice != null)
        //    {
        //        DeviceModel deviceModel = null;
        //        for (var i = 0; i < 4; i++)
        //        {
        //            try
        //            {
        //                deviceModel = _commonDeviceService.GetDeviceModelByName(_deviceTypes[info.ModelNo]);
        //            }
        //            catch (Exception)
        //            {
        //                Thread.Sleep(500);
        //                WinApi.GetTerminalInfo((uint)terminalId, ref info);
        //            }
        //        }
        //        device = new DeviceBasicInfo
        //        {
        //            Code = (uint)terminalId,
        //            Name = existDevice.Name,
        //            Brand = deviceModel != null ? deviceModel.Brand : existDevice.Brand,
        //            Model = deviceModel ?? existDevice.Model,
        //            IpAddress = TerminalOption.TerminalIP,
        //            Port = TerminalOption.Port,
        //            MacAddress = UcsApi.TerminalMacAddr[terminalId],
        //            RegisterDate = existDevice.RegisterDate,
        //            TimeSync = existDevice.TimeSync,
        //            Active = existDevice.Active,
        //            DeviceTypeId = existDevice.DeviceTypeId,
        //            DeviceId = existDevice.DeviceId
        //        };
        //        if (!string.IsNullOrEmpty(device.MacAddress))
        //        {
        //            device.MacAddress = Regex.Replace(device.MacAddress, ".{2}", "$0:");
        //            device.MacAddress = device.MacAddress.Substring(0, device.MacAddress.Length - 1);
        //        }
        //        _commonDeviceService.ModifyDevice(device);
        //        try
        //        {
        //            Task.Run(async () =>
        //            {
        //                //_localCommunicationManager.CallRest(
        //                //    device.Active == false
        //                //        ? "/biovation/api/Virdi/VirdiDevice/LockDevice"
        //                //        : "/biovation/api/Virdi/VirdiDevice/UnlockDevice", "Post",
        //                //    null,
        //                //    $"{device.Code}");
        //                var restRequest = new RestRequest(device.Active == false ? "Virdi/VirdiDevice/LockDevice" : "Virdi/VirdiDevice/UnlockDevice", Method.POST);
        //                restRequest.AddJsonBody(device.Code);
        //                await _biovationRestClient.ExecuteAsync(restRequest);
        //            });
        //        }
        //        catch (Exception)
        //        {
        //            //ignore
        //        }
        //        //try
        //        //{
        //        //    _commonDeviceService.UpdateDeviceBasicInfoByID(device);
        //        //}
        //        //catch (Exception)
        //        //{
        //        //    //ignore
        //        //}
        //    }
        //    else
        //    {
        //        var deviceModel = new DeviceModel();
        //        if (_deviceTypes.ContainsKey(info.ModelNo))
        //        {
        //            deviceModel = _commonDeviceService.GetDeviceModelByName(_deviceTypes[info.ModelNo]);
        //        }
        //        else
        //        {
        //            if (_onlineDevices.ContainsKey((uint)terminalId))
        //            {
        //                var deviceBrand = _commonDeviceService.GetDeviceBrandById(DeviceBrands.VirdiCode);
        //                var acModelNumCharacterIndex = _onlineDevices[(uint)terminalId].FirmwareVersion.IndexOf("AC", StringComparison.Ordinal) + 2;
        //                //deviceModel.Id = deviceBrand.Models.Aggregate((m1, m2) => m1.Id > m2.Id ? m1 : m2).Id + 1;
        //                deviceModel.Id = Convert.ToInt32(_onlineDevices[(uint)terminalId].FirmwareVersion.Substring(acModelNumCharacterIndex, 4));
        //                deviceModel.Name = "AC" + _onlineDevices[(uint)terminalId].FirmwareVersion.Substring(acModelNumCharacterIndex, 4);
        //                deviceModel.Description = "AC-" + _onlineDevices[(uint)terminalId].FirmwareVersion.Substring(acModelNumCharacterIndex, 4);
        //                deviceModel.ManufactureCode = info.ModelNo;
        //                deviceModel.Brand = deviceBrand;
        //                _commonDeviceService.AddDeviceModel(deviceModel);
        //                _deviceTypes.Add(deviceModel.ManufactureCode, deviceModel.Name);
        //            }
        //        }
        //        device = new DeviceBasicInfo
        //        {
        //            Code = (uint)terminalId,
        //            Brand = deviceModel.Brand,
        //            Model = deviceModel,
        //            IpAddress = TerminalOption.TerminalIP,
        //            Port = TerminalOption.Port,
        //            MacAddress = UcsApi.TerminalMacAddr[terminalId],
        //            RegisterDate = DateTime.Now,
        //            TimeSync = true,
        //            Active = true
        //        };
        //        device.Name = terminalId + "[" + device.IpAddress + "]";
        //        //device.FirmwareVersion = Version;
        //        _commonDeviceService.ModifyDevice(device);
        //    }
        //    TerminalOption.Clear();
        //    if (!_onlineDevices.ContainsKey((uint)terminalId))
        //    {
        //        _onlineDevices.Add((uint)terminalId, device);
        //    }
        //    else
        //    {
        //        device.FirmwareVersion = _onlineDevices[(uint)terminalId].FirmwareVersion;
        //        _onlineDevices[(uint)terminalId] = device;
        //    }
        //    Logger.Log("Device Info: { DeviceID:" + terminalId + "} terminal options retrieved");
        //    //this.ucsAPI.SetTerminalTime(2015, 10, 20, 11, 22, 33);
        //}

        private void GetTerminalOptionCallback(int clientId, int terminalId)
        {
            Logger.Log($@"<--Event Get Terminal Option
    +ClientID: {clientId}   
    +TerminalID:  {terminalId}
    +ErrorCode: {_ucsApi.ErrorCode}");

            //var termOp = new
            //{
            //    TerminalOption.ServerIP,
            //    TerminalOption.Port,
            //    TerminalOption.Application,
            //    TerminalOption.Authentication,
            //    TerminalOption.InputIDType,
            //    TerminalOption.NetWorkType,
            //    TerminalOption.TerminalIP
            //};

            //terminalOption.flagServer = 1;

            var info = new TerminalInfo();

            Thread.Sleep(500);
            WinApi.GetTerminalInfo((uint)terminalId, ref info);

            //var existDevice = _commonDeviceService.GetDevices(code:(uint)terminalId, brandId:DeviceBrands.VirdiCode)[0];
            //DeviceBasicInfo device;



            _terminalOption.Clear();



            Logger.Log("Device Info: { DeviceID:" + terminalId + "} terminal options retrieved");

            //this.ucsAPI.SetTerminalTime(2015, 10, 20, 11, 22, 33);
        }

        private void RegisterFace(int clientId, int terminalId, int currentIndex, int totalNumber, object eventData)
        {
            Task.Run(() =>
            {

                if (currentIndex == 0 && totalNumber == 0)
                {
                    Logger.Log($@"<--EventRegisterFace
                   +CID, TID : {clientId}, {terminalId}
                   Process Canceled.({clientId})");
                }
                else
                {
                    var taskItemAwaiter = _taskService.GetTaskItem(clientId);
                    Logger.Log($@"<--EventRegisterFace
                    +CID, TID : {clientId}, {terminalId}
                    +Process(Current, Total) : {currentIndex}, {totalNumber}
                    +RegFace Length : {((byte[])eventData).Length}");

                    //if (currentIndex == 1)
                    //{
                    //    // first
                    //    FaceData = null;
                    //    FaceData = new FACE_DATA();
                    //    FaceData.FaceNum = totalNumber;
                    //    FaceData.FaceBlock = new FACE_BLOCK[totalNumber];
                    //}

                    //FaceData.FaceBlock[currentIndex - 1] = new FACE_BLOCK();
                    //FaceData.FaceBlock[currentIndex - 1].Length = RegFaceData.Length;
                    //FaceData.FaceBlock[currentIndex - 1].Data = new byte[RegFaceData.Length];
                    //FaceData.FaceBlock[currentIndex - 1].Data = RegFaceData;

                    //if (currentIndex == totalNumber)
                    //{
                    var taskItem = taskItemAwaiter;
                    var userId = Convert.ToInt64(JsonConvert.DeserializeObject<JObject>(taskItem.Data)["UserId"]);
                    var user = _commonUserService.GetUsers(code: userId).FirstOrDefault();
                    //var userFaceTemplates = _faceTemplateService.FaceTemplates(userId: userId);

                    if (user is null)
                        return;

                    var faceData = (byte[])eventData;
                    var faceTemplate = new FaceTemplate
                    {
                        Index = currentIndex,
                        FaceTemplateType = _faceTemplateTypes.VFACE,
                        UserId = userId,
                        Template = faceData,
                        CheckSum = faceData.Sum(x => x),
                        Size = faceData.Length
                    };

                    if (user.FaceTemplates is null)
                        user.FaceTemplates = new List<FaceTemplate>();

                    if (!user.FaceTemplates.Exists(ft =>
                        ft.FaceTemplateType.Code == FaceTemplateTypes.VFACECode && ft.Index == faceTemplate.Index))
                    {
                        user.FaceTemplates.Add(faceTemplate);
                        _faceTemplateService.ModifyFaceTemplate(faceTemplate);
                    }

                    //if (user.FaceTemplates.Any() && newTemplateAdded)
                    //        foreach (var faceTemplates in user.FaceTemplates)
                    //        {
                    //            _faceTemplateService.ModifyFaceTemplate(faceTemplates);
                    //        }

                    // last
                    // File Write ==> user for download UserData
                    //FileStream fileStream = new FileStream(fileFaceData, FileMode.Create, FileAccess.Write);

                    //BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                    //binaryWriter.Write(totalNumber);
                    //for (int i = 0; i < totalNumber; i++)
                    //{
                    //    binaryWriter.Write(FaceData.FaceBlock[i].Length);
                    //    binaryWriter.Write(FaceData.FaceBlock[i].Data);
                    //}

                    //fileStream.Close();
                    //FaceData = null;
                    //}
                }
            });
        }

        private async void PictureLog(int terminalId)
        {
            Logger.Log($@"<--EventPictureLog
    +TerminalID: {terminalId}
    +ErrorCode: {_ucsApi.ErrorCode}
    +TerminalID: {terminalId}
    +UserID: {_accessLogData.UserID}
    +DataTime: {_accessLogData.DateTime}
    +AuthMode: {_accessLogData.AuthMode}
    +AuthType: {_accessLogData.AuthType}
    +IsAuthorized: {_accessLogData.IsAuthorized}
    +Result: {_accessLogData.AuthResult}
    +RFID: {_accessLogData.RFID}
    +PictureDataLength: {_accessLogData.PictureDataLength}
    +Progress: {_accessLogData.CurrentIndex}/{_accessLogData.TotalNumber}");

            byte[] picture = null;
            try
            {
                if (_accessLogData.PictureDataLength > 0)
                    picture = _accessLogData.PictureData as byte[];
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            //var deviceBasicInfo = _commonDeviceService.GetDevices(code:(uint)terminalId, brandId:DeviceBrands.VirdiCode)[0];

            var log = new Log
            {
                DeviceCode = (uint)terminalId,
                EventLog = _accessLogData.IsAuthorized == 1 ? _logEvents.Authorized : _logEvents.UnAuthorized,
                UserId = _accessLogData.UserID,
                LogDateTime = DateTime.Now,
                //MatchingType = authMode.BioCode,
                MatchingType = _virdiCodeMappings.GetMatchingTypeGenericLookup(_accessLogData.AuthMode),
                SubEvent = _virdiCodeMappings.GetLogSubEventGenericLookup(_accessLogData.AuthMode),
                TnaEvent = 0,
                PicByte = picture
            };

            await _virdiLogService.AddLog(log);
        }

        private void VerifyPassword(int terminalId, int userId, int authMode, int antipassbackLevel, string password)
        {
            Task.Run(() =>
            {
                Logger.Log($@"<--EventVerifyPassword
                   +ErrorCode:{_ucsApi.ErrorCode}
                   +TerminalID:{terminalId}
                   +UserID:{userId}
                   +AuthMode:{authMode}
                   +Antipassback Level:{antipassbackLevel}
                   +Password:{password}");

                var user = _commonUserService.GetUsers(code: userId).FirstOrDefault();


                var isAuthorized = string.Equals(user?.Password, password, StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
                var txtEventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var errorCode = isAuthorized == 1 ? 0 : 770;

                _serverAuthentication.SetAuthType(0, 0, 0, 1, 0, 0);
                _serverAuthentication.SendAuthResultToTerminal(terminalId, userId, 0, 0, isAuthorized, txtEventTime, errorCode);
            });
        }

        private void VerifyFace1To1(int terminalId, int userId, int authMode, int antiPassBackLevel, object faceData)
        {
            Logger.Log("<--EventVerifyFace1to1");

            // Perform server 1:1 face authentication
            Logger.Log($@"   +ErrorCode:{_ucsApi.ErrorCode}
    +TerminalID:{terminalId}
    +UserID:{userId}
    +AuthMode:{authMode}
    +AntiPassBack Level:{antiPassBackLevel}
    +FaceData:{faceData}", logType: LogType.Information);

            var isAuthorized = 0;
            var authErrorCode = 770; // 770 -> Matching failure
            var txtEventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _serverAuthentication.SetAuthType(0, 0, 0, 0, 0, 0);
            _serverAuthentication.SetAuthTypeEx(1/*IsFace*/, 0, 0, 0, 0, 0, 0, 0);
            _serverAuthentication.SendAuthResultToTerminal(terminalId, userId, 1, 0, isAuthorized, txtEventTime, authErrorCode);
        }

        private void VerifyFace1ToN(int terminalId, int authMode, int inputIdLength, int antiPassBackLevel, object faceData)
        {
            Logger.Log(@"<--EventVerifyFace1toN");

            // Perform server 1:N face authentication
            var isAuthorized = 0;
            var authErrorCode = 770; // 770 -> Matching failure

            var txtEventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //_FACE_NEUROTEC
            //IsAuthorized = IdentifyFace((byte[])FaceData);


            //this.serverAuthentication.SetAuthTypeEx(1/*IsFace*/, 0, 0, 0, 0, 0, 0, 0);
            //this.serverAuthentication.SendAuthResultToTerminal(TerminalID, 1, 1, 0, IsAuthorized, txtEventTime, authErrorCode);
            _serverAuthentication.SetAuthType(0, 0, 0, 0, 0, 0);
            _serverAuthentication.SetAuthTypeEx(1/*IsFace*/, 0, 0, 0, 0, 0, 0, 0);
            _serverAuthentication.SendAuthResultToTerminal(terminalId, 1, 1, 0, isAuthorized, txtEventTime, authErrorCode);

            //+UserID:{userId}
            Logger.Log($@" + ErrorCode: {_ucsApi.ErrorCode}
            +TerminalID: {terminalId}
            +AuthMode: {authMode}
            +AntiPassBack Level: {antiPassBackLevel}
            +FaceData: {faceData}
            ", logType: LogType.Information);
        }

        #region Face authentication processing

        /* _FACE_NEUROTEC
        int IdentifyFace(byte[] inputFaceData)
        {
            // This code is an example of using facial data acquired from the terminal as registration data and input data.

            int IsAuthorized = 0;
            const string Components = "Biometrics.FaceMatching";

            NLExtractor extractor = null;
            NMatcher matcher = null;

            // obtain license            
            try
            {
                if (!NLicense.ObtainComponents("/local", 5000, Components))
                {
                    Console.WriteLine("Could not obtain licenses for components: {0}", Components);
                    return 0;
                }

                // create an extractor
                extractor = new NLExtractor();

                // extract probe template
                NBuffer probeTemplate;
                try
                {
                    probeTemplate = new NBuffer(inputFaceData);
                    // 파일에서 가져올 경우
                    //probeTemplate = ExtractTemplate(extractor, file name, false);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("error reading candidate image -> {0}", ex);
                    return 0;
                }

                // test buff                
                int nDBUserCount = 1;
                byte[] dbBuff = (byte[])inputFaceData.Clone();
                //

                // extract db templates
                List<NBuffer> dbTemplates = new List<NBuffer>();
                for (int i = 0; i < nDBUserCount; ++i)
                {
                    try
                    {
                        dbTemplates.Add(new NBuffer(dbBuff));
                        // When importing from a file
                        //dbTemplates.Add(ExtractTemplate(extractor, args[i], false);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("error reading reference image -> {0}", ex);
                        return 0;
                    }
                }

                // create a matcher
                matcher = new NMatcher();
                // identify face from the image by comparing to each template from db buff
                Console.WriteLine("=== identification started ===");
                NMatchingDetails matchingDetails;
                matcher.IdentifyStart(probeTemplate, out matchingDetails);
                try
                {
                    // Recognize the largest score as the same user
                    // If score is 120 or more, authentication is successful.
                    // The score value is adjustable. For more information, refer to the manual of Face Recognition SDK (Neuro techrology SDK)

                    for (int i = 0; i < dbTemplates.Count; i++)
                    {
                        int score = matcher.IdentifyNext(dbTemplates[i], matchingDetails);
                        if (score > 120)
                        {
                            Console.WriteLine("=> template[{0}] identified (score: {1})", i, score);
                            IsAuthorized = 1;
                        }
                        else
                        {
                            Console.WriteLine("=> template[{0}] not identified (score: {1})", i, score);
                        }
                        Console.WriteLine(MatchingDetailsToString(matchingDetails));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    matcher.IdentifyEnd();
                }
                Console.WriteLine("=== identification finished ===");

                return 1;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                INeurotecException neurotecException = ex as INeurotecException;
                if (neurotecException != null)
                {
                    return neurotecException.Code;
                }
                return 0;
            }
            finally
            {
                NLicense.ReleaseComponents(Components);

                if (matcher != null)
                {
                    matcher.Dispose();
                }
            }
        }


        private static string MatchingDetailsToString(NMatchingDetails details)
        {
            StringBuilder detailsStr = new StringBuilder();
            if ((details.BiometricType & NBiometricType.Finger) == NBiometricType.Finger)
            {
                detailsStr.Append("    Fingerprint match details:");
                detailsStr.AppendLine(string.Format(" score = {0}", details.FingersScore));
                foreach (NFMatchingDetails fngrDetails in details.Fingers)
                {
                    detailsStr.AppendLine(string.Format("    fingerprint index: {0}; score: {1};", fngrDetails.MatchedIndex, fngrDetails.Score));
                }
            }
            if ((details.BiometricType & NBiometricType.Face) == NBiometricType.Face)
            {
                detailsStr.Append("    Face match details:");
                detailsStr.AppendLine(string.Format(" face index: {0}; score: {1}", details.FacesMatchedIndex, details.FacesScore));
                foreach (NLMatchingDetails faceDetails in details.Faces)
                {
                    detailsStr.AppendLine(string.Format("    face index: {0}; score: {1};", faceDetails.MatchedIndex, faceDetails.Score));
                }
            }
            if ((details.BiometricType & NBiometricType.Iris) == NBiometricType.Iris)
            {
                detailsStr.Append("    Irises match details:");
                detailsStr.AppendLine(string.Format(" score = {0}", details.IrisesScore));
                foreach (NEMatchingDetails irisesDetails in details.Irises)
                {
                    detailsStr.AppendLine(string.Format("    irises index: {0}; score: {1};", irisesDetails.MatchedIndex, irisesDetails.Score));
                }
            }
            if ((details.BiometricType & NBiometricType.Palm) == NBiometricType.Palm)
            {
                detailsStr.Append("    Palmprint match details:");
                detailsStr.AppendLine(string.Format(" score = {0}", details.PalmsScore));
                foreach (NFMatchingDetails fngrDetails in details.Palms)
                {
                    detailsStr.AppendLine(string.Format("    palmprint index: {0}; score: {1};", fngrDetails.MatchedIndex, fngrDetails.Score));
                }
            }
            if ((details.BiometricType & NBiometricType.Voice) == NBiometricType.Voice)
            {
                detailsStr.Append("    Voice match details:");
                detailsStr.AppendLine(string.Format(" score = {0}", details.VoicesScore));
                foreach (NSMatchingDetails voicesDetails in details.Voices)
                {
                    detailsStr.AppendLine(string.Format("    voices index: {0}; score: {1};", voicesDetails.MatchedIndex, voicesDetails.Score));
                }
            }
            return detailsStr.ToString();
        }

        // 파일에서 가져왔을 경우
        NBuffer ExtractTemplate(NLExtractor extractor, string fileName, bool isProbe)
        {
            NImage image = null;
            NGrayscaleImage grayscaleImage = null;
            NLTemplate template = null;

            // Set the template size (recommended for matching medium, for enrolling - large) (optional)
            extractor.TemplateSize = (isProbe) ? NleTemplateSize.Medium : NleTemplateSize.Large;
            try
            {
                image = NImage.FromFile(fileName);
                grayscaleImage = image.ToGrayscale();
                NleDetectionDetails detectionDetails;
                NleExtractionStatus extractionStatus;
                template = extractor.Extract(grayscaleImage, out detectionDetails, out extractionStatus);
                if (extractionStatus == NleExtractionStatus.TemplateCreated)
                {
                    return template.Save();
                }
                else
                {
                    throw new ApplicationException("failed to extract template. extraction status: " + extractionStatus);
                }
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                }
                if (grayscaleImage != null)
                {
                    grayscaleImage.Dispose();
                }
                if (template != null)
                {
                    template.Dispose();
                }
            }
        }
        */

        #endregion

        private void AuthTypeWithUserId(int terminalId, int userId)
        {
            CallSetAuthTypeAndSendAuthInfoToTerminal(terminalId, userId);

            Logger.Log($@"<--EventAuthTypeWithUserID
               +TerminalID:{terminalId}
               +UserID: {userId}
               +ErrorCode:{_ucsApi.ErrorCode}");
        }

        private void AuthTypeWithUniqueId(int terminalId, string uniqueId)
        {
            CallSetAuthTypeAndSendAuthInfoToTerminal(terminalId, 1);

            Logger.Log($@"<--EventAuthTypeWithUniqueID
               +TerminalID:{terminalId}
               +UniqueID: {uniqueId}
               +ErrorCode:{_ucsApi.ErrorCode}");
        }

        private void CallSetAuthTypeAndSendAuthInfoToTerminal(int terminalId, int userId)
        {
            var isAndOperation = 0;
            var isFPCard = 0;
            var isCard = 0;
            var isPassword = 0;
            var isCardID = 0;
            var isFinger = 1;

            var user = _commonUserService.GetUsers(code: userId).FirstOrDefault();
            //var isVisitor = user!= null && user.IsAdmin ? 0 : 1;
            var hasAccess = user != null && (user.StartDate < DateTime.Now && user.EndDate > DateTime.Now || user.StartDate == user.EndDate || user.StartDate == default || user.EndDate == default) && user.IsActive ? 1 : 0;

            //var txtEventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //var isAuthorized = hasAccess;
            var authErrorCode = hasAccess == 0 ? (int)VirdiError.InvalidUser : (int)VirdiError.None;

            //ServerAuthentication.SendAuthResultToTerminal(terminalId, userId, hasAccess, isVisitor, isAuthorized, txtEventTime, authErrorCode);

            _serverAuthentication.SetAuthType(isAndOperation, isFinger, isFPCard, isPassword, isCard, isCardID);
            _serverAuthentication.SendAuthInfoToTerminal(terminalId, userId, hasAccess, authErrorCode);
        }

        private void GetAntiPassBack(int terminalId, int userId)
        {
            Logger.Log($@"<--EventAntipassback
        +TerminalID: {terminalId}
        +UserID: {userId}");
            const int result = 1; // 0 = Fail, 1 = Success
            _serverAuthentication.SendAntipassbackResultToTerminal(terminalId, userId, result);
            Logger.Log($@"-->SendAntipassbackResultToTerminal
               +TerminalID: {terminalId}
               +UserID: {userId}
               +Result: {result}");
        }

        private void FirmwareUpgrading(int clientId, int terminalId, int currentIndex, int totalNumber)
        {
            UploadFirmwareFileTaskFinished = false;
            UpgradeFirmwareTaskFinished = false;

            if (currentIndex == totalNumber)
                UploadFirmwareFileTaskFinished = true;

            Logger.Log($@"<--EventFirmwareUpgrading
               +ErrorCode: {_ucsApi.ErrorCode}
               +ClientID: {clientId}
               +TerminalID: {terminalId}
               +CurrentIndex: {currentIndex}
               +TotalNumber: {totalNumber}");
        }

        private void FirmwareUpgraded(int clientId, int terminalId)
        {
            Logger.Log($@"<--EventFirmwareUpgraded
               +ErrorCode: {_ucsApi.ErrorCode}
               +ClientID: {clientId}
               +TerminalID: {terminalId}");

            UpgradeFirmwareTaskFinished = true;
        }

        public void Dispose()
        {
            try
            {
                _fixDaylightSavingTimer?.Dispose();
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}

