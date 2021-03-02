using Biovation.Brands.EOS.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using zkemkeeper;
using Log = Biovation.Domain.Log;
using TimeZone = Biovation.Domain.TimeZone;
//using TimeZone = Biovation.CommonClasses.Models.TimeZone;
// ReSharper disable InconsistentlySynchronizedField


namespace Biovation.Brands.EOS.Devices
{
    public class ZkBaseDevice : Device
    {
        protected readonly DeviceBasicInfo DeviceInfo;
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly AccessGroupService _accessGroupService;
        protected readonly UserService UserService;
        protected readonly UserCardService UserCardService;
        protected readonly FaceTemplateService FaceTemplateService;
        private readonly Dictionary<uint, Device> _onlineDevices;
        //private readonly CommunicationManager<ResultViewModel> _communicationManager = new CommunicationManager<ResultViewModel>();
        private readonly RestClient _restClient;
        protected CancellationTokenSource TokenSource = new CancellationTokenSource();

        protected readonly CZKEMClass ZkTecoSdk = new CZKEMClass(); //create Standalone _zkTecoSdk class dynamically
        private bool _reconnecting;

        private readonly bool _isGetLogEnable;

        private readonly TaskTypes _taskTypes;
        private readonly LogService _logService;
        private readonly TaskStatuses _taskStatuses;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FaceTemplateTypes _faceTemplateTypes;

        protected static readonly object LockObject = new object();
        internal ZkBaseDevice(DeviceBasicInfo deviceInfo, LogService logService, EosCodeMappings eosCodeMappings,
            TaskService taskService, UserService userService, DeviceService deviceService,
            AccessGroupService accessGroupService, UserCardService userCardService,
            FaceTemplateService faceTemplateService, RestClient restClient, Dictionary<uint, Device> onlineDevices,
            BiovationConfigurationManager biovationConfigurationManager, LogEvents logEvents, LogSubEvents logSubEvents,
            TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes,
            DeviceBrands deviceBrands, BiometricTemplateManager biometricTemplateManager,
            FingerTemplateTypes fingerTemplateTypes, FaceTemplateTypes faceTemplateTypes)
            : base(deviceInfo, logEvents, logSubEvents, eosCodeMappings)
        {
            DeviceInfo = deviceInfo;
            _logService = logService;
            _taskService = taskService;
            UserService = userService;
            _userService = userService;
            _deviceService = deviceService;
            _accessGroupService = accessGroupService;
            UserCardService = userCardService;
            FaceTemplateService = faceTemplateService;
            _restClient = restClient;
            _onlineDevices = onlineDevices;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _deviceBrands = deviceBrands;
            _biometricTemplateManager = biometricTemplateManager;
            _fingerTemplateTypes = fingerTemplateTypes;
            _faceTemplateTypes = faceTemplateTypes;
            _isGetLogEnable = biovationConfigurationManager.GetAllLogWhenConnect;
        }

