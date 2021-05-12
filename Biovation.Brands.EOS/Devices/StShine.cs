using Biovation.Brands.EOS.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using EosClocks;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Log = Biovation.Domain.Log;
using Logger = Biovation.CommonClasses.Logger;

namespace Biovation.Brands.EOS.Devices
{
    /// <summary>
    /// برای ساعت ST-Pro
    /// </summary>
    /// <seealso cref="Device" />
    public class StShineDevice : Device, IDisposable
    {
        private Clock _clock;
        private int _counter;
        private readonly ProtocolType _protocolType;
        private readonly object _clockInstantiationLock = new object();
        private Timer _fixDaylightSavingTimer;

        private readonly ILogger _logger;
        private readonly DeviceBasicInfo _deviceInfo;
        private readonly LogService _logService;

        private readonly RestClient _restClient;
        private readonly TaskService _taskService;
        private readonly DeviceBrands _deviceBrands;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private const int _maxRecordCount = 350000;

        public StShineDevice(ProtocolType protocolType, DeviceBasicInfo deviceInfo, LogService logService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, RestClient restClient, Dictionary<uint, Device> onlineDevices, ILogger logger, TaskService taskService, DeviceBrands deviceBrands)
         : base(deviceInfo, logEvents, logSubEvents, eosCodeMappings)
        {
            _logService = logService;
            _restClient = restClient;
            _deviceInfo = deviceInfo;
            _protocolType = protocolType;
            _onlineDevices = onlineDevices;
            _taskService = taskService;
            _deviceBrands = deviceBrands;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;

            TotalLogCount = 250000;
            _logger = logger.ForContext<StShineDevice>();
        }


