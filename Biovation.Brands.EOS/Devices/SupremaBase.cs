﻿using Biovation.Brands.EOS.Manager;
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
    public class SupremaBaseDevice : Device
    {
        private Clock _clock;
        private int _counter;
        private readonly object _clockInstantiationLock = new object();

        private readonly DeviceBasicInfo _deviceInfo;
        private readonly LogService _logService;

        private readonly RestClient _restClient;
        private readonly TaskManager _taskManager;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public SupremaBaseDevice(DeviceBasicInfo deviceInfo, LogService logService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, TaskManager taskManager, RestClient restClient, Dictionary<uint, Device> onlineDevices)
         : base(deviceInfo, logEvents, logSubEvents, eosCodeMappings)
        {
            Valid = true;
            _deviceInfo = deviceInfo;
            _logService = logService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _taskManager = taskManager;
            _restClient = restClient;
            _onlineDevices = onlineDevices;
            _biometricTemplateManager = biometricTemplateManager;
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
                if (_onlineDevices.ContainsKey(_deviceInfo.Code))
                {
                    _onlineDevices[_deviceInfo.Code].Disconnect();
                    _onlineDevices.Remove(_deviceInfo.Code);

                    var disconnectConnectionStatus = new ConnectionStatus
                    {
                        DeviceId = _deviceInfo.DeviceId,
                        IsConnected = false
                    };

                    try
                    {
                        var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                        restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(disconnectConnectionStatus));

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

            if (_deviceInfo.TimeSync)
            {
                var setDateTimeResult = SetDateTime();
                if (!setDateTimeResult)
                    Logger.Log($"Could not set the time of device {_deviceInfo.Code}");
            }

            _taskManager.ProcessQueue();

            lock (_onlineDevices)
            {
                if (!_onlineDevices.ContainsKey(_deviceInfo.Code))
                {
                    _onlineDevices.Add(_deviceInfo.Code, this);


                    var connectionStatus = new ConnectionStatus
                    {
                        DeviceId = _deviceInfo.DeviceId,
                        IsConnected = true
                    };

                    try
                    {
                        var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                        restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                        _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                        _logService.AddLog(new Log
                        {
                            DeviceId = _deviceInfo.DeviceId,
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
                    Logger.Log(exception);
                    Thread.Sleep(++i * 200);
                }
            }

            return false;
        }

        private bool IsConnected()
        {
            var connection = ConnectionFactory.CreateTCPIPConnection(_deviceInfo.IpAddress, _deviceInfo.Port, 1000, 500, 0,
                false, string.Empty, string.Empty, NetworkModuleType.Tibbo);

            lock (_clockInstantiationLock)
                _clock = new Clock(connection, ProtocolType.RS485, 1, ProtocolType.Suprema);

            lock (_clock)
                if (_clock.TestConnection())
                {
                    Logger.Log($"Successfully connected to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}", logType: LogType.Information);
                    return true;
                }

            while (true)
            {
                Logger.Log($"Could not connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");

                Thread.Sleep(10000);
                Logger.Log($"Retrying connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");

                lock (_clock)
                    if (_clock.TestConnection())
                    {
                        Logger.Log($"Successfully connected to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}", logType: LogType.Information);
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

                Logger.Log($"--> Retrieving Log from Terminal : {_deviceInfo.Code} Device type: {eosDeviceType}");

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
                                                Logger.Log("Clock " + _deviceInfo.Code + ": " + "Bad record: " + badRecordRawData);
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
                                                        //RawData = generatedRecord,
                                                        EventLog = LogEvents.Authorized,
                                                        SubEvent = LogSubEvents.Normal,
                                                        InOutMode = _deviceInfo.DeviceTypeId,
                                                        TnaEvent = 0,
                                                    };

                                                    //var logService = new EOSLogService();
                                                    _logService.AddLog(receivedLog);
                                                    test = false;
                                                    Logger.Log($@"<--
   +TerminalID:{_deviceInfo.Code}
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
                                                Logger.Log(ex, "Clock " + _deviceInfo.Code);
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Logger.Log(exception, "Clock " + _deviceInfo.Code);
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
                                            EventLog = LogEvents.Authorized,
                                            InOutMode = _deviceInfo.DeviceTypeId,
                                            TnaEvent = 0,
                                        };

                                        _logService.AddLog(receivedLog);
                                        test = false;
                                        Logger.Log($@"<--
   +TerminalID:{_deviceInfo.Code}
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
                                    Logger.Log(ex, "Clock " + _deviceInfo.Code + ": " +
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

                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 1, Message = "0" };
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Clock " + _deviceInfo.Code);
            }

            Logger.Log("Connection fail. Cannot connect to device: " + _deviceInfo.Code + ", IP: " + _deviceInfo.IpAddress);

            if (Valid)
                Connect();

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
                        Logger.Log($"Could not connect to device {_deviceInfo.DeviceId} sensor.");
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
                            //Logger.Log($"User {userId} may not be on device {_deviceInfo.DeviceId}");
                        }
                    }

                    SensorRecord deletionResult;
                    if (userFingerTemplates == null || userFingerTemplates.Count == 0)
                    {
                        Logger.Log($"User {userId} may not be on device {_deviceInfo.DeviceId}");
                        deletionResult = null;
                        try
                        {
                            deletionResult = _clock.Sensor.DeleteByID(userId);
                        }
                        catch (Exception innerException)
                        {
                            Logger.Log(innerException);
                        }

                        return deletionResult?.ScanState == ScanState.Success ||
                               deletionResult?.ScanState == ScanState.Scan_Success;
                    }

                    for (var i = 0; i < userFingerTemplates.Count; i++)
                    {
                        var fingerTemplate = userFingerTemplates[i];
                        var templateDeletionResult = _clock.Sensor.DeleteTemplate(userId, fingerTemplate);
                        if (templateDeletionResult.ScanState == ScanState.Not_Found)
                            _clock.Sensor.DeleteTemplate(userId, i);
                    }

                    deletionResult = _clock.Sensor.DeleteByID(userId);

                    return deletionResult?.ScanState == ScanState.Success ||
                           deletionResult?.ScanState == ScanState.Scan_Success;
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
                try
                {
                    var isConnectToSensor = ConnectToSensor();
                    if (!isConnectToSensor)
                        return false;

                    if (user.FingerTemplates is null || user.FingerTemplates.Count <= 0) return true;

                    var supremaMatcher = new UFMatcher();
                    foreach (var fingerTemplate in user.FingerTemplates.Where(fingerTemplate => fingerTemplate.FingerTemplateType.Code == FingerTemplateTypes.SU384Code))
                    {
                        var sendTemplateResult = false;
                        supremaMatcher.RotateTemplate(fingerTemplate.Template, fingerTemplate.Template.Length);

                        for (var i = 0; i < 5;)
                        {
                            try
                            {
                                var enrollResult = _clock.Sensor.EnrollByTemplate((int)user.Code, fingerTemplate.Template, EnrollOptions.Add_New);
                                sendTemplateResult = enrollResult.ScanState == ScanState.Success || enrollResult.ScanState == ScanState.Scan_Success || enrollResult.ScanState == ScanState.Data_Ok || enrollResult.ID > 0;
                                break;
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                                Thread.Sleep(++i * 200);
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
                        Logger.Log($"Could not connect to device {_deviceInfo.DeviceId} sensor.");
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
                        Logger.Log($"Error in retrieving user {userId} from device {_deviceInfo.DeviceId}, user may be not available on device.");
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

                        supremaMatcher.RotateTemplate(firstTemplateBytes, firstTemplateBytes.Length);

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

                        supremaMatcher.RotateTemplate(secondTemplateBytes, secondTemplateBytes.Length);

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
                        Logger.Log($"Could not connect to device {_deviceInfo.DeviceId} sensor.");
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
                                Logger.Log($"Error in retrieving user {user.Code} from device {_deviceInfo.DeviceId}, user may be not available on device.");
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
                //_logger.Debug("Successfully connected to sensor of device:{deviceId}", _deviceInfo.DeviceId);
                Logger.Log($"Successfully connected to sensor of device:{_deviceInfo.DeviceId}");

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

            //_logger.Debug(
            //    disconnectedFromSensor
            //        ? "Successfully disconnected from sensor of device:{deviceId}"
            //        : "Could not disconnect from sensor of device:{deviceId}", _deviceInfo.DeviceId);

            Logger.Log(
                disconnectedFromSensor
                    ? "Successfully disconnected from sensor of device:{deviceId}"
                    : $"Could not disconnect from sensor of device:{_deviceInfo.DeviceId}");
        }
    }
}

