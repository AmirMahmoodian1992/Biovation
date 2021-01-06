using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using zkemkeeper;
using Log = Biovation.Domain.Log;
using TimeZone = Biovation.Domain.TimeZone;
//using TimeZone = Biovation.CommonClasses.Models.TimeZone;
// ReSharper disable InconsistentlySynchronizedField


namespace Biovation.Brands.ZK.Devices
{
    public class Device : IDevices
    {
        private readonly ILogger _logger;
        protected readonly DeviceBasicInfo DeviceInfo;
        private readonly LogService _logService;
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        protected readonly LogService LogService;
        private readonly AccessGroupService _accessGroupService;
        protected readonly UserService UserService;
        protected readonly UserCardService UserCardService;
        protected readonly FingerTemplateService FingerTemplateService;
        protected readonly FaceTemplateService FaceTemplateService;
        private readonly Dictionary<uint, Device> _onlineDevices;
        //private readonly CommunicationManager<ResultViewModel> _communicationManager = new CommunicationManager<ResultViewModel>();
        private readonly RestClient _restClient;
        protected CancellationTokenSource TokenSource = new CancellationTokenSource();

        protected readonly CZKEMClass ZkTecoSdk = new CZKEMClass(); //create Standalone _zkTecoSdk class dynamically
        private bool _reconnecting;

        private readonly bool _isGetLogEnable;



        private readonly LogEvents _logEvents;
        private readonly ZkCodeMappings _zkCodeMappings;

        private readonly TaskManager _taskManager;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly DeviceBrands _deviceBrands;
        private readonly MatchingTypes _matchingTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FaceTemplateTypes _faceTemplateTypes;


        protected static readonly object LockObject = new object();
        internal Device(DeviceBasicInfo info, TaskService taskService, UserService userService, DeviceService deviceService, LogService logService, AccessGroupService accessGroupService, FingerTemplateService fingerTemplateService, UserCardService userCardService, FaceTemplateService faceTemplateService, RestClient restClient, Dictionary<uint, Device> onlineDevices, BiovationConfigurationManager biovationConfigurationManager, LogEvents logEvents, ZkCodeMappings zkCodeMappings, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands, TaskManager taskManager, MatchingTypes matchingTypes, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, FaceTemplateTypes faceTemplateTypes, ILogger logger)
        {
            DeviceInfo = info;
            _taskService = taskService;
            UserService = userService;
            _userService = userService;
            _deviceService = deviceService;
            _logService = logService;
            LogService = logService;
            _accessGroupService = accessGroupService;
            FingerTemplateService = fingerTemplateService;
            UserCardService = userCardService;
            FaceTemplateService = faceTemplateService;
            _restClient = restClient;
            _onlineDevices = onlineDevices;
            _logEvents = logEvents;
            _zkCodeMappings = zkCodeMappings;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _deviceBrands = deviceBrands;
            _taskManager = taskManager;
            _matchingTypes = matchingTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _fingerTemplateTypes = fingerTemplateTypes;
            _faceTemplateTypes = faceTemplateTypes;
            _isGetLogEnable = biovationConfigurationManager.GetAllLogWhenConnect;

            _logger = logger.ForContext<Device>();
        }

        public DeviceBasicInfo GetDeviceInfo()
        {
            return DeviceInfo;
        }

