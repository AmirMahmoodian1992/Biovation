using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Service;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using EosClocks;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Constants;
using Biovation.Domain;
using Logger = Biovation.CommonClasses.Logger;
using ProtocolType = EosClocks.ProtocolType;

namespace Biovation.Brands.EOS.Devices
{
    /// <summary>
    /// کلاس پدر ساعت ها که تمامی انواع ساعت ها از آن ارث بری می کنند.
    /// </summary>
    /// <seealso cref="IDevices" />
    public class Device : IDevices
    {
        protected CancellationToken Token;
        
        private Clock _clock;
        private readonly DeviceBasicInfo _deviceInfo;
        //public Semaphore DeviceAccessSemaphore;
        private bool _valid;
        private int _counter;
        //private readonly LogService _commonLogService = new LogService();

        private readonly EosServer _eosServer;
        private readonly EosLogService _eosLogService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly EosCodeMappings _eosCodeMappings;

        internal Device(DeviceBasicInfo deviceInfo, EosLogService eosLogService, EosServer eosServer, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings)
        {
            _valid = true;
            _deviceInfo = deviceInfo;
            _eosLogService = eosLogService;
            _eosServer = eosServer;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _eosCodeMappings = eosCodeMappings;
        }
        public bool Connect()
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

        public bool Disconnect()
        {
            _valid = false;
            _clock?.Disconnect();
            _clock?.Dispose();
            return true;
        }


        public DeviceBasicInfo GetDeviceInfo()
        {
            return _deviceInfo;
        }

        /// <summary>
        /// <En>Transfer user to device.</En>
        /// <Fa>کاربر را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="userId">شماره کاربر</param>
        /// <returns></returns>
        public bool TransferUser(int userId)
        {
            return true;
        }

        /// <summary>
        /// <En>Read all log data from device, since last disconnect.</En>
        /// <Fa>داده های اتفاقات در طول زمان قطعی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
        public ResultViewModel ReadOnlineLog(object token)
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

                                        _eosServer.Count++;
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

        /// <summary>
        /// <En>Add new device to database.</En>
        /// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public int AddDeviceToDataBase()
        {
            return 0;
        }

        public ResultViewModel ReadOfflineLog(object token, bool fileSave = false)
        {
            return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };
        }

        /// <summary>
        /// <En>Delete user from device.</En>
        /// <Fa>کاربر را از دستکاه حذف می کند.</Fa>
        /// </summary>
        /// <param name="sUserId">شماره کاربر</param>
        /// <returns></returns>
        public bool DeleteUser(uint sUserId)
        {
            return false;
        }

        public bool TransferUser(User nUserIdn)
        {
            return true;
        }
    }
}
