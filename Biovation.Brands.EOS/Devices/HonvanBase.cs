using System;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Service;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using EosClocks;
using Logger = Biovation.CommonClasses.Logger;

namespace Biovation.Brands.EOS.Devices
{
    public class HonvanBase : Device
    {
        private StFace _stFace;
        private readonly DeviceBasicInfo _deviceInfo;
        private readonly EosLogService _eosLogService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly EosCodeMappings _eosCodeMappings;
        private bool _valid;

        internal HonvanBase(DeviceBasicInfo deviceInfo, EosLogService eosLogService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings) : base(deviceInfo, eosLogService, logEvents, logSubEvents, eosCodeMappings)
        {
            _valid = true;
            _deviceInfo = deviceInfo;
        }



        private int _counter;


        public override bool Connect()
        {

            var isConnect = IsConnected();
            if (!isConnect) return false;

            try
            {
                if (_deviceInfo.TimeSync)
                    _stFace.SetDateTime(DateTime.Now);
            }
            catch (Exception exception)
            {
                CommonClasses.Logger.Log(exception);
                Thread.Sleep(500);
                try
                {
                    if (_deviceInfo.TimeSync)
                        _stFace.SetDateTime(DateTime.Now);
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
            _stFace = new StFace(new TCPIPConnection { IP = "192.168.3.111", Port = 9922, ReadTimeout = 100, WriteTimeout = 100 });
            if (_stFace.TestConnection())
            // if (!(_stFace.GetModel() is null))
            {
                Logger.Log($"Successfully connected to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}", logType: LogType.Information);
                return true;
            }
            while (true)
            {
                Logger.Log($"Could not connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");

                Thread.Sleep(600);
                Logger.Log($"Retrying connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");
                if (_stFace.TestConnection())
                {
                    return true;
                }
            }
        }

        public override bool Disconnect()
        {
            _valid = false;
            _stFace?.Disconnect();
            _stFace?.Dispose();
            return true;
        }
        //public override void ReadOfflineLog(object token)
        //{
        //    var Object = new object();

        //    //Logger.Log("Discharging Clock: {0}", DeviceInfo.ReaderId);
        //    Connection connection = new TCPIPConnection();

        //    try
        //    {
        //        connection = ConnectionFactory.CreateTCPIPConnection(DeviceInfo.IpAddress,
        //                                                             DeviceInfo.Port,
        //                                                             DeviceInfo.ReadTimeout,
        //                                                             DeviceInfo.WriteTimeout,
        //                                                             DeviceInfo.WaitBeforRead);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex.Message);
        //    }

        //    //ProtocolType.Suprema
        //    var clock = new Clock(connection, DeviceInfo.DeviceProtocolType,
        //      DeviceInfo.TRT, DeviceInfo.SensorProtocolType);

        //    lock (Object)
        //    {
        //        //Logger.Log("Testing connection to device: {0}", DeviceInfo.ReaderId);

        //        if (clock.TestConnection())
        //        {
        //            //                    Logger.Log("Discharging Clock: {0}", DeviceInfo.ReaderId);
        //            Thread.Sleep(1000);
        //            try
        //            {
        //                //                        clock.Dump()
        //                DeviceInfo.EosDeviceType = clock.GetModel();
        //                Logger.Log("\nConnected to Device: {0}. Discharging Clock. Device type: {1}\n", DeviceInfo.DeviceId, DeviceInfo.EosDeviceType);

        //                while (clock.Connected)
        //                {
        //                    while (!clock.IsEmpty())
        //                    {
        //                        var test = true;
        //                        var exceptionTester = false;
        //                        while (test)
        //                        {
        //                            //                            ClockRecord record = null;
        //                            ClockRecord record = null;

        //                            try
        //                            {
        //                                //                                                            record2 = (ClockRecord) clock.GetRecord();
        //                                while (record == null)
        //                                {
        //                                    record = (ClockRecord)clock.GetRecord();
        //                                    //                                        Thread.Sleep(2000);
        //                                }

        //                                EosServer.Count++;

        //                                //try
        //                                //{
        //                                //    System.IO.File.AppendAllText(@".\records" + DeviceInfo.ReaderId + ".txt",
        //                                //        Environment.NewLine + "Rec: " + EosServer.Count + "," + record);
        //                                //    //                                        test = false;
        //                                //    System.IO.File.AppendAllText(@".\recordsRAW" + DeviceInfo.ReaderId + ".txt",
        //                                //        Environment.NewLine + record.RawData);
        //                                //}
        //                                //catch (Exception ex)
        //                                //{
        //                                //    Logger.Log("Clock " + DeviceInfo.ReaderId + ": " + ex.Message);
        //                                //}
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                //                                    Logger.Log("Clock " + DeviceInfo.ReaderId + ": " + ex.Message);
        //                                try
        //                                {
        //                                    if (ex is InvalidRecordException ||
        //                                        ex is InvalidDataInRecordException)
        //                                    {
        //                                        var badRecordRawData = ex.Data["RecordRawData"].ToString();
        //                                        if (ex is InvalidDataInRecordException)
        //                                        {
        //                                            Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + "Bad record: " + badRecordRawData);
        //                                        }
        //                                        // you can find bad record data in ex.Data["RecordRawData"]
        //                                        //                                    MessageBox.Show("Bad Record :" + Environment.NewLine + ex.Data["RecordRawData"]);
        //                                        System.IO.File.AppendAllText(@".\Exceptions" + DeviceInfo.DeviceId + ".txt",
        //                                            Environment.NewLine + "Bad Record: " + badRecordRawData);

        //                                        if (badRecordRawData != "")
        //                                        {
        //                                            try
        //                                            {
        //                                                System.IO.File.AppendAllText(@".\BadRecordsRAW" + DeviceInfo.DeviceId + ".txt",
        //                                                                                        Environment.NewLine + ex.Data["RecordRawData"]);


        //                                                var year = Convert.ToInt32(badRecordRawData.Substring(24, 2)) + 1300;
        //                                                var month = Convert.ToInt32(badRecordRawData.Substring(19, 2));
        //                                                var day = Convert.ToInt32(badRecordRawData.Substring(21, 2));

        //                                                var hour = Convert.ToInt32(badRecordRawData.Substring(15, 2));
        //                                                var minute = Convert.ToInt32(badRecordRawData.Substring(17, 2));

        //                                                var userId = Convert.ToInt32(badRecordRawData.Substring(6, 8));
        //                                                //int readerId = Convert.ToInt32(badRecordRawData.Substring(3, 3));

        //                                                var gregorianDateOfRec = new DateTime(year, month, day, hour, minute, 10, new PersianCalendar());

        //                                                var generatedRecord = "BadRecord ID: " + userId + " ," + gregorianDateOfRec;
        //                                                Logger.Log("Clock: " + DeviceInfo.DeviceId + generatedRecord);

        //                                                System.IO.File.AppendAllText(@".\BadRecords" + DeviceInfo.DeviceId + ".txt",
        //                                                    Environment.NewLine + badRecordRawData +
        //                                                    Environment.NewLine + year + "/" + month + "/" + day +
        //                                                    Environment.NewLine + userId);

        //                                                var recivedLog = new EOSLog
        //                                                {
        //                                                    LogDateTime = gregorianDateOfRec,
        //                                                    UserId = userId,
        //                                                    DeviceId = DeviceInfo.DeviceId,
        //                                                    EventId = 55,
        //                                                    RawData = generatedRecord
        //                                                };

        //                                                var logService = new EOSLogService();
        //                                                logService.AddLog(recivedLog, "KASRACONNECTIONSTRING");
        //                                                test = false;

        //                                            }
        //                                            catch (Exception)
        //                                            {
        //                                                Logger.Log("Error in parsing bad record.");
        //                                                System.IO.File.AppendAllText(@".\Exceptions" + DeviceInfo.DeviceId + ".txt",
        //                                                    Environment.NewLine + "Error in parsing bad record.");
        //                                            }
        //                                        }

        //                                        if (!(ex is InvalidRecordException))
        //                                            _counter++;
        //                                        if (_counter == 4)
        //                                        {
        //                                            test = false;
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        if (ex is InvalidRecordException)
        //                                            exceptionTester = true;
        //                                        else
        //                                            Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + ex.Message);
        //                                    }
        //                                    //                                lock (EOSServer.ExceptionWriteLockObject)
        //                                    //                                {
        //                                    System.IO.File.AppendAllText(@".\Exceptions" + DeviceInfo.DeviceId + ".txt",
        //                                        Environment.NewLine + "Exception: " + ex.Message);
        //                                    //                                }
        //                                }
        //                                catch (Exception e)
        //                                {
        //                                    Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + e.Message);
        //                                }
        //                            }


        //                            try
        //                            {
        //                                //                                    while (record == null)
        //                                //                                    {
        //                                //                                        record = clock.GetRecord();
        //                                ////                                        Thread.Sleep(2000);
        //                                //                                    }

        //                                if (record != null)
        //                                {
        //                                    var subEvent = 0;

        //                                    try
        //                                    {
        //                                        subEvent = Convert.ToInt32(record.RecType1);
        //                                    }
        //                                    catch (Exception)
        //                                    {
        //                                        //ignore
        //                                    }

        //                                    var recivedLog = new EOSLog
        //                                    {
        //                                        LogDateTime = record.DateTime,
        //                                        UserId = (int)record.ID,
        //                                        DeviceId = DeviceInfo.DeviceId,
        //                                        EventId = 55,
        //                                        SubEvent = subEvent,
        //                                        RawData = new string(record.RawData.Where(c => !char.IsControl(c)).ToArray())
        //                                    };

        //                                    var logService = new EOSLogService();
        //                                    logService.AddLog(recivedLog, "KASRACONNECTIONSTRING");
        //                                    test = false;
        //                                    Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + record);

        //                                    clock.NextRecord();
        //                                    Thread.Sleep(1000);
        //                                }

        //                                else
        //                                {
        //                                    if (!exceptionTester)
        //                                    {
        //                                        Logger.Log("Null record.");
        //                                    }
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                Logger.Log("Clock " + DeviceInfo.DeviceId + ": " +
        //                                    "Error while Inserting Data to Attendance :" + ex.Message + " record: " + record);

        //                                try
        //                                {
        //                                    System.IO.File.AppendAllText(@".\Exceptions" + DeviceInfo.DeviceId + ".txt",
        //                                        Environment.NewLine + "Error in Insert Data to Attendance : " + ex.Message
        //                                        + record);
        //                                }
        //                                catch (Exception)
        //                                {
        //                                    //                        Logger.Log(e.Message);
        //                                }
        //                            }
        //                        }

        //                        //                            Thread.Sleep(1500);

        //                        //                            if (DeviceInfo.EOSDeviceType != "ST-PRO+")
        //                        //                            {
        //                        //                                Thread.Sleep(1000);
        //                        //                            }
        //                    }
        //                }

        //                clock?.Disconnect();
        //                clock?.Dispose();
        //            }



        //            catch (Exception ex)
        //            {
        //                Logger.Log("Clock " + DeviceInfo.DeviceId + ": " + ex.Message);
        //                try
        //                {
        //                    //                        System.IO.File.AppendAllText(@".\Exceptions.txt",
        //                    //                            Environment.NewLine + "Error Read Data from Clock " + ex.Message);
        //                }
        //                catch (Exception)
        //                {
        //                    //                        Logger.Log(e.Message);
        //                }
        //            }

        //            //                    Logger.Log("Finished, Clock: {0} discharged to last record.", DeviceInfo.ReaderId);
        //        }

        //        Logger.Log("Connection fail. Cannot connect to device: " + DeviceInfo.DeviceId + ", IP: " + DeviceInfo.IpAddress);
        //    }

        //    clock?.Disconnect();
        //    clock?.Dispose();

        //    EosServer.IsRunnig[DeviceInfo.DeviceId] = false;
        //}
        ///// <summary>
        ///// <En>Add new device to database.</En>
        ///// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        ///// </summary>
        ///// <returns></returns>



        //public override bool AddDeviceToDataBase()
        //{
        //    return true;
        //}


        //public override bool DeleteUser(uint sUserId, uint deviceCode)
        //{
        //    Connection connection;
        //    if (SerialRadioButton.Checked)
        //    {
        //        connection = ConnectionFactory.CreateSerialConnection(serialPortsCombobox.SelectedIndex + 1,
        //            Convert.ToInt32(baudRateCombobox.Items[baudRateCombobox.SelectedIndex].ToString()),
        //            (System.IO.Ports.Parity)(parityCombobox.SelectedIndex),
        //            Convert.ToInt32(readTimeoutTextBox.Text),
        //            Convert.ToInt32(writeTimeoutTextBox.Text),
        //            Convert.ToInt32(waitBeforReadTextBox.Text));
        //    }
        //    else
        //    {
        //        var device = _deviceservice
        //                connection = ConnectionFactory.CreateTCPIPConnection(, 1001,
        //            Convert.ToInt32(waitBeforReadTextBox.Text),
        //            Convert.ToInt32(readTimeoutTextBox.Text),
        //            Convert.ToInt32(writeTimeoutTextBox.Text));
        //    }

        //    _stFace = new Clock(connection, (ProtocolType)protocolTypeCombobox.SelectedIndex,
        //        Convert.ToInt32(addressNUP.Value), ProtocolType.Suprema);

        //    //ProtocolType.Suprema
        //    var clock = new Clock(connection, DeviceInfo.DeviceProtocolType,
        //        DeviceInfo.TRT, DeviceInfo.SensorProtocolType);

        //    return true;
        //}


    }

}
