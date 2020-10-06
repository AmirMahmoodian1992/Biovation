using Biovation.Brands.ZK.Manager;
using Biovation.Brands.ZK.Service;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using RestSharp;
using zkemkeeper;
using TimeZone = Biovation.CommonClasses.Models.TimeZone;
// ReSharper disable InconsistentlySynchronizedField


namespace Biovation.Brands.ZK.Devices
{
    public class Device : IDevices
    {
        protected readonly DeviceBasicInfo DeviceInfo;
        protected readonly ZkLogService ZkLogService;
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

        protected readonly CZKEMClass ZKTecoSdk = new CZKEMClass(); //create Standalone _zkTecoSdk class dynamically
        private bool _reconnecting;

        private readonly bool _isGetLogEnable = ConfigurationManager.GetAllLogWhenConnect;
        protected static readonly object LockObject = new object();
        internal Device(DeviceBasicInfo info, ZkLogService zkLogService, TaskService taskService, UserService userService, DeviceService deviceService, LogService logService, AccessGroupService accessGroupService, FingerTemplateService fingerTemplateService, UserCardService userCardService, FaceTemplateService faceTemplateService, RestClient restClient, Dictionary<uint, Device> onlineDevices)
        {
            DeviceInfo = info;
            ZkLogService = zkLogService;
            _taskService = taskService;
            UserService = userService;
            _userService = userService;
            _deviceService = deviceService;
            LogService = logService;
            _accessGroupService = accessGroupService;
            FingerTemplateService = fingerTemplateService;
            UserCardService = userCardService;
            FaceTemplateService = faceTemplateService;
            _restClient = restClient;
            _onlineDevices = onlineDevices;
        }

        public DeviceBasicInfo GetDeviceInfo()
        {
            return DeviceInfo;
        }

