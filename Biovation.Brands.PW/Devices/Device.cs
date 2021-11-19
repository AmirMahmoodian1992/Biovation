using Biovation.Brands.PW.Manager;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using xLink;
using Log = Biovation.Domain.Log;

namespace Biovation.Brands.PW.Devices
{
    public class Device : IDevices, IDisposable
    {
        internal CancellationToken ServiceCancellationToken;
        private bool _valid;

        private readonly xLinkClass _pwSdk = new xLinkClass();//create Standalone _PWSdk class dynamically
        private readonly NetTypes.LINK_PARAMS_TYPE _linkParam = new NetTypes.LINK_PARAMS_TYPE();

        private static readonly object LockObject = new object();

        //private readonly bool _isGetLogEnable = ConfigurationManager.GetAllLogWhenConnect;
        private int _lastLogReadCount;
        private int _offlineLogReadCount = 1;
        private readonly bool _clearLogAfterRetrieving;
        private Timer _fixDaylightSavingTimer;

        private readonly ILogger _logger;
        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly DeviceBrands _deviceBrands;
        private readonly PwCodeMappings _pwCodeMappings;

        private readonly LogService _logService;
        private readonly TaskService _taskService;
        private readonly DeviceBasicInfo _deviceInfo;

        internal Device(DeviceBasicInfo deviceInfo, BiovationConfigurationManager biovationConfigurationManager, LogEvents logEvents, LogSubEvents logSubEvents, PwCodeMappings pwCodeMappings, LogService logService, DeviceBrands deviceBrands, TaskService taskService, ILogger logger)
        {
            _valid = true;
            _logger = logger;
            _logEvents = logEvents;
            _deviceInfo = deviceInfo;
            _logService = logService;
            _logSubEvents = logSubEvents;
            _pwCodeMappings = pwCodeMappings;
            _logService = logService;
            _deviceBrands = deviceBrands;
            _taskService = taskService;
            _clearLogAfterRetrieving = biovationConfigurationManager.ClearLogAfterRetrieving;

            _logger = logger.ForContext<Device>();
        }

        public DeviceBasicInfo GetDeviceInfo()
        {
            return _deviceInfo;
        }

        public bool Connect(CancellationToken cancellationToken)
        {
            ServiceCancellationToken = cancellationToken != CancellationToken.None && !ServiceCancellationToken.Equals(cancellationToken) ? cancellationToken : ServiceCancellationToken;
            FillLinkParameters();
            var isConnect = IsConnected();
            if (isConnect)
            {
                _logger.Debug($"Successfully connected to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");

                SetDateTime();

                try
                {
                    var dueTime = (DateTime.Today.AddDays(1).AddMinutes(1) - DateTime.Now).TotalMilliseconds;
                    _fixDaylightSavingTimer = new Timer(FixDaylightSavingTimer_Elapsed, null, (long)dueTime, (long)TimeSpan.FromHours(24).TotalMilliseconds);
                }
                catch (Exception exception)
                {
                    _logger.Warning(exception, exception.Message);
                }

                //Task.Run(() => { ReadOnlineLog(Token); }, Token);
                ReadOnlineLog();
            }

            _taskService.ProcessQueue(_deviceBrands.ProcessingWorld, _deviceInfo.DeviceId).ConfigureAwait(false);
            return isConnect;
        }

        public bool Disconnect()
        {
            try
            {
                _valid = false;
                Dispose();
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, exception.Message);
                return false;
            }

            return true;
        }

        private bool IsConnected()
        {
            var stat = new NetTypes.PW_STATUS_TYPE
            {
                dateTime = new byte[7],
                ver = new byte[3],
                biosVer = new byte[3],
                grpCodes = new ushort[5]
            };
            short connectResult;
            lock (_pwSdk)
            {
                connectResult = _pwSdk.testLink(_linkParam, false, ref stat);
            }

            if (connectResult == 0 || connectResult == 2039)
            {
                _logger.Debug($"DeviceInfo: Id: {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}\n   Time: {stat.dateTime[0]}-{stat.dateTime[1]}-{stat.dateTime[2]} {stat.dateTime[4]}:{stat.dateTime[5]}\n  Device Type: {stat.sysCode}");
                return true;
            }

            while (!ServiceCancellationToken.IsCancellationRequested && _valid)
            {
                _logger.Debug($"Could not connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");
                _logger.Debug($"Error code: {connectResult} ");

                Task.Delay(TimeSpan.FromSeconds(120), ServiceCancellationToken).Wait(ServiceCancellationToken);
                _logger.Debug($"Retrying connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");
                short retryConnectResult;
                lock (_pwSdk)
                {
                    retryConnectResult = _pwSdk.testLink(_linkParam, false, ref stat);
                }

                if (retryConnectResult == 0 || retryConnectResult == 2039)
                {
                    _logger.Debug($"DeviceInfo: Id: {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}\n   Time: {stat.dateTime[0]}-{stat.dateTime[1]}-{stat.dateTime[2]} {stat.dateTime[4]}:{stat.dateTime[5]}\n  Device Type: {stat.sysCode}");
                    return true;
                }
            }

            return false;
        }

