﻿using Biovation.Brands.PW.Manager;
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
        internal CancellationToken CancellationToken;
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

        //public void UpdateDeviceInfo(DeviceBasicInfo deviceInfo)
        //{
        //    _valid = false;
        //    _deviceInfo = deviceInfo;
        //}

        public DeviceBasicInfo GetDeviceInfo()
        {
            return _deviceInfo;
        }

        public bool Connect(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            FillLinkParameters();
            var isConnect = IsConnected();
            if (isConnect)
            {
                _logger.Debug($"Successfully connected to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");

                SetDateTime();

                try
                {
                    //var daylightSaving = DateTime.Now.DayOfYear <= 81 || DateTime.Now.DayOfYear > 265 ? new DateTime(DateTime.Now.Year, 3, 22, 0, 2, 0) : new DateTime(DateTime.Now.Year, 9, 22, 0, 2, 0);
                    //var dueTime = (daylightSaving.Ticks - DateTime.Now.Ticks) / 10000;
                    var dueTime = (DateTime.Today.AddDays(1).AddMinutes(1) - DateTime.Now).TotalMilliseconds;
                    _fixDaylightSavingTimer = new Timer(FixDaylightSavingTimer_Elapsed, null, (long)dueTime, (long)TimeSpan.FromHours(24).TotalMilliseconds);
                }
                catch (Exception exception)
                {
                    _logger.Warning(exception, exception.Message);
                }

                //Task.Run(() => { ReadOnlineLog(Token); }, Token);
                ReadOnlineLog();

                //if (_isGetLogEnable)
                //{
                //Task.Run(() => { ReadOfflineLog(Token); }, Token);
                //PWServer.LogReaderQueue.Enqueue(new Task(() => ReadOfflineLog(Token), Token));
                //PWServer.StartReadLogs();



                //Task.Run(() =>
                //{
                //while (!Token.IsCancellationRequested && _valid &&
                //       PWServer.GetOnlineDevices().ContainsKey(_deviceInfo.Code))
                //{
                //    var device =
                //        _deviceService.GetDeviceBasicInfoWithCode(_deviceInfo.Code,
                //            DeviceBrands.ProcessingWorldCode);
                //    var creatorUser = _userService.GetUser(123456789, false);


                //    var task = new TaskInfo
                //    {
                //        CreatedAt = DateTimeOffset.Now,
                //        CreatedBy = creatorUser,
                //        TaskType = TaskTypes.GetServeLogs,
                //        Priority = TaskPriorities.Medium,
                //        DeviceBrand = DeviceBrands.ProcessingWorld,
                //        TaskItems = new List<TaskItem>()
                //    };
                //    task.TaskItems.Add(new TaskItem
                //    {

                //        Status = TaskStatuses.Queued,
                //        TaskItemType = TaskItemTypes.GetServeLogs,
                //        Priority = TaskPriorities.Medium,
                //        DueDate = DateTime.Today,
                //        DeviceId = device.DeviceId,
                //        Data = JsonConvert.SerializeObject(new {device.DeviceId}),
                //        IsParallelRestricted = true,
                //        IsScheduled = false,
                //        OrderIndex = 1
                //    });
                //    _taskService.InsertTask(task).Wait(Token);
                //    PWServer.ProcessQueue();

                //    Thread.Sleep(120);
                //}
                //}, Token);
                //}
            }

            //_taskManager.ProcessQueue(_deviceInfo.DeviceId);
            _taskService.ProcessQueue(_deviceBrands.ProcessingWorld, _deviceInfo.DeviceId).ConfigureAwait(false);
            return isConnect;
        }

        public bool Disconnect()
        {
            _valid = false;
            //try
            //{
            //    Token.Cancel(false);
            //}
            //catch (Exception)
            //{
            //    //ignore
            //}
            //var onlineDevices = PWServer.GetOnlineDevices();
            //lock (onlineDevices)
            //{
            //    if (onlineDevices.ContainsKey(_deviceInfo.Code))
            //    {
            //        onlineDevices.Remove(_deviceInfo.Code);
            //    }
            //}

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

            while (!CancellationToken.IsCancellationRequested)
            {
                _logger.Debug($"Could not connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");
                _logger.Debug($"Error code: {connectResult} ");

                Thread.Sleep(120000);
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
                //ushort recordsCount = 0;
                //var bonesFn = "";
                //var records = new NetTypes.IN_OUT_RECORD_TYPE[100000]; //for 10000 records.

                //var gateNumber = _deviceInfo.Code;

                //var filePath = @"";
                //var textFile = "";
                //short result;
                //lock (_pwSdk)
                //{
                //    _linkParam.blkLen = 6000;
                //    _pwSdk.getIOs(_linkParam, filePath, ref recordsCount, ref bonesFn, ref records, gateNumber,
                //        textFile);
                //}

                //lock (_pwSdk)
                //{
                //    if (_offlineLogReadCount % 5 == 0 && _clearLogAfterRetrieving)
                //        _linkParam.clearPW = true;

                //    result = _pwSdk.getIOs(_linkParam, filePath, ref recordsCount, ref bonesFn, ref records, gateNumber, textFile);
                //    _linkParam.clearPW = false;
                //    if (recordsCount > 0 && _lastLogReadCount == recordsCount)
                //        _offlineLogReadCount++;
                //    else
                //        _offlineLogReadCount = 1;

                //    _lastLogReadCount = recordsCount;
                //}

                //DeletePhysicalFile(bonesFn);

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
                        var userId = 0;
                        //Task.Run(() =>
                        //{
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
                                Thread.Sleep(120000);
                                continue;
                            }

                            lock (LockObject) //make the object exclusive 
                            {
                                try
                                {
                                    //var len = records.Length;
                                    var logs = new List<Log>();
                                    for (var i = 0; i < newRecordsCount; i++)
                                    {
                                        try
                                        {
                                            userId = Convert.ToInt32(newRecords[i].cardNo);


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
                                        foreach (var log in logs.TakeWhile(log => !CancellationToken.IsCancellationRequested))
                                        {
                                            _logger.Debug($@"RTEvent OnAttTransaction Has been Triggered,Verified OK
                                                ...UserID: {log.UserId}
                                                ...DeviceCode: {log.DeviceCode}
                                                ...MatchingType: {log.MatchingType}
                                                ...VerifyMethod: {log.AuthType}
                                                ...Time: {log.LogDateTime}");
                                        }
                                    }, CancellationToken);

                                    //innerRecord = newRecordsCount;
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

                    Thread.Sleep(120000);
                } while (IsConnected() && _valid);

                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };
            }, CancellationToken);
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
                //if (result != 0)
                //    return new ResultViewModel
                //    { Id = _deviceInfo.DeviceId, Validate = 1, Message = recordsCount.ToString() };

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
                                    var userId = Convert.ToInt32(records[i].cardNo);

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

                            //var logs = records.Take(recordsCount).Select(rec =>
                            //{
                            //    try
                            //    {
                            //        if (rec.xDate.y < 1990 || rec.xDate.y < 2050 || rec.xDate.m < 1 || rec.xDate.m > 12 || rec.xDate.d < 1 || rec.xDate.d > 31 || rec.hh < 1 || rec.hh > 24 || rec.mn < 1 || rec.mn > 60 || rec.ss < 1 || rec.ss > 60)
                            //            return null;

                            //        return new Log
                            //        {
                            //            DeviceId = _deviceInfo.DeviceId,
                            //            DeviceCode = _deviceInfo.Code,
                            //            LogDateTime = new DateTime(rec.xDate.y, rec.xDate.m, rec.xDate.d, rec.hh, rec.mn,
                            //                rec.ss),
                            //            UserId = Convert.ToInt32(rec.cardNo),
                            //            MatchingType = rec.ioType,
                            //            TnaEvent = 0,
                            //            SubEvent = LogSubEvents.Normal
                            //        };
                            //    }
                            //    catch (Exception)
                            //    {
                            //        //_logger.Debug(exception);
                            //        return null;
                            //    }
                            //}).Where(log => log != null).ToList();

                            var count = recordsCount;
                            Task.Run(() =>
                            {
                                for (var i = 0; i < logs.Count;)
                                {
                                    var log = logs[i];
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{++i}/{count}");
                                }
                            }, CancellationToken);

                            //Task.Run(() =>
                            //{
                            _logService.AddLog(logs).ConfigureAwait(false);
                            //});

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
                            //var len = records.Length;

                            for (var i = recordsCount - 1; i >= 0; i--)
                            {
                                try
                                {
                                    var userId = Convert.ToInt32(records[i].cardNo);

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

                            //_logService.AddLog(logs).ConfigureAwait(false);
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

                            Task.WaitAll(logInsertionTasks.ToArray());
                            Task.Run(() =>
                            {
                                for (var i = 0; i < logs.Count;)
                                {
                                    var log = logs[i];
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{++i}/{recordsCount}");
                                }
                            }, CancellationToken);

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
                //if (result != 0)
                //    return new ResultViewModel
                //    { Id = _deviceInfo.DeviceId, Validate = 1, Message = recordsCount.ToString() };

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
                                    var userId = Convert.ToInt32(records[i].cardNo);

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
                                for (var i = 0; i < logs.Count;)
                                {
                                    var log = logs[i];
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{++i}/{count}");
                                }
                            }, CancellationToken);

                            //Task.Run(() =>
                            //{
                            //});

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

                //var userId = 0;
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
                                    var userId = Convert.ToInt32(records[i].cardNo);

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
                            //_logService.AddLog(logs).ConfigureAwait(false);

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

                            Task.WaitAll(logInsertionTasks.ToArray());

                            Task.Run(() =>
                            {
                                for (var i = 0; i < logs.Count;)
                                {
                                    var log = logs[i];
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{++i}/{recordsCount}");
                                }
                            }, CancellationToken);

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

                //var userId = 0;
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
                                    var userId = Convert.ToInt32(deviceLogs[i].userId);

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
                            //_logService.AddLog(logs).ConfigureAwait(false);

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

                            Task.WaitAll(logInsertionTasks.ToArray());

                            Task.Run(() =>
                            {
                                for (var i = 0; i < logs.Count;)
                                {
                                    var log = logs[i];
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{++i}/{recordsCount}");
                                }
                            }, CancellationToken);

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

                //var userId = 0;
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
                                    var userId = Convert.ToInt32(records[i].cardNo);

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
                            //_logService.AddLog(logs).ConfigureAwait(false);

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

                            Task.WaitAll(logInsertionTasks.ToArray());

                            Task.Run(() =>
                            {
                                for (var i = 0; i < logs.Count;)
                                {
                                    var log = logs[i];
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{++i}/{recordsCount}");
                                }
                            }, CancellationToken);

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

                //var userId = 0;
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
                                    var userId = Convert.ToInt32(records[i].cardNo);

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
                            //_logService.AddLog(logs).ConfigureAwait(false);

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

                            Task.WaitAll(logInsertionTasks.ToArray());

                            Task.Run(() =>
                            {
                                for (var i = 0; i < logs.Count;)
                                {
                                    var log = logs[i];
                                    _logger.Debug($@"<--
                                    +TerminalID:{_deviceInfo.Code}
                                    +UserID:{log.UserId}
                                    +DateTime:{log.LogDateTime}
                                    +AuthType:{log.MatchingType}
                                    +Progress:{++i}/{recordsCount}");
                                }
                            }, CancellationToken);

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
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, exception.Message);
            }
        }
    }
}