        public override bool Connect()
        {
            lock (ZkTecoSdk)
            {
                if (!string.IsNullOrEmpty(DeviceInfo.DeviceLockPassword))
                {
                    ZkTecoSdk.SetCommPassword(Convert.ToInt32(DeviceInfo.DeviceLockPassword));
                }

                var connectResult = ZkTecoSdk.Connect_Net(DeviceInfo.IpAddress, DeviceInfo.Port);
                while (!connectResult)
                {
                    Logger.Log(
                        $"Could not connect to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}:{DeviceInfo.Port}");

                    Thread.Sleep(20000);
                    connectResult = ZkTecoSdk.Connect_Net(DeviceInfo.IpAddress, DeviceInfo.Port);
                }

                Thread.Sleep(500);

                //Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                if (_reconnecting)
                {
                    if (ZkTecoSdk.RegEvent((int)DeviceInfo.Code, 65535))
                    {
                        //_zkTecoSdk.OnFinger -= _zkTecoSdk_OnFinger;
                        //_zkTecoSdk.OnVerify -= _zkTecoSdk_OnVerify;
                        Thread.Sleep(500);
                        ZkTecoSdk.OnAttTransaction -= OnAttendanceTransactionCallback;
                        ZkTecoSdk.OnAttTransactionEx -= OnAttendanceTransactionExCallback;
                        //_zkTecoSdk.OnFingerFeature -= _zkTecoSdk_OnFingerFeature;
                        ZkTecoSdk.OnKeyPress -= OnKeyPressCallback;
                        //_zkTecoSdk.OnEnrollFinger -= _zkTecoSdk_OnEnrollFinger;
                        //_zkTecoSdk.OnDeleteTemplate -= _zkTecoSdk_OnDeleteTemplate;
                        //_zkTecoSdk.OnNewUser -= _zkTecoSdk_OnNewUser;
                        //_zkTecoSdk.OnHIDNum -= _zkTecoSdk_OnHIDNum;
                        //_zkTecoSdk.OnAlarm -= _zkTecoSdk_OnAlarm;
                        //_zkTecoSdk.OnDoor -= _zkTecoSdk_OnDoor;
                        //_zkTecoSdk.OnWriteCard -= _zkTecoSdk_OnWriteCard;
                        //_zkTecoSdk.OnEmptyCard -= _zkTecoSdk_OnEmptyCard;
                        ZkTecoSdk.OnDisConnected -= OnDisconnectedCallback;
                    }
                }

                if (ZkTecoSdk.RegEvent((int)DeviceInfo.Code, 65535))
                {
                    //_zkTecoSdk.OnFinger += _zkTecoSdk_OnFinger;
                    //_zkTecoSdk.OnVerify += _zkTecoSdk_OnVerify;
                    Thread.Sleep(500);
                    ZkTecoSdk.OnAttTransaction += OnAttendanceTransactionCallback;
                    ZkTecoSdk.OnAttTransactionEx += OnAttendanceTransactionExCallback;
                    //_zkTecoSdk.OnFingerFeature += _zkTecoSdk_OnFingerFeature;
                    ZkTecoSdk.OnKeyPress += OnKeyPressCallback;
                    //_zkTecoSdk.OnEnrollFinger += _zkTecoSdk_OnEnrollFinger;
                    //_zkTecoSdk.OnDeleteTemplate += _zkTecoSdk_OnDeleteTemplate;
                    //_zkTecoSdk.OnNewUser += _zkTecoSdk_OnNewUser;
                    //_zkTecoSdk.OnHIDNum += _zkTecoSdk_OnHIDNum;
                    //_zkTecoSdk.OnAlarm += _zkTecoSdk_OnAlarm;
                    //_zkTecoSdk.OnDoor += _zkTecoSdk_OnDoor;
                    //_zkTecoSdk.OnWriteCard += _zkTecoSdk_OnWriteCard;
                    //_zkTecoSdk.OnEmptyCard += _zkTecoSdk_OnEmptyCard;
                    ZkTecoSdk.OnDisConnected += OnDisconnectedCallback;
                }

                try
                {
                    var firmwareVersion = "";
                    var macAddress = "";

                    Thread.Sleep(500);
                    ZkTecoSdk.GetDeviceFirmwareVersion((int)DeviceInfo.Code, ref firmwareVersion);
                    Thread.Sleep(500);
                    ZkTecoSdk.GetDeviceMAC((int)DeviceInfo.Code, ref macAddress);
                    Thread.Sleep(500);
                    ZkTecoSdk.GetSerialNumber((int)DeviceInfo.Code, out var serialNumber);

                    DeviceInfo.FirmwareVersion = firmwareVersion;
                    DeviceInfo.MacAddress = macAddress;
                    DeviceInfo.SerialNumber = serialNumber;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }

                _deviceService.ModifyDevice(DeviceInfo);

                if (DeviceInfo.TimeSync)
                {
                    try
                    {
                        try
                        {
                            var result = ZkTecoSdk.SetDeviceTime2((int)DeviceInfo.Code, DateTime.Now.Year, DateTime.Now.Month,
                                DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            if (result)
                                Logger.Log($"Device {DeviceInfo.Code} time has been set to: {DateTime.Now:u}");
                            else
                            {
                                result = ZkTecoSdk.SetDeviceTime((int)DeviceInfo.Code);
                                Logger.Log(result
                                    ? $"Device {DeviceInfo.Code} time has been set to server time: {DateTime.Now:u}"
                                    : $"Could not set time for device {DeviceInfo.Code}");
                            }
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                            ZkTecoSdk.SetDeviceTime((int)DeviceInfo.Code);
                            Logger.Log($"Device {DeviceInfo.Code} time has been set to server time: {DateTime.Now:u}");
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                }
            }

            if (!_onlineDevices.ContainsKey(DeviceInfo.Code))
            {
                _onlineDevices.Add(DeviceInfo.Code, this);
            }
            var connectionStatus = new ConnectionStatus
            {
                DeviceId = DeviceInfo.DeviceId,
                IsConnected = true
            };

            try
            {
                //_communicationManager.CallRest(
                //    "/api/Biovation/DeviceConnectionState/DeviceConnectionState", "SignalR",
                //    new List<object> { data });
                var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            }
            catch (Exception)
            {
                //ignore
            }

            _logService.AddLog(new Log
            {
                DeviceId = DeviceInfo.DeviceId,
                DeviceCode = DeviceInfo.Code,
                LogDateTime = DateTime.Now,
                EventLog = LogEvents.Connect
            });

            if (_isGetLogEnable)
            {
                var creatorUser = _userService.GetUsers(code: 123456789)?.Data?.Data?.FirstOrDefault();
                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.GetLogsInPeriod,
                    Priority = _taskPriorities.Medium,
                    TaskItems = new List<TaskItem>(),
                    DeviceBrand = _deviceBrands.Eos,
                    DueDate = DateTimeOffset.Now
                };

                

                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.GetLogsInPeriod,
                    Priority = _taskPriorities.Medium,
                    DeviceId = DeviceInfo.DeviceId,
                    Data = JsonConvert.SerializeObject(new { fromDate= DateTime.Now.AddMonths(-3), toDate = DateTime.Now.AddDays(5) }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1
                });
                _taskService.InsertTask(task);
                _taskService.ProcessQueue(_deviceBrands.Eos, DeviceInfo.DeviceId).ConfigureAwait(false);
            }

            Task.Run(CheckConnection, TokenSource.Token);
            Logger.Log($"Successfully connected to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}:{DeviceInfo.Port}");

            return true;
        }

        public void CheckConnection()
        {
            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;

            bool deviceConnectionStatus;

            do
            {
                if (TokenSource.IsCancellationRequested)
                    return;
                lock (ZkTecoSdk)
                    deviceConnectionStatus = ZkTecoSdk.GetDeviceTime((int)DeviceInfo.Code, ref year, ref month,
                        ref day,
                        ref hour, ref minute, ref second);

                Thread.Sleep(5000);
                if (TokenSource.IsCancellationRequested)
                    return;
            } while (deviceConnectionStatus);

            Logger.Log($"Connection lost on device {DeviceInfo.Code}");
            Disconnect(false);

            if (TokenSource.IsCancellationRequested) return;
            _reconnecting = true;
            Task.Run(() => { Connect(); }, TokenSource.Token);
        }

        public bool Disconnect(bool cancelReconnecting = true)
        {
            lock (ZkTecoSdk)
            {
                ////_zkTecoSdk.OnFinger -= _zkTecoSdk_OnFinger;
                ////_zkTecoSdk.OnVerify -= _zkTecoSdk_OnVerify;
                //_zkTecoSdk.OnAttTransaction -= OnAttendanceTransactionCallback;
                //_zkTecoSdk.OnAttTransactionEx -= OnAttendanceTransactionExCallback;
                ////_zkTecoSdk.OnFingerFeature -= _zkTecoSdk_OnFingerFeature;
                //_zkTecoSdk.OnKeyPress -= OnKeyPressCallback;
                ////_zkTecoSdk.OnEnrollFinger -= _zkTecoSdk_OnEnrollFinger;
                ////_zkTecoSdk.OnDeleteTemplate -= _zkTecoSdk_OnDeleteTemplate;
                ////_zkTecoSdk.OnNewUser -= _zkTecoSdk_OnNewUser;
                ////_zkTecoSdk.OnHIDNum -= _zkTecoSdk_OnHIDNum;
                ////_zkTecoSdk.OnAlarm -= _zkTecoSdk_OnAlarm;
                ////_zkTecoSdk.OnDoor -= _zkTecoSdk_OnDoor;
                ////_zkTecoSdk.OnWriteCard -= _zkTecoSdk_OnWriteCard;
                ////_zkTecoSdk.OnEmptyCard -= _zkTecoSdk_OnEmptyCard;
                //_zkTecoSdk.RegEvent((int)_deviceInfo.Code, 0);

                ZkTecoSdk.Disconnect();

                if (cancelReconnecting)
                    TokenSource.Cancel(false);
            }

            lock (_onlineDevices)
                if (_onlineDevices.ContainsKey(DeviceInfo.Code))
                    _onlineDevices.Remove(DeviceInfo.Code);

            var connectionStatus = new ConnectionStatus
            {
                DeviceId = DeviceInfo.DeviceId,
                IsConnected = false
            };

            try
            {
                //_communicationManager.CallRest(
                //    "/api/Biovation/DeviceConnectionState/DeviceConnectionState", "SignalR",
                //    new List<object> { data });
                var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            }
            catch (Exception)
            {
                //ignore
            }

            _logService.AddLog(new Log
            {
                DeviceId = DeviceInfo.DeviceId,
                DeviceCode = DeviceInfo.Code,
                LogDateTime = DateTime.Now,
                EventLog = LogEvents.Disconnect
            });

            Logger.Log($"Disconnected from device: {DeviceInfo.Code} IPAddress => {DeviceInfo.IpAddress}:{DeviceInfo.Port}", logType: LogType.Information);
            return true;
        }
        //If your fingerprint(or your card) passes the verification,this event will be triggered
        private void OnAttendanceTransactionCallback(int iUserId, int iIsInValid, int iInOutMode, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond)
        {
            Logger.Log($@"RTEvent OnAttTransaction Has been Triggered,Verified OK
     ...UserID: {iUserId}
     ...TerminalID: {DeviceInfo.Code}
     ...isInvalid: {iIsInValid}
     ...inOutMode: {iInOutMode}
     ...VerifyMethod: {iVerifyMethod}
     ...Time: {iYear}-{iMonth}-{iDay}  {iHour}:{iMinute}:{iSecond}");


            var log = new Log
            {
                DeviceId = DeviceInfo.DeviceId,
                DeviceCode = DeviceInfo.Code,
                LogDateTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond),
                EventLog = LogEvents.Authorized,
                UserId = iUserId,
                MatchingType = EosCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                InOutMode = DeviceInfo.DeviceTypeId,
                TnaEvent = (ushort)iInOutMode,
                SubEvent = LogSubEvents.Normal
            };

            _logService.AddLog(log);
        }
        //If your fingerprint(or your card) passes the verification,this event will be triggered

        private void OnAttendanceTransactionExCallback(string iUserId, int iIsInValid, int iInOutMode, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            //WorkCode : the difference between the event OnAttTransaction and OnAttTransactionEx
            Logger.Log($@"RTEvent OnAttTransactionEx Has been Triggered,Verified OK
     ...UserID: {iUserId}
     ...TerminalID: {DeviceInfo.Code}
     ...isInvalid: {iIsInValid}
     ...inOutMode: {iInOutMode}
     ...VerifyMethod: {iVerifyMethod}
     ...WorkCode: {iWorkCode} 
     ...Time: {iYear}-{iMonth}-{iDay}  {iHour}:{iMinute}:{iSecond}");

            try
            {
                var userId = Convert.ToInt64(iUserId);

                var log = new Log
                {
                    DeviceId = DeviceInfo.DeviceId,
                    DeviceCode = DeviceInfo.Code,
                    LogDateTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond),
                    EventLog = LogEvents.Authorized,
                    UserId = userId,
                    InOutMode = DeviceInfo.DeviceTypeId,
                    MatchingType = EosCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                    TnaEvent = (ushort)iInOutMode,
                    SubEvent = LogSubEvents.Normal
                };

                _logService.AddLog(log);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

        }
        //When you press the keypad,this event will be triggered.
        private void OnKeyPressCallback(int iKey)
        {
            Logger.Log("RTEvent OnKeyPress Has been Triggered, Key: " + iKey);
        }

        private void OnDisconnectedCallback()
        {
            if (_onlineDevices.ContainsKey(DeviceInfo.Code))
            {
                _onlineDevices.Remove(DeviceInfo.Code);
            }

            var connectionStatus = new ConnectionStatus
            {
                DeviceId = DeviceInfo.DeviceId,
                IsConnected = false
            };

            try
            {
                //_communicationManager.CallRest(
                //    "/api/Biovation/DeviceConnectionState/DeviceConnectionState", "SignalR",
                //    new List<object> { data });
                var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                _logService.AddLog(new Log
                {
                    DeviceId = DeviceInfo.DeviceId,
                    DeviceCode = DeviceInfo.Code,
                    LogDateTime = DateTime.Now,
                    EventLog = LogEvents.Disconnect
                });
            }
            catch (Exception)
            {
                //ignore
            }

            _reconnecting = true;
            Logger.Log($"Device {DeviceInfo.Code} disconnected.", logType: LogType.Information);
        }

        public override bool TransferUser(User user)
        {
            lock (ZkTecoSdk)
            {
                var errorCode = 0;
                // _zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);
                //var card = UserCardService.GetCardsByFilter(user.Id).FirstOrDefault(c => c.IsActive);
                if (user.IdentityCard != null && user.IdentityCard.IsActive)
                {
                    if (ZkTecoSdk.SetStrCardNumber(user.IdentityCard.Number))
                        Logger.Log($"Successfully set card for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                    else
                    {
                        ZkTecoSdk.GetLastError(ref errorCode);
                        Logger.Log($"Cannot set card for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                    }
                }

                var name = user.FirstName + " " + user.SurName;
                if (ZkTecoSdk.SSR_SetUserInfo((int)DeviceInfo.Code, user.Id.ToString(), name.Trim(), user.Password,
                    user.IsAdmin ? 3 : 0, true))
                {
                    Logger.Log($"UserId {user.Id} successfully added to DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                    try
                    {
                        if (user.FingerTemplates.Any())
                        {
                            var zkFinger = user.FingerTemplates
                                .Where(x => x.FingerTemplateType.Code == FingerTemplateTypes.VX10Code)
                                .ToList();
                            foreach (var finger in zkFinger)
                            {
                                for (var i = 0; i < 9; i++)
                                {
                                    if (ZkTecoSdk.SetUserTmpExStr((int)DeviceInfo.Code, user.Id.ToString(), finger.Index,
                                        1,
                                        Encoding.ASCII.GetString(finger.Template)))
                                    {
                                        //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                                        Logger.Log(
                                            $"Successfully set template for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                                        break;
                                    }

                                    ZkTecoSdk.GetLastError(ref errorCode);
                                    Thread.Sleep(50);
                                    Logger.Log(
                                        $"Cannot set template for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                                }
                            }
                        }

                        //var faceZk = FaceTemplateService.FaceTemplates(userId: user.Id, index: 50);
                        if (user.FaceTemplates.Any(template => template.FaceTemplateType.Code == FaceTemplateTypes.ZKVX7Code))
                        {
                            var faceTemplate = user.FaceTemplates.First(template =>
                                template.FaceTemplateType.Code == FaceTemplateTypes.ZKVX7Code);
                            //foreach (var face in user.FaceTemplates.Where(template => template.FaceTemplateType.Code == FaceTemplateTypes.ZKVX7Code))
                            //{
                            for (var i = 0; i < 9; i++)
                            {
                                if (ZkTecoSdk.SetUserFaceStr((int)DeviceInfo.Code, user.Id.ToString(), 50,
                                    Encoding.ASCII.GetString(faceTemplate.Template), faceTemplate.Size))
                                {
                                    //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                                    Logger.Log(
                                        $"Successfully set face template for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                                    break;
                                }

                                ZkTecoSdk.GetLastError(ref errorCode);
                                Thread.Sleep(50);
                                Logger.Log(
                                    $"Cannot set face template for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                            }
                            //}
                        }

                        var userAccessGroups = user.Id == default ? null : _accessGroupService.GetAccessGroups(user.Id)?.Data?.Data;
                        var validAccessGroup =
                            userAccessGroups?.FirstOrDefault(ag =>
                                ag.DeviceGroup.Any(dg => dg.Devices.Any(d => d.DeviceId == DeviceInfo.DeviceId)));
                        if (ZkTecoSdk.SetUserGroup((int)DeviceInfo.Code, (int)user.Id,
                            validAccessGroup?.Id ?? 1))
                        {
                            ZkTecoSdk.RefreshData((int)DeviceInfo.Code);
                            Logger.Log(
                                $"Successfully set access group for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                            //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);
                            return true;
                        }

                        ZkTecoSdk.RefreshData((int)DeviceInfo.Code);
                        ZkTecoSdk.GetLastError(ref errorCode);
                        //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                        Logger.Log($"Cannot set access group for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception, exception.Message);
                    }
                    // _zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                    return true;
                }

                errorCode = 0;
                ZkTecoSdk.GetLastError(ref errorCode);
                //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                Logger.Log($"Cannot add user {user.Id} to device {DeviceInfo.Code}. ErrorCode={errorCode}",
                    logType: LogType.Warning);
                return false;
            }
        }

        public override ResultViewModel ReadOfflineLog(object cancellationToken, bool saveFile = false)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    var iLogCount = 0;
                    Logger.Log($"Retrieving offline logs of DeviceId: {DeviceInfo.Code}.");

                    //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);//disable the device
                    var idwErrorCode = 0;

                    if (!ZkTecoSdk.ReadNewGLogData((int)DeviceInfo.Code))
                    {
                        ZkTecoSdk.GetLastError(ref idwErrorCode);
                        if (idwErrorCode != 0)
                            if (!ZkTecoSdk.ReadGeneralLogData((int)DeviceInfo.Code))
                            {
                                ZkTecoSdk.GetLastError(ref idwErrorCode);
                                //Thread.Sleep(2000);
                                //Connect();
                                Logger.Log(
                                    $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}",
                                    logType: LogType.Warning);
                                return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0", Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                            }
                    }

                    var iWorkCode = 0;
                    var lstLogs = new List<Log>();
                    var recordCnt = 0;
                    while (ZkTecoSdk.SSR_GetGeneralLogData((int)DeviceInfo.Code, out var iUserId,
                        out var iVerifyMethod, out var iInOutMode, out var iYear, out var iMonth, out var iDay, out var iHour, out var iMinute,
                        out var iSecond, ref iWorkCode))
                    {
                        if (iLogCount > 100000)
                            break;

                        iLogCount++; //increase the number of attendance records

                        lock (LockObject) //make the object exclusive 
                        {
                            try
                            {
                                var userId = Convert.ToInt32(iUserId);

                                var log = new Log
                                {
                                    DeviceId = DeviceInfo.DeviceId,
                                    DeviceCode = DeviceInfo.Code,
                                    LogDateTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond),
                                    EventLog = LogEvents.Authorized,
                                    UserId = userId,
                                    InOutMode = DeviceInfo.DeviceTypeId,
                                    MatchingType = EosCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                                    TnaEvent = (ushort)iInOutMode
                                };

                                //_EosLogService.AddLog(log);
                                lstLogs.Add(log);
                                Logger.Log($@"<--
       +TerminalID: {DeviceInfo.Code}
       +UserID: {userId}
       +DateTime: {log.LogDateTime}
       +AuthType: {iVerifyMethod}
       +TnaEvent: {(ushort)iInOutMode}
       +Progress: {iLogCount}/{recordCnt}");
                            }
                            catch (Exception)
                            {
                                Logger.Log($"User id of log is not in a correct format. UserId : {iUserId}", logType: LogType.Warning);
                            }
                        }
                    }

                    Task.Run(() =>
                    {
                        _logService.AddLog(lstLogs);

                        //if (!saveFile) return;
                        //EosLogService.SaveLogsInFile(lstLogs, "ZK", DeviceInfo.Code);
                    }, TokenSource.Token);

                    Logger.Log($"{iLogCount} Offline log retrieved from DeviceId: {DeviceInfo.Code}.", logType: LogType.Information);


                    //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);//enable the device
                    return new ResultViewModel
                    { Id = DeviceInfo.DeviceId, Validate = 1, Message = iLogCount.ToString(), Code = Convert.ToInt32(TaskStatuses.DoneCode) };
                }
                catch (Exception exception)
                {
                    //Thread.Sleep(2000);
                    //Connect();
                    Logger.Log(exception, exception.Message);
                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0", Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                }
            }
        }

        public override ResultViewModel ReadOfflineLogInPeriod(object cancellationToken, DateTime? startTime, DateTime? endTime, bool saveFile = false)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    var iLogCount = 0;
                    var fDate = startTime?.ToString(CultureInfo.InvariantCulture);
                    var eDate = endTime?.ToString(CultureInfo.InvariantCulture);
                    Logger.Log($"Retrieving offline logs of DeviceId: {DeviceInfo.Code}.");

                    //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);//disable the device
                    //if (_zkTecoSdk.ReadTimeGLogData((int)_deviceInfo.Code, fDate, eDate))
                    if (fDate == default && eDate == default)
                    {
                        if (!ZkTecoSdk.ReadGeneralLogData((int)DeviceInfo.Code))
                        {
                            var idwErrorCode = 0;
                            ZkTecoSdk.GetLastError(ref idwErrorCode);
                            //Thread.Sleep(2000);
                            //Connect();
                            Logger.Log(
                                $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}",
                                logType: LogType.Warning);
                            return new ResultViewModel
                            {
                                Id = DeviceInfo.DeviceId,
                                Validate = 0,
                                Message = "0",
                                Code = Convert.ToInt32(TaskStatuses.FailedCode)
                            };
                        }
                    }

                    else if (!ZkTecoSdk.ReadTimeGLogData((int)DeviceInfo.Code, fDate, eDate))
                        if (!ZkTecoSdk.ReadGeneralLogData((int)DeviceInfo.Code))
                        {
                            var idwErrorCode = 0;
                            ZkTecoSdk.GetLastError(ref idwErrorCode);
                            //Thread.Sleep(2000);
                            //Connect();
                            Logger.Log(
                                $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}", logType: LogType.Warning);
                            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0", Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                        }


                    var iWorkCode = 0;

                    var lstLogs = new List<Log>();
                    var recordsCount = 0;
                    while (ZkTecoSdk.SSR_GetGeneralLogData((int)DeviceInfo.Code, out var iUserId,
                        out var iVerifyMethod, out var iInOutMode, out var iYear, out var iMonth, out var iDay, out var iHour, out var iMinute,
                        out var iSecond, ref iWorkCode))
                    {
                        if (iLogCount > 100000)
                            break;

                        iLogCount++; //increase the number of attendance records
                        lock (LockObject) //make the object exclusive 
                        {
                            try
                            {
                                var userId = Convert.ToInt32(iUserId);

                                var log = new Log
                                {
                                    DeviceId = DeviceInfo.DeviceId,
                                    DeviceCode = DeviceInfo.Code,
                                    LogDateTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond),
                                    EventLog = LogEvents.Authorized,
                                    UserId = userId,
                                    InOutMode = DeviceInfo.DeviceTypeId,
                                    MatchingType = EosCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                                    TnaEvent = (ushort)iInOutMode
                                };

                                //_EosLogService.AddLog(log);
                                lstLogs.Add(log);
                                Logger.Log($@"<--
    +TerminalID:{DeviceInfo.Code}
    +UserID:{userId}
    +DateTime:{log.LogDateTime}
    +AuthType:{iVerifyMethod}
    +TnaEvent:{(ushort)iInOutMode}
    +Progress:{iLogCount}/{recordsCount}");
                            }
                            catch (Exception)
                            {
                                Logger.Log($"User id of log is not in a correct format. UserId : {iUserId}", logType: LogType.Warning);
                            }
                        }
                    }

                    Task.Run(() => { _logService.AddLog(lstLogs); }, TokenSource.Token);

                    Logger.Log($"{iLogCount} Offline log retrieved from DeviceId: {DeviceInfo.Code}.", logType: LogType.Information);

                    //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);//enable the device
                    return new ResultViewModel
                    { Id = DeviceInfo.DeviceId, Validate = 1, Message = iLogCount.ToString(), Code = Convert.ToInt32(TaskStatuses.DoneCode) };

                }
                catch (Exception exception)
                {
                    //Thread.Sleep(2000);
                    //Connect();

                    Logger.Log(exception, exception.Message);
                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0", Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                }
            }
        }

        public bool TransferAccessGroup(AccessGroup accessGroup)
        {
            var errorCode = 0;
            try
            {
                TransferTimeZone(accessGroup.TimeZone);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            lock (ZkTecoSdk)
            {
                //int iGroupNo = Convert.ToInt32(cbGroupNo.Text.Trim());
                var validHoliday = 0;
                var verifyStyle = 0;

                if (ZkTecoSdk.SSR_SetGroupTZ((int)DeviceInfo.Code, accessGroup.Id, accessGroup.TimeZone.Id, 0, 0,
                    validHoliday, verifyStyle))
                {
                    ZkTecoSdk.RefreshData((int)DeviceInfo.Code); //the data in the device should be refreshed
                    Logger.Log($"Group Number: {accessGroup.Id} VerifyStyle: {verifyStyle} Success");
                    return true;
                }

                ZkTecoSdk.GetLastError(ref errorCode);
                Logger.Log($"Operation failed, ErrorCode = {errorCode}, Error");
                return false;
            }
        }

        public bool TransferTimeZone(TimeZone timeZone)
        {
            var errorCode = 0;

            var timeZoneString = "";

            for (var i = 2; i < 8; i++)
            {
                timeZoneString += timeZone.Details.FirstOrDefault(tz => tz.DayNumber == i)?.FromTime.ToString("hhmm");
                timeZoneString += timeZone.Details.FirstOrDefault(tz => tz.DayNumber == i)?.ToTime.ToString("hhmm");
            }

            timeZoneString += timeZone.Details.FirstOrDefault(tz => tz.DayNumber == 1)?.FromTime.ToString("hhmm");
            timeZoneString += timeZone.Details.FirstOrDefault(tz => tz.DayNumber == 1)?.ToTime.ToString("hhmm");

            lock (ZkTecoSdk)
            {
                if (ZkTecoSdk.SetTZInfo((int)DeviceInfo.Code, timeZone.Id, timeZoneString))
                {
                    ZkTecoSdk.RefreshData((int)DeviceInfo.Code); //the data in the device should be refreshed
                    Logger.Log($"SetTZInfo, TimeZone Index: {timeZone.Id} TimeZone: {timeZoneString}, Success", logType: LogType.Information);
                    return true;
                }

                ZkTecoSdk.GetLastError(ref errorCode);
                Logger.Log($"Operation failed, ErrorCode= {errorCode}, Error.", logType: LogType.Warning);
                return false;
            }
        }

        public override bool DeleteUser(uint sUserId)
        {
            try
            {
                Task.Run(() =>
                {
                    lock (ZkTecoSdk)
                    {
                        ZkTecoSdk.SSR_DeleteEnrollData((int)DeviceInfo.Code, sUserId.ToString(), 12);
                        ZkTecoSdk.RefreshData((int)DeviceInfo.Code);
                    }
                }, TokenSource.Token);

                return true;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $" --> Error on deleting user: {sUserId}");
                return false;
            }

        }

        public bool LockDevice()
        {
            try
            {
                Logger.Log($"Locking the Device: {DeviceInfo.Code}.", logType: LogType.Information);
                ZkTecoSdk.EnableDevice((int)DeviceInfo.Code, false); //disable the device

                return true;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $" --> Error On LockDevice : {DeviceInfo.Code}");
                return false;
            }
        }
        public bool UnLockDevice()
        {
            try
            {
                Logger.Log($"UnLocking the Device: {DeviceInfo.Code}.", logType: LogType.Information);
                ZkTecoSdk.EnableDevice((int)DeviceInfo.Code, false);//disable the device
                return true;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $" --> Error On UnLocking : {DeviceInfo.Code}");
                return false;
            }
        }

        public override List<User> GetAllUsers(bool embedTemplate = false)
        {
            lock (ZkTecoSdk)
            {
                Logger.Log("in GetAllUserInfo");
                if (ZkTecoSdk.ReadAllUserID((int)DeviceInfo.Code))
                {
                    var lstUsers = new List<User>();
                    Logger.Log($"Starting Retrieved user");

                    var index = 0;
                    while (ZkTecoSdk.SSR_GetAllUserInfo((int)DeviceInfo.Code, out var iUserId, out var name, out _,
                        out var privilege, out var enable))
                    {
                        lock (LockObject) //make the object exclusive 
                        {
                            try
                            {
                                Logger.Log($"Retrieved user {iUserId}");
                                var user = new User
                                {
                                    Code = Convert.ToInt32(iUserId),
                                    UserName = name,
                                    IsActive = enable,
                                    AdminLevel = privilege,
                                    SurName = name.Split(' ').LastOrDefault(),
                                    FirstName = name.Split(' ').FirstOrDefault(),
                                    StartDate = DateTime.Parse("1970/01/01"),
                                    EndDate = DateTime.Parse("2050/01/01")
                                };

                                if (embedTemplate)
                                {
                                    index++;
                                    Logger.Log($"Retrieving templates of user {iUserId}, index: {index}");

                                    try
                                    {
                                        if (ZkTecoSdk.GetStrCardNumber(out var cardNumber) && cardNumber != "0")
                                        {
                                            user.IdentityCard = new IdentityCard
                                            {
                                                Number = cardNumber,
                                                IsActive = true
                                                //Id = (int)user.Id
                                            };
                                        }

                                        Logger.Log($"Retried user card of user {iUserId}, index: {index}");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Log(e, e.Message);
                                    }


                                    var retrievedFingerTemplates = new List<FingerTemplate>();
                                    var retrievedFaceTemplates = new List<FaceTemplate>();

                                    try
                                    {
                                        for (var i = 0; i <= 9; i++)
                                        {
                                            if (!ZkTecoSdk.SSR_GetUserTmpStr((int)DeviceInfo.Code, user.Code.ToString(), i,
                                                out var tempData, out var tempLength))
                                            {
                                                Thread.Sleep(50);
                                                continue;
                                            }

                                            var fingerTemplate = new FingerTemplate
                                            {
                                                FingerIndex = _biometricTemplateManager.GetFingerIndex(i),
                                                FingerTemplateType = _fingerTemplateTypes.VX10,
                                                UserId = user.Id,
                                                Template = Encoding.ASCII.GetBytes(tempData),
                                                CheckSum = Encoding.ASCII.GetBytes(tempData).Sum(x => x),
                                                Size = tempLength,
                                                Index = i
                                            };

                                            retrievedFingerTemplates.Add(fingerTemplate);
                                        }

                                        user.FingerTemplates = retrievedFingerTemplates;
                                        Logger.Log($"Retrieving finger templates of user {iUserId}, index: {index}");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Log(e, e.Message);
                                    }

                                    try
                                    {
                                        var faceStr = "";
                                        var faceLen = 0;
                                        for (var i = 0; i < 9; i++)
                                        {
                                            if (!ZkTecoSdk.GetUserFaceStr((int)DeviceInfo.Code, user.Code.ToString(), 50,
                                                ref faceStr, ref faceLen))
                                            {
                                                Thread.Sleep(50);
                                                continue;
                                            }

                                            var faceTemplate = new FaceTemplate
                                            {
                                                Index = 50,
                                                FaceTemplateType = _faceTemplateTypes.ZKVX7,
                                                UserId = user.Id,
                                                Template = Encoding.ASCII.GetBytes(faceStr),
                                                CheckSum = Encoding.ASCII.GetBytes(faceStr).Sum(x => x),
                                                Size = faceLen,
                                            };

                                            retrievedFaceTemplates.Add(faceTemplate);
                                            Logger.Log($"Retrieving face templates of user {iUserId}, index: {index}");
                                            break;
                                        }

                                        user.FaceTemplates = retrievedFaceTemplates;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Log(e, e.Message);
                                    }
                                }
                                //_EosLogService.AddLog(log);
                                lstUsers.Add(user);
                                Logger.Log($"Added user {iUserId}");
                            }
                            catch (Exception)
                            {
                                Logger.Log($"User id of log is not in a correct format. UserId : {iUserId}", logType: LogType.Warning);
                            }
                        }
                    }

                    return lstUsers;
                }

                var error = 0;
                ZkTecoSdk.GetLastError(ref error);
                Logger.Log($"Cannot Get Users Of Device {DeviceInfo.Code}, ErrorCode : {error}", logType: LogType.Warning);
                return new List<User>();
            }
        }

        internal override User GetUser(uint userId)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    Logger.Log("<--EventGetUserData");

                    if (ZkTecoSdk.SSR_GetUserInfo((int)DeviceInfo.Code, userId.ToString(), out var name,
                        out var password, out var privilege, out var enabled))
                    {
                        var user = new User
                        {
                            Code = userId,
                            AdminLevel = privilege,
                            IsActive = enabled,
                            SurName = name.Split(' ').LastOrDefault(),
                            FirstName = name.Split(' ').FirstOrDefault(),
                            StartDate = DateTime.Parse("1970/01/01"),
                            EndDate = DateTime.Parse("2050/01/01"),
                            Password = password,
                            UserName = name,
                        };
                        var existUser = UserService.GetUsers(code: userId)?.Data?.Data?.FirstOrDefault();
                        if (existUser != null)
                        {
                            user = new User
                            {
                                Id = existUser.Id,
                                Code = userId,
                                AdminLevel = privilege,
                                IsActive = existUser.IsActive,
                                SurName = existUser.SurName,
                                FirstName = existUser.FirstName,
                                Email = existUser.Email,
                                EntityId = existUser.EntityId,
                                TelNumber = existUser.TelNumber,
                                UserName = name,
                                StartDate = DateTime.Parse("1970/01/01"),
                                EndDate = DateTime.Parse("2050/01/01"),
                                Password = password,
                                IsAdmin = existUser.IsAdmin,
                                Type = existUser.Type
                            };
                        }

                        //UserService.ModifyUser(user);
                        Logger.Log("<--User is Modified");

                        user.FingerTemplates = new List<FingerTemplate>();
                        user.FaceTemplates = new List<FaceTemplate>();

                        try
                        {
                            if (ZkTecoSdk.GetStrCardNumber(out var cardNumber) && cardNumber != "0")
                            {
                                var card = new IdentityCard
                                {
                                    Number = cardNumber,
                                    IsActive = true,
                                    DataCheck = 0
                                };

                                user.IdentityCard = card;
                                //UserCardService.ModifyUserCard(card);
                                Logger.Log("<--User card is Modified");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Log(e);
                            //ignore
                        }

                        var retrievedFingerTemplates = new List<FingerTemplate>();
                        var retrievedFaceTemplates = new List<FaceTemplate>();

                        try
                        {
                            for (var i = 0; i <= 9; i++)
                            {
                                if (!ZkTecoSdk.SSR_GetUserTmpStr((int)DeviceInfo.Code, user.Code.ToString(), i,
                                    out var tempData, out var tempLength))
                                {
                                    Thread.Sleep(50);
                                    continue;
                                }
                                var fingerTemplate = new FingerTemplate
                                {
                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(i),
                                    FingerTemplateType = _fingerTemplateTypes.VX10,
                                    UserId = user.Id,
                                    Template = Encoding.ASCII.GetBytes(tempData),
                                    CheckSum = Encoding.ASCII.GetBytes(tempData).Sum(x => x),
                                    Size = tempLength,
                                    Index = i
                                };

                                retrievedFingerTemplates.Add(fingerTemplate);

                                if (existUser != null)
                                {
                                    if (!existUser.FingerTemplates?.Any(fp =>
                                        fp.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(i).Code && fp.FingerTemplateType.Code == FingerTemplateTypes.VX10Code) ?? false)
                                    {
                                        user.FingerTemplates.Add(fingerTemplate);
                                        Logger.Log($"A finger print with index: {i} is retrieved for user: {user.Code}");
                                    }
                                    else
                                    {
                                        Logger.Log($"The User: {user.Id} has a finger print with index: {i}");
                                    }
                                }
                                else
                                {
                                    user.FingerTemplates.Add(fingerTemplate);
                                    Logger.Log($"A finger print with index: {i} is retrieved for user: {user.Code}");
                                }
                            }

                            //if (user.FingerTemplates.Count > 0)
                            //{
                            //    foreach (var fingerTemplate in user.FingerTemplates)
                            //    {
                            //        FingerTemplateService.ModifyFingerTemplate(fingerTemplate);
                            //    }

                            //    Logger.Log("<-- Finger Template is modified");
                            //}
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, $"Error in getting finger template from device: {DeviceInfo.Code}");
                        }

                        try
                        {
                            var faceStr = "";
                            var faceLen = 0;
                            for (var i = 0; i < 9; i++)
                            {
                                if (!ZkTecoSdk.GetUserFaceStr((int)DeviceInfo.Code, userId.ToString(), 50,
                                    ref faceStr, ref faceLen))
                                {
                                    Thread.Sleep(50);
                                    continue;
                                }
                                var faceTemplate = new FaceTemplate
                                {
                                    Index = 50,
                                    FaceTemplateType = _faceTemplateTypes.ZKVX7,
                                    UserId = user.Id,
                                    Template = Encoding.ASCII.GetBytes(faceStr),
                                    CheckSum = Encoding.ASCII.GetBytes(faceStr).Sum(x => x),
                                    Size = faceLen,
                                };

                                retrievedFaceTemplates.Add(faceTemplate);

                                if (existUser != null)
                                {
                                    if (existUser.FaceTemplates.Any(fp => fp.Index == 50)) break;
                                    user.FaceTemplates.Add(faceTemplate);
                                    break;
                                }

                                user.FaceTemplates.Add(faceTemplate);
                                break;
                            }

                            //if (user.FaceTemplates.Count > 0)
                            //{
                            //    foreach (var faceTemplates in user.FaceTemplates)
                            //    {
                            //        FaceTemplateService.ModifyFaceTemplate(faceTemplates);
                            //    }

                            //    Logger.Log("<-- face Template is modified");
                            //}
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, $"Error in getting face template from device: {DeviceInfo.Code}");
                        }

                        Logger.Log($@" The user: {userId} is retrieved from device:{DeviceInfo.Code}
    Info: Finger retrieved count: {retrievedFingerTemplates.Count}, inserted count: {user.FingerTemplates.Count}, 
          Face retrieved count: {retrievedFaceTemplates.Count}, inserted count: {user.FaceTemplates.Count}");

                        return user;
                    }


                    return new User();
                }
                catch (Exception e)
                {
                    Logger.Log($" --> Error On GetUserData {e.Message}", logType: LogType.Warning);
                    return new User();
                }
            }
        }
        public Dictionary<string, string> GetAdditionalData(int code)
        {
            lock (ZkTecoSdk)
            {
                var dic = new Dictionary<string, string>();
                var adminCnt = 0;
                var userCount = 0;
                var fpCnt = 0;
                var recordCnt = 0;
                var pwdCnt = 0;
                var opLogCnt = 0;
                var faceCnt = 0;
                ZkTecoSdk.EnableDevice(code, false); //disable the device

                ZkTecoSdk.GetDeviceStatus(code, 2, ref userCount);
                ZkTecoSdk.GetDeviceStatus(code, 1, ref adminCnt);
                ZkTecoSdk.GetDeviceStatus(code, 3, ref fpCnt);
                ZkTecoSdk.GetDeviceStatus(code, 4, ref pwdCnt);
                ZkTecoSdk.GetDeviceStatus(code, 5, ref opLogCnt);
                ZkTecoSdk.GetDeviceStatus(code, 6, ref recordCnt);
                ZkTecoSdk.GetDeviceStatus(code, 21, ref faceCnt);
                dic.Add("UserCount", userCount.ToString());
                dic.Add("AdminCount", adminCnt.ToString());
                dic.Add("FPCount", fpCnt.ToString());
                dic.Add("PasswordCount", pwdCnt.ToString());
                dic.Add("OpLogCount", opLogCnt.ToString());
                dic.Add("RecordCount", recordCnt.ToString());
                dic.Add("FaceCount", faceCnt.ToString());

                var sFirmwareVersion = "";
                var sMac = "";
                var sPlatform = "";
                var sProducer = "";

                ZkTecoSdk.GetSysOption(code, "~ZKFPVersion", out var iFpAlg);
                ZkTecoSdk.GetSysOption(code, "ZKFaceVersion", out var iFaceAlg);
                ZkTecoSdk.GetVendor(ref sProducer);
                ZkTecoSdk.GetProductCode(code, out var sDeviceName);
                ZkTecoSdk.GetDeviceMAC(code, ref sMac);
                ZkTecoSdk.GetFirmwareVersion(code, ref sFirmwareVersion);
                ZkTecoSdk.GetPlatform(code, ref sPlatform);
                ZkTecoSdk.GetSerialNumber(code, out var sn);
                ZkTecoSdk.GetDeviceStrInfo(code, 1, out var sProductTime);

                ZkTecoSdk.EnableDevice(code, true); //enable the device
                dic.Add("FPAlg", iFpAlg);
                dic.Add("FaceAlg", iFaceAlg);
                dic.Add("Producer", sProducer);
                dic.Add("DeviceName", sDeviceName);
                dic.Add("Mac", sMac);
                dic.Add("Firmware", sFirmwareVersion);
                dic.Add("Platform", sPlatform);
                dic.Add("SN", sn);
                dic.Add("ProductTime", sProductTime);
                return dic;
            }
        }
        public bool ClearLog(int code, string fDate, string eDate)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    ZkTecoSdk.EnableDevice(code, false); //disable the device

                    if (string.IsNullOrEmpty(fDate) && string.IsNullOrEmpty(eDate))
                    {
                        if (!ZkTecoSdk.ClearGLog(code)) return false;
                    }
                    else
                    {
                        if (!ZkTecoSdk.DeleteAttlogBetweenTheDate(code, fDate, eDate)) return false;
                    }

                    ZkTecoSdk.RefreshData(code);
                    return true;
                }
                catch (Exception)
                {
                    Logger.Log($"Error in Clear Log device code: {code}", logType: LogType.Warning);
                    return false;
                }
            }
        }
    }
}