        /// <summary>
        /// <En>Add new device to database.</En>
        /// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            lock (_onlineDevices)
            {
                if (_onlineDevices.ContainsKey(_deviceInfo.Code))
                {
                    _onlineDevices[_deviceInfo.Code].Disconnect();
                    _onlineDevices.Remove(_deviceInfo.Code);

                    var connectionStatus = new ConnectionStatus
                    {
                        DeviceId = _deviceInfo.DeviceId,
                        IsConnected = false
                    };

                    try
                    {
                        var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                        restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                        _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                        _logService.AddLog(new Log
                        {
                            DeviceId = _deviceInfo.DeviceId,
                            DeviceCode = _deviceInfo.Code,
                            LogDateTime = DateTime.Now,
                            EventLog = LogEvents.Disconnect
                        });
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            }

            var isConnect = IsConnected();
            if (!isConnect) return false;

            var setDateTimeResult = SetDateTime();
            if (!setDateTimeResult)
                _logger.Warning("Could not set the time of device {deviceCode}", _deviceInfo.Code);

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

            _taskService.ProcessQueue(_deviceBrands.Eos, _deviceInfo.DeviceId).ConfigureAwait(false);

            Valid = true;
            Task.Run(() => { ReadOnlineLog(Token); }, Token);
            return true;
        }

        private bool SetDateTime()
        {
            lock (_deviceInfo)
                if (!_deviceInfo.TimeSync)
                    return true;

            for (var i = 0; i < 5; i++)
            {
                try
                {
                    lock (_clock)
                        _clock.SetDateTime(DateTime.Now);

                    return true;
                }
                catch (Exception exception)
                {
                    _logger.Debug(exception, exception.Message);
                    Thread.Sleep(++i * 200);
                }
            }

            return false;
        }

        private void FixDaylightSavingTimer_Elapsed(object state)
        {
            SetDateTime();
        }

        private bool IsConnected()
        {
            var connection = ConnectionFactory.CreateTCPIPConnection(_deviceInfo.IpAddress, _deviceInfo.Port, 1000, 500, 500);

            lock (_clockInstantiationLock)
                _clock = new Clock(connection, ProtocolType.Hdlc, 1, _protocolType);

            lock (_clock)
                if (_clock.TestConnection())
                {
                    _logger.Information("Successfully connected to device {deviceCode} --> IP: {deviceIpAddress}", _deviceInfo.Code, _deviceInfo.IpAddress);
                    return true;
                }

            while (true)
            {
                _logger.Debug("Could not connect to device {deviceCode} --> IP: {deviceIpAddress}", _deviceInfo.Code, _deviceInfo.IpAddress);

                Thread.Sleep(10000);
                _logger.Debug("Retrying connect to device {deviceCode} --> IP: {deviceIpAddress}", _deviceInfo.Code, _deviceInfo.IpAddress);

                lock (_clock)
                    if (_clock.TestConnection())
                    {
                        _logger.Information("Successfully connected to device {deviceCode} --> IP: {deviceIpAddress}", _deviceInfo.Code, _deviceInfo.IpAddress);
                        return true;
                    }
            }

        }
        public virtual ResultViewModel ReadOnlineLog(object token)
        {
            Thread.Sleep(1000);
            try
            {
                string eosDeviceType;
                lock (_clock)
                    eosDeviceType = _clock.GetModel();

                _logger.Debug($"--> Retrieving Log from Terminal : {_deviceInfo.Code} Device type: {eosDeviceType}");

                bool deviceConnected;

                lock (_clock)
                    deviceConnected = _clock.TestConnection();

                while (deviceConnected && Valid)
                {
                    try
                    {
                        bool newRecordExists;

                        lock (_clock)
                            newRecordExists = !_clock.IsEmpty();

                        while (newRecordExists && Valid)
                        {
                            var test = true;
                            var exceptionTester = false;
                            while (test && Valid)
                            {
                                ClockRecord record = null;

                                try
                                {
                                    while (record == null)
                                    {
                                        lock (_clock)
                                            record = (ClockRecord)_clock.GetRecord();
                                        Thread.Sleep(300);
                                    }

                                    //_eosServer.Count++;
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        if (ex is InvalidRecordException ||
                                            ex is InvalidDataInRecordException)
                                        {
                                            var badRecordRawData = ex.Data["RecordRawData"].ToString();
                                            if (ex is InvalidDataInRecordException)
                                            {
                                                _logger.Debug("Clock " + _deviceInfo.Code + ": " + "Bad record: " + badRecordRawData);
                                            }

                                            if (badRecordRawData != "")
                                            {
                                                try
                                                {
                                                    var year = Convert.ToInt32(badRecordRawData.Substring(24, 2)) + 1300;
                                                    var month = Convert.ToInt32(badRecordRawData.Substring(19, 2));
                                                    var day = Convert.ToInt32(badRecordRawData.Substring(21, 2));
                                                    var hour = Convert.ToInt32(badRecordRawData.Substring(15, 2));
                                                    var minute = Convert.ToInt32(badRecordRawData.Substring(17, 2));
                                                    var userId = Convert.ToInt32(badRecordRawData.Substring(6, 8));

                                                    var gregorianDateOfRec = new DateTime(year, month, day, hour, minute, 10, new PersianCalendar());

                                                    var receivedLog = new Log
                                                    {
                                                        LogDateTime = gregorianDateOfRec,
                                                        UserId = userId,
                                                        DeviceId = _deviceInfo.DeviceId,
                                                        DeviceCode = _deviceInfo.Code,
                                                        InOutMode = _deviceInfo.DeviceTypeId,
                                                        //RawData = generatedRecord,
                                                        EventLog = LogEvents.Authorized,
                                                        SubEvent = LogSubEvents.Normal,
                                                        TnaEvent = 0,
                                                    };

                                                    //var logService = new EOSLogService();
                                                    _logService.AddLog(receivedLog);
                                                    test = false;
                                                    _logger.Information($@"<--
   +TerminalID:{_deviceInfo.Code}
   +UserID:{userId}
   +DateTime:{receivedLog.LogDateTime}");
                                                }
                                                catch (Exception)
                                                {
                                                    _logger.Debug("Error in parsing bad record.");
                                                }
                                            }

                                            if (!(ex is InvalidRecordException))
                                                _counter++;
                                            if (_counter == 4)
                                            {
                                                test = false;
                                            }
                                        }
                                        else
                                        {
                                            if (ex is InvalidRecordException)
                                                exceptionTester = true;
                                            else
                                                _logger.Debug(ex, "Clock " + _deviceInfo.Code);
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        _logger.Debug(exception, "Clock " + _deviceInfo.Code);
                                    }
                                }

                                try
                                {
                                    if (record != null)
                                    {
                                        var receivedLog = new Log
                                        {
                                            LogDateTime = record.DateTime,
                                            UserId = (int)record.ID,
                                            DeviceId = _deviceInfo.DeviceId,
                                            DeviceCode = _deviceInfo.Code,
                                            SubEvent = EosCodeMappings.GetLogSubEventGenericLookup(record.RecType1),
                                            //RawData = new string(record.RawData.Where(c => !char.IsControl(c)).ToArray()),
                                            InOutMode = _deviceInfo.DeviceTypeId,
                                            EventLog = LogEvents.Authorized,
                                            TnaEvent = 0,
                                        };

                                        _logService.AddLog(receivedLog);
                                        test = false;
                                        _logger.Information($@"<--
   +TerminalID:{_deviceInfo.Code}
   +UserID:{receivedLog.UserId}
   +DateTime:{receivedLog.LogDateTime}");

                                        lock (_clock)
                                            _clock.NextRecord();
                                        //Thread.Sleep(200);
                                    }
                                    else
                                    {
                                        if (!exceptionTester)
                                        {
                                            _logger.Debug("Null record.");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Debug(ex, "Clock " + _deviceInfo.Code + ": " +
                                        "Error while Inserting Data to Attendance . record: " + record);
                                }
                            }

                            lock (_clock)
                                newRecordExists = !_clock.IsEmpty();
                        }

                        Thread.Sleep(2000);
                    }
                    catch (Exception exception)
                    {
                        _logger.Debug(exception, exception.Message);
                    }

                    lock (_clock)
                        deviceConnected = _clock.TestConnection();
                }

                //_clock?.Disconnect();
                // _clock?.Dispose();
                //Disconnect();
                if (Valid)
                    Connect();

                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 1, Message = "0" };

            }

            catch (Exception ex)
            {
                _logger.Debug(ex, "Clock " + _deviceInfo.Code);
            }

            _logger.Debug("Connection fail. Cannot connect to device: " + _deviceInfo.Code + ", IP: " + _deviceInfo.IpAddress);
            //}


            //_clock?.Disconnect();
            //_clock?.Dispose();
            if (Valid)
            {
                Connect();
            }

            //EosServer.IsRunning[(uint)_deviceInfo.Code] = false;
            return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };

        }
        public override bool Disconnect()
        {
            Valid = false;
            lock (_clock)
            {
                _clock?.Disconnect();
                _clock?.Dispose();
            }

            return true;
        }

        public override bool DeleteUser(uint userCode)
        {
            bool deviceConnected;

            lock (_clock)
                deviceConnected = _clock.TestConnection();
            if (!deviceConnected) return false;

            lock (_clock)
            {
                try
                {
                    var isConnectToSensor = ConnectToSensor();

                    if (!isConnectToSensor)
                    {
                        _logger.Debug($"Could not connect to device {_deviceInfo.DeviceId} sensor.");
                        return false;
                    }

                    var userId = Convert.ToInt32(userCode);
                    List<byte[]> userFingerTemplates = null;

                    for (var i = 0; i < 3;)
                    {
                        try
                        {
                            userFingerTemplates = _clock.Sensor.GetUserTemplates(userId);
                            if (userFingerTemplates != null && userFingerTemplates.Count > 0)
                                break;
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            Thread.Sleep(100 * ++i);
                        }
                    }

                    if (userFingerTemplates == null || userFingerTemplates.Count == 0)
                    {
                        var deleteUser = false;
                        _logger.Debug($"User {userId} may not be on device {_deviceInfo.DeviceId}");
                        try
                        {
                            DisconnectFromSensor();
                            deleteUser = _clock.DeleteUser(userCode);
                        }
                        catch (Exception innerException)
                        {
                            _logger.Debug(innerException, innerException.Message);
                        }

                        return deleteUser;
                    }

                    var deletedTemplatesCount = 0;

                    foreach (var fingerTemplate in userFingerTemplates)
                    {
                        for (var j = 0; j < 5; j++)
                        {
                            try
                            {
                                var templateDeletionResult = _clock.Sensor.DeleteTemplate(userId, fingerTemplate);
                                if (templateDeletionResult.ScanState != ScanState.Success &&
                                    templateDeletionResult.ScanState != ScanState.Scan_Success) continue;
                                _logger.Debug($"A finger print of user {userId} deleted");
                                deletedTemplatesCount++;
                                break;

                                //if (templateDeletionResult.ScanState == ScanState.Not_Found)
                                //    _clock.Sensor.DeleteTemplate(userId, i);
                            }
                            catch (Exception exception)
                            {
                                _logger.Warning(exception, exception.Message);
                            }
                        }
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        for (var index = 0; index < 10; index++)
                        {
                            try
                            {
                                var templateDeletionResult = _clock.Sensor.DeleteTemplate(userId, index);
                                if (templateDeletionResult.ScanState != ScanState.Success &&
                                    templateDeletionResult.ScanState != ScanState.Scan_Success) continue;
                                Logger.Log($"A finger print of user {userId} deleted");
                                deletedTemplatesCount++;
                            }
                            catch (Exception exception)
                            {
                                _logger.Warning(exception, exception.Message);
                            }
                        }

                        if (deletedTemplatesCount >= userFingerTemplates.Count)
                            break;
                    }

                    //_clock.Sensor.DeleteByID(userId);

                    DisconnectFromSensor();

                    var deletionResult = false;
                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            deletionResult = _clock.DeleteUser(userCode);
                            break;
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                        }
                    }

                    return deletionResult;
                }
                catch (Exception exception)
                {
                    _logger.Warning(exception, exception.Message);
                }
                //finally
                //{
                //    _clock.DisconnectFromSensor();
                //}
            }

            return false;
        }

        public override bool TransferUser(User user)
        {
            lock (_clock)
            {
                var userTemplates = user.FingerTemplates?.Where(fingerTemplate =>
                    fingerTemplate.FingerTemplateType.Code == (_protocolType == ProtocolType.Suprema
                        ? FingerTemplateTypes.SU384Code
                        : FingerTemplateTypes.VX10Code)).ToList();

                if (userTemplates is null || !userTemplates.Any())
                    return true;

                var isConnectToSensor = false;
                try
                {
                    bool addUserResult;
                    try
                    {
                        var addUserHdlcResult = _clock.AddUpdateUser(new EmployeeRecord
                        {
                            CardId = user.Code.ToString(),
                            EmployeeId = user.Code,
                            FingerprintCount = 0,
                            FirstName = "",
                            LastName = "",
                            SensorUserId = 0,
                            SectionId = 0,
                            Templates = new List<TemplateRecord>()
                        });

                        addUserResult = addUserHdlcResult == HdlcResults.Success;
                    }
                    catch (Exception exception)
                    {
                        _logger.Debug(exception, exception.Message);
                        return false;
                    }

                    if (!addUserResult)
                        return false;

                    isConnectToSensor = ConnectToSensor();

                    if (!isConnectToSensor)
                    {
                        _logger.Debug($"Could not connect to device {_deviceInfo.DeviceId} sensor.");
                        return false;
                    }

                    for (var index = 0; index < userTemplates.Count; index += 2)
                    {
                        var sendTemplateResult = false;
                        var firstFingerTemplate = userTemplates[index];
                        var checkExistenceResult = new SensorRecord { ScanState = ScanState.Unsupported };

                        for (var i = 0; i < 5;)
                        {
                            try
                            {
                                try
                                {
                                    _clock.Connection.ClearInputBuffer(false);
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }

                                checkExistenceResult = _clock.Sensor.EnrollByTemplate((int)user.Code,
                                    firstFingerTemplate.Template, EnrollOptions.Check_Finger);
                                sendTemplateResult = checkExistenceResult.ScanState == ScanState.Success ||
                                                     checkExistenceResult.ScanState == ScanState.Scan_Success ||
                                                     checkExistenceResult.ScanState == ScanState.Data_Ok || checkExistenceResult.ID > 0;
                                if (checkExistenceResult.ScanState != ScanState.Exist_Finger) break;
                                _logger.Debug($"The {index + 1} of {userTemplates.Count} finger template of user {user.Code} exists on device");
                                break;
                            }
                            catch (Exception exception)
                            {
                                _logger.Warning(exception, exception.Message);
                                Thread.Sleep(++i * 200);
                            }
                        }

                        if (checkExistenceResult.ScanState == ScanState.Exist_Finger) continue;

                        if (userTemplates.Count > index + 1)
                        {
                            var secondFingerTemplate = userTemplates[index + 1];
                            for (var i = 0; i < 5;)
                            {
                                try
                                {
                                    try
                                    {
                                        _clock.Connection.ClearInputBuffer(false);
                                    }
                                    catch (Exception)
                                    {
                                        //ignore
                                    }

                                    var templateEnrollResult = _clock.Sensor.EnrollByTemplate((int)user.Code,
                                        secondFingerTemplate.Template, EnrollOptions.Add_New);
                                    sendTemplateResult = templateEnrollResult.ScanState == ScanState.Success ||
                                                         templateEnrollResult.ScanState == ScanState.Scan_Success ||
                                                         templateEnrollResult.ScanState == ScanState.Data_Ok ||
                                                         templateEnrollResult.ID > 0;

                                    _logger.Debug(
                                        $"The {index + 2} finger template of user {user.Code} has been sent to device");
                                    break;
                                }
                                catch (Exception exception)
                                {
                                    _logger.Warning(exception, exception.Message);
                                    Thread.Sleep(++i * 200);
                                }
                            }
                        }

                        if (!sendTemplateResult)
                            return false;
                    }

                    return true;
                }
                catch (Exception exception)
                {
                    _logger.Debug(exception, exception.Message);
                }
                finally
                {
                    try
                    {
                        if (isConnectToSensor)
                        {
                            DisconnectFromSensor();
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Debug(exception, exception.Message);
                    }
                }
            }

            return false;
        }

        internal override User GetUser(uint userId)
        {
            lock (_clock)
            {
                try
                {
                    var isConnectToSensor = ConnectToSensor();

                    if (!isConnectToSensor)
                    {
                        _logger.Debug($"Could not connect to device {_deviceInfo.DeviceId} sensor.");
                        return new User();
                    }

                    //var intId = checked((int)userId);
                    //  var x = _clock.Sensor.GetUserIDList();

                    List<byte[]> fingerTemplates = null;

                    for (var i = 0; i < 5;)
                    {
                        try
                        {
                            fingerTemplates = _clock.Sensor.GetUserTemplates((int)userId);
                            break;
                        }
                        catch (Exception exception)
                        {
                            _logger.Debug(exception, exception.Message);
                            Thread.Sleep(++i * 200);
                        }
                    }

                    if (fingerTemplates is null || fingerTemplates.Count <= 0)
                    {
                        _logger.Debug($"Error in retrieving user {userId} from device {_deviceInfo.DeviceId}, user may be not available on device.");
                        return null;
                    }

                    var retrievedUser = new User
                    {
                        Code = userId,
                        FingerTemplates = new List<FingerTemplate>(),
                        StartDate = default,
                        EndDate = default
                    };

                    for (var i = 0; i < fingerTemplates.Count; i += 2)
                    {
                        var firstTemplateBytes = fingerTemplates[i];

                        var fingerTemplate = new FingerTemplate
                        {
                            FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                            FingerTemplateType = _protocolType == ProtocolType.Suprema ? _fingerTemplateTypes.SU384 : _fingerTemplateTypes.VX10,
                            UserId = retrievedUser.Id,
                            Template = firstTemplateBytes,
                            CheckSum = firstTemplateBytes.Sum(b => b),
                            Size = firstTemplateBytes.ToList()
                                .LastIndexOf(firstTemplateBytes.LastOrDefault(b => b != 0)),
                            Index = i / 2,
                            CreateAt = DateTime.Now,
                            TemplateIndex = 0
                        };

                        retrievedUser.FingerTemplates.Add(fingerTemplate);

                        if (fingerTemplates.Count <= i + 1)
                            continue;

                        var secondTemplateBytes = fingerTemplates[i + 1];

                        var secondFingerTemplateSample = new FingerTemplate
                        {
                            FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                            FingerTemplateType = _protocolType == ProtocolType.Suprema ? _fingerTemplateTypes.SU384 : _fingerTemplateTypes.VX10,
                            UserId = retrievedUser.Id,
                            Template = secondTemplateBytes,
                            CheckSum = secondTemplateBytes.Sum(b => b),
                            Size = secondTemplateBytes.ToList()
                                .LastIndexOf(secondTemplateBytes.LastOrDefault(b => b != 0)),
                            Index = i / 2,
                            EnrollQuality = 0,
                            SecurityLevel = 0,
                            Duress = true,
                            CreateAt = DateTime.Now,
                            TemplateIndex = 1
                        };

                        retrievedUser.FingerTemplates.Add(secondFingerTemplateSample);
                    }

                    return retrievedUser;

                    //  retrievedUser.SetStartDateFromTicks((int)(new DateTime(1970, 1, 1).AddSeconds(userHdr.startDateTime)).Ticks);
                    //  retrievedUser.SetEndDateFromTicks((int)(new DateTime(1970, 1, 1).AddSeconds(userHdr.expireDateTime)).Ticks);
                }
                catch (Exception exception)
                {
                    _logger.Debug(exception, exception.Message);
                    return null;
                }
                finally
                {
                    try
                    {
                        DisconnectFromSensor();
                    }
                    catch (Exception exception)
                    {
                        _logger.Debug(exception, exception.Message);
                    }
                }
            }
        }


        public override List<User> GetAllUsers(bool embedTemplates = false)
        {
            _logger.Information("Getting user list of device:{deviceId}, with embed templates option set to:{embedTemplatesToUserList}", _deviceInfo.DeviceId, embedTemplates);
            var usersList = new List<User>();

            lock (_clock)
            {
                var isConnectToSensor = false;
                try
                {
                    var users = new List<long>();
                    for (var i = 0; i < 5;)
                    {
                        try
                        {
                            users = _clock.GetUserList();
                            break;
                        }
                        catch (Exception exception)
                        {
                            _logger.Warning(exception, exception.Message);
                            Thread.Sleep(++i * 200);
                        }
                    }

                    usersList.AddRange(users.Select(userCode => new User { Code = userCode }));
                    usersList = usersList.DistinctBy(user => user.Code).ToList();

                    _logger.Debug("Successfully fetched {deviceUserCount} users from device:{deviceId}", usersList.Count, _deviceInfo.DeviceId);

                    if (embedTemplates)
                    {
                        _logger.Debug("Connecting to sensor of device:{deviceId}", _deviceInfo.DeviceId);

                        isConnectToSensor = ConnectToSensor();

                        if (!isConnectToSensor)
                        {
                            _logger.Debug($"Could not connect to device {_deviceInfo.DeviceId} sensor.");
                            return usersList;
                        }

                        _logger.Debug("Successfully connected to sensor of device:{deviceId}", _deviceInfo.DeviceId);

                        foreach (var user in usersList)
                        {
                            List<byte[]> fingerTemplates;
                            _logger.Debug("Fetching user templates of user with code:{userCode} from device:{deviceId}", user.Code, _deviceInfo.DeviceId);

                            try
                            {
                                fingerTemplates = _clock.Sensor.GetUserTemplates((int)user.Code);
                            }
                            catch (Exception exception)
                            {
                                _logger.Debug(exception, exception.Message);
                                _logger.Debug($"Error in retrieving user {user.Code} from device {_deviceInfo.DeviceId}, user may be not available on device.");
                                continue;
                            }

                            if (fingerTemplates is null || fingerTemplates.Count <= 0) continue;

                            user.FingerTemplates = new List<FingerTemplate>();
                            for (var i = 0; i < fingerTemplates.Count; i += 2)
                            {
                                var firstTemplateBytes = fingerTemplates[i];

                                var fingerTemplate = new FingerTemplate
                                {
                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                                    FingerTemplateType = _protocolType == ProtocolType.Suprema ? _fingerTemplateTypes.SU384 : _fingerTemplateTypes.VX10,
                                    Template = firstTemplateBytes,
                                    CheckSum = firstTemplateBytes.Sum(b => b),
                                    Size = firstTemplateBytes.ToList()
                                        .LastIndexOf(firstTemplateBytes.LastOrDefault(b => b != 0)),
                                    Index = i / 2,
                                    CreateAt = DateTime.Now,
                                    TemplateIndex = 0
                                };

                                user.FingerTemplates.Add(fingerTemplate);

                                if (fingerTemplates.Count <= i + 1)
                                    continue;

                                var secondTemplateBytes = fingerTemplates[i + 1];

                                var secondFingerTemplateSample = new FingerTemplate
                                {
                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                                    FingerTemplateType = _protocolType == ProtocolType.Suprema ? _fingerTemplateTypes.SU384 : _fingerTemplateTypes.VX10,
                                    Template = secondTemplateBytes,
                                    CheckSum = secondTemplateBytes.Sum(b => b),
                                    Size = secondTemplateBytes.ToList()
                                        .LastIndexOf(secondTemplateBytes.LastOrDefault(b => b != 0)),
                                    Index = i / 2,
                                    EnrollQuality = 0,
                                    SecurityLevel = 0,
                                    Duress = true,
                                    CreateAt = DateTime.Now,
                                    TemplateIndex = 1
                                };

                                user.FingerTemplates.Add(secondFingerTemplateSample);
                            }

                            _logger.Debug("Successfully fetched {userTemplateCount} templates for user with code:{userCode} from device:{deviceId}", fingerTemplates.Count, user.Code, _deviceInfo.DeviceId);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.Debug(exception, exception.Message);
                }
                finally
                {
                    try
                    {
                        if (embedTemplates && isConnectToSensor)
                        {
                            DisconnectFromSensor();
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Debug(exception, exception.Message);
                    }
                }
            }

            return usersList;
        }

        public bool DeleteAllUser()
        {
            lock (_clock)
            {
                try
                {
                    if (!ConnectToSensor()) return false;
                    _clock.Sensor.DeleteAllTemplates();
                    _clock.Sensor.DeleteAllUsers();
                    return true;
                }
                catch (Exception exception)
                {
                    _logger.Debug(exception, exception.Message);
                }
                finally
                {
                    DisconnectFromSensor();
                }
            }

            return false;
        }

        private bool ConnectToSensor()
        {
            var isConnectToSensor = false;

            try
            {
                isConnectToSensor = _clock.ConnectToSensor();
            }
            catch
            {
                //ignore
            }

            for (var i = 0; i < 5; i++)
            {
                if (isConnectToSensor)
                    break;

                Thread.Sleep(500);
                try
                {
                    isConnectToSensor = _clock.ConnectToSensor();
                }
                catch
                {
                    //ignore
                }
            }

            if (isConnectToSensor)
                _logger.Debug("Successfully connected to sensor of device:{deviceId}", _deviceInfo.DeviceId);

            return isConnectToSensor;
        }

        private void DisconnectFromSensor()
        {
            var disconnectedFromSensor = false;

            try
            {
                disconnectedFromSensor = _clock.DisconnectFromSensor();
            }
            catch
            {
                //ignore
            }

            for (var i = 0; i < 5; i++)
            {
                if (disconnectedFromSensor)
                    break;

                Thread.Sleep((i + 1) * 200);
                try
                {
                    disconnectedFromSensor = _clock.DisconnectFromSensor();
                }
                catch
                {
                    //ignore
                }
            }

            _logger.Debug(
                disconnectedFromSensor
                    ? "Successfully disconnected from sensor of device:{deviceId}"
                    : "Could not disconnect from sensor of device:{deviceId}", _deviceInfo.DeviceId);
        }

        //public override ResultViewModel ReadOfflineLogInPeriod(object cancellationToken, DateTime? startTime,
        //DateTime? endTime,
        //bool saveFile = false)
        //{
        //    //lock (_clock)
        //    //{
        //    //    _logger.Information("dumping device");
        //    //    var records = _clock.Dump(TotalLogCount, (DateTime)startTime, (DateTime)endTime, out var badRecords);
        //    //    _logger.Information("Dumping finished, {recordsCount} records and {badRecordsCount} bad records retrieved", records.Count, badRecords.Count);
        //    //    return new ResultViewModel { Success = true };
        //    //}

        //    var invalidTime = false;
        //    if (startTime is null || startTime < new DateTime(1921, 3, 21) || startTime > new DateTime(2021, 3, 19))
        //    {
        //        startTime = new DateTime(1921, 3, 21);
        //        invalidTime = true;
        //    }

        //    if (endTime is null || endTime > new DateTime(2021, 3, 19) || endTime < new DateTime(1921, 3, 21))
        //    {
        //        endTime = new DateTime(2021, 3, 19);
        //        invalidTime = true;
        //    }

        //    if (invalidTime)
        //        Logger.Log("The chosen Time Period is wrong.");

        //    Thread.Sleep(1000);
        //    string eosDeviceType;
        //    lock (_clock)
        //        eosDeviceType = _clock.GetModel();

        //    lock (_onlineDevices)
        //    {
        //        Logger.Log($"--> Retrieving Log from Terminal : {_deviceInfo.Code} Device type: {eosDeviceType}");
        //    }

        //    bool deviceConnected;

        //    lock (_clock)
        //        deviceConnected = _clock.TestConnection();

        //    var writePointer = -1;
        //    var successSetPointer = false;

        //    for (var i = 0; i < 5; i++)
        //    {
        //        try
        //        {
        //            lock (_clock)
        //            {
        //                Thread.Sleep(500);
        //                writePointer = _clock.GetWritePointer();
        //            }
        //            break;
        //        }
        //        catch (Exception exception)
        //        {
        //            Logger.Log(exception, exception.Message);
        //            Thread.Sleep(++i * 100);
        //        }

        //    }

        //    if (deviceConnected && Valid && writePointer != -1)
        //    {
        //        var initialReadPointer = writePointer;
        //        _logger.Information("The initial pointer is: {initialPointer}", initialReadPointer);
        //        for (var i = 0; i < 5; i++)
        //        {
        //            try
        //            {
        //                lock (_clock)
        //                {
        //                    Thread.Sleep(500);
        //                    initialReadPointer = _clock.GetReadPointer();
        //                    _logger.Information("The Read Pointer is: {initialPointer}", initialReadPointer);
        //                }
        //                break;
        //            }
        //            catch (Exception exception)
        //            {
        //                Logger.Log(exception, exception.Message);
        //                Thread.Sleep(++i * 100);
        //            }

        //        }

        //        for (var i = 0; i < 5; i++)
        //        {
        //            try
        //            {
        //                byte[] command;
        //                (command = new byte[1])[0] = 32;
        //                lock (_clock)
        //                {
        //                    _clock.Connection.SendCommandAndGetHdlcResult(command);
        //                }

        //                break;
        //            }
        //            catch (Exception exception)
        //            {
        //                Logger.Log(exception, exception.Message);
        //                Thread.Sleep(++i * 100);
        //            }
        //        }



        //        for (var i = 0; i < 5; i++)
        //        {
        //            try
        //            {
        //                lock (_clock)
        //                {
        //                    successSetPointer = _clock.SetReadPointer(9000);
        //                    //_clock.Connection.SendCommandAndGetResult("AUR=1", "\r");
        //                    Thread.Sleep(500);
        //                    var clockRecord = (ClockRecord)_clock.GetRecord();
        //                    _logger.Debug("Before 1 set pointer: {dateTime}", clockRecord.DateTime);
        //                    Thread.Sleep(500);
        //                }

        //                break;
        //            }
        //            catch (Exception exception)
        //            {
        //                Logger.Log(exception, exception.Message);
        //                Thread.Sleep(++i * 100);
        //            }
        //        }



        //        for (var i = 0; i < 5; i++)
        //        {
        //            try
        //            {
        //                byte[] command;
        //                (command = new byte[1])[0] = 32;
        //                lock (_clock)
        //                {
        //                    _clock.Connection.SendCommandAndGetHdlcResult(command);
        //                }

        //                break;
        //            }
        //            catch (Exception exception)
        //            {
        //                Logger.Log(exception, exception.Message);
        //                Thread.Sleep(++i * 100);
        //            }
        //        }


        //        for (var i = 0; i < 5; i++)
        //        {
        //            try
        //            {
        //                lock (_clock)
        //                {
        //                    successSetPointer = _clock.SetReadPointer(10000);
        //                    //_clock.Connection.SendCommandAndGetResult("AUR=100", "\r");
        //                    Thread.Sleep(500);
        //                    var clockRecord = (ClockRecord)_clock.GetRecord();
        //                    _logger.Debug("Before 2 set pointer: {dateTime}", clockRecord.DateTime);
        //                    Thread.Sleep(500);
        //                }

        //                break;
        //            }
        //            catch (Exception exception)
        //            {
        //                Logger.Log(exception, exception.Message);
        //                Thread.Sleep(++i * 100);
        //            }
        //        }


        //        var rightBoundary = writePointer;
        //        var leftBoundary = writePointer + 2;

        //        for (var i = 0; i < 5; i++)
        //        {
        //            try
        //            {
        //                lock (_clock)
        //                {
        //                    Thread.Sleep(500);
        //                    successSetPointer = _clock.SetReadPointer(leftBoundary);
        //                }
        //                break;
        //            }
        //            catch (Exception exception)
        //            {
        //                Logger.Log(exception, exception.Message);
        //                Thread.Sleep(++i * 100);
        //            }

        //        }

        //        ClockRecord record = null;
        //        for (var i = 0; i < 5; i++)
        //        {
        //            try
        //            {
        //                lock (_clock)
        //                {
        //                    Thread.Sleep(500);
        //                    record = (ClockRecord)_clock.GetRecord();
        //                    _logger.Debug("First record: {dateTime}", record.DateTime);
        //                }
        //                break;
        //            }
        //            catch (Exception exception)
        //            {
        //                Logger.Log(exception, exception.Message);
        //                Thread.Sleep(++i * 100);
        //                if (i == 4)
        //                {
        //                    leftBoundary = 1;
        //                }
        //            }
        //        }
        //        if (record is null)
        //        {
        //            leftBoundary = 1;
        //        }

        //        if (successSetPointer)
        //        {
        //            for (var i = 0; i < 5; i++)
        //            {
        //                try
        //                {
        //                    lock (_clock)
        //                    {
        //                        Thread.Sleep(500);
        //                        var clockRecord = (ClockRecord)_clock.GetRecord();
        //                        _logger.Debug("second record: {dateTime}", clockRecord.DateTime);
        //                    }

        //                    break;
        //                }
        //                catch (Exception exception)
        //                {
        //                    Logger.Log(exception, exception.Message);
        //                    Thread.Sleep(++i * 100);
        //                }
        //            }


        //            (int, long) nearestIndex = (writePointer, new DateTime(DateTime.Today.Year + 10, 1, 1).Ticks);
        //            BinarySearch(leftBoundary, rightBoundary, Convert.ToDateTime(startTime), ref nearestIndex,
        //                (new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)), 0, false);

        //            if (nearestIndex.Item1 < initialReadPointer)
        //                for (var i = 0; i < 5; i++)
        //                {
        //                    try
        //                    {
        //                        lock (_clock)
        //                        {
        //                            Thread.Sleep(500);
        //                            successSetPointer = _clock.SetReadPointer(nearestIndex.Item1);
        //                        }
        //                        break;
        //                    }
        //                    catch (Exception exception)
        //                    {
        //                        Logger.Log(exception, exception.Message);
        //                        Thread.Sleep(++i * 100);
        //                    }

        //                }
        //        }

        //        if (!successSetPointer)
        //        {
        //            for (var i = 0; i < 5; i++)
        //            {
        //                try
        //                {
        //                    lock (_clock)
        //                    {
        //                        Thread.Sleep(500);
        //                        successSetPointer = _clock.SetReadPointer(initialReadPointer);
        //                    }
        //                    break;
        //                }
        //                catch (Exception exception)
        //                {
        //                    Logger.Log(exception, exception.Message);
        //                    Thread.Sleep(++i * 100);
        //                }

        //            }
        //        }

        //        return new ResultViewModel { Id = _deviceInfo.DeviceId, Success = successSetPointer, Code = Convert.ToInt32(TaskStatuses.DoneCode) };
        //    }

        //    return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0", Code = Convert.ToInt32(TaskStatuses.FailedCode) };

        //}


        public override ResultViewModel ReadOfflineLogInPeriod(object cancellationToken, DateTime? startTime,
           DateTime? endTime,
           bool saveFile = false)
        {
            //lock (_clock)
            //{
            //    _logger.Information("dumping device");
            //    var records = _clock.Dump(TotalLogCount, (DateTime)startTime, (DateTime)endTime, out var badRecords);
            //    _logger.Information("Dumping finished, {recordsCount} records and {badRecordsCount} bad records retrieved", records.Count, badRecords.Count);
            //    return new ResultViewModel { Success = true };
            //}

            var invalidTime = false;
            Logger.Log($"The datetime start with {startTime}");
            if (startTime is null ||
                startTime < new DateTime(DateTime.Now.Year - 2, DateTime.Now.Month, DateTime.Now.Day) ||
                startTime > DateTime.Now)
            {
                startTime = new DateTime(DateTime.Now.Year - 2, DateTime.Now.Month, DateTime.Now.Day);
                invalidTime = true;
            }

            if (endTime is null || endTime > DateTime.Now ||
                endTime < new DateTime(DateTime.Now.Year - 2, DateTime.Now.Month, DateTime.Now.Day))
            {
                //endTime = new DateTime(2021, 3, 19);
                invalidTime = true;
            }

            if (invalidTime)
                Logger.Log("The chosen Time Period is wrong.");

            Thread.Sleep(1000);
            string eosDeviceType;
            lock (_clock)
                eosDeviceType = _clock.GetModel();

            lock (_onlineDevices)
            {
                Logger.Log($"--> Retrieving Log from Terminal : {_deviceInfo.Code} Device type: {eosDeviceType} from {startTime}");
            }

            bool deviceConnected;

            lock (_clock)
                deviceConnected = _clock.TestConnection();

            var writePointer = -1;
            var successSetPointer = false;

            for (var i = 0; i < 5; i++)
            {
                try
                {
                    lock (_clock)
                    {
                        Thread.Sleep(500);
                        writePointer = _clock.GetWritePointer();
                    }

                    break;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, exception.Message);
                    Thread.Sleep(++i * 100);
                }

            }

            lock (_clock)

                if (deviceConnected && Valid && writePointer != -1)
                {
                    var initialReadPointer = writePointer;
                    Logger.Log($"The initial pointer is: {initialReadPointer}");

                    var leftBoundary = writePointer - 1;

                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            lock (_clock)
                            {
                                Thread.Sleep(500);
                                successSetPointer = _clock.SetReadPointer(leftBoundary);
                                if (!successSetPointer) continue;
                                var reconnect = Connect();
                                if (!reconnect) continue;
                            }

                            break;
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, exception.Message);
                            Thread.Sleep(++i * 100);
                        }

                    }

                    

                    Logger.Log(successSetPointer ? "Successfully set read pointer" : "FAILED in set read pointer");
                    if (successSetPointer)
                    {
                        var firstIndex = leftBoundary;
                        var clockRecord = new ClockRecord();
                        try
                        {
                            for (var i = 0; i < 5; i++)
                            {
                                try
                                {
                                    lock (_clock)
                                    {
                                        clockRecord = (ClockRecord)_clock.GetRecord();
                                    }
                                    break;
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception, exception.Message);
                                    Thread.Sleep(++i * 100);
                                }
                            }

                            Logger.Log($"First datetime {clockRecord.DateTime}");
                            var goalDateTime = new DateTime(startTime.Value.Year, startTime.Value.Month, startTime.Value.Day - 1);
                            var firstReturnedDateTime = EOSsearch(ref firstIndex, goalDateTime, 0, leftBoundary, clockRecord.DateTime);
                            if ( goalDateTime.Subtract(firstReturnedDateTime)> new TimeSpan(1,0,0,0) || goalDateTime.Subtract(firstReturnedDateTime) < new TimeSpan(-1,0,0,0))
                            {
                                var secondIndex = firstIndex;
                                var secondReturnedDateTime = EOSsearch(ref secondIndex, goalDateTime, leftBoundary, _maxRecordCount, clockRecord.DateTime);
                                if ((firstReturnedDateTime > goalDateTime && secondReturnedDateTime > goalDateTime && firstReturnedDateTime.Subtract(goalDateTime) < secondReturnedDateTime.Subtract(goalDateTime)) ||
                                    (firstReturnedDateTime < goalDateTime && secondReturnedDateTime < goalDateTime && goalDateTime.Subtract(firstReturnedDateTime) < goalDateTime.Subtract(secondReturnedDateTime)) ||
                                    (firstReturnedDateTime < goalDateTime && secondReturnedDateTime > goalDateTime && goalDateTime.Subtract(firstReturnedDateTime) < secondReturnedDateTime.Subtract(goalDateTime)) ||
                                    (firstReturnedDateTime > goalDateTime && secondReturnedDateTime < goalDateTime && firstReturnedDateTime.Subtract(goalDateTime) < goalDateTime.Subtract(secondReturnedDateTime)))
                                {
                                    for (var i = 0; i < 5; i++)
                                    {
                                        try
                                        {
                                            lock (_clock)
                                            {
                                                Thread.Sleep(500);
                                                successSetPointer = _clock.SetReadPointer(firstIndex);
                                                if (!successSetPointer) continue;
                                                var reconnect = Connect();
                                                if (!reconnect) continue;
                                            }

                                            break;
                                        }
                                        catch (Exception exception)
                                        {
                                            Logger.Log(exception, exception.Message);
                                            Thread.Sleep(++i * 100);
                                        }

                                    }
                                    Logger.Log($@"SUccessfully First Read Offline log from {firstReturnedDateTime} with index {firstIndex}");
                                }
                                else
                                {
                                    Logger.Log($@"SUccessfully Second Read Offline log from {secondReturnedDateTime} with index {secondIndex}");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }


                        //(int, long) nearestIndex = (writePointer, new DateTime(DateTime.Today.Year + 10, 1, 1).Ticks);
                        //BinarySearch(writePointer + 1, writePointer, Convert.ToDateTime(startTime), ref nearestIndex,
                        //    (new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)), 0, false);


                        //for (var i = 0; i < 5; i++)
                        //{
                        //    try
                        //    {
                        //        lock (_clock)
                        //        {
                        //            Thread.Sleep(500);
                        //            successSetPointer = _clock.SetReadPointer(nearestIndex.Item1);
                        //        }
                        //        break;
                        //    }
                        //    catch (Exception exception)
                        //    {
                        //        Logger.Log(exception, exception.Message);
                        //        Thread.Sleep(++i * 100);
                        //    }

                        //}

                        if (!successSetPointer)
                        {
                            for (var i = 0; i < 5; i++)
                            {
                                try
                                {
                                    lock (_clock)
                                    {
                                        Thread.Sleep(500);
                                        successSetPointer = _clock.SetReadPointer(initialReadPointer);
                                    }

                                    break;
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception, exception.Message);
                                    Thread.Sleep(++i * 100);
                                }
                            }
                        }

                        return new ResultViewModel
                        {
                            Id = _deviceInfo.DeviceId,
                            Success = successSetPointer,
                            Code = Convert.ToInt32(TaskStatuses.DoneCode)
                        };
                    }
                }

            return new ResultViewModel
            {
                Id = _deviceInfo.DeviceId,
                Validate = 0,
                Message = "0",
                Code = Convert.ToInt32(TaskStatuses.FailedCode)
            };
        }

        private void BinarySearch(int left, int right, DateTime goalDateTime, ref (int, long) nearestIndex, (DateTime, DateTime, DateTime) previousDateTimes, int previousmid, bool previousFlag)
        {
            if (Math.Abs(right - left) <= 1)
            {
                return;
            }

            _logger.Debug("Searching for appropriate log pointer value.");

            var successSetPointer = false;
            ClockRecord clockRecord = null;
            var flag = false;

            var interval = (right - left) > 0 ? (right - left) : TotalLogCount + (right - left);
            var mid = (left + interval / 2);

            _logger.Debug("The interval is: {interval} and the mid pointer is: {mid}", interval, mid);

            for (var i = 0; i < 5; i++)
            {
                try
                {
                    lock (_clock)
                    {
                        if (!successSetPointer)
                        {
                            Thread.Sleep(500);
                            successSetPointer = _clock.SetReadPointer(mid);
                            _logger.Debug("The read pointer is successfully set to {mid}", mid);
                        }
                        Thread.Sleep(500);
                        clockRecord = (ClockRecord)_clock.GetRecord();
                        _logger.Debug("The log date is: {dateTime}", clockRecord.DateTime);
                    }
                    break;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, exception.Message);
                    Thread.Sleep(++i * 100);
                }

            }

            if (clockRecord == null)
                BinarySearch(0, mid - 1, goalDateTime, ref nearestIndex, previousDateTimes, mid, false);
            else
            {

                if (previousDateTimes.Item1 != new DateTime(1900, 1, 1) &&
                    previousDateTimes.Item2 != new DateTime(1900, 1, 1) &&
                    previousDateTimes.Item3 != new DateTime(1900, 1, 1) &&
                    ((clockRecord.DateTime.Ticks - goalDateTime.Ticks) >
                     (previousDateTimes.Item2.Ticks - goalDateTime.Ticks)))
                {
                    _logger.Warning("Wrong value detected");
                    flag = previousDateTimes.Item2.Ticks - previousDateTimes.Item3.Ticks < 0 &&
                   (Math.Sign(previousDateTimes.Item1.Ticks - previousDateTimes.Item2.Ticks) !=
                    Math.Sign(previousDateTimes.Item2.Ticks - previousDateTimes.Item3.Ticks));
                }

                if (!(flag && !previousFlag))
                {
                    _logger.Debug("Trying to handling the situation");
                    previousDateTimes.Item3 = previousDateTimes.Item2;
                    previousDateTimes.Item2 = previousDateTimes.Item1;
                    previousDateTimes.Item1 = clockRecord.DateTime;
                }

                if (Math.Abs(clockRecord.DateTime.Ticks - goalDateTime.Ticks) < Math.Abs(nearestIndex.Item2))
                {
                    nearestIndex = (mid, clockRecord.DateTime.Ticks - goalDateTime.Ticks);
                    _logger.Debug("The nearest value has been changes to: {nearestIndex} with date of: {dateTime}, and the difference is: {difference}", nearestIndex.Item1, clockRecord.DateTime, new DateTime(nearestIndex.Item2));
                }

                if (flag && !(previousFlag))
                {
                    if (previousmid > mid)
                    {
                        BinarySearch(right, right + (right - left), goalDateTime, ref nearestIndex,
                            previousDateTimes, previousmid, true);
                    }

                    _logger.Debug("Searching, considering the wrong values");
                    BinarySearch(left - (right - left), left, goalDateTime, ref nearestIndex, previousDateTimes,
                        previousmid, true);
                }
                else if (clockRecord.DateTime > goalDateTime)
                {
                    _logger.Debug("Searching left side of mid");
                    BinarySearch(left, mid - 1, goalDateTime, ref nearestIndex, previousDateTimes, mid, flag);
                }
                else if (clockRecord.DateTime < goalDateTime)
                {
                    _logger.Debug("Searching right side of mid");
                    BinarySearch(mid + 1, right, goalDateTime, ref nearestIndex, previousDateTimes, mid, flag);
                }
            }
        }

