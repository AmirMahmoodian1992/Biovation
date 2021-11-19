using Biovation.Brands.EOS.Manager;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using EosClocks;
using MoreLinq;
using Suprema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logger = Biovation.CommonClasses.Logger;

namespace Biovation.Brands.EOS.Devices
{
    /// <summary>
    /// برای ساعت ST-Pro
    /// </summary>
    /// <seealso cref="Device" />
    public class SupremaBaseDevice : Device, IDisposable
    {
        private Clock _clock;
        private int _counter;
        private readonly object _clockInstantiationLock = new object();

        private readonly LogService _logService;
        private Timer _fixDaylightSavingTimer;

        private readonly TaskService _taskService;
        private readonly DeviceBrands _deviceBrands;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public SupremaBaseDevice(DeviceBasicInfo deviceInfo, LogService logService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, Dictionary<uint, Device> onlineDevices, TaskService taskService, DeviceBrands deviceBrands)
         : base(deviceInfo, logEvents, logSubEvents, eosCodeMappings)
        {
            Valid = true;
            _logService = logService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _onlineDevices = onlineDevices;
            _taskService = taskService;
            _deviceBrands = deviceBrands;
            _biometricTemplateManager = biometricTemplateManager;
            TotalLogCount = 20000;
        }

        /// <summary>
        /// <En>Add new device to database.</En>
        /// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override bool Connect(CancellationToken cancellationToken)
        {
            ServiceCancellationToken = cancellationToken != CancellationToken.None && !ServiceCancellationToken.Equals(cancellationToken) ? cancellationToken : ServiceCancellationToken;

            lock (_onlineDevices)
            {
                if (_onlineDevices.ContainsKey(DeviceInfo.Code))
                {
                    _onlineDevices[DeviceInfo.Code].Disconnect();
                    _onlineDevices.Remove(DeviceInfo.Code);
                }
            }

            var isConnect = IsConnected();
            if (!isConnect) return false;

            try
            {
                lock (DeviceInfo)
                {
                    _ = _logService.AddLog(new Log
                    {
                        DeviceId = DeviceInfo.DeviceId,
                        DeviceCode = DeviceInfo.Code,
                        LogDateTime = DateTime.Now,
                        EventLog = LogEvents.Disconnect
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                //ignore
            }

            var setDateTimeResult = SetDateTime();
            if (!setDateTimeResult)
                Logger.Log($"Could not set the time of device {DeviceInfo.Code}");

            try
            {
                var dueTime = (DateTime.Today.AddDays(1).AddMinutes(1) - DateTime.Now).TotalMilliseconds;
                _fixDaylightSavingTimer = new Timer(FixDaylightSavingTimer_Elapsed, null, (long)dueTime, (long)TimeSpan.FromHours(24).TotalMilliseconds);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, exception.Message);
            }

            _ = _taskService.ProcessQueue(_deviceBrands.Eos, DeviceInfo.DeviceId).ConfigureAwait(false);

            lock (_onlineDevices)
            {
                if (!_onlineDevices.ContainsKey(DeviceInfo.Code))
                {
                    _onlineDevices.Add(DeviceInfo.Code, this);

                    try
                    {
                        _ = _logService.AddLog(new Log
                        {
                            DeviceId = DeviceInfo.DeviceId,
                            LogDateTime = DateTime.Now,
                            EventLog = LogEvents.Connect
                        }).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            }

            Valid = true;
            _ = Task.Run(() => { ReadOnlineLog(Token); }, Token).ConfigureAwait(false);
            return true;
        }

        private bool SetDateTime()
        {
            lock (DeviceInfo)
                if (!DeviceInfo.TimeSync)
                    return true;

            for (var i = 0; i < 5; i++)
            {
                try
                {
                    lock (_clock)
                    {
                        Task.Delay(TimeSpan.FromSeconds(3), ServiceCancellationToken).Wait(ServiceCancellationToken);
                        var deviceTime = _clock.GetDateTime();
                        Logger.Log($"The device time is: {deviceTime.ToString(CultureInfo.CurrentCulture)}");
                        Task.Delay(TimeSpan.FromSeconds(3), ServiceCancellationToken).Wait(ServiceCancellationToken);
                        _clock.SetDateTime(DateTime.Now);
                        Logger.Log($"Successfully SetDateTime to {DateTime.Now}");
                    }
                    return true;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, exception.Message);
                    Task.Delay(TimeSpan.FromMilliseconds(++i * 200), ServiceCancellationToken).Wait(ServiceCancellationToken);
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
            var connection = ConnectionFactory.CreateTCPIPConnection(DeviceInfo.IpAddress, DeviceInfo.Port, 1000, 500, 0,
                false, string.Empty, string.Empty, NetworkModuleType.Tibbo);

            lock (_clockInstantiationLock)
                _clock = new Clock(connection, ProtocolType.RS485, 1, ProtocolType.Suprema);

            lock (_clock)
                if (_clock.TestConnection())
                {
                    Logger.Log($"Successfully connected to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}", logType: LogType.Information);
                    return true;
                }

            while (!ServiceCancellationToken.IsCancellationRequested)
            {
                Logger.Log($"Could not connect to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}");

                Task.Delay(TimeSpan.FromSeconds(10), ServiceCancellationToken).Wait(ServiceCancellationToken);
                Logger.Log($"Retrying connect to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}");

                lock (_clock)
                    if (_clock.TestConnection())
                    {
                        Logger.Log($"Successfully connected to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}", logType: LogType.Information);
                        return true;
                    }
            }

            return false;
        }

        public virtual ResultViewModel ReadOnlineLog(object token)
        {
            Task.Delay(TimeSpan.FromSeconds(1), ServiceCancellationToken).Wait(ServiceCancellationToken); ;
            try
            {
                string eosDeviceType;
                lock (_clock)
                    eosDeviceType = _clock.GetModel();

                Logger.Log($"--> Retrieving Log from Terminal : {DeviceInfo.Code} Device type: {eosDeviceType}");

                bool deviceConnected;

                lock (_clock)
                    deviceConnected = _clock.TestConnection();

                while (deviceConnected && Valid && !ServiceCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        bool newRecordExists;

                        lock (_clock)
                            newRecordExists = !_clock.IsEmpty();

                        while (newRecordExists && !ServiceCancellationToken.IsCancellationRequested)
                        {
                            if (!Valid)
                            {
                                Logger.Log($"Disconnect requested for device {DeviceInfo.Code}");
                                return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
                            }

                            var test = true;
                            var exceptionTester = false;
                            while (test && !ServiceCancellationToken.IsCancellationRequested)
                            {
                                if (!Valid)
                                {
                                    Logger.Log($"Disconnect requested for device {DeviceInfo.Code}");
                                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
                                }

                                ClockRecord record = null;

                                try
                                {
                                    while (record == null && !ServiceCancellationToken.IsCancellationRequested)
                                    {
                                        if (!Valid)
                                        {
                                            Logger.Log($"Disconnect requested for device {DeviceInfo.Code}");
                                            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
                                        }

                                        lock (_clock)
                                            record = (ClockRecord)_clock.GetRecord();
                                        Task.Delay(TimeSpan.FromMilliseconds(300), ServiceCancellationToken).Wait(ServiceCancellationToken);
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
                                                Logger.Log("Clock " + DeviceInfo.Code + ": " + "Bad record: " + badRecordRawData);
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
                                                        DeviceId = DeviceInfo.DeviceId,
                                                        DeviceCode = DeviceInfo.Code,
                                                        //RawData = generatedRecord,
                                                        EventLog = LogEvents.Authorized,
                                                        SubEvent = LogSubEvents.Normal,
                                                        InOutMode = DeviceInfo.DeviceTypeId,
                                                        TnaEvent = 0,
                                                    };

                                                    //var logService = new EOSLogService();
                                                    _ = _logService.AddLog(receivedLog).ConfigureAwait(false);
                                                    test = false;
                                                    Logger.Log($@"<--
   +TerminalID:{DeviceInfo.Code}
   +UserID:{userId}
   +DateTime:{receivedLog.LogDateTime}", logType: LogType.Information);
                                                }
                                                catch (Exception)
                                                {
                                                    Logger.Log("Error in parsing bad record.");
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
                                                Logger.Log(ex, "Clock " + DeviceInfo.Code);
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Logger.Log(exception, "Clock " + DeviceInfo.Code);
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
                                            DeviceId = DeviceInfo.DeviceId,
                                            DeviceCode = DeviceInfo.Code,
                                            SubEvent = EosCodeMappings.GetLogSubEventGenericLookup(record.RecType1),
                                            //RawData = new string(record.RawData.Where(c => !char.IsControl(c)).ToArray()),
                                            EventLog = LogEvents.Authorized,
                                            InOutMode = DeviceInfo.DeviceTypeId,
                                            TnaEvent = 0,
                                        };

                                        _ = _logService.AddLog(receivedLog).ConfigureAwait(false);
                                        test = false;
                                        Logger.Log($@"<--
   +TerminalID:{DeviceInfo.Code}
   +UserID:{receivedLog.UserId}
   +DateTime:{receivedLog.LogDateTime}", logType: LogType.Information);

                                        lock (_clock)
                                            _clock.NextRecord();
                                        //Thread.Sleep(200);
                                    }
                                    else
                                    {
                                        if (!exceptionTester)
                                        {
                                            Logger.Log("Null record.");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex, "Clock " + DeviceInfo.Code + ": " +
                                        "Error while Inserting Data to Attendance . record: " + record);
                                }
                            }

                            lock (_clock)
                                newRecordExists = !_clock.IsEmpty();
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }

                    lock (_clock)
                        deviceConnected = _clock.TestConnection();
                }

                if (Valid)
                    Connect(ServiceCancellationToken);

                return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 1, Message = "0" };
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Clock " + DeviceInfo.Code);
            }

            Logger.Log("Connection fail. Cannot connect to device: " + DeviceInfo.Code + ", IP: " + DeviceInfo.IpAddress);

            if (Valid)
                Connect(ServiceCancellationToken);

            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
        }
        public override bool Disconnect()
        {
            try
            {
                Valid = false;
                lock (_clock)
                {
                    _clock?.Disconnect();
                    _clock?.Dispose();
                }

                Dispose();
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return false;
            }

            return true;
        }

        public override bool DeleteUser(uint sUserId)
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
                        Logger.Log($"Could not connect to device {DeviceInfo.DeviceId} sensor.");
                        return false;
                    }

                    var userId = Convert.ToInt32(sUserId);
                    List<byte[]> userFingerTemplates = null;

                    for (var i = 0; i < 3; i++)
                    {
                        try
                        {
                            userFingerTemplates = _clock.Sensor.GetUserTemplates(userId);
                            if (userFingerTemplates.Count > 0)
                                break;
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                            //Logger.Log($"User {userId} may not be on device {DeviceInfo.DeviceId}");
                        }
                    }

                    SensorRecord deletionResult = null;
                    if (userFingerTemplates == null || userFingerTemplates.Count == 0)
                    {
                        Logger.Log($"User {userId} may not be on device {DeviceInfo.DeviceId}");
                        try
                        {
                            deletionResult = _clock.Sensor.DeleteByID(userId);
                        }
                        catch (Exception innerException)
                        {
                            Logger.Log(innerException);
                        }

                        return deletionResult?.ScanState == ScanState.Success ||
                               deletionResult?.ScanState == ScanState.Scan_Success ||
                               deletionResult?.ScanState == ScanState.Not_Found;
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
                                Logger.Log($"A finger print of user {userId} deleted");
                                deletedTemplatesCount++;
                                break;

                                //if (templateDeletionResult.ScanState == ScanState.Not_Found)
                                //    _clock.Sensor.DeleteTemplate(userId, i);
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
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
                                Logger.Log(exception);
                            }
                        }

                        if (deletedTemplatesCount >= userFingerTemplates.Count)
                            break;
                    }

                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            deletionResult = _clock.Sensor.DeleteByID(userId);
                            break;
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                        }
                    }