        public bool Connect()
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
                    _logger.Debug(
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
                    _logger.Warning(exception, exception.Message);
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
                                _logger.Debug($"Device {DeviceInfo.Code} time has been set to: {DateTime.Now:u}");
                            else
                            {
                                result = ZkTecoSdk.SetDeviceTime((int)DeviceInfo.Code);
                                _logger.Debug(result
                                    ? $"Device {DeviceInfo.Code} time has been set to server time: {DateTime.Now:u}"
                                    : $"Could not set time for device {DeviceInfo.Code}");
                            }
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            ZkTecoSdk.SetDeviceTime((int)DeviceInfo.Code);
                            _logger.Debug($"Device {DeviceInfo.Code} time has been set to server time: {DateTime.Now:u}");
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
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

            LogService.AddLog(new Log
            {
                DeviceId = DeviceInfo.DeviceId,
                LogDateTime = DateTime.Now,
                EventLog = _logEvents.Connect

            });

            if (_isGetLogEnable)
            {
                //Task.Run(() => { ReadOfflineLog(Token); }, Token);
                //ZKTecoServer.LogReaderQueue.Enqueue(new Task(() => ReadOfflineLog(TokenSource.Token), TokenSource.Token));
                //ZKTecoServer.StartReadLogs();
                var creatorUser = _userService.GetUsers(code: 123456789).FirstOrDefault();
                var lastLogsOfDevice = _logService.GetLastLogsOfDevice((uint)DeviceInfo.DeviceId).Result;

                if (lastLogsOfDevice != null)
                {
                    foreach (var log in lastLogsOfDevice)
                        _logger.Information(
                            "Last logs of device {deviceId}: LogTime: {logDateTime}, eventId: {eventId}",
                            DeviceInfo.Code, log.LogDateTime, log.EventLog.Code);

                    var lastLogOfDevice = lastLogsOfDevice.LastOrDefault();
                    if (lastLogOfDevice != null)
                    {
                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.GetLogsInPeriod,
                            Priority = _taskPriorities.Medium,
                            TaskItems = new List<TaskItem>(),
                            DeviceBrand = _deviceBrands.ZkTeco,
                            DueDate = DateTimeOffset.Now
                        };

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.GetLogsInPeriod,
                            Priority = _taskPriorities.Medium,
                            DeviceId = DeviceInfo.DeviceId,
                            Data = JsonConvert.SerializeObject(new { fromDate = lastLogOfDevice.LogDateTime, toDate = DateTime.Now.AddHours(1) }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });

                        _taskService.InsertTask(task);
                    }
                }
                else
                {
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.GetLogs,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = _deviceBrands.ZkTeco,
                        DueDate = DateTimeOffset.Now
                    };

                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.GetLogs,
                        Priority = _taskPriorities.Medium,
                        DeviceId = DeviceInfo.DeviceId,
                        Data = JsonConvert.SerializeObject(DeviceInfo.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });

