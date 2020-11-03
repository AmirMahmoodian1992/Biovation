using System;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Service;
using Logger = Biovation.CommonClasses.Logger;
using Biovation.Constants;
using Biovation.Domain;
using EosClocks;
using System.Globalization;
using Biovation.CommonClasses;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.EOS.Devices.SupremaBase
{
    /// <summary>
    /// برای ساعت ST-Pro
    /// </summary>
    /// <seealso cref="Device" />
    public class SupremaBaseDevice : Device
    {
        private Clock _clock;
        private bool _valid;
        private int _counter;

        private readonly DeviceBasicInfo _deviceInfo;
        private readonly EosLogService _eosLogService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly EosCodeMappings _eosCodeMappings;

        public SupremaBaseDevice(DeviceBasicInfo deviceInfo, EosLogService eosLogService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings)
         : base(deviceInfo, eosLogService, logEvents, logSubEvents, eosCodeMappings)
        {
            _valid = true;
            _deviceInfo = deviceInfo;
            _eosLogService = eosLogService;
            //  _eosServer = eosServer;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _eosCodeMappings = eosCodeMappings;
        }



        /// <summary>
        /// <En>Transfer user with all data (finger template , card, Id , Access Group ,....) on validated FaceStation devices.</En>
        /// <Fa>کاربر را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="userId">شماره کاربر</param>
        /// <returns></returns>


        /// <summary>
        /// <En>Read all log data from device, since last disconnect.</En>
        /// <Fa>داده های اتفاقات در طول زمان قطعی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
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



        /*public override bool AddDeviceToDataBase()
        {
            return true;
        }*/



        public override bool Connect()
        {
              var isConnect = IsConnected();
               if (!isConnect) return false;

               try
               {
                   if (_deviceInfo.TimeSync)
                       _clock.SetDateTime(DateTime.Now);
               }
               catch (Exception exception)
               {
                   Logger.Log(exception);
                   Thread.Sleep(500);
                   try
                   {
                       if (_deviceInfo.TimeSync)
                           _clock.SetDateTime(DateTime.Now);
                   }
                   catch (Exception innerException)
                   {
                       Logger.Log(innerException);
                   }
               }

               //_valid = true;
               Task.Run(() => { ReadOnlineLog(Token); }, Token);
               return true;
            
        }

    

        private bool IsConnected()
        {
            var connection = ConnectionFactory.CreateTCPIPConnection(_deviceInfo.IpAddress, _deviceInfo.Port, 1000, 500, 0);
          
            _clock = new Clock(connection, ProtocolType.RS485, 1, ProtocolType.Suprema);
            if (_clock.TestConnection())
            {
                Logger.Log($"Successfully connected to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}", logType: LogType.Information);

                return true;
            }
            while (true)
            {
                Logger.Log($"Could not connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");

                Thread.Sleep(600);
                Logger.Log($"Retrying connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");
                if (_clock.TestConnection())
                {
                    return true;
                }
            }
       
        }
        public override ResultViewModel ReadOnlineLog(object token)
        {
            var Object = new object();
            Thread.Sleep(1000);
            lock (Object)
            {
                try
                {
                    var eosDeviceType = _clock.GetModel();
                    Logger.Log($"--> Retrieving Log from Terminal : {_deviceInfo.Code} Device type: {eosDeviceType}");

                    while (_clock.Connected && _valid)
                    {
                        try
                        {
                            while (!_clock.IsEmpty() && _valid)
                            {
                                var test = true;
                                var exceptionTester = false;
                                while (test && _valid)
                                {
                                    ClockRecord record = null;

                                    try
                                    {
                                        while (record == null)
                                        {
                                            record = (ClockRecord)_clock.GetRecord();
                                        }

                                        //_eosServer.Count++;
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            if ((ex is InvalidRecordException) ||
                                                (ex is InvalidDataInRecordException))
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
                                                            EventLog = _logEvents.Authorized,
                                                            SubEvent = _logSubEvents.Normal,
                                                            TnaEvent = 0,
                                                        };

                                                        //var logService = new EOSLogService();
                                                        _eosLogService.AddLog(receivedLog);
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
                                                SubEvent = _eosCodeMappings.GetLogSubEventGenericLookup(record.RecType1),
                                                //RawData = new string(record.RawData.Where(c => !char.IsControl(c)).ToArray()),
                                                EventLog = _logEvents.Authorized,
                                                TnaEvent = 0,
                                            };

                                            //var logService = new EOSLogService();
                                            _eosLogService.AddLog(receivedLog);
                                            test = false;
                                            //Logger.Log("Clock " + _deviceInfo.Code + ": " + record);
                                            Logger.Log($@"<--
   +TerminalID:{_deviceInfo.Code}
   +UserID:{receivedLog.UserId}
   +DateTime:{receivedLog.LogDateTime}", logType: LogType.Information);

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
                            }
                        }
                        catch (Exception)
                        {
                            //ignore
                        }

                    }

                    //_clock?.Disconnect();
                    // _clock?.Dispose();
                    //Disconnect();
                    if (_valid)
                    {
                        Connect();
                    }
                    return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 1, Message = "0" };

                }

                catch (Exception ex)
                {
                    Logger.Log(ex, "Clock " + _deviceInfo.Code);
                }

                Logger.Log("Connection fail. Cannot connect to device: " + _deviceInfo.Code + ", IP: " + _deviceInfo.IpAddress);
            }


            //_clock?.Disconnect();
            //_clock?.Dispose();
            if (_valid)
            {
                Connect();
            }

            //EosServer.IsRunning[(uint)_deviceInfo.Code] = false;
            return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };

        }
        public override  bool Disconnect()
        {
            _valid = false;
            _clock?.Disconnect();
            _clock?.Dispose();
            return true;
        }

        public override bool DeleteUser(uint sUserId)
        {

            if (_clock.TestConnection())
            {
                try
                {
                    _clock.ConnectToSensor();
                    int userId = Convert.ToInt32(sUserId);
                    var userFingerTemplates = _clock.Sensor.GetUserTemplates(userId);
                    foreach (var fingerTemplate in userFingerTemplates)
                    {
                        _clock.Sensor.DeleteTemplate(userId, fingerTemplate);

                    }


                    _clock.DisconnectFromSensor();

                    return true;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    
                }
                finally
                {
                    _clock.DisconnectFromSensor();
                }
            }
            return false;

        }

        public override bool TransferUser(User user)
        {
            try
            {
                _clock.ConnectToSensor();
                var fingerTemplates = user.FingerTemplates;
                foreach (var fingerTemplate in fingerTemplates)
                {

                    _clock.Sensor.EnrollByTemplate((int)user.Id, fingerTemplate.Template, EnrollOptions.Add_New);
                }

            
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            finally
            {
                _clock.DisconnectFromSensor();
            }

            return false;
            }
        internal override User GetUser(uint userId)
        {
            int userCode = Convert.ToInt32(userId);
            List<SensorUser> users = _clock.Sensor.GetUserIDList();
            var newUser = new User();
            bool userExist=false;
            for (int i=0; i<users.Count;i++)
            {
                if (users[i].UserId == userId)
                {
                    userExist = true;
                    newUser.Code = users[i].UserId;
                    newUser.AdminLevel = users[i].AdministrationLevel;
                    newUser.AuthMode = users[i].AuthenticationMode;

                }
            }
            var fingerTemplates = _clock.Sensor.GetUserTemplates(userCode);
            if(userExist)
            {
                foreach (var fingerTemplate in fingerTemplates)
                {

                    newUser.FingerTemplates.Add(new FingerTemplate
                    {
                        //    FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                        //    Index = _fingerTemplateService.FingerTemplates((int)existUser.Id)?.Count(ft =>
                        //       ft.FingerIndex.Code ==
                        //       userOfDevice.FingerTemplates[i].FingerIndex.Code) ?? 0 + 1,
                        //    TemplateIndex = 0,
                        //    Size = userOfDevice.FingerTemplates[i].Size,
                        Template = fingerTemplate,
                        //    CheckSum = firstTemplateSampleChecksum,
                        //    UserId = user.Id,
                        //    FingerTemplateType = _fingerTemplateTypes.SU384
                    });
                }
            }
        

            return newUser;
        }


        public override List<User> GetAllUsers()
        {

            var usersList = new List<User>();
    
            try
            {
                lock (_clock)
                {
                    _clock.ConnectToSensor();

                    var users = _clock.Sensor.GetUserIDList();
                    foreach (var user in users)
                    {
                       
                        var tempUser = new User
                        {
                            Code = Convert.ToInt32(user.UserId),                          
                            AdminLevel = user.AdministrationLevel
                        };
                   
                        usersList.Add(tempUser);

                    }

                }
            }
            catch (Exception)
            {
                //ignore
            }
            finally
            {
                try
                {
                    lock (_clock)
                    {
                        _clock.DisconnectFromSensor();
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }
      
            return usersList;
        }

        public bool DeleteAllUser()
        {
            try
            {
                _clock.ConnectToSensor();
                _clock.Sensor.DeleteAllTemplates();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            finally
            {
                _clock.DisconnectFromSensor();
            }

            return false;

        }
    }
    }