                    return deletionResult?.ScanState == ScanState.Success ||
                           deletionResult?.ScanState == ScanState.Scan_Success ||
                           deletionResult?.ScanState == ScanState.Not_Found;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
                finally
                {
                    DisconnectFromSensor();
                }
            }

            return false;
        }

        public override bool TransferUser(User user)
        {
            lock (_clock)
            {
                var userTemplates = user.FingerTemplates?.Where(fingerTemplate =>
                    fingerTemplate.FingerTemplateType.Code == (FingerTemplateTypes.SU384Code)).ToList();

                if (userTemplates is null || !userTemplates.Any())
                    return true;

                try
                {
                    var isConnectToSensor = ConnectToSensor();
                    if (!isConnectToSensor)
                        return false;

                    Logger.Log($"Transferring user {user.Code} to device {DeviceInfo.Code},  the user has {userTemplates.Count} valid templates");

                    var supremaMatcher = new UFMatcher();

                    for (var index = 0; index < userTemplates.Count; index += 2)
                    {
                        var firstFingerTemplate = userTemplates[index];
                        //if (retrievedUser.FingerTemplates.Any(template => template.CheckSum == fingerTemplate.CheckSum))
                        //    continue;

                        Task.Delay(TimeSpan.FromSeconds(1), ServiceCancellationToken).Wait(ServiceCancellationToken); ;
                        var sendTemplateResult = false;
                        supremaMatcher.RotateTemplate(firstFingerTemplate.Template, 384);

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
                                Logger.Log($"The {index + 1} of {userTemplates.Count} finger template of user {user.Code} exists on device");
                                break;
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                                Task.Delay(TimeSpan.FromMilliseconds(++i * 200), ServiceCancellationToken).Wait(ServiceCancellationToken);
                            }
                        }

                        if (checkExistenceResult.ScanState == ScanState.Exist_Finger) continue;

                        if (userTemplates.Count > index + 1)
                        {
                            var secondFingerTemplate = userTemplates[index + 1];
                            //if (retrievedUser.FingerTemplates.Any(template => template.CheckSum == fingerTemplate.CheckSum))
                            //    continue;

                            Task.Delay(TimeSpan.FromSeconds(1), ServiceCancellationToken).Wait(ServiceCancellationToken); ;
                            supremaMatcher.RotateTemplate(secondFingerTemplate.Template, 384);

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

                                    Logger.Log(
                                        $"The {index + 2} finger template of user {user.Code} has been sent to device");
                                    break;
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception);
                                    Task.Delay(TimeSpan.FromMilliseconds(++i * 200), ServiceCancellationToken).Wait(ServiceCancellationToken);
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
                    Logger.Log(exception);
                }
                finally
                {
                    try
                    {
                        DisconnectFromSensor();
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
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
                        Logger.Log($"Could not connect to device {DeviceInfo.DeviceId} sensor.");
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
                            Logger.Log(exception);
                            Task.Delay(TimeSpan.FromMilliseconds(++i * 200), ServiceCancellationToken).Wait(ServiceCancellationToken);
                        }
                    }

                    if (fingerTemplates is null || fingerTemplates.Count <= 0)
                    {
                        Logger.Log($"Error in retrieving user {userId} from device {DeviceInfo.DeviceId}, user may be not available on device.");
                        return null;
                    }

                    var retrievedUser = new User
                    {
                        Code = userId,
                        FingerTemplates = new List<FingerTemplate>(),
                        StartDate = default,
                        EndDate = default
                    };

                    var supremaMatcher = new UFMatcher();

                    for (var i = 0; i < fingerTemplates.Count; i += 2)
                    {
                        var firstTemplateBytes = fingerTemplates[i];

                        supremaMatcher.RotateTemplate(firstTemplateBytes, 384);

                        var fingerTemplate = new FingerTemplate
                        {
                            FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                            FingerTemplateType = _fingerTemplateTypes.SU384,
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

                        supremaMatcher.RotateTemplate(secondTemplateBytes, 384);

                        var secondFingerTemplateSample = new FingerTemplate
                        {
                            FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                            FingerTemplateType = _fingerTemplateTypes.SU384,
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
                    Logger.Log(exception);
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
                        Logger.Log(exception);
                    }
                }
            }
        }


        public override List<User> GetAllUsers(bool embedTemplates = false)
        {
            var usersList = new List<User>();

            lock (_clock)
            {


                var isConnectToSensor = false;

                try
                {
                    isConnectToSensor = ConnectToSensor();
                    if (!isConnectToSensor)
                    {
                        Logger.Log($"Could not connect to device {DeviceInfo.DeviceId} sensor.");
                        return usersList;
                    }


                    //List<SensorInfo> sensorInfo = new List<SensorInfo>();

                    //Task.Delay(TimeSpan.FromSeconds(1), ServiceCancellationToken).Wait(ServiceCancellationToken);;
                    //try
                    //{
                    //    sensorInfo = GetUserList(out var fingerprintsHelpList);
                    //}
                    //catch (Exception e)
                    //{
                    //}

                    //var fingerList = new List<SensorFingerprint>();

                    //foreach (SensorFingerprint fingerprint2 in from fing in sensorInfo
                    //    select new SensorFingerprint
                    //    {
                    //        FingerIndex = new int?(fing.FingerIndex),
                    //        Template = null,
                    //        Person = fing.FingPerson,
                    //        SensorUserID = fing.UserId,
                    //        PersonID = fing.UserId,
                    //        FingerNo = fing.FingerNo,
                    //        FingerprintType = FingerprintType.Suprema
                    //    })
                    //{
                    //    fingerList.Add(fingerprint2);
                    //}


                    var users = _clock.Sensor.GetUserIDList();
                    usersList.AddRange(users.Select(user => new User { Code = user.UserId, AdminLevel = user.AdministrationLevel }));
                    usersList = usersList.DistinctBy(user => user.Code).ToList();

                    if (embedTemplates)
                    {
                        foreach (var user in usersList)
                        {
                            List<byte[]> fingerTemplates;

                            try
                            {
                                fingerTemplates = _clock.Sensor.GetUserTemplates((int)user.Code);
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                                Logger.Log($"Error in retrieving user {user.Code} from device {DeviceInfo.DeviceId}, user may be not available on device.");
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
                                    FingerTemplateType = _fingerTemplateTypes.SU384,
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
                                    FingerTemplateType = _fingerTemplateTypes.SU384,
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
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
                finally
                {
                    try
                    {
                        if (isConnectToSensor)
                            DisconnectFromSensor();
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
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
                    if (!ConnectToSensor()) return true;
                    _clock.Sensor.DeleteAllTemplates();
                    _clock.Sensor.DeleteAllUsers();

                    return true;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
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
                lock (_clock)
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

                Task.Delay(TimeSpan.FromMilliseconds(500), ServiceCancellationToken).Wait(ServiceCancellationToken);
                try
                {
                    lock (_clock)
                        isConnectToSensor = _clock.ConnectToSensor();
                }
                catch
                {
                    //ignore
                }
            }

            if (isConnectToSensor)
                //Logger.Log("Successfully connected to sensor of device:{deviceId}", DeviceInfo.DeviceId);
                Logger.Log($"Successfully connected to sensor of device:{DeviceInfo.DeviceId}");

            return isConnectToSensor;
        }

        private void DisconnectFromSensor()
        {
            var disconnectedFromSensor = false;

            try
            {
                lock (_clock)
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

                Task.Delay(TimeSpan.FromMilliseconds((i + 1) * 200), ServiceCancellationToken).Wait(ServiceCancellationToken);
                try
                {
                    lock (_clock)
                        disconnectedFromSensor = _clock.DisconnectFromSensor();
                }
                catch
                {
                    //ignore
                }
            }

            //Logger.Log(
            //    disconnectedFromSensor
            //        ? "Successfully disconnected from sensor of device:{deviceId}"
            //        : "Could not disconnect from sensor of device:{deviceId}", DeviceInfo.DeviceId);

            Logger.Log(
                disconnectedFromSensor
                    ? "Successfully disconnected from sensor of device:{deviceId}"
                    : $"Could not disconnect from sensor of device:{DeviceInfo.DeviceId}");
        }

        public override ResultViewModel ReadOfflineLogInPeriod(object cancellationToken, DateTime? startTime,
            DateTime? endTime,
            bool saveFile = false)
        {

            var invalidTime = false;
            Logger.Log($"The datetime start with {startTime}");
            if (startTime is null ||
                startTime < new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day) ||
                startTime > DateTime.Now)
            {
                startTime = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day);
                invalidTime = true;
            }

            if (endTime is null || endTime > DateTime.Now ||
                endTime < new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day))
            {
                //endTime = new DateTime(2021, 3, 19);
                invalidTime = true;
            }

            if (invalidTime)
                Logger.Log("The chosen Time Period is wrong.");

            Task.Delay(TimeSpan.FromSeconds(1), ServiceCancellationToken).Wait(ServiceCancellationToken); ;
            string eosDeviceType;
            lock (_clock)
                eosDeviceType = _clock.GetModel();

            lock (_onlineDevices)
            {
                Logger.Log($"--> Retrieving Log from Terminal : {DeviceInfo.Code} Device type: {eosDeviceType}");
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
                        Task.Delay(TimeSpan.FromMilliseconds(500), ServiceCancellationToken).Wait(ServiceCancellationToken);
                        writePointer = _clock.GetWritePointer();
                    }

                    break;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, exception.Message);
                    Task.Delay(TimeSpan.FromMilliseconds(++i * 100), ServiceCancellationToken).Wait(ServiceCancellationToken);
                }

            }

            lock (_clock)

                if (deviceConnected && Valid && writePointer != -1)
                {
                    var initialReadPointer = writePointer;
                    Logger.Log($"The initial pointer is: {initialReadPointer}");
                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            lock (_clock)
                            {
                                Task.Delay(TimeSpan.FromMilliseconds(500), ServiceCancellationToken).Wait(ServiceCancellationToken);
                                initialReadPointer = _clock.GetReadPointer();
                            }

                            break;
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, exception.Message);
                            Task.Delay(TimeSpan.FromMilliseconds(++i * 100), ServiceCancellationToken).Wait(ServiceCancellationToken);
                        }

                    }

                    ClockRecord initialClockRecord = null;
                    try
                    {
                        initialClockRecord = (ClockRecord)_clock.GetRecord();
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }


                    var leftBoundary = writePointer - 1;

                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            lock (_clock)
                            {
                                Task.Delay(TimeSpan.FromMilliseconds(500), ServiceCancellationToken).Wait(ServiceCancellationToken);
                                successSetPointer = _clock.SetReadPointer(leftBoundary);
                            }

                            break;
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, exception.Message);
                            Task.Delay(TimeSpan.FromMilliseconds(++i * 100), ServiceCancellationToken).Wait(ServiceCancellationToken);
                        }

                    }

                    Logger.Log(successSetPointer ? "Successfully set read pointer" : "FAILED in set read pointer");
                    if (successSetPointer)
                    {
                        var dic = new Dictionary<int, int>();
                        for (var i = 1; i <= 12; i++)
                        {
                            dic.Add(i, 0);
                        }// added count of seen month

                        var firstIndex = leftBoundary;
                        try
                        {
                            var clockRecord = (ClockRecord)_clock.GetRecord();
                            var recordDateTime = clockRecord.DateTime;
                            if (initialClockRecord != null && initialClockRecord.DateTime.Month < recordDateTime.Month)
                            {
                                recordDateTime = recordDateTime.AddYears(-2);
                            }
                            Logger.Log($"First datetime {recordDateTime}");

                            var goalDateTime = startTime.Value.AddDays(-7);

                            EosSearch(ref firstIndex, goalDateTime, 10, recordDateTime, dic,
                                initialClockRecord?.DateTime.Month ?? DateTime.Now.Month);
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                        }



                        return new ResultViewModel
                        {
                            Id = DeviceInfo.DeviceId,
                            Success = true,
                            Code = Convert.ToInt32(TaskStatuses.DoneCode)
                        };
                    }

                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            lock (_clock)
                            {
                                Task.Delay(TimeSpan.FromMilliseconds(500), ServiceCancellationToken).Wait(ServiceCancellationToken);
                                _clock.SetReadPointer(initialReadPointer);
                            }

                            break;
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, exception.Message);
                            Task.Delay(TimeSpan.FromMilliseconds(++i * 100), ServiceCancellationToken).Wait(ServiceCancellationToken);
                        }
                    }
                }

            return new ResultViewModel
            {
                Id = DeviceInfo.DeviceId,
                Validate = 0,
                Message = "0",
                Code = Convert.ToInt32(TaskStatuses.FailedCode)
            };
        }

        private void EosSearch(ref int currentIndex, DateTime goalDateTime, int stepLenght, DateTime prevDateTime, IDictionary<int, int> seenMonth, int firstSeenMonth)
        {
            var successSetPointer = false;
            ClockRecord clockRecord = null;
            if (currentIndex < stepLenght)
            {
                return;
            }

            currentIndex -= stepLenght;
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    lock (_clock)
                    {
                        if (!successSetPointer)
                        {
                            Task.Delay(TimeSpan.FromMilliseconds(500), ServiceCancellationToken).Wait(ServiceCancellationToken);
                            successSetPointer = _clock.SetReadPointer(currentIndex);
                        }
                        Task.Delay(TimeSpan.FromMilliseconds(500), ServiceCancellationToken).Wait(ServiceCancellationToken);
                        clockRecord = (ClockRecord)_clock.GetRecord();
                    }
                    break;
                }
                catch (Exception)
                {
                    //Logger.Log(exception, exception.Message);
                    Thread.Sleep(++i * 100);
                }
            }

            if (clockRecord == null)
            {
                //ignore
            }
            else
            {
                var recordDateTime = clockRecord.DateTime;
                Logger.Log($"NEW datetime {recordDateTime}");
                if (recordDateTime.Month > firstSeenMonth)
                {
                    recordDateTime = recordDateTime.AddYears(1);
                }
                if (prevDateTime.Month != recordDateTime.Month)
                {
                    seenMonth[recordDateTime.Month]++;
                    if (seenMonth[recordDateTime.Month] > 1)
                    {
                        return;
                    }
                }

                if (recordDateTime.Month > goalDateTime.Month)
                {
                    if (prevDateTime - recordDateTime < recordDateTime - goalDateTime)
                    {
                        stepLenght += stepLenght * 2 <= 80 ? stepLenght * 2 : stepLenght;
                    }
                    EosSearch(ref currentIndex, goalDateTime, stepLenght * 2, recordDateTime, seenMonth, firstSeenMonth);
                }
                else if (recordDateTime.Month < goalDateTime.Month)
                {
                    EosSearch(ref currentIndex, goalDateTime, stepLenght / 3, recordDateTime, seenMonth, firstSeenMonth);
                }
                else
                {
                    if (Math.Abs(recordDateTime.Day - goalDateTime.Day) <= 2)
                    {
                        return;
                    }
                    if (recordDateTime.Day > goalDateTime.Day - 1)
                    {
                        EosSearch(ref currentIndex, goalDateTime, 1, recordDateTime, seenMonth, firstSeenMonth);
                    }
                    else if (recordDateTime.Day + 2 < goalDateTime.Day - 1)
                    {
                        EosSearch(ref currentIndex, goalDateTime, -1, recordDateTime, seenMonth, firstSeenMonth);
                    }

                }
            }

        }

        public void Dispose()
        {
            try
            {
                _clock?.Dispose();
                _fixDaylightSavingTimer?.Dispose();
                GC.SuppressFinalize(this);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, exception.Message);
            }
        }


        public override Dictionary<string, string> GetAdditionalData(int code)
        {
            var dictionary = new Dictionary<string, string>();
            int userCount;
            int templateCount;
            int packetCount;
            lock (_clock)
            {
                dictionary.Add("DateTime", _clock.GetDateTime().ToString(CultureInfo.InvariantCulture));
                dictionary.Add("Calender", _clock.GetCalendarType().ToString());
                _clock.GetCountOfUsersAndTemplatesStShine(out userCount, out templateCount, out packetCount);
                dictionary.Add("Firmware Version", _clock.GetFirmwareVersion());
                dictionary.Add("Mac Address", _clock.GetMacAddress());
                dictionary.Add("Wifi Mac Address", _clock.GetWifiMacAddress());
                dictionary.Add("Write Pointer", _clock.GetWritePointer().ToString());
                dictionary.Add("Read Pointer", _clock.GetReadPointer().ToString());
            }
            dictionary.Add("User Counts", userCount.ToString());
            dictionary.Add("Template Counts", templateCount.ToString());
            dictionary.Add("Packet Counts", packetCount.ToString());
            return dictionary;
        }
    }
}

