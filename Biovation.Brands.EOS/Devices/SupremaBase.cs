using Biovation.Brands.EOS.Manager;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using EosClocks;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;
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

        private readonly RestClient _restClient;
        private readonly TaskService _taskService;
        private readonly DeviceBrands _deviceBrands;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public SupremaBaseDevice(DeviceBasicInfo deviceInfo, LogService logService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, RestClient restClient, Dictionary<uint, Device> onlineDevices, TaskService taskService, DeviceBrands deviceBrands)
         : base(deviceInfo, logEvents, logSubEvents, eosCodeMappings)
        {
            Valid = true;
            _logService = logService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _restClient = restClient;
            _onlineDevices = onlineDevices;
            _taskService = taskService;
            _deviceBrands = deviceBrands;
            _biometricTemplateManager = biometricTemplateManager;
            TotalLogCount = 20000;
        }

        ///// <summary>
        ///// <En>Read all log data from device, since last disconnect.</En>
        ///// <Fa>داده های اتفاقات در طول زمان قطعی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        ///// </summary>
        /*   public override void ReadOfflineLog(object token)
           {
               var Object = new object();

               //Logger.Log("Discharging Clock: {0}", DeviceInfo.ReaderId);
               Connection connection = new TCPIPConnection();

               try
               {
                   connection = ConnectionFactory.CreateTCPIPConnection(DeviceInfo.IpAddress,
                                                                        DeviceInfo.Port,
                                                                        DeviceInfo.ReadTimeout,
                                                                        DeviceInfo.WriteTimeout,
                                                                        DeviceInfo.WaitBeforRead);
               }
               catch (Exception ex)
               {
                   Logger.Log(ex.Message);
               }

               //ProtocolType.Suprema
               var clock = new Clock(connection, DeviceInfo.DeviceProtocolType,
                 DeviceInfo.TRT, DeviceInfo.SensorProtocolType);

               lock (Object)
               {
                   //Logger.Log("Testing connection to device: {0}", DeviceInfo.ReaderId);

                   if (clock.TestConnection())
                   {
                       //                    Logger.Log("Discharging Clock: {0}", DeviceInfo.ReaderId);
                       Thread.Sleep(1000);
                       try
                       {
                           //                        clock.Dump()
                           DeviceInfo.EosDeviceType = clock.GetModel();
                           Logger.Log("\nConnected to Device: {0}. Discharging Clock. Device type: {1}\n", DeviceInfo.DeviceId, DeviceInfo.EosDeviceType);

                           while (clock.Connected)
                           {
                               while (!clock.IsEmpty())
                               {
                                   var test = true;
                                   var exceptionTester = false;
                                   while (test)
                                   {
                                       //                            ClockRecord record = null;
                                       ClockRecord record = null;

                                       try
                                       {
                                           //                                                            record2 = (ClockRecord) clock.GetRecord();
                                           while (record == null)
                                           {
                                               record = (ClockRecord)clock.GetRecord();
                                               //                                        Thread.Sleep(2000);
                                           }

                                           EosServer.Count++;

                                           //try
                                           //{
                                           //    System.IO.File.AppendAllText(@".\records" + DeviceInfo.ReaderId + ".txt",
                                           //        Environment.NewLine + "Rec: " + EosServer.Count + "," + record);
                                           //    //                                        test = false;
                                           //    System.IO.File.AppendAllText(@".\recordsRAW" + DeviceInfo.ReaderId + ".txt",
                                           //        Environment.NewLine + record.RawData);
                                           //}
                                           //catch (Exception ex)
                                           //{
                                           //    Logger.Log("Clock " + DeviceInfo.ReaderId + ": " + ex.Message);
                                           //}
                                       }
                                       catch (Exception ex)
                                       {
                                           //                                    Logger.Log("Clock " + DeviceInfo.ReaderId + ": " + ex.Message);
                                           try
                                           {
                                               if (ex is InvalidRecordException ||
                                                   ex is InvalidDataInRecordException)
                                               {
                                                   var badRecordRawData = ex.Data["RecordRawData"].ToString();
                                                   if (ex is InvalidDataInRecordException)
                                                   {
                                                       Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + "Bad record: " + badRecordRawData);
                                                   }
                                                   // you can find bad record data in ex.Data["RecordRawData"]
                                                   //                                    MessageBox.Show("Bad Record :" + Environment.NewLine + ex.Data["RecordRawData"]);
                                                   System.IO.File.AppendAllText(@".\Exceptions" + DeviceInfo.DeviceId + ".txt",
                                                       Environment.NewLine + "Bad Record: " + badRecordRawData);

                                                   if (badRecordRawData != "")
                                                   {
                                                       try
                                                       {
                                                           System.IO.File.AppendAllText(@".\BadRecordsRAW" + DeviceInfo.DeviceId + ".txt",
                                                                                                   Environment.NewLine + ex.Data["RecordRawData"]);


                                                           var year = Convert.ToInt32(badRecordRawData.Substring(24, 2)) + 1300;
                                                           var month = Convert.ToInt32(badRecordRawData.Substring(19, 2));
                                                           var day = Convert.ToInt32(badRecordRawData.Substring(21, 2));

                                                           var hour = Convert.ToInt32(badRecordRawData.Substring(15, 2));
                                                           var minute = Convert.ToInt32(badRecordRawData.Substring(17, 2));

                                                           var userId = Convert.ToInt32(badRecordRawData.Substring(6, 8));
                                                           //int readerId = Convert.ToInt32(badRecordRawData.Substring(3, 3));

                                                           var gregorianDateOfRec = new DateTime(year, month, day, hour, minute, 10, new PersianCalendar());

                                                           var generatedRecord = "BadRecord ID: " + userId + " ," + gregorianDateOfRec;
                                                           Logger.Log("Clock: " + DeviceInfo.DeviceId + generatedRecord);

                                                           System.IO.File.AppendAllText(@".\BadRecords" + DeviceInfo.DeviceId + ".txt",
                                                               Environment.NewLine + badRecordRawData +
                                                               Environment.NewLine + year + "/" + month + "/" + day +
                                                               Environment.NewLine + userId);

                                                           var recivedLog = new EOSLog
                                                           {
                                                               LogDateTime = gregorianDateOfRec,
                                                               UserId = userId,
                                                               DeviceId = DeviceInfo.DeviceId,
                                                               EventId = 55,
                                                               RawData = generatedRecord
                                                           };

                                                           var logService = new EOSLogService();
                                                           logService.AddLog(recivedLog, "KASRACONNECTIONSTRING");
                                                           test = false;

                                                       }
                                                       catch (Exception)
                                                       {
                                                           Logger.Log("Error in parsing bad record.");
                                                           System.IO.File.AppendAllText(@".\Exceptions" + DeviceInfo.DeviceId + ".txt",
                                                               Environment.NewLine + "Error in parsing bad record.");
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
                                                       Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + ex.Message);
                                               }
                                               //                                lock (EOSServer.ExceptionWriteLockObject)
                                               //                                {
                                               System.IO.File.AppendAllText(@".\Exceptions" + DeviceInfo.DeviceId + ".txt",
                                                   Environment.NewLine + "Exception: " + ex.Message);
                                               //                                }
                                           }
                                           catch (Exception e)
                                           {
                                               Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + e.Message);
                                           }
                                       }


                                       try
                                       {
                                           //                                    while (record == null)
                                           //                                    {
                                           //                                        record = clock.GetRecord();
                                           ////                                        Thread.Sleep(2000);
                                           //                                    }

                                           if (record != null)
                                           {
                                               var subEvent = 0;

                                               try
                                               {
                                                   subEvent = Convert.ToInt32(record.RecType1);
                                               }
                                               catch (Exception)
                                               {
                                                   //ignore
                                               }

                                               var recivedLog = new EOSLog
                                               {
                                                   LogDateTime = record.DateTime,
                                                   UserId = (int)record.ID,
                                                   DeviceId = DeviceInfo.DeviceId,
                                                   EventId = 55,
                                                   SubEvent = subEvent,
                                                   RawData = new string(record.RawData.Where(c => !char.IsControl(c)).ToArray())
                                               };

                                               var logService = new EOSLogService();
                                               logService.AddLog(recivedLog, "KASRACONNECTIONSTRING");
                                               test = false;
                                               Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + record);

                                               clock.NextRecord();
                                               Thread.Sleep(1000);
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
                                           Logger.Log("Clock " + DeviceInfo.DeviceId + ": " +
                                               "Error while Inserting Data to Attendance :" + ex.Message + " record: " + record);

                                           try
                                           {
                                               System.IO.File.AppendAllText(@".\Exceptions" + DeviceInfo.DeviceId + ".txt",
                                                   Environment.NewLine + "Error in Insert Data to Attendance : " + ex.Message
                                                   + record);
                                           }
                                           catch (Exception)
                                           {
                                               //                        Logger.Log(e.Message);
                                           }
                                       }
                                   }

                                   //                            Thread.Sleep(1500);

                                   //                            if (DeviceInfo.EOSDeviceType != "ST-PRO+")
                                   //                            {
                                   //                                Thread.Sleep(1000);
                                   //                            }
                               }
                           }

                           clock?.Disconnect();
                           clock?.Dispose();
                       }



                       catch (Exception ex)
                       {
                           Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + ex.Message);
                           try
                           {
                               //                        System.IO.File.AppendAllText(@".\Exceptions.txt",
                               //                            Environment.NewLine + "Error Read Data from Clock " + ex.Message);
                           }
                           catch (Exception)
                           {
                               //                        Logger.Log(e.Message);
                           }
                       }

                       //                    Logger.Log("Finished, Clock: {0} discharged to last record.", DeviceInfo.ReaderId);
                   }

                   Logger.Log("Connection fail. Cannot connect to device: " + DeviceInfo.DeviceId + ", IP: " + DeviceInfo.IpAddress);
               }

               clock?.Disconnect();
               clock?.Dispose();

               EosServer.IsRunnig[DeviceInfo.DeviceId] = false;
           }
   */
        /// <summary>
        /// <En>Add new device to database.</En>
        /// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            lock (_onlineDevices)
            {
                if (_onlineDevices.ContainsKey(DeviceInfo.Code))
                {
                    _onlineDevices[DeviceInfo.Code].Disconnect();
                    _onlineDevices.Remove(DeviceInfo.Code);

                    var disconnectConnectionStatus = new ConnectionStatus
                    {
                        DeviceId = DeviceInfo.DeviceId,
                        IsConnected = false
                    };

                    try
                    {
                        var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                        restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(disconnectConnectionStatus));

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
                }
            }

            var isConnect = IsConnected();
            if (!isConnect) return false;

            var setDateTimeResult = SetDateTime();
            if (!setDateTimeResult)
                Logger.Log($"Could not set the time of device {DeviceInfo.Code}");

            try
            {
                //var daylightSaving = DateTime.Now.DayOfYear <= 81 || DateTime.Now.DayOfYear > 265 ? new DateTime(DateTime.Now.Year, 3, 22, 0, 2, 0) : new DateTime(DateTime.Now.Year, 9, 22, 0, 2, 0);
                //var dueTime = (daylightSaving.Ticks - DateTime.Now.Ticks) / 10000;
                var dueTime = (DateTime.Today.AddDays(1).AddMinutes(1) - DateTime.Now).TotalMilliseconds;
                _fixDaylightSavingTimer = new Timer(FixDaylightSavingTimer_Elapsed, null, (long)dueTime, (long)TimeSpan.FromHours(24).TotalMilliseconds);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, exception.Message);
            }

            _taskService.ProcessQueue(_deviceBrands.Eos, DeviceInfo.DeviceId).ConfigureAwait(false);

            lock (_onlineDevices)
            {
                if (!_onlineDevices.ContainsKey(DeviceInfo.Code))
                {
                    _onlineDevices.Add(DeviceInfo.Code, this);


                    var connectionStatus = new ConnectionStatus
                    {
                        DeviceId = DeviceInfo.DeviceId,
                        IsConnected = true
                    };

                    try
                    {
                        var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                        restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                        _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                        _logService.AddLog(new Log
                        {
                            DeviceId = DeviceInfo.DeviceId,
                            LogDateTime = DateTime.Now,
                            EventLog = LogEvents.Connect
                        });
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            }

            Valid = true;
            Task.Run(() => { ReadOnlineLog(Token); }, Token);
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
                        Thread.Sleep(3000);
                        var deviceTime = _clock.GetDateTime();
                        Logger.Log($"The device time is: {deviceTime.ToString(CultureInfo.CurrentCulture)}");
                        Thread.Sleep(3000);
                        _clock.SetDateTime(DateTime.Now);
                        Logger.Log($"Successfully SetDateTime to {DateTime.Now}");
                    }
                    return true;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, exception.Message);
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

            while (true)
            {
                Logger.Log($"Could not connect to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}");

                Thread.Sleep(10000);
                Logger.Log($"Retrying connect to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}");

                lock (_clock)
                    if (_clock.TestConnection())
                    {
                        Logger.Log($"Successfully connected to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}", logType: LogType.Information);
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

                Logger.Log($"--> Retrieving Log from Terminal : {DeviceInfo.Code} Device type: {eosDeviceType}");

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

                        while (newRecordExists)
                        {
                            if (!Valid)
                            {
                                Logger.Log($"Disconnect requested for device {DeviceInfo.Code}");
                                return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
                            }

                            var test = true;
                            var exceptionTester = false;
                            while (test)
                            {
                                if (!Valid)
                                {
                                    Logger.Log($"Disconnect requested for device {DeviceInfo.Code}");
                                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
                                }

                                ClockRecord record = null;

                                try
                                {
                                    while (record == null)
                                    {
                                        if (!Valid)
                                        {
                                            Logger.Log($"Disconnect requested for device {DeviceInfo.Code}");
                                            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
                                        }

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
                                                    _logService.AddLog(receivedLog);
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

                                        _logService.AddLog(receivedLog);
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
                    Connect();

                return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 1, Message = "0" };
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Clock " + DeviceInfo.Code);
            }

            Logger.Log("Connection fail. Cannot connect to device: " + DeviceInfo.Code + ", IP: " + DeviceInfo.IpAddress);

            if (Valid)
                Connect();

            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
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

                        Thread.Sleep(1000);
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
                                Thread.Sleep(++i * 200);
                            }
                        }

                        if (checkExistenceResult.ScanState == ScanState.Exist_Finger) continue;

                        if (userTemplates.Count > index + 1)
                        {
                            var secondFingerTemplate = userTemplates[index + 1];
                            //if (retrievedUser.FingerTemplates.Any(template => template.CheckSum == fingerTemplate.CheckSum))
                            //    continue;

                            Thread.Sleep(1000);
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
                            Thread.Sleep(++i * 200);
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

                    //Thread.Sleep(1000);
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

                Thread.Sleep(500);
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

                Thread.Sleep((i + 1) * 200);
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

            Thread.Sleep(1000);
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
                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            lock (_clock)
                            {
                                Thread.Sleep(500);
                                initialReadPointer = _clock.GetReadPointer();
                            }

                            break;
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, exception.Message);
                            Thread.Sleep(++i * 100);
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


                    var rightBoundary = writePointer;
                    var leftBoundary = writePointer - 1;

                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            lock (_clock)
                            {
                                Thread.Sleep(500);
                                successSetPointer = _clock.SetReadPointer(leftBoundary);
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
                        var dic = new Dictionary<int, int>()
                        {
                            {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}, {6, 0}, {7, 0}, {8, 0}, {9, 0}, {10, 0}, {11, 0},
                            {12, 0}
                        };

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
                            //EOSsearch(ref index, (DateTime) startTime, 10, DateTime.Now, dic,
                            //    clockRecord.DateTime.Month);

                            var goalDateTime = startTime.Value.AddDays(-7);

                            EOSsearch(ref firstIndex, (DateTime)startTime, 10, recordDateTime, dic,
                                initialClockRecord?.DateTime.Month ?? DateTime.Now.Month);
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
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



                        return new ResultViewModel
                        {
                            Id = DeviceInfo.DeviceId,
                            Success = successSetPointer,
                            Code = Convert.ToInt32(TaskStatuses.DoneCode)
                        };
                    }

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
                Id = DeviceInfo.DeviceId,
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

            Logger.Log("Searching for appropriate log pointer value.");

            var successSetPointer = false;
            ClockRecord clockRecord = null;
            var flag = false;

            var interval = (right - left) > 0 ? (right - left) : TotalLogCount + (right - left);
            var mid = (left + interval / 2);

            Logger.Log($"The interval is: {interval} and the mid pointer is: {mid}");

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
                            Logger.Log($"The read pointer is successfully set to {mid}");
                        }
                        Thread.Sleep(500);
                        clockRecord = (ClockRecord)_clock.GetRecord();
                        Logger.Log($"The log date is: {clockRecord.DateTime}");
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
                     (previousDateTimes.Item2.Ticks - goalDateTime.Ticks)) && nearestIndex.Item2 > (previousDateTimes.Item2.Ticks - goalDateTime.Ticks))
                {
                    Logger.Log("Wrong value detected", string.Empty, LogType.Warning);
                    flag = previousDateTimes.Item2.Ticks - previousDateTimes.Item3.Ticks < 0 &&
                   (Math.Sign(previousDateTimes.Item1.Ticks - previousDateTimes.Item2.Ticks) !=
                    Math.Sign(previousDateTimes.Item2.Ticks - previousDateTimes.Item3.Ticks));
                }

                if (!(flag && !previousFlag))
                {
                    Logger.Log("Trying to handling the situation");
                    previousDateTimes.Item3 = previousDateTimes.Item2;
                    previousDateTimes.Item2 = previousDateTimes.Item1;
                    previousDateTimes.Item1 = clockRecord.DateTime;
                }

                if (Math.Abs(clockRecord.DateTime.Ticks - goalDateTime.Ticks) < Math.Abs(nearestIndex.Item2))
                {
                    nearestIndex = (mid, clockRecord.DateTime.Ticks - goalDateTime.Ticks);
                    Logger.Log($"The nearest value has been changes to: {nearestIndex.Item1} with date of: {clockRecord.DateTime}, and the difference is: {nearestIndex.Item2}");
                }

                if (flag && !(previousFlag))
                {
                    if (previousmid > mid)
                    {
                        BinarySearch(right, right + (right - left), goalDateTime, ref nearestIndex,
                            previousDateTimes, previousmid, true);
                    }

                    Logger.Log("Searching, considering the wrong values");
                    BinarySearch(left - (right - left), left, goalDateTime, ref nearestIndex, previousDateTimes,
                        previousmid, true);
                }
                else if (clockRecord.DateTime > goalDateTime)
                {
                    Logger.Log("Searching left side of mid");
                    BinarySearch(left, mid - 1, goalDateTime, ref nearestIndex, previousDateTimes, mid, flag);
                }
                else if (clockRecord.DateTime < goalDateTime)
                {
                    Logger.Log("Searching right side of mid");
                    BinarySearch(mid + 1, right, goalDateTime, ref nearestIndex, previousDateTimes, mid, flag);
                }
            }
        }

        private void EOSsearch(ref int currentIndex, DateTime startDateTime, int stepLenght, DateTime prevDateTime, IDictionary<int, int> seenMonth, int firstSeenMonth)
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
                            Thread.Sleep(500);
                            successSetPointer = _clock.SetReadPointer(currentIndex);
                        }
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

                if (recordDateTime.Month > startDateTime.Month)
                {
                    if (prevDateTime - recordDateTime < recordDateTime - startDateTime)
                    {
                        stepLenght += stepLenght * 2 <= 80 ? stepLenght * 2 : stepLenght;
                    }
                    EOSsearch(ref currentIndex, startDateTime, stepLenght * 2, recordDateTime, seenMonth, firstSeenMonth);
                }
                else if (recordDateTime.Month < startDateTime.Month)
                {
                    EOSsearch(ref currentIndex, startDateTime, stepLenght / 3, recordDateTime, seenMonth, firstSeenMonth);
                }
                else
                {
                    if (recordDateTime.Day > startDateTime.Day - 1)
                    {
                        EOSsearch(ref currentIndex, startDateTime, 1, recordDateTime, seenMonth, firstSeenMonth);
                    }
                    else if (recordDateTime.Day + 2 < startDateTime.Day - 1)
                    {
                        EOSsearch(ref currentIndex, startDateTime, -1, recordDateTime, seenMonth, firstSeenMonth);
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

