using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Service;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using EosClocks;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logger = Biovation.CommonClasses.Logger;

namespace Biovation.Brands.EOS.Devices
{
    public class HanvonBase : Device
    {
        private readonly StFace _stFace;
        private readonly DeviceBasicInfo _deviceInfo;
        private readonly EosLogService _eosLogService;

        private readonly LogEvents _logEvents;
        private readonly RestClient _restClient;
        private readonly TaskManager _taskManager;
        private readonly LogSubEvents _logSubEvents;
        private readonly UserCardService _userCardService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly Dictionary<uint, Device> _onlineDevices;

        private bool _valid;
        private int _counter;

        //private readonly EosCodeMappings _eosCodeMappings;
        //private readonly DateTime _startDateTimeThreshold;
        //private readonly DateTime _endDateTimeThreshold;

        internal HanvonBase(DeviceBasicInfo deviceInfo, EosLogService eosLogService, LogEvents logEvents,
            LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, FaceTemplateTypes faceTemplateTypes,
            UserCardService userCardService, TaskManager taskManager, RestClient restClient, Dictionary<uint, Device> onlineDevices) : base(deviceInfo, eosLogService, logEvents, logSubEvents, eosCodeMappings)
        {
            _valid = false;
            _logEvents = logEvents;
            _restClient = restClient;
            _onlineDevices = onlineDevices;
            _deviceInfo = deviceInfo;
            _taskManager = taskManager;
            _logSubEvents = logSubEvents;
            _eosLogService = eosLogService;
            _userCardService = userCardService;
            _faceTemplateTypes = faceTemplateTypes;
            _stFace = new StFace(new TCPIPConnection
            { IP = _deviceInfo.IpAddress, Port = _deviceInfo.Port, ReadTimeout = 100, WriteTimeout = 100 });
        }

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