        private DateTime EOSsearch(ref int currentIndex, DateTime goalDateTime, int beginingOfInterval, int endOfInterval, DateTime prevDateTime)
        {
            var successSetPointer = false;
            ClockRecord clockRecord = null;
            //if (currentIndex < stepLenght)
            //{
            //    return;
            //}
            if (Math.Abs(beginingOfInterval - endOfInterval) < 1)
            {
                return prevDateTime;
            }
            currentIndex = (beginingOfInterval + endOfInterval) / 2;
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    lock (_clock)
                    {
                        if (!successSetPointer)
                        {
                            Thread.Sleep(500);
                            successSetPointer = _clock.SetReadPointer(currentIndex);
                            if(!successSetPointer) continue;
                        }
                        Thread.Sleep(500);
                        var reconnect = Connect();
                        if(!reconnect) continue;
                        Thread.Sleep(500);
                        clockRecord = (ClockRecord)_clock.GetRecord();
                    }
                    break;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, exception.Message);
                    Thread.Sleep(++i * 100);
                }
            }

            if (clockRecord == null)
            {
                return EOSsearch(ref currentIndex, goalDateTime, beginingOfInterval, currentIndex, prevDateTime);
            }
            var recordDateTime = clockRecord.DateTime;
            Logger.Log($"NEW datetime {recordDateTime}");
            if (recordDateTime > goalDateTime)
            {
                return EOSsearch(ref currentIndex, goalDateTime,beginingOfInterval,currentIndex,recordDateTime);
            }
            return recordDateTime < goalDateTime ? EOSsearch(ref currentIndex, goalDateTime, currentIndex, endOfInterval, recordDateTime) : recordDateTime;

        }

        public void Dispose()
        {
            try
            {
                _clock?.Dispose();
                _fixDaylightSavingTimer?.Dispose();
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, exception.Message);
            }
        }
    }
}