        private void FixDaylightSavingTimer_Elapsed(object state)
        {
            SetDateTime();
        }

        public void SetDateTime()
        {
            if (!_deviceInfo.TimeSync) return;
            lock (_pwSdk)
            {
                try
                {
                    _pwSdk.sendTime(_linkParam);
                }
                catch (Exception)
                {
                    // ignore
                }
            }
        }
        public bool TransferUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<ResultViewModel> ReadOnlineLog()
        {
            return Task.Run(() =>
            {
                ushort newRecordsCount = 0;

                do
                {
                    try
                    {
                        var newGateNumber = _deviceInfo.Code;
                        var newBonesFn = "";
                        var newRecords = new NetTypes.IN_OUT_RECORD_TYPE[100000]; //for 10000 records.
                        var newFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';
                        var newTextFile = "";
                        long userId = 0;

                        short newResult;
                        lock (_pwSdk)
                        {
                            if (_offlineLogReadCount != 0 && _offlineLogReadCount % 5 == 0 && _clearLogAfterRetrieving)
                                _linkParam.clearPW = true;

                            newResult = _pwSdk.getIOs(_linkParam, newFilePath, ref newRecordsCount, ref newBonesFn,
                                ref newRecords, newGateNumber, newTextFile);
                            _linkParam.clearPW = false;

                            if (newRecordsCount > 0 && _lastLogReadCount == newRecordsCount)
                                _offlineLogReadCount++;
                            else
                                _offlineLogReadCount = 1;
                        }

                        DeletePhysicalFile(Path.Combine(newFilePath, newBonesFn));
                        if (newResult == 0 && newRecordsCount > 0)
                        {
                            if (newRecordsCount == _lastLogReadCount && _offlineLogReadCount > 3)
                            {
                                if (_offlineLogReadCount % 5 == 1 && _clearLogAfterRetrieving)
                                    _lastLogReadCount = 0;
                                Task.Delay(TimeSpan.FromSeconds(120), ServiceCancellationToken).Wait(ServiceCancellationToken);
                                continue;
                            }

                            lock (LockObject) //make the object exclusive 
                            {
                                try
                                {
                                    var logs = new List<Log>();
                                    for (var i = 0; i < newRecordsCount; i++)
                                    {
                                        try
                                        {
                                            userId = Convert.ToInt64(newRecords[i].cardNo);

                                            var log = new Log
                                            {
                                                DeviceId = _deviceInfo.DeviceId,
                                                DeviceCode = _deviceInfo.Code,
                                                LogDateTime = new DateTime(newRecords[i].xDate.y, newRecords[i].xDate.m,
                                                    newRecords[i].xDate.d, newRecords[i].hh, newRecords[i].mn,
                                                    newRecords[i].ss),
                                                EventLog = _logEvents.Authorized,
                                                UserId = userId,
                                                MatchingType = _pwCodeMappings.GetMatchingTypeGenericLookup(newRecords[i].ioType),
                                                InOutMode = _deviceInfo.DeviceTypeId,
                                                TnaEvent = 0,
                                                SubEvent = _logSubEvents.Normal
                                            };

                                            logs.Add(log);
                                            //_pwLogService.AddLog(log);
                                        }
                                        catch (Exception exception)
                                        {
                                            _logger.Warning(exception, exception.Message);
                                        }
                                    }

                                    _logService.AddLog(logs).ConfigureAwait(false);

                                    Task.Run(() =>
                                    {
                                        var retrievedLogsCount = logs.Count;
                                        foreach (var (log, index) in logs.TakeWhile(_ => !ServiceCancellationToken.IsCancellationRequested).Select((log, index) => (log, index)))
                                        {
                                            _logger.Debug($@"RTEvent OnAttTransaction Has been Triggered,Verified OK
                                                ...UserID: {log.UserId}
                                                ...DeviceCode: {log.DeviceCode}
                                                ...MatchingType: {log.MatchingType}
                                                ...VerifyMethod: {log.AuthType}
                                                ...Time: {log.LogDateTime}
                                                ...Progress: {index}/{retrievedLogsCount}");
                                        }
                                    }, ServiceCancellationToken);

                                    _lastLogReadCount = newRecordsCount;
                                }

                                catch (Exception)
                                {
                                    _logger.Debug($"User id of log is not in a correct format. UserId : {userId}");
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                    }

                    Task.Delay(TimeSpan.FromSeconds(120), ServiceCancellationToken).Wait(ServiceCancellationToken);
                } while (IsConnected() && _valid && !ServiceCancellationToken.IsCancellationRequested);

                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };
            }, ServiceCancellationToken);
        }

        public ResultViewModel ReadOfflineLog(object cancellationToken, bool fileSave = false)
        {
            try
            {
                _logger.Debug($"Retrieving offline logs of DeviceId: {_deviceInfo.Code}.");
                ushort recordsCount = 0;
                var bonesFn = "";
                var records = new NetTypes.IN_OUT_RECORD_TYPE[100000]; //for 10000 records.

                var gateNumber = _deviceInfo.Code;

                var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';
                var textFile = "";
                short result;


                lock (_pwSdk)
                {
                    _linkParam.clearPW = false;
                    result = _pwSdk.getIOs(_linkParam, filePath, ref recordsCount, ref bonesFn, ref records, gateNumber, textFile);
                }

                DeletePhysicalFile(Path.Combine(filePath, bonesFn));

                if (result == 0)
                {
                    lock (LockObject) //make the object exclusive 
                    {
                        try
                        {
                            var logs = new List<Log>();

                            for (var i = recordsCount - 1; i >= 0; i--)
                            {
                                try
                                {
                                    var userId = Convert.ToInt64(records[i].cardNo);

                                    var log = new Log
                                    {
                                        DeviceId = _deviceInfo.DeviceId,
                                        DeviceCode = _deviceInfo.Code,
                                        LogDateTime = new DateTime(records[i].xDate.y, records[i].xDate.m,
                                            records[i].xDate.d, records[i].hh, records[i].mn, records[i].ss),
                                        EventLog = _logEvents.Authorized,
                                        UserId = userId,
                                        MatchingType = _pwCodeMappings.GetMatchingTypeGenericLookup(records[i].ioType),
                                        InOutMode = _deviceInfo.DeviceTypeId,
                                        TnaEvent = 0,
                                        SubEvent = _logSubEvents.Normal
                                    };

                                    logs.Add(log);
                                }
                                catch (Exception exception)
                                {
                                    _logger.Warning(exception, exception.Message);
                                }
                            }

                            var count = recordsCount;
                            Task.Run(() =>
                            {
                                foreach (var (log, index) in logs.TakeWhile(_ => !ServiceCancellationToken.IsCancellationRequested).Select((log, index) => (log, index)))
                                {
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{index}/{count}");
                                }
                            }, ServiceCancellationToken);

                            _ = _logService.AddLog(logs).ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            //_logger.Debug($"User id of log is not in a correct format. UserId : {userId}", logType: LogType.Warning);
                        }
                    }
                }

                //recovering deleted logs
                lock (_pwSdk)
                {
                    _linkParam.clearPW = false;
                    result = _pwSdk.recoverIOs(_linkParam, filePath, new NetTypes.DATE_TYPE { y = 1, m = 1, d = 1 },
                        new NetTypes.DATE_TYPE { y = (short)(DateTime.Now.Year % 100), m = 12, d = 30 },
                        ref recordsCount, ref bonesFn,
                        ref records, gateNumber);
                }

                //var userId = 0;
                DeletePhysicalFile(Path.Combine(filePath, bonesFn));
                if (result == 0)
                {
                    lock (LockObject) //make the object exclusive 
                    {
                        try
                        {
                            var logs = new List<Log>();
                            var thousandIndex = 0;
                            var logInsertionTasks = new List<Task>();

                            for (var i = recordsCount - 1; i >= 0; i--)
                            {
                                try
                                {
                                    var userId = Convert.ToInt64(records[i].cardNo);

                                    var log = new Log
                                    {
                                        DeviceId = _deviceInfo.DeviceId,
                                        DeviceCode = _deviceInfo.Code,
                                        LogDateTime = new DateTime(records[i].xDate.y, records[i].xDate.m,
                                            records[i].xDate.d, records[i].hh, records[i].mn, records[i].ss),
                                        EventLog = _logEvents.Authorized,
                                        UserId = userId,
                                        MatchingType =
                                            _pwCodeMappings.GetMatchingTypeGenericLookup(records[i].ioType),
                                        InOutMode = _deviceInfo.DeviceTypeId,
                                        TnaEvent = 0,
                                        SubEvent = _logSubEvents.Normal
                                    };

                                    logs.Add(log);

                                    if (logs.Count % 1000 == 0)
                                    {
                                        var partedLogs = logs.Skip(thousandIndex * 1000).Take(1000).ToList();

                                        try
                                        {
                                            logInsertionTasks.Add(_logService.AddLog(partedLogs));
                                            //if (saveFile) _logService.SaveLogsInFile(partedLogs, "ZK", _deviceInfo.Code);
                                        }
                                        catch (Exception exception)
                                        {
                                            _logger.Warning(exception, exception.Message);
                                            return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = exception.Message, Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                                        }

                                        thousandIndex++;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    _logger.Warning(exception, exception.Message);

                                }
                            }

                            try
                            {
                                var partedLogs = logs.Skip(thousandIndex * 1000).Take(1000).ToList();
                                logInsertionTasks.Add(_logService.AddLog(partedLogs));
                                //if (saveFile) _logService.SaveLogsInFile(partedLogs, "ZK", DeviceInfo.Code);
                            }
                            catch (Exception exception)
                            {
                                _logger.Warning(exception, exception.Message);
                                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = exception.Message, Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                            }

                            Task.Run(() =>
                            {
                                foreach (var (log, index) in logs.TakeWhile(_ => !ServiceCancellationToken.IsCancellationRequested).Select((log, index) => (log, index)))
                                {
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{index}/{recordsCount}");
                                }
                            }, ServiceCancellationToken);
                            Task.WaitAll(logInsertionTasks.ToArray());

                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            //_logger.Debug($"User id of log is not in a correct format. UserId : {userId}", logType: LogType.Warning);
                        }
                    }

                    _logger.Debug($"{recordsCount} Offline log retrieved from DeviceId: {_deviceInfo.Code}.");
                }
                //else if (result == 1001)
                //{
                //    _logger.Debug($"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} Because the device is disconnected ErrorCode={result}");
                //    Disconnect();
                //}
                else
                {
                    _logger.Debug(
                        $"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} General Log Data Count:0 ErrorCode={result}");
                }

                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 1, Message = recordsCount.ToString() };
            }
            catch (Exception exception)
            {
                _logger.Debug(exception, $"Error on reading offline logs of device: {_deviceInfo.Code}");
                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };
            }
        }

        public ResultViewModel ReadOfflineLogInPeriod(object cancellationToken, DateTime fromDate, DateTime toDate, bool saveFile = false)
        {
            try
            {
                _logger.Debug($"Retrieving offline logs of DeviceId: {_deviceInfo.Code}.");
                ushort recordsCount = 0;
                var bonesFn = "";
                var records = new NetTypes.IN_OUT_RECORD_TYPE[100000]; //for 10000 records.

                var gateNumber = _deviceInfo.Code;

                var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';
                var textFile = "";
                short result;

                lock (_pwSdk)
                {
                    _linkParam.clearPW = false;
                    result = _pwSdk.getIOs(_linkParam, filePath, ref recordsCount, ref bonesFn, ref records, gateNumber, textFile);
                }

                DeletePhysicalFile(Path.Combine(filePath, bonesFn));

                if (result == 0)
                {
                    lock (LockObject) //make the object exclusive 
                    {
                        try
                        {
                            var logs = new List<Log>();

                            for (var i = recordsCount - 1; i >= 0; i--)
                            {
                                try
                                {
                                    var userId = Convert.ToInt64(records[i].cardNo);

                                    var log = new Log
                                    {
                                        DeviceId = _deviceInfo.DeviceId,
                                        DeviceCode = _deviceInfo.Code,
                                        LogDateTime = new DateTime(records[i].xDate.y, records[i].xDate.m,
                                            records[i].xDate.d, records[i].hh, records[i].mn, records[i].ss),
                                        EventLog = _logEvents.Authorized,
                                        UserId = userId,
                                        MatchingType = _pwCodeMappings.GetMatchingTypeGenericLookup(records[i].ioType),
                                        InOutMode = _deviceInfo.DeviceTypeId,
                                        TnaEvent = 0,
                                        SubEvent = _logSubEvents.Normal
                                    };

                                    logs.Add(log);
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }
                            }

                            logs = logs.Where(log => log.LogDateTime > fromDate && log.LogDateTime < toDate).ToList();

                            var count = recordsCount;
                            _logService.AddLog(logs).ConfigureAwait(false);

                            Task.Run(() =>
                            {
                                foreach (var (log, index) in logs.TakeWhile(_ => !ServiceCancellationToken.IsCancellationRequested).Select((log, index) => (log, index)))
                                {
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{index}/{count}");
                                }
                            }, ServiceCancellationToken);
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            //_logger.Debug($"User id of log is not in a correct format. UserId : {userId}", logType: LogType.Warning);
                        }
                    }
                }

                //recovering deleted logs
                lock (_pwSdk)
                {
                    _logger.Information("Recovering logs of device {deviceCode} (OLD)", _deviceInfo.Code);
                    _linkParam.clearPW = false;
                    result = _pwSdk.recoverIOs(_linkParam, filePath, new NetTypes.DATE_TYPE { y = 1, m = 1, d = 1 },
                        new NetTypes.DATE_TYPE { y = (short)(DateTime.Now.Year % 100), m = 12, d = 30 },
                        ref recordsCount, ref bonesFn,
                        ref records, gateNumber);
                }

                DeletePhysicalFile(Path.Combine(filePath, bonesFn));
                if (result == 0)
                {
                    lock (LockObject) //make the object exclusive 
                    {
                        try
                        {
                            var logs = new List<Log>();

                            for (var i = recordsCount - 1; i >= 0; i--)
                            {
                                try
                                {
                                    var userId = Convert.ToInt64(records[i].cardNo);

                                    var log = new Log
                                    {
                                        DeviceId = _deviceInfo.DeviceId,
                                        DeviceCode = _deviceInfo.Code,
                                        LogDateTime = new DateTime(records[i].xDate.y, records[i].xDate.m,
                                            records[i].xDate.d, records[i].hh, records[i].mn, records[i].ss),
                                        EventLog = _logEvents.Authorized,
                                        UserId = userId,
                                        MatchingType =
                                            _pwCodeMappings.GetMatchingTypeGenericLookup(records[i].ioType),
                                        InOutMode = _deviceInfo.DeviceTypeId,
                                        TnaEvent = 0,
                                        SubEvent = _logSubEvents.Normal
                                    };

                                    logs.Add(log);
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }
                            }

                            logs = logs.Where(log => log.LogDateTime >= fromDate && log.LogDateTime <= toDate).ToList();

                            var count = recordsCount;
                            var logInsertionTasks = new List<Task>();

                            for (var i = 0; i < count / 1000; i++)
                            {
                                try
                                {
                                    var partedLogs = logs.Skip(i * 1000).Take(1000).ToList();
                                    if (partedLogs.Count > 0)
                                        logInsertionTasks.Add(_logService.AddLog(partedLogs));
                                    //if (saveFile) _logService.SaveLogsInFile(partedLogs, "ZK", DeviceInfo.Code);
                                }
                                catch (Exception exception)
                                {
                                    _logger.Warning(exception, exception.Message);
                                    return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = exception.Message, Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                                }
                            }

                            Task.Run(() =>
                            {
                                foreach (var (log, index) in logs.TakeWhile(_ => !ServiceCancellationToken.IsCancellationRequested).Select((log, index) => (log, index)))
                                {
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{index}/{count}");
                                }
                            }, ServiceCancellationToken);
                            Task.WaitAll(logInsertionTasks.ToArray());

                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            //_logger.Debug($"User id of log is not in a correct format. UserId : {userId}", logType: LogType.Warning);
                        }
                    }


                    _logger.Debug($"{recordsCount} Offline log retrieved from DeviceId: {_deviceInfo.Code}.");
                }
                //else if (result == 1001)
                //{
                //    _logger.Debug($"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} Because the device is disconnected ErrorCode={result}");
                //    Disconnect();
                //}
                else
                {
                    _logger.Debug(
                        $"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} General Log Data Count:0 ErrorCode={result}");
                }

                var deviceLogs = new NetTypes.LOG_RECORD_TYPE[10000];

                //recovering deleted logs (NEW)
                lock (_pwSdk)
                {
                    _logger.Information("Recovering logs of device {deviceCode} (get logs)", _deviceInfo.Code);
                    _linkParam.clearPW = false;
                    result = _pwSdk.get_logs(_linkParam, filePath, ref recordsCount, ref bonesFn, ref deviceLogs,
                        _deviceInfo.Code);
                }

                DeletePhysicalFile(Path.Combine(filePath, bonesFn));
                if (result == 0)
                {
                    lock (LockObject) //make the object exclusive 
                    {
                        try
                        {
                            var logs = new List<Log>();

                            for (var i = recordsCount - 1; i >= 0; i--)
                            {
                                try
                                {
                                    var userId = Convert.ToInt64(deviceLogs[i].userId);

                                    var log = new Log
                                    {
                                        DeviceId = _deviceInfo.DeviceId,
                                        DeviceCode = _deviceInfo.Code,
                                        LogDateTime = new DateTime(deviceLogs[i].xDate.y, deviceLogs[i].xDate.m,
                                            deviceLogs[i].xDate.d, deviceLogs[i].hh, deviceLogs[i].mn, deviceLogs[i].ss),
                                        EventLog = _logEvents.Authorized,
                                        UserId = userId,
                                        MatchingType =
                                            _pwCodeMappings.GetMatchingTypeGenericLookup(deviceLogs[i].logCode),
                                        InOutMode = _deviceInfo.DeviceTypeId,
                                        TnaEvent = 0,
                                        SubEvent = _logSubEvents.Normal
                                    };

                                    logs.Add(log);
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }
                            }

                            logs = logs.Where(log => log.LogDateTime >= fromDate && log.LogDateTime <= toDate).ToList();

                            var count = recordsCount;
                            var logInsertionTasks = new List<Task>();

                            for (var i = 0; i < count / 1000; i++)
                            {
                                try
                                {
                                    var partedLogs = logs.Skip(i * 1000).Take(1000).ToList();
                                    if (partedLogs.Count > 0)
                                        logInsertionTasks.Add(_logService.AddLog(partedLogs));
                                    //if (saveFile) _logService.SaveLogsInFile(partedLogs, "ZK", DeviceInfo.Code);
                                }
                                catch (Exception exception)
                                {
                                    _logger.Warning(exception, exception.Message);
                                    return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = exception.Message, Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                                }
                            }

                            Task.Run(() =>
                            {
                                foreach (var (log, index) in logs.TakeWhile(_ => !ServiceCancellationToken.IsCancellationRequested).Select((log, index) => (log, index)))
                                {
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{index}/{count}");
                                }
                            }, ServiceCancellationToken);
                            Task.WaitAll(logInsertionTasks.ToArray());
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            //_logger.Debug($"User id of log is not in a correct format. UserId : {userId}", logType: LogType.Warning);
                        }
                    }


                    _logger.Debug($"{recordsCount} Offline log retrieved from DeviceId: {_deviceInfo.Code}.");
                }
                //else if (result == 1001)
                //{
                //    _logger.Debug($"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} Because the device is disconnected ErrorCode={result}");
                //    Disconnect();
                //}
                else
                {
                    _logger.Debug(
                        $"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} General Log Data Count:0 ErrorCode={result}");
                }

                //recovering deleted logs (NEW)
                lock (_pwSdk)
                {
                    _logger.Information("Recovering logs of device {deviceCode} (NEW with from backup -false)", _deviceInfo.Code);
                    _linkParam.clearPW = false;
                    result = _pwSdk.NEW_recoverIOs(_linkParam, filePath, new NetTypes.DATE_TYPE { y = 1, m = 1, d = 1 },
                        new NetTypes.DATE_TYPE { y = (short)(DateTime.Now.Year % 100), m = 12, d = 30 },
                        ref recordsCount, ref bonesFn,
                        ref records, gateNumber, 0, false);
                }

                DeletePhysicalFile(Path.Combine(filePath, bonesFn));
                if (result == 0)
                {
                    lock (LockObject) //make the object exclusive 
                    {
                        try
                        {
                            var logs = new List<Log>();

                            for (var i = recordsCount - 1; i >= 0; i--)
                            {
                                try
                                {
                                    var userId = Convert.ToInt64(records[i].cardNo);

                                    var log = new Log
                                    {
                                        DeviceId = _deviceInfo.DeviceId,
                                        DeviceCode = _deviceInfo.Code,
                                        LogDateTime = new DateTime(records[i].xDate.y, records[i].xDate.m,
                                            records[i].xDate.d, records[i].hh, records[i].mn, records[i].ss),
                                        EventLog = _logEvents.Authorized,
                                        UserId = userId,
                                        MatchingType =
                                            _pwCodeMappings.GetMatchingTypeGenericLookup(records[i].ioType),
                                        InOutMode = _deviceInfo.DeviceTypeId,
                                        TnaEvent = 0,
                                        SubEvent = _logSubEvents.Normal
                                    };

                                    logs.Add(log);
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }
                            }

                            logs = logs.Where(log => log.LogDateTime >= fromDate && log.LogDateTime <= toDate).ToList();

                            var count = recordsCount;
                            var logInsertionTasks = new List<Task>();

                            for (var i = 0; i < count / 1000; i++)
                            {
                                try
                                {
                                    var partedLogs = logs.Skip(i * 1000).Take(1000).ToList();
                                    if (partedLogs.Count > 0)
                                        logInsertionTasks.Add(_logService.AddLog(partedLogs));
                                    //if (saveFile) _logService.SaveLogsInFile(partedLogs, "ZK", DeviceInfo.Code);
                                }
                                catch (Exception exception)
                                {
                                    _logger.Warning(exception, exception.Message);
                                    return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = exception.Message, Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                                }
                            }

                            Task.Run(() =>
                            {
                                foreach (var (log, index) in logs.TakeWhile(_ => !ServiceCancellationToken.IsCancellationRequested).Select((log, index) => (log, index)))
                                {
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{index}/{count}");
                                }
                            }, ServiceCancellationToken);
                            Task.WaitAll(logInsertionTasks.ToArray());
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            //_logger.Debug($"User id of log is not in a correct format. UserId : {userId}", logType: LogType.Warning);
                        }
                    }


                    _logger.Debug($"{recordsCount} Offline log retrieved from DeviceId: {_deviceInfo.Code}.");
                }
                //else if (result == 1001)
                //{
                //    _logger.Debug($"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} Because the device is disconnected ErrorCode={result}");
                //    Disconnect();
                //}
                else
                {
                    _logger.Debug(
                        $"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} General Log Data Count:0 ErrorCode={result}");
                }

                //recovering deleted logs (NEW)
                lock (_pwSdk)
                {
                    _logger.Information("Recovering logs of device {deviceCode} (NEW with from backup -true)", _deviceInfo.Code);
                    _linkParam.clearPW = false;
                    result = _pwSdk.NEW_recoverIOs(_linkParam, filePath, new NetTypes.DATE_TYPE { y = 1, m = 1, d = 1 },
                        new NetTypes.DATE_TYPE { y = (short)(DateTime.Now.Year % 100), m = 12, d = 30 },
                        ref recordsCount, ref bonesFn,
                        ref records, gateNumber, 0, true);
                }

                DeletePhysicalFile(Path.Combine(filePath, bonesFn));
                if (result == 0)
                {
                    lock (LockObject) //make the object exclusive 
                    {
                        try
                        {
                            var logs = new List<Log>();

                            for (var i = recordsCount - 1; i >= 0; i--)
                            {
                                try
                                {
                                    var userId = Convert.ToInt64(records[i].cardNo);

                                    var log = new Log
                                    {
                                        DeviceId = _deviceInfo.DeviceId,
                                        DeviceCode = _deviceInfo.Code,
                                        LogDateTime = new DateTime(records[i].xDate.y, records[i].xDate.m,
                                            records[i].xDate.d, records[i].hh, records[i].mn, records[i].ss),
                                        EventLog = _logEvents.Authorized,
                                        UserId = userId,
                                        MatchingType =
                                            _pwCodeMappings.GetMatchingTypeGenericLookup(records[i].ioType),
                                        InOutMode = _deviceInfo.DeviceTypeId,
                                        TnaEvent = 0,
                                        SubEvent = _logSubEvents.Normal
                                    };

                                    logs.Add(log);
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }
                            }

                            logs = logs.Where(log => log.LogDateTime >= fromDate && log.LogDateTime <= toDate).ToList();

                            var count = recordsCount;
                            var logInsertionTasks = new List<Task>();

                            for (var i = 0; i < count / 1000; i++)
                            {
                                try
                                {
                                    var partedLogs = logs.Skip(i * 1000).Take(1000).ToList();
                                    if (partedLogs.Count > 0)
                                        logInsertionTasks.Add(_logService.AddLog(partedLogs));
                                    //if (saveFile) _logService.SaveLogsInFile(partedLogs, "ZK", DeviceInfo.Code);
                                }
                                catch (Exception exception)
                                {
                                    _logger.Warning(exception, exception.Message);
                                    return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = exception.Message, Code = Convert.ToInt32(TaskStatuses.FailedCode) };
                                }
                            }

                            Task.Run(() =>
                            {
                                foreach (var (log, index) in logs.TakeWhile(_ => !ServiceCancellationToken.IsCancellationRequested).Select((log, index) => (log, index)))
                                {
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{index}/{count}");
                                }
                            }, ServiceCancellationToken);
                            Task.WaitAll(logInsertionTasks.ToArray());
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            //_logger.Debug($"User id of log is not in a correct format. UserId : {userId}", logType: LogType.Warning);
                        }
                    }


                    _logger.Debug($"{recordsCount} Offline log retrieved from DeviceId: {_deviceInfo.Code}.");
                }
                //else if (result == 1001)
                //{
                //    _logger.Debug($"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} Because the device is disconnected ErrorCode={result}");
                //    Disconnect();
                //}
                else
                {
                    _logger.Debug(
                        $"Could not retrieve offline logs from DeviceId:{_deviceInfo.Code} General Log Data Count:0 ErrorCode={result}");
                }

                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 1, Message = recordsCount.ToString() };
            }
            catch (Exception exception)
            {
                _logger.Debug(exception, $"Error on reading offline logs of device: {_deviceInfo.Code}");
                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };
            }
        }

        private void DeletePhysicalFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public int AddDeviceToDataBase()
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(uint sUserId)
        {
            throw new NotImplementedException();
        }

        private void FillLinkParameters()
        {
            //cbf.pwPort = 100001;
            // cbf.pcPort = ;
            try
            {
                lock (_linkParam)
                {
                    _linkParam.sysId = NetConsts.SYS_PW1XXX;
                    _linkParam.chType = NetConsts.CH_ETNET;
                    _linkParam.sysNo = (ushort)_deviceInfo.Code;
                    _linkParam.comPort = 1;
                    _linkParam.viaF = 0;
                    _linkParam.ioAuto = 0;
                    _linkParam.pwIP = _deviceInfo.IpAddress;
                    _linkParam.pwPort = _deviceInfo.Port == 0 ? 10001 : _deviceInfo.Port;
                    _linkParam.hwType = GetDeviceType(_deviceInfo.ModelId) == 0 ? (byte)_deviceInfo.ManufactureCode : GetDeviceType(_deviceInfo.ModelId);
                    _linkParam.maxRetry = 2;
                    _linkParam.timeout = 600;
                    _linkParam.baudrate = 115200;
                    _linkParam.blkLen = 6000;
                    _linkParam.clearPW = false;
                    _linkParam.textFormat = NetConsts.TEXT_NONE;  //no text file.
                    _linkParam.saveInStructF = true;
                    _linkParam.dateType = NetConsts.DATE_CHRIST;
                    _linkParam.appendText = false;
                    _linkParam.encryptF = false;
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public byte GetDeviceType(int modelId)
        {
            switch (modelId)
            {
                case 5001:
                    return NetConsts.DV_PW1100;
                case 5002:
                    return NetConsts.DV_PW1200;
                case 5003:
                    return NetConsts.DV_PW1400;
                case 5004:
                    return NetConsts.DV_PW1410;
                case 5005:
                    return NetConsts.DV_PW1500;
                case 5006:
                    return NetConsts.DV_PW1510;
                case 5007:
                    return NetConsts.DV_PW1520;
                case 5008:
                    return NetConsts.DV_PW1600;
                case 5009:
                    return NetConsts.DV_PW1650;
                case 5010:
                    return NetConsts.DV_PW1680;
                case 5011:
                    return NetConsts.DV_PW1700;
                default:
                    return 0;
            }
        }

        public void Dispose()
        {
            try
            {
                _fixDaylightSavingTimer?.Dispose();
                GC.SuppressFinalize(this);
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, exception.Message);
            }
        }
    }
}