                        _eosLogService.AddLog(new Log
                        {
                            DeviceId = _deviceInfo.DeviceId,
                            LogDateTime = DateTime.Now,
                            EventLog = _logEvents.Disconnect
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

            try
            {
                if (_deviceInfo.TimeSync)
                    lock (_stFace)
                    {
                        _stFace.SetDateTime(DateTime.Now);
                    }
            }
            catch (Exception)
            {
                Thread.Sleep(500);
                try
                {
                    if (_deviceInfo.TimeSync)
                        lock (_stFace)
                        {
                            _stFace.SetDateTime(DateTime.Now);
                        }
                }
                catch (Exception innerException)
                {
                    Logger.Log(innerException);
                }
            }

            _taskManager.ProcessQueue();
            _valid = true;

            Task.Run(() => { ReadOnlineLog(Token); }, Token);
            return true;
        }

        private bool IsConnected()
        {
            try
            {
                lock (_stFace)
                {
                    _stFace.Connect();

                    if (_stFace.Connected)
                    {
                        Logger.Log($"Successfully connected to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}",
                            logType: LogType.Information);
                        return true;
                    }
                }

                while (true)
                {
                    Logger.Log($"Could not connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");

                    Thread.Sleep(600);
                    Logger.Log($"Retrying connect to device {_deviceInfo.Code} --> IP: {_deviceInfo.IpAddress}");
                    lock (_stFace)
                        if (_stFace.TestConnection())
                            return true;
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error in define and connect to STFace" + e.Message);
            }

            return false;
        }

        internal override User GetUser(uint userId)
        {
            try
            {
                StFaceUserInfo terminalUserData;
                lock (_stFace)
                {
                    terminalUserData = _stFace.GetUserInfo(userId);
                }
                if (terminalUserData is null) return new User();
                var user = new User()
                {
                    UserName = terminalUserData.UserName,
                    Password = terminalUserData.Password,
                    Code = terminalUserData.Id,
                    // UniqueId = (- long.Parse(terminalUserData.PersonalNumber)),
                    IsAdmin = terminalUserData.Privilege == 1
                };

                if (!(terminalUserData.CardNumber is null || terminalUserData.CardNumber == "0xffffffff"))
                {
                    user.IdentityCard = new IdentityCard()
                    {
                        Id = (int)terminalUserData.Id,
                        Number = terminalUserData.CardNumber,
                        DataCheck = 0,
                        IsActive = terminalUserData.CardNumber != "0xffffffff"
                    };
                }

                //Face
                try
                {
                    user.FaceTemplates ??= new List<FaceTemplate>();
                    var faceData = terminalUserData.FaceData.SelectMany(s =>
                            Encoding.ASCII.GetBytes(s)).ToArray();
                    var faceTemplate = new FaceTemplate
                    {
                        Index = 1,
                        FaceTemplateType = _faceTemplateTypes.EOSHanvon,
                        UserId = user.Id,
                        Template = faceData,
                        CheckSum = faceData.Sum(x => x),
                        Size = faceData.Length
                    };
                    user.FaceTemplates.Add(faceTemplate);
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                }

                return user;
            }
            catch (Exception e)
            {
                Logger.Log("Error in GetUser from STFace " + e.Message);
                return new User();
            }
        }

        public override bool Disconnect()
        {
            lock (_stFace)
            {
                _stFace?.Disconnect();
                _stFace?.Dispose();
            }
            _valid = false;
            return true;
        }

        public override bool DeleteUser(uint sUserId)
        {
            lock (_stFace)
                if (!_stFace.TestConnection()) return false;

            try
            {
                StFaceUserInfo user;
                bool deletion;
                lock (_stFace)
                    user = _stFace.GetUserInfo((int)sUserId);

                if (user == null) return false;
                lock (_stFace)
                    deletion = _stFace.DeleteUser((int)sUserId);

                return deletion;
            }
            catch (Exception)
            {
                //var message = ex.Message;
            }

            return false;
        }

        public override bool TransferUser(User user)
        {
            if (user == null) return false;
            try
            {
                var transfereeUser = new StFaceUserInfo()
                {
                    Authority = FaceIdAuthority.AttendanceAndAccessControl,
                    UserName = user.UserName,
                    Id = user.Code,
                    //PersonalNumber = (-user.UniqueId).ToString(),
                    Privilege = 1,
                    VerifyStyle = ZkVerifyStyle.FaceOrPasswordOrCard,
                };
                // pwd
                if (!(user.Password is null))
                {
                    transfereeUser.Password = user.Password;
                }


                #region card

                var hasCard = false;
                try
                {

                    var userCards = _userCardService.GetCardsByFilter(user.Id, true);
                    if (userCards != null)
                    {
                        var userCard = userCards.Data.Data.FirstOrDefault();
                        if (userCard != null)
                        {
                            hasCard = true;
                            transfereeUser.CardNumber = userCard.CardNum;
                        }
                    }
                    else
                    {
                        transfereeUser.CardNumber = "0Xffffffff";
                        Logger.Log($"The User : {user.Id} Do not have card or query do not return any thing");
                    }

                }
                catch (Exception e)
                {
                    transfereeUser.CardNumber = "0Xffffffff";
                    Logger.Log(e.ToString());
                }

                #endregion


                //Face
                var hasFace = false;
                try
                {
                    var userFaces = user.FaceTemplates;
                    var userFace = userFaces?.FirstOrDefault();
                    if (userFace != null)
                    {
                        hasFace = true;
                        transfereeUser.FaceData = Encoding.ASCII.GetString(userFace.Template, 0, userFace.Template.Length).Split('=').SkipLast(1).ToList();
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Error in Face part of Transfer User" + e.Message);
                    return false;
                }

                if (hasFace && hasCard)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.BothFaceAndCard;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.BothFaceAndCard;
                }
                else if (hasFace)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.Face;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.Face;
                }
                else if (hasCard)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.Card;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.Card;
                }
                else
                {
                    return false;
                }
                bool result;
                lock (_stFace)
                {
                    result = _stFace.SetUserInfo(transfereeUser);
                }
                return result;
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
                return false;
            }
        }

        public bool DeleteAllUser()
        {
            try
            {
                bool deleted;
                lock (_stFace)
                    deleted = _stFace.DeleteAllUsers();

                return deleted;
            }
            catch (Exception ex)
            {
                Logger.Log("Error in Delete All User " + ex.Message);
            }

            return false;
        }

        public override List<User> GetAllUsers()
        {
            var usersList = new List<User>();
            try
            {
                List<StFaceUserInfo> usersOfDevice;
                lock (_stFace)
                    usersOfDevice = _stFace.GetUserList();

                foreach (var retrievedUser in usersOfDevice)
                {
                    var user = new User
                    {
                        UserName = retrievedUser.UserName,
                        Password = retrievedUser.Password,
                        Code = retrievedUser.Id,
                        // UniqueId = (- long.Parse(user.PersonalNumber)),
                        IsAdmin = retrievedUser.Privilege == 1
                    };

                    if (!(retrievedUser.CardNumber is null || string.Equals(retrievedUser.CardNumber, "0xffffffff", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        user.IdentityCard = new IdentityCard()
                        {
                            Id = (int)retrievedUser.Id,
                            Number = retrievedUser.CardNumber,
                            DataCheck = 0,
                            IsActive = !string.Equals(retrievedUser.CardNumber, "0xffffffff", StringComparison.InvariantCultureIgnoreCase)
                        };
                    }

                    //Face
                    try
                    {
                        user.FaceTemplates ??= new List<FaceTemplate>();
                        var faceData = retrievedUser.FaceData.SelectMany(s =>
                                Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
                        var faceTemplate = new FaceTemplate
                        {
                            Index = 1,
                            FaceTemplateType = _faceTemplateTypes.EOSHanvon,
                            UserId = retrievedUser.Id,
                            Template = faceData,
                            CheckSum = faceData.Sum(x => x),
                            Size = faceData.Length
                        };

                        user.FaceTemplates.Add(faceTemplate);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e);
                    }
                    usersList.Add(user);
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return usersList;
        }

        public bool ExistOnDevice(uint id)
        {
            try
            {
                StFaceUserInfo terminalUser;
                lock (_stFace)
                    terminalUser = _stFace.GetUserInfo(id);

                if (!(terminalUser is null))
                    return true;
            }
            catch (Exception)
            {
                Logger.Log("Failed to getUser");
            }

            return false;
        }

        public override ResultViewModel ReadOnlineLog(object token)
        {
            Thread.Sleep(1000);

            try
            {
                string eosDeviceType;
                lock (_stFace)
                    eosDeviceType = _stFace.GetModel();

                Logger.Log($"--> Retrieving Log from Terminal : {_deviceInfo.Code} Device type: {eosDeviceType}");

                bool deviceConnected;

                lock (_stFace)
                    deviceConnected = _stFace.Connected;

                while (deviceConnected && _valid)
                {
                    try
                    {
                        bool empty;
                        lock (_stFace)
                            empty = _stFace.IsEmpty();

                        while (!empty && _valid)
                        {
                            var test = true;
                            var exceptionTester = false;
                            while (test && _valid)
                            {
                                Record record = null;

                                try
                                {
                                    while (record == null)
                                    {
                                        lock (_stFace)
                                        {
                                            record = _stFace.GetRecord();
                                            Thread.Sleep(500);
                                            //////Danger: maybe infinite loop
                                            //TODO: Fix it
                                        }
                                    }
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
                                            //SubEvent = _eosCodeMappings.GetLogSubEventGenericLookup(record.RawData),
                                            //RawData = new string(record.RawData.Where(c => !char.IsControl(c)).ToArray()),
                                            EventLog = _logEvents.Authorized,
                                            TnaEvent = 0,
                                        };

                                        _eosLogService.AddLog(receivedLog);
                                        test = false;
                                        Logger.Log($@"<--
   +TerminalID:{_deviceInfo.Code}
   +UserID:{receivedLog.UserId}
   +DateTime:{receivedLog.LogDateTime}", logType: LogType.Information);

                                        lock (_stFace)
                                        {
                                            _stFace.NextRecord();
                                        }
                                    }
                                    else
                                    {
                                        if (!exceptionTester)
                                        {
                                            Logger.Log("Null record.");
                                        }
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex, "Clock " + _deviceInfo.Code + ": " +
                                        "Error while Inserting Data to Attendance . record: " + record);
                                }
                            }

                            lock (_stFace)
                                empty = _stFace.IsEmpty();
                        }
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    lock (_stFace)
                        deviceConnected = _stFace.Connected;
                }

                //_stFace?.Disconnect();
                // _stFace?.Dispose();
                //Disconnect();
                if (_valid)
                    Connect();

                return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 1, Message = "0" };
            }
            catch (Exception exception)
            {
                Logger.Log(exception, "Clock " + _deviceInfo.Code);
            }

            Logger.Log("Connection fail. Cannot connect to device: " + _deviceInfo.Code + ", IP: " + _deviceInfo.IpAddress);

            if (_valid)
                Connect();

            //EosServer.IsRunning[(uint)_deviceInfo.Code] = false;
            return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };
        }

        public DateTime TimeZone()
        {
            DateTime timezone;
            lock (_stFace)
                timezone = _stFace.GetDateTime();

            return timezone;
        }

        public string FirmwareVersion()
        {
            string firmwareVersion;
            lock (_stFace)
                firmwareVersion = _stFace.GetFirmwareVersion();

            return firmwareVersion;
        }

        public string Model()
        {
            string model;
            lock (_stFace)
                model = _stFace.GetModel();

            return model;
        }

        public int DeviceCapacity()
        {
            int capacity;
            lock (_stFace)
                capacity = _stFace.GetDeviceCapacity();

            return capacity;
        }

        public string Serial()
        {
            string serial;
            lock (_stFace)
                serial = _stFace.GetSerial();

            return serial;
        }

        public bool TransferTimeZone(DateTime dateTime)
        {
            try
            {
                DateTime changedTimeZone;
                lock (_stFace)
                {
                    _stFace.SetDateTime(dateTime);
                    changedTimeZone = _stFace.GetDateTime();
                }

                return Math.Abs(changedTimeZone.Hour - dateTime.Hour) == 0 && changedTimeZone.Minute - dateTime.Minute < 1;
            }
            catch (Exception)
            {
                Logger.Log("can't change TimeZone");
                return false;
            }
        }

        public bool UpdateFirmware(string filePath)
        {
            bool updateFirm;
            lock (_stFace)
                updateFirm = _stFace.UpdateFirmware(filePath);

            return updateFirm;
        }

        public override List<Log> ReadLogOfPeriod(DateTime startTime, DateTime endTime)
        {
            var logs = new List<string>();
            var eosLogs = new List<Log>();
            var invalidTime = false;
            if (startTime < new DateTime(1921, 3, 21) || startTime > new DateTime(2021, 3, 19))
            {
                startTime = new DateTime(1921, 3, 21);
                invalidTime = true;
            }

            if (endTime > new DateTime(2021, 3, 19) || endTime < new DateTime(1921, 3, 21))
            {
                endTime = new DateTime(2021, 3, 19);
                invalidTime = true;
            }

            if (invalidTime)
                Logger.Log("The chosen Time Period is wrong.");

            string text;
            bool flag;

            lock (_stFace)
            {
                var command =
                    $"GetRecord(start_time= \"{_stFace.FormatDateTime(startTime)}\" end_time=\"{_stFace.FormatDateTime(endTime)}\" )";
                flag = _stFace.SendCommandAndGetResult(command, out text);
            }

            if (!flag) return eosLogs;
            var num = text.IndexOf("time=", 0, StringComparison.Ordinal);
            while (num > 0 && num + "time=".Length < text.Length)
            {
                var num2 = text.IndexOf("time=", num + "time=".Length, StringComparison.Ordinal);
                var flag2 = num2 == -1;
                if (flag2)
                {
                    num2 = text.Length - 1;
                }
                var item = text.Substring(num, num2 - num);
                logs.Add(item);
                num = num2;
            }

            if (logs.Count <= 0) return eosLogs;
            var records = logs.Select(FaceIdRecord.Parse).Cast<Record>().ToList();

            foreach (var record in records)
            {
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
                            //SubEvent = _eosCodeMappings.GetLogSubEventGenericLookup(record.RawData),
                            //RawData = new string(record.RawData.Where(c => !char.IsControl(c)).ToArray()),
                            EventLog = _logEvents.Authorized,
                            TnaEvent = 0,
                        };
                        eosLogs.Add(receivedLog);
                        Logger.Log($@"<--
   +TerminalID:{_deviceInfo.Code}
   +UserID:{receivedLog.UserId}
   +DateTime:{receivedLog.LogDateTime}", logType: LogType.Information);
                    }
                    else
                    {
                        Logger.Log("Null record.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, "Clock " + _deviceInfo.Code + ": " +
                                   "Error while Inserting Data to Attendance . record: " + record);
                }
            }

            _eosLogService.AddLog(eosLogs);
            return eosLogs;
        }
    }
}