        public bool Connect()
        {
            lock (ZKTecoSdk)
            {
                if (!string.IsNullOrEmpty(DeviceInfo.DeviceLockPassword))
                {
                    ZKTecoSdk.SetCommPassword(Convert.ToInt32(DeviceInfo.DeviceLockPassword));
                }

                var connectResult = ZKTecoSdk.Connect_Net(DeviceInfo.IpAddress, DeviceInfo.Port);
                while (!connectResult)
                {
                    Logger.Log(
                        $"Could not connect to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}:{DeviceInfo.Port}");

                    Thread.Sleep(20000);
                    connectResult = ZKTecoSdk.Connect_Net(DeviceInfo.IpAddress, DeviceInfo.Port);
                }

                Thread.Sleep(500);

                //Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                if (_reconnecting)
                {
                    if (ZKTecoSdk.RegEvent((int)DeviceInfo.Code, 65535))
                    {
                        //_zkTecoSdk.OnFinger -= _zkTecoSdk_OnFinger;
                        //_zkTecoSdk.OnVerify -= _zkTecoSdk_OnVerify;
                        Thread.Sleep(500);
                        ZKTecoSdk.OnAttTransaction -= OnAttendanceTransactionCallback;
                        ZKTecoSdk.OnAttTransactionEx -= OnAttendanceTransactionExCallback;
                        //_zkTecoSdk.OnFingerFeature -= _zkTecoSdk_OnFingerFeature;
                        ZKTecoSdk.OnKeyPress -= OnKeyPressCallback;
                        //_zkTecoSdk.OnEnrollFinger -= _zkTecoSdk_OnEnrollFinger;
                        //_zkTecoSdk.OnDeleteTemplate -= _zkTecoSdk_OnDeleteTemplate;
                        //_zkTecoSdk.OnNewUser -= _zkTecoSdk_OnNewUser;
                        //_zkTecoSdk.OnHIDNum -= _zkTecoSdk_OnHIDNum;
                        //_zkTecoSdk.OnAlarm -= _zkTecoSdk_OnAlarm;
                        //_zkTecoSdk.OnDoor -= _zkTecoSdk_OnDoor;
                        //_zkTecoSdk.OnWriteCard -= _zkTecoSdk_OnWriteCard;
                        //_zkTecoSdk.OnEmptyCard -= _zkTecoSdk_OnEmptyCard;
                        ZKTecoSdk.OnDisConnected -= OnDisconnectedCallback;
                    }
                }

                if (ZKTecoSdk.RegEvent((int)DeviceInfo.Code, 65535))
                {
                    //_zkTecoSdk.OnFinger += _zkTecoSdk_OnFinger;
                    //_zkTecoSdk.OnVerify += _zkTecoSdk_OnVerify;
                    Thread.Sleep(500);
                    ZKTecoSdk.OnAttTransaction += OnAttendanceTransactionCallback;
                    ZKTecoSdk.OnAttTransactionEx += OnAttendanceTransactionExCallback;
                    //_zkTecoSdk.OnFingerFeature += _zkTecoSdk_OnFingerFeature;
                    ZKTecoSdk.OnKeyPress += OnKeyPressCallback;
                    //_zkTecoSdk.OnEnrollFinger += _zkTecoSdk_OnEnrollFinger;
                    //_zkTecoSdk.OnDeleteTemplate += _zkTecoSdk_OnDeleteTemplate;
                    //_zkTecoSdk.OnNewUser += _zkTecoSdk_OnNewUser;
                    //_zkTecoSdk.OnHIDNum += _zkTecoSdk_OnHIDNum;
                    //_zkTecoSdk.OnAlarm += _zkTecoSdk_OnAlarm;
                    //_zkTecoSdk.OnDoor += _zkTecoSdk_OnDoor;
                    //_zkTecoSdk.OnWriteCard += _zkTecoSdk_OnWriteCard;
                    //_zkTecoSdk.OnEmptyCard += _zkTecoSdk_OnEmptyCard;
                    ZKTecoSdk.OnDisConnected += OnDisconnectedCallback;
                }

                try
                {
                    var firmwareVersion = "";
                    var macAddress = "";

                    Thread.Sleep(500);
                    ZKTecoSdk.GetDeviceFirmwareVersion((int)DeviceInfo.Code, ref firmwareVersion);
                    Thread.Sleep(500);
                    ZKTecoSdk.GetDeviceMAC((int)DeviceInfo.Code, ref macAddress);
                    Thread.Sleep(500);
                    ZKTecoSdk.GetSerialNumber((int)DeviceInfo.Code, out var serialNumber);

                    DeviceInfo.FirmwareVersion = firmwareVersion;
                    DeviceInfo.MacAddress = macAddress;
                    DeviceInfo.SerialNumber = serialNumber;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }

                _deviceService.ModifyDeviceBasicInfoByID(DeviceInfo);

                if (DeviceInfo.TimeSync)
                {
                    try
                    {
                        try
                        {
                            var result = ZKTecoSdk.SetDeviceTime2((int)DeviceInfo.Code, DateTime.Now.Year, DateTime.Now.Month,
                                DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            if (result)
                                Logger.Log($"Device {DeviceInfo.Code} time has been set to: {DateTime.Now:u}");
                            else
                            {
                                result = ZKTecoSdk.SetDeviceTime((int)DeviceInfo.Code);
                                Logger.Log(result
                                    ? $"Device {DeviceInfo.Code} time has been set to server time: {DateTime.Now:u}"
                                    : $"Could not set time for device {DeviceInfo.Code}");
                            }
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                            ZKTecoSdk.SetDeviceTime((int)DeviceInfo.Code);
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

            var data = JsonConvert.SerializeObject(connectionStatus);
            data = "jsoninput=" + data;

            try
            {
                _communicationManager.CallRest(
                    "/api/Biovation/DeviceConnectionState/DeviceConnectionState", "SignalR",
                    new List<object> { data });
            }
            catch (Exception)
            {
                //ignore
            }

            LogService.AddLog(new Log
            {
                DeviceId = DeviceInfo.DeviceId,
                LogDateTime = DateTime.Now,
                EventLog = LogEvents.Connect
              
            });

            if (_isGetLogEnable)
            {
                //Task.Run(() => { ReadOfflineLog(Token); }, Token);
                //ZKTecoServer.LogReaderQueue.Enqueue(new Task(() => ReadOfflineLog(TokenSource.Token), TokenSource.Token));
                //ZKTecoServer.StartReadLogs();
                var creatorUser = _userService.GetUser(123456789, false);
                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = TaskTypes.GetLogs,
                    Priority = TaskPriorities.Medium,
                    TaskItems = new List<TaskItem>(),
                    DeviceBrand = DeviceBrands.ZkTeco,
                };

                task.TaskItems.Add(new TaskItem
                {
                    Status = TaskStatuses.Queued,
                    TaskItemType = TaskItemTypes.GetLogs,
                    Priority = TaskPriorities.Medium,
                    DueDate = DateTime.Today,
                    DeviceId = DeviceInfo.DeviceId,
                    Data = JsonConvert.SerializeObject(DeviceInfo.DeviceId),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1
                });
                _taskService.InsertTask(task).Wait();
                ZkTecoServer.ProcessQueue();

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
                lock (ZKTecoSdk)
                    deviceConnectionStatus = ZKTecoSdk.GetDeviceTime((int)DeviceInfo.Code, ref year, ref month,
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
            lock (ZKTecoSdk)
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

                ZKTecoSdk.Disconnect();

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

            var data = JsonConvert.SerializeObject(connectionStatus);

            data = "jsoninput=" + data;

            try
            {
                _communicationManager.CallRest(
                    "/api/Biovation/DeviceConnectionState/DeviceConnectionState", "SignalR",
                    new List<object> { data });
            }
            catch (Exception)
            {
                //ignore
            }

            LogService.AddLog(new Log
            {
                DeviceId = DeviceInfo.DeviceId,
                LogDateTime = DateTime.Now,
                EventLog = LogEvents.Disconnect,
                SuccessTransfer = true,
                MatchingType = MatchingTypes.Finger
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
                MatchingType = ZkCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                    
                TnaEvent = (ushort)iInOutMode,
                SubEvent = LogSubEvents.Normal
            };

            ZkLogService.AddLog(log);
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
                    MatchingType = ZkCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                    TnaEvent = (ushort)iInOutMode,
                    SubEvent = LogSubEvents.Normal
                };

                ZkLogService.AddLog(log);
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

            var data = JsonConvert.SerializeObject(connectionStatus);

            data = "jsoninput=" + data;

            try
            {
                _communicationManager.CallRest(
                    "/api/Biovation/DeviceConnectionState/DeviceConnectionState", "SignalR",
                    new List<object> { data });
                LogService.AddLog(new Log
                {
                    DeviceId = DeviceInfo.DeviceId,
                    LogDateTime = DateTime.Now,
                    EventLog = LogEvents.Disconnect,
                    SuccessTransfer = true,
                    MatchingType = MatchingTypes.Finger
                });
            }
            catch (Exception)
            {
                //ignore
            }

            _reconnecting = true;
            Logger.Log($"Device {DeviceInfo.Code} disconnected.", logType: LogType.Information);
        }

        public bool TransferUser(User user)
        {
            lock (ZKTecoSdk)
            {
                var errorCode = 0;
                // _zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);
                var card = UserCardService.GetActiveUserCard(user.Id).FirstOrDefault(c => c.IsActive);
                if (card != null)
                {
                    if (ZKTecoSdk.SetStrCardNumber(card.CardNum))
                    {
                        //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                        Logger.Log($"Successfully set card for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                    }
                    else
                    {
                        ZKTecoSdk.GetLastError(ref errorCode);
                        Logger.Log($"Cannot set card for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                    }
                }

                var name = user.FirstName + " " + user.SurName;
                if (ZKTecoSdk.SSR_SetUserInfo((int)DeviceInfo.Code, user.Id.ToString(), name.Trim(), user.Password,
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
                                    if (ZKTecoSdk.SetUserTmpExStr((int)DeviceInfo.Code, user.Id.ToString(), finger.Index,
                                        1,
                                        Encoding.ASCII.GetString(finger.Template)))
                                    {
                                        //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                                        Logger.Log(
                                            $"Successfully set template for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                                        break;
                                    }

                                    ZKTecoSdk.GetLastError(ref errorCode);
                                    Thread.Sleep(50);
                                    Logger.Log(
                                        $"Cannot set template for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                                }
                            }
                        }

                        var faceZk = FaceTemplateService.GetFaceTemplateByUserIdAndIndex(user.Id, 50);
                        if (faceZk.Any())
                        {
                            foreach (var face in faceZk)
                            {
                                for (var i = 0; i < 9; i++)
                                {
                                    if (ZKTecoSdk.SetUserFaceStr((int)DeviceInfo.Code, user.Id.ToString(), 50,
                                        Encoding.ASCII.GetString(face.Template), face.Size))
                                    {
                                        //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                                        Logger.Log(
                                            $"Successfully set face template for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                                        break;
                                    }

                                    ZKTecoSdk.GetLastError(ref errorCode);
                                    Thread.Sleep(50);
                                    Logger.Log(
                                        $"Cannot set face template for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                                }
                            }

                        }

                        var userAccessGroups = _accessGroupService.GetAccessGroupsOfUser(user.Id);
                        var validAccessGroup =
                            userAccessGroups.FirstOrDefault(ag =>
                                ag.DeviceGroup.Any(dg => dg.Devices.Any(d => d.DeviceId == DeviceInfo.DeviceId)));
                        if (ZKTecoSdk.SetUserGroup((int)DeviceInfo.Code, (int)user.Id,
                            validAccessGroup?.Id ?? 1))
                        {
                            ZKTecoSdk.RefreshData((int)DeviceInfo.Code);
                            Logger.Log(
                                $"Successfully set access group for UserId {user.Id} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                            //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);
                            return true;
                        }

                        ZKTecoSdk.RefreshData((int)DeviceInfo.Code);
                        ZKTecoSdk.GetLastError(ref errorCode);
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
                ZKTecoSdk.GetLastError(ref errorCode);
                //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                Logger.Log($"Cannot add user {user.Id} to device {DeviceInfo.Code}. ErrorCode={errorCode}",
                    logType: LogType.Warning);
                return false;
            }
        }

        public virtual ResultViewModel ReadOfflineLog(object cancelationToken, bool saveFile = false)
        {
            lock (ZKTecoSdk)
            {
                try
                {
                    var iLogCount = 0;
                    Logger.Log($"Retrieving offline logs of DeviceId: {DeviceInfo.Code}.");

                    //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);//disable the device
                    var idwErrorCode = 0;

                    if (!ZKTecoSdk.ReadNewGLogData((int)DeviceInfo.Code))
                    {
                        ZKTecoSdk.GetLastError(ref idwErrorCode);
                        if (idwErrorCode != 0)
                            if (!ZKTecoSdk.ReadGeneralLogData((int)DeviceInfo.Code))
                            {
                                ZKTecoSdk.GetLastError(ref idwErrorCode);
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
                    while (ZKTecoSdk.SSR_GetGeneralLogData((int)DeviceInfo.Code, out var iUserId,
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
                                    //EventLog = Event.ATHORIZED,
                                    EventLog = LogEvents.Authorized,
                                    UserId = userId,
                                    MatchingType = ZkCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                                    TnaEvent = (ushort)iInOutMode
                                };

                                //_zkLogService.AddLog(log);
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
                        ZkLogService.AddLog(lstLogs);
                        if (!saveFile) return;

                        LogService.SaveLogsInFile(lstLogs, "ZK", DeviceInfo.Code);
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

        public ResultViewModel ReadOfflineLogInPeriod(object cancelationToken, string fDate, string eDate, bool saveFile = false)
        {
            lock (ZKTecoSdk)
            {
                try
                {
                    var iLogCount = 0;
                    Logger.Log($"Retrieving offline logs of DeviceId: {DeviceInfo.Code}.");

                    //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);//disable the device
                    //if (_zkTecoSdk.ReadTimeGLogData((int)_deviceInfo.Code, fDate, eDate))
                    if (fDate == default && eDate == default)
                    {
                        if (!ZKTecoSdk.ReadGeneralLogData((int)DeviceInfo.Code))
                        {
                            var idwErrorCode = 0;
                            ZKTecoSdk.GetLastError(ref idwErrorCode);
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

                    else if (!ZKTecoSdk.ReadTimeGLogData((int)DeviceInfo.Code, fDate, eDate))
                        if (!ZKTecoSdk.ReadGeneralLogData((int)DeviceInfo.Code))
                        {
                            var idwErrorCode = 0;
                            ZKTecoSdk.GetLastError(ref idwErrorCode);
                            //Thread.Sleep(2000);
                            //Connect();
                            Logger.Log(
                                $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}", logType: LogType.Warning);
                            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0", Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                        }


                    var iWorkCode = 0;

                    var lstLogs = new List<Log>();
                    var recordsCount = 0;
                    while (ZKTecoSdk.SSR_GetGeneralLogData((int)DeviceInfo.Code, out var iUserId,
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
                                    //EventLog = Event.ATHORIZED,
                                    EventLog = LogEvents.Authorized,
                                    UserId = userId,
                                    MatchingType = ZkCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                                    TnaEvent = (ushort)iInOutMode
                                };

                                //_zkLogService.AddLog(log);
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

                    Task.Run(() => { ZkLogService.AddLog(lstLogs); }, TokenSource.Token);

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

            lock (ZKTecoSdk)
            {
                //int iGroupNo = Convert.ToInt32(cbGroupNo.Text.Trim());
                var validHoliday = 0;
                var verifyStyle = 0;

                if (ZKTecoSdk.SSR_SetGroupTZ((int)DeviceInfo.Code, accessGroup.Id, accessGroup.TimeZone.Id, 0, 0,
                    validHoliday, verifyStyle))
                {
                    ZKTecoSdk.RefreshData((int)DeviceInfo.Code); //the data in the device should be refreshed
                    Logger.Log($"Group Number: {accessGroup.Id} VerifyStyle: {verifyStyle} Success");
                    return true;
                }

                ZKTecoSdk.GetLastError(ref errorCode);
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

            lock (ZKTecoSdk)
            {
                if (ZKTecoSdk.SetTZInfo((int)DeviceInfo.Code, timeZone.Id, timeZoneString))
                {
                    ZKTecoSdk.RefreshData((int)DeviceInfo.Code); //the data in the device should be refreshed
                    Logger.Log($"SetTZInfo, TimeZone Index: {timeZone.Id} TimeZone: {timeZoneString}, Success", logType: LogType.Information);
                    return true;
                }

                ZKTecoSdk.GetLastError(ref errorCode);
                Logger.Log($"Operation failed, ErrorCode= {errorCode}, Error.", logType: LogType.Warning);
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
                    lock (ZKTecoSdk)
                    {
                        ZKTecoSdk.SSR_DeleteEnrollData((int)DeviceInfo.Code, sUserId.ToString(), 12);
                        ZKTecoSdk.RefreshData((int)DeviceInfo.Code);
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
                ZKTecoSdk.EnableDevice((int)DeviceInfo.Code, false); //disable the device

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
                ZKTecoSdk.EnableDevice((int)DeviceInfo.Code, false);//disable the device
                return true;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $" --> Error On UnLocking : {DeviceInfo.Code}");
                return false;
            }
        }

        public List<User> GetAllUserInfo()
        {
            lock (ZKTecoSdk)
            {
                if (ZKTecoSdk.ReadAllUserID((int)DeviceInfo.Code))
                {
                    var lstUsers = new List<User>();

                    while (ZKTecoSdk.SSR_GetAllUserInfo((int)DeviceInfo.Code, out var iUserId, out var name, out _,
                        out var privilege, out var enable))
                    {
                        lock (LockObject) //make the object exclusive 
                        {
                            try
                            {
                                var user = new User
                                {
                                    Id = Convert.ToInt32(iUserId),
                                    UserName = name,
                                    IsActive = enable,
                                    AdminLevel = privilege
                                };

                                //_zkLogService.AddLog(log);
                                lstUsers.Add(user);
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
                ZKTecoSdk.GetLastError(ref error);
                Logger.Log($"Cannot Get Users Of Device {DeviceInfo.Code}, ErrorCode : {error}", logType: LogType.Warning);
                return new List<User>();
            }
        }
        public virtual bool GetAndSaveUser(long userId)
        {
            lock (ZKTecoSdk)
            {
                try
                {
                    Logger.Log("<--EventGetUserData");

                    if (ZKTecoSdk.SSR_GetUserInfo((int)DeviceInfo.Code, userId.ToString(), out var name,
                        out var password, out var privilege, out var enabled))
                    {
                        var user = new User
                        {
                            Id = userId,
                            AdminLevel = privilege,
                            IsActive = enabled,
                            SurName = name.Split(' ').LastOrDefault(),
                            FirstName = name.Split(' ').FirstOrDefault(),
                            StartDate = DateTime.Parse("1970/01/01"),
                            EndDate = DateTime.Parse("2050/01/01"),
                            Password = password,
                            UserName = name,
                        };
                        var existUser = UserService.GetUser(userId, false);
                        if (existUser != null)
                        {
                            user = new User
                            {
                                Id = userId,
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

                        UserService.ModifyUser(user);
                        Logger.Log("<--User is Modified");

                        user.FingerTemplates = new List<FingerTemplate>();
                        user.FaceTemplates = new List<FaceTemplate>();

                        try
                        {
                            if (ZKTecoSdk.GetStrCardNumber(out var cardNumber) && cardNumber != "0")
                            {
                                var card = new UserCard
                                {
                                    CardNum = cardNumber,
                                    IsActive = true,
                                    UserId = user.Id
                                };

                                UserCardService.ModifyUserCard(card);
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
                                if (!ZKTecoSdk.SSR_GetUserTmpStr((int)DeviceInfo.Code, user.Id.ToString(), i,
                                    out var tempData, out var tempLength))
                                {
                                    Thread.Sleep(50);
                                    continue;
                                }
                                var fingerTemplate = new FingerTemplate
                                {
                                    FingerIndex = BiometricTemplateManager.GetFingerIndex(i),
                                    FingerTemplateType = FingerTemplateTypes.VX10,
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
                                        fp.FingerIndex.Code == BiometricTemplateManager.GetFingerIndex(i).Code && fp.FingerTemplateType.Code == FingerTemplateTypes.VX10Code) ?? false)
                                    {
                                        user.FingerTemplates.Add(fingerTemplate);
                                        Logger.Log($"A finger print with index: {i} is retrieved for user: {user.Id}");
                                    }
                                    else
                                    {
                                        Logger.Log($"The User: {user.Id} has a finger print with index: {i}");
                                    }
                                }
                                else
                                {
                                    user.FingerTemplates.Add(fingerTemplate);
                                    Logger.Log($"A finger print with index: {i} is retrieved for user: {user.Id}");
                                }
                            }

                            if (user.FingerTemplates.Count > 0)
                            {
                                foreach (var fingerTemplate in user.FingerTemplates)
                                {
                                    FingerTemplateService.ModifyFingerTemplate(fingerTemplate);
                                }

                                Logger.Log("<-- Finger Template is modified");
                            }
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
                                if (!ZKTecoSdk.GetUserFaceStr((int)DeviceInfo.Code, userId.ToString(), 50,
                                    ref faceStr, ref faceLen))
                                {
                                    Thread.Sleep(50);
                                    continue;
                                }
                                var faceTemplate = new FaceTemplate
                                {
                                    Index = 50,
                                    FaceTemplateType = FaceTemplateTypes.ZKVX7,
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

                                Logger.Log("<-- face Template is modified");
                            }

                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, $"Error in getting face template from device: {DeviceInfo.Code}");
                        }

                        Logger.Log($@" The user: {userId} is retrieved from device:{DeviceInfo.Code}
    Info: Finger retrieved count: {retrievedFingerTemplates.Count}, inserted count: {user.FingerTemplates.Count}, 
          Face retrieved count: {retrievedFaceTemplates.Count}, inserted count: {user.FaceTemplates.Count}");
                    }


                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log($" --> Error On GetUserData {e.Message}", logType: LogType.Warning);
                    return false;
                }
            }
        }
        public Dictionary<string, string> GetAdditionalData(int code)
        {
            lock (ZKTecoSdk)
            {
                var dic = new Dictionary<string, string>();
                var adminCnt = 0;
                var userCount = 0;
                var fpCnt = 0;
                var recordCnt = 0;
                var pwdCnt = 0;
                var opLogCnt = 0;
                var faceCnt = 0;
                ZKTecoSdk.EnableDevice(code, false); //disable the device

                ZKTecoSdk.GetDeviceStatus(code, 2, ref userCount);
                ZKTecoSdk.GetDeviceStatus(code, 1, ref adminCnt);
                ZKTecoSdk.GetDeviceStatus(code, 3, ref fpCnt);
                ZKTecoSdk.GetDeviceStatus(code, 4, ref pwdCnt);
                ZKTecoSdk.GetDeviceStatus(code, 5, ref opLogCnt);
                ZKTecoSdk.GetDeviceStatus(code, 6, ref recordCnt);
                ZKTecoSdk.GetDeviceStatus(code, 21, ref faceCnt);
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

                ZKTecoSdk.GetSysOption(code, "~ZKFPVersion", out var iFPAlg);
                ZKTecoSdk.GetSysOption(code, "ZKFaceVersion", out var iFaceAlg);
                ZKTecoSdk.GetVendor(ref sProducer);
                ZKTecoSdk.GetProductCode(code, out var sDeviceName);
                ZKTecoSdk.GetDeviceMAC(code, ref sMac);
                ZKTecoSdk.GetFirmwareVersion(code, ref sFirmwareVersion);
                ZKTecoSdk.GetPlatform(code, ref sPlatform);
                ZKTecoSdk.GetSerialNumber(code, out var sn);
                ZKTecoSdk.GetDeviceStrInfo(code, 1, out var sProductTime);

                ZKTecoSdk.EnableDevice(code, true); //enable the device
                dic.Add("FPAlg", iFPAlg);
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
            lock (ZKTecoSdk)
            {
                try
                {
                    ZKTecoSdk.EnableDevice(code, false); //disable the device

                    if (string.IsNullOrEmpty(fDate) && string.IsNullOrEmpty(eDate))
                    {
                        if (!ZKTecoSdk.ClearGLog(code)) return false;
                    }
                    else
                    {
                        if (!ZKTecoSdk.DeleteAttlogBetweenTheDate(code, fDate, eDate)) return false;
                    }

                    ZKTecoSdk.RefreshData(code);
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