                    _taskService.InsertTask(task);
                }


                _taskManager.ProcessQueue();
            }

            Task.Run(CheckConnection, TokenSource.Token);
            _logger.Debug($"Successfully connected to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}:{DeviceInfo.Port}");

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

            _logger.Debug($"Connection lost on device {DeviceInfo.Code}");
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
                var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            }
            catch (Exception)
            {
                //ignore
            }

            LogService.AddLog(new Log
            {
                DeviceId = DeviceInfo.DeviceId,
                LogDateTime = DateTime.Now,
                EventLog = _logEvents.Disconnect,
                SuccessTransfer = true,
                MatchingType = _matchingTypes.Finger
            });

            _logger.Information($"Disconnected from device: {DeviceInfo.Code} IPAddress => {DeviceInfo.IpAddress}:{DeviceInfo.Port}");
            return true;
        }
        //If your fingerprint(or your card) passes the verification,this event will be triggered
        private void OnAttendanceTransactionCallback(int iUserId, int iIsInValid, int iInOutMode, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond)
        {
            _logger.Debug($@"RTEvent OnAttTransaction Has been Triggered,Verified OK
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
                EventLog = _logEvents.Authorized,
                UserId = iUserId,
                MatchingType = _zkCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                SubEvent = _zkCodeMappings.GetLogSubEventGenericLookup(iInOutMode),
                InOutMode = DeviceInfo.DeviceTypeId,
                TnaEvent = (ushort)iInOutMode
            };

            LogService.AddLog(log);
        }
        //If your fingerprint(or your card) passes the verification,this event will be triggered

        private void OnAttendanceTransactionExCallback(string iUserId, int iIsInValid, int iInOutMode, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            //WorkCode : the difference between the event OnAttTransaction and OnAttTransactionEx
            _logger.Debug($@"RTEvent OnAttTransactionEx Has been Triggered,Verified OK
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
                    EventLog = _logEvents.Authorized,
                    UserId = userId,
                    MatchingType = _zkCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                    SubEvent = _zkCodeMappings.GetLogSubEventGenericLookup(iInOutMode),
                    InOutMode = DeviceInfo.DeviceTypeId,
                    TnaEvent = (ushort)iInOutMode
                };

                LogService.AddLog(log);
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, exception.Message);
            }

        }
        //When you press the keypad,this event will be triggered.
        private void OnKeyPressCallback(int iKey)
        {
            _logger.Debug("RTEvent OnKeyPress Has been Triggered, Key: " + iKey);
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
                var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                LogService.AddLog(new Log
                {
                    DeviceId = DeviceInfo.DeviceId,
                    LogDateTime = DateTime.Now,
                    EventLog = _logEvents.Disconnect,
                    SuccessTransfer = true,
                    MatchingType = _matchingTypes.Finger
                });
            }
            catch (Exception)
            {
                //ignore
            }

            _reconnecting = true;
            _logger.Information($"Device {DeviceInfo.Code} disconnected.");
        }

        public virtual bool TransferUser(User user)
        {
            lock (ZkTecoSdk)
            {
                var errorCode = 0;
                // _zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);
                //var card = UserCardService.GetCardsByFilter(user.Id).FirstOrDefault(c => c.IsActive);
                if (user.IdentityCard != null && user.IdentityCard.IsActive)
                {
                    if (ZkTecoSdk.SetStrCardNumber(user.IdentityCard.Number))
                        _logger.Information($"Successfully set card for UserId {user.Code} in DeviceId {DeviceInfo.Code}.");
                    else
                    {
                        ZkTecoSdk.GetLastError(ref errorCode);
                        _logger.Warning($"Cannot set card for UserId {user.Code} in DeviceId {DeviceInfo.Code}.");
                    }
                }

                var name = user.FirstName + " " + user.SurName;
                if (ZkTecoSdk.SSR_SetUserInfo((int)DeviceInfo.Code, user.Code.ToString(), name.Trim(), user.Password,
                    user.IsAdmin ? 3 : 0, true))
                {
                    _logger.Information($"UserId {user.Code} successfully added to DeviceId {DeviceInfo.Code}.");
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
                                    if (ZkTecoSdk.SetUserTmpExStr((int)DeviceInfo.Code, user.Code.ToString(), finger.Index,
                                        1,
                                        Encoding.ASCII.GetString(finger.Template)))
                                    {
                                        //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                                        _logger.Information(
                                            $"Successfully set template for UserId {user.Code} in DeviceId {DeviceInfo.Code}.");
                                        break;
                                    }

                                    ZkTecoSdk.GetLastError(ref errorCode);
                                    Thread.Sleep(50);
                                    _logger.Warning(
                                        $"Cannot set template for UserId {user.Code} in DeviceId {DeviceInfo.Code}.");
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
                                if (ZkTecoSdk.SetUserFaceStr((int)DeviceInfo.Code, user.Code.ToString(), 50,
                                    Encoding.ASCII.GetString(faceTemplate.Template), faceTemplate.Size))
                                {
                                    //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                                    _logger.Information(
                                        $"Successfully set face template for UserId {user.Code} in DeviceId {DeviceInfo.Code}.");
                                    break;
                                }

                                ZkTecoSdk.GetLastError(ref errorCode);
                                Thread.Sleep(50);
                                _logger.Warning(
                                    $"Cannot set face template for UserId {user.Code} in DeviceId {DeviceInfo.Code}.");
                            }
                            //}
                        }

                        var userAccessGroups = user.Id == default ? null : _accessGroupService.GetAccessGroups(user.Id);
                        var validAccessGroup =
                            userAccessGroups?.FirstOrDefault(ag =>
                                ag.DeviceGroup.Any(dg => dg.Devices.Any(d => d.DeviceId == DeviceInfo.DeviceId)));
                        if (ZkTecoSdk.SetUserGroup((int)DeviceInfo.Code, (int)user.Code,
                            validAccessGroup?.Id ?? 1))
                        {
                            ZkTecoSdk.RefreshData((int)DeviceInfo.Code);
                            _logger.Information(
                                $"Successfully set access group for UserId {user.Code} in DeviceId {DeviceInfo.Code}.");
                            //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);
                            return true;
                        }

                        ZkTecoSdk.RefreshData((int)DeviceInfo.Code);
                        ZkTecoSdk.GetLastError(ref errorCode);
                        //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                        _logger.Warning($"Cannot set access group for UserId {user.Code} in DeviceId {DeviceInfo.Code}.");
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                    }
                    // _zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                    return true;
                }

                errorCode = 0;
                ZkTecoSdk.GetLastError(ref errorCode);
                //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                _logger.Warning($"Cannot add user {user.Code} to device {DeviceInfo.Code}. ErrorCode={errorCode}");
                return false;
            }
        }

        public virtual ResultViewModel ReadOfflineLog(object cancelationToken, bool saveFile = false)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    var iLogCount = 0;
                    _logger.Debug($"Retrieving offline logs of DeviceId: {DeviceInfo.Code}.");

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
                                _logger.Warning(
                                    $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}");
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
                                    EventLog = _logEvents.Authorized,
                                    UserId = userId,
                                    MatchingType = _zkCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                                    SubEvent = _zkCodeMappings.GetLogSubEventGenericLookup(iInOutMode),
                                    InOutMode = DeviceInfo.DeviceTypeId,
                                    TnaEvent = (ushort)iInOutMode
                                };

                                //_zkLogService.AddLog(log);
                                lstLogs.Add(log);
                                _logger.Debug($@"<--
       +TerminalID: {DeviceInfo.Code}
       +UserID: {userId}
       +DateTime: {log.LogDateTime}
       +AuthType: {iVerifyMethod}
       +TnaEvent: {(ushort)iInOutMode}
       +Progress: {iLogCount}/{recordCnt}");
                            }
                            catch (Exception)
                            {
                                _logger.Warning($"User id of log is not in a correct format. UserId : {iUserId}");
                            }
                        }
                    }

                    try
                    {
                        LogService.AddLog(lstLogs).Wait();
                        if (saveFile) LogService.SaveLogsInFile(lstLogs, "ZK", DeviceInfo.Code);
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                        return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = exception.Message, Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                    }

                    _logger.Information($"{iLogCount} Offline log retrieved from DeviceId: {DeviceInfo.Code}.");


                    //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);//enable the device
                    return new ResultViewModel
                    { Id = DeviceInfo.DeviceId, Validate = 1, Message = iLogCount.ToString(), Code = Convert.ToInt32(TaskStatuses.DoneCode) };
                }
                catch (Exception exception)
                {
                    //Thread.Sleep(2000);
                    //Connect();
                    _logger.Warning(exception, exception.Message);
                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0", Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                }
            }
        }

        public ResultViewModel ReadOfflineLogInPeriod(object cancelationToken, string fDate, string eDate, bool saveFile = false)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    var iLogCount = 0;
                    _logger.Debug($"Retrieving offline logs of DeviceId: {DeviceInfo.Code}.");

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
                            _logger.Warning(
                                $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}");
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
                            _logger.Information(
                                $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}");
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
                                    EventLog = _logEvents.Authorized,
                                    UserId = userId,
                                    MatchingType = _zkCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                                    SubEvent = _zkCodeMappings.GetLogSubEventGenericLookup(iInOutMode),
                                    InOutMode = DeviceInfo.DeviceTypeId,
                                    TnaEvent = (ushort)iInOutMode
                                };

                                //_zkLogService.AddLog(log);
                                lstLogs.Add(log);
                                _logger.Debug($@"<--
    +TerminalID:{DeviceInfo.Code}
    +UserID:{userId}
    +DateTime:{log.LogDateTime}
    +AuthType:{iVerifyMethod}
    +TnaEvent:{(ushort)iInOutMode}
    +Progress:{iLogCount}/{recordsCount}");
                            }
                            catch (Exception)
                            {
                                _logger.Warning($"User id of log is not in a correct format. UserId : {iUserId}");
                            }
                        }
                    }

                    try
                    {
                        LogService.AddLog(lstLogs).Wait();

                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                        return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0", Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                    }

                    _logger.Information($"{iLogCount} Offline log retrieved from DeviceId: {DeviceInfo.Code}.");

                    //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);//enable the device
                    return new ResultViewModel
                    { Id = DeviceInfo.DeviceId, Validate = 1, Message = iLogCount.ToString(), Code = Convert.ToInt32(TaskStatuses.DoneCode) };

                }
                catch (Exception exception)
                {
                    //Thread.Sleep(2000);
                    //Connect();

                    _logger.Warning(exception, exception.Message);
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
                _logger.Warning(exception, exception.Message);
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
                    _logger.Debug($"Group Number: {accessGroup.Id} VerifyStyle: {verifyStyle} Success");
                    return true;
                }

                ZkTecoSdk.GetLastError(ref errorCode);
                _logger.Debug($"Operation failed, ErrorCode = {errorCode}, Error");
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
                    _logger.Information($"SetTZInfo, TimeZone Index: {timeZone.Id} TimeZone: {timeZoneString}, Success");
                    return true;
                }

                ZkTecoSdk.GetLastError(ref errorCode);
                _logger.Warning($"Operation failed, ErrorCode= {errorCode}, Error.");
                return false;
            }
        }

        public int AddDeviceToDataBase()
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(uint sUserId)
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
                _logger.Warning(exception, $" --> Error on deleting user: {sUserId}");
                return false;
            }

        }

        public bool LockDevice()
        {
            try
            {
                _logger.Information($"Locking the Device: {DeviceInfo.Code}.");
                ZkTecoSdk.EnableDevice((int)DeviceInfo.Code, false); //disable the device

                return true;
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, $" --> Error On LockDevice : {DeviceInfo.Code}");
                return false;
            }
        }
        public bool UnLockDevice()
        {
            try
            {
                _logger.Information($"UnLocking the Device: {DeviceInfo.Code}.");
                ZkTecoSdk.EnableDevice((int)DeviceInfo.Code, false);//disable the device
                return true;
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, $" --> Error On UnLocking : {DeviceInfo.Code}");
                return false;
            }
        }

        public List<User> GetAllUserInfo(bool template = false)
        {
            lock (ZkTecoSdk)
            {
                if (ZkTecoSdk.ReadAllUserID((int)DeviceInfo.Code))
                {
                    var lstUsers = new List<User>();

                    var index = 0;

                    while (ZkTecoSdk.SSR_GetAllUserInfo((int)DeviceInfo.Code, out var iUserId, out var name, out var password,
                            out var privilege, out var enable))
                    {
                        lock (LockObject) //make the object exclusive 
                        {
                            try
                            {
                                _logger.Debug($"Retrieved user {iUserId}, index: {index}");

                                var user = new User
                                {
                                    Code = Convert.ToInt32(iUserId),
                                    UserName = name,
                                    IsActive = enable,
                                    AdminLevel = privilege,
                                    Password = password,
                                    SurName = name.Split(' ').LastOrDefault(),
                                    FirstName = name.Split(' ').FirstOrDefault(),
                                    StartDate = DateTime.Parse("1970/01/01"),
                                    EndDate = DateTime.Parse("2050/01/01")
                                };

                                if (template)
                                {
                                    index++;
                                    _logger.Debug($"Retrieving templates of user {iUserId}, index: {index}");

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

                                        _logger.Debug($"Retried user card of user {iUserId}, index: {index}");
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.Warning(e, e.Message);
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
                                        _logger.Debug($"Retrieving finger templates of user {iUserId}, index: {index}");
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.Warning(e, e.Message);
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
                                            _logger.Debug($"Retrieving face templates of user {iUserId}, index: {index}");
                                            break;
                                        }

                                        user.FaceTemplates = retrievedFaceTemplates;
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.Warning(e, e.Message);
                                    }
                                }

                                //_zkLogService.AddLog(log);
                                lstUsers.Add(user);
                            }
                            catch (Exception)
                            {
                                _logger.Warning($"User id of log is not in a correct format. UserId : {iUserId}");
                            }
                        }
                    }

                    return lstUsers;
                }

                var error = 0;
                ZkTecoSdk.GetLastError(ref error);
                _logger.Warning($"Cannot Get Users Of Device {DeviceInfo.Code}, ErrorCode : {error}");
                return new List<User>();
            }
        }
        public virtual bool GetAndSaveUser(long userId)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    _logger.Debug("<--EventGetUserData");

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
                        var existUser = UserService.GetUsers(code: userId).FirstOrDefault();
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

                        var userInsertionResult = UserService.ModifyUser(user);
                        if (!userInsertionResult.Success)
                            return false;

                        _logger.Debug("<--User is Modified");
                        user.Id = userInsertionResult.Id;

                        user.FingerTemplates = new List<FingerTemplate>();
                        user.FaceTemplates = new List<FaceTemplate>();

                        try
                        {
                            if (ZkTecoSdk.GetStrCardNumber(out var cardNumber) && cardNumber != "0")
                            {
                                var card = new UserCard
                                {
                                    CardNum = cardNumber,
                                    IsActive = true,
                                    UserId = user.Id
                                };

                                UserCardService.ModifyUserCard(card);
                                _logger.Debug("<--User card is Modified");
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Warning(e, e.Message);
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
                                        _logger.Debug($"A finger print with index: {i} is retrieved for user: {user.Code}");
                                    }
                                    else
                                    {
                                        _logger.Debug($"The User: {user.Code} has a finger print with index: {i}");
                                    }
                                }
                                else
                                {
                                    user.FingerTemplates.Add(fingerTemplate);
                                    _logger.Debug($"A finger print with index: {i} is retrieved for user: {user.Code}");
                                }
                            }

                            if (user.FingerTemplates.Count > 0)
                            {
                                foreach (var fingerTemplate in user.FingerTemplates)
                                {
                                    FingerTemplateService.ModifyFingerTemplate(fingerTemplate);
                                }

                                _logger.Debug("<-- Finger Template is modified");
                            }
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, $"Error in getting finger template from device: {DeviceInfo.Code}");
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

                            if (user.FaceTemplates.Count > 0)
                            {
                                foreach (var faceTemplates in user.FaceTemplates)
                                {
                                    FaceTemplateService.ModifyFaceTemplate(faceTemplates);
                                }

                                _logger.Debug("<-- face Template is modified");
                            }

                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, $"Error in getting face template from device: {DeviceInfo.Code}");
                        }

                        _logger.Debug($@" The user: {userId} is retrieved from device:{DeviceInfo.Code}
    Info: Finger retrieved count: {retrievedFingerTemplates.Count}, inserted count: {user.FingerTemplates.Count}, 
          Face retrieved count: {retrievedFaceTemplates.Count}, inserted count: {user.FaceTemplates.Count}");
                    }


                    return true;
                }
                catch (Exception e)
                {
                    _logger.Warning($" --> Error On GetUserData {e.Message}");
                    return false;
                }
            }
        }

        public virtual User GetUser(long userId)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    _logger.Debug("<--EventGetUserData");

                    if (!ZkTecoSdk.SSR_GetUserInfo((int)DeviceInfo.Code, userId.ToString(), out var name,
                        out var password, out var privilege, out var enabled)) return new User();
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
                        FingerTemplates = new List<FingerTemplate>(),
                        FaceTemplates = new List<FaceTemplate>(),
                    };


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


                            _logger.Debug("<--User card Fetched");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Warning(e, e.Message);
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


                            user.FingerTemplates.Add(fingerTemplate);
                            _logger.Debug($"A finger print with index: {i} is retrieved for user: {user.Code}");
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, $"Error in getting finger template from device: {DeviceInfo.Code}");
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

                            user.FaceTemplates.Add(faceTemplate);
                            break;
                        }

                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, $"Error in getting face template from device: {DeviceInfo.Code}");
                    }

                    _logger.Debug($@" The user: {userId} is retrieved from device:{DeviceInfo.Code}
    Info: Finger retrieved count: {retrievedFingerTemplates.Count}, inserted count: {user.FingerTemplates.Count}, 
          Face retrieved count: {retrievedFaceTemplates.Count}, inserted count: {user.FaceTemplates.Count}");
                    return user;

                }
                catch (Exception e)
                {
                    _logger.Warning($" --> Error On GetUserData {e.Message}");
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
                var dwYear = 0;
                var dwMonth = 0;
                var dwDay = 0;
                var dwHour = 0;
                var dwMinute = 0;
                var dwSecond = 0;
                ZkTecoSdk.EnableDevice(code, false); //disable the device

                ZkTecoSdk.GetDeviceStatus(code, 2, ref userCount);
                ZkTecoSdk.GetDeviceStatus(code, 1, ref adminCnt);
                ZkTecoSdk.GetDeviceStatus(code, 3, ref fpCnt);
                ZkTecoSdk.GetDeviceStatus(code, 4, ref pwdCnt);
                ZkTecoSdk.GetDeviceStatus(code, 5, ref opLogCnt);
                ZkTecoSdk.GetDeviceStatus(code, 6, ref recordCnt);
                ZkTecoSdk.GetDeviceStatus(code, 21, ref faceCnt);
                ZkTecoSdk.GetDeviceTime(code, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond);
                var deviceDateTime = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond);
                dic.Add("UserCount", userCount.ToString());
                dic.Add("AdminCount", adminCnt.ToString());
                dic.Add("FPCount", fpCnt.ToString());
                dic.Add("PasswordCount", pwdCnt.ToString());
                dic.Add("OpLogCount", opLogCnt.ToString());
                dic.Add("RecordCount", recordCnt.ToString());
                dic.Add("FaceCount", faceCnt.ToString());
                dic.Add("Date", deviceDateTime.Date.ToString(CultureInfo.InvariantCulture));
                dic.Add("Time", deviceDateTime.TimeOfDay.ToString());

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
                    _logger.Warning($"Error in Clear Log device code: {code}");
                    return false;
                }
            }
        }

        public ResultViewModel DownloadUserPhotos()
        {
            try
            {
                _logger.Debug($" Downloading user photos of device {DeviceInfo.Code}");
                var outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "ZkDeviceUserPhotos",
                    DeviceInfo.Code.ToString());

                _logger.Debug($" Downloading user photos of device {DeviceInfo.Code} in path {outputFolder}");

                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                if (ZkTecoSdk.GetAllUserPhoto((int)DeviceInfo.Code, outputFolder.Trim('/') + '/'))
                    return new ResultViewModel
                    {
                        Success = true,
                        Id = DeviceInfo.Code,
                        Message = $" All user photos of device {DeviceInfo.Code} are downloaded",
                        Code = Convert.ToInt64(TaskStatuses.DoneCode)
                    };

                return new ResultViewModel
                {
                    Success = false,
                    Id = DeviceInfo.Code,
                    Message = $" Couldn't download user photos of device {DeviceInfo.Code}",
                    Code = Convert.ToInt64(TaskStatuses.FailedCode)
                };
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, exception.Message);
                return new ResultViewModel
                {
                    Success = false,
                    Id = DeviceInfo.Code,
                    Message = $" Couldn't download user photos of device {DeviceInfo.Code}",
                    Code = Convert.ToInt64(TaskStatuses.FailedCode)
                };
            }
        }

        public ResultViewModel UploadUserPhotos(string photosFolderPath = default)
        {
            try
            {
                var outputFolder = string.IsNullOrWhiteSpace(photosFolderPath) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "ZkDeviceUserPhotos",
                    DeviceInfo.Code.ToString()) : photosFolderPath;

                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                if (ZkTecoSdk.UploadUserPhoto((int)DeviceInfo.Code, outputFolder))
                    return new ResultViewModel
                    {
                        Success = true,
                        Id = DeviceInfo.Code,
                        Message = $" All user photos of device {DeviceInfo.Code} are uploaded",
                        Code = Convert.ToInt64(TaskStatuses.DoneCode)
                    };

                return new ResultViewModel
                {
                    Success = false,
                    Id = DeviceInfo.Code,
                    Message = $" Couldn't upload user photos of device {DeviceInfo.Code}",
                    Code = Convert.ToInt64(TaskStatuses.FailedCode)
                };
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, exception.Message);
                return new ResultViewModel
                {
                    Success = false,
                    Id = DeviceInfo.Code,
                    Message = $" Couldn't upload user photos of device {DeviceInfo.Code}",
                    Code = Convert.ToInt64(TaskStatuses.FailedCode)
                };
            }
        }
    }
}