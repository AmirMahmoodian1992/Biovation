using Biovation.Brands.EOS.Manager;
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
using Logger = Biovation.CommonClasses.Logger;
using Timer = System.Timers.Timer;

namespace Biovation.Brands.EOS.Devices
{
    public class HanvonBase : Device, IDisposable
    {
        private readonly StFace _stFace;

        private readonly RestClient _restClient;
        private readonly LogService _logService;
        private readonly TaskService _taskService;
        private readonly DeviceBrands _deviceBrands;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly Dictionary<uint, Device> _onlineDevices;

        private System.Threading.Timer _fixDaylightSavingTimer;

        private readonly Dictionary<uint, Timer> _readOnlineLogTimer = new Dictionary<uint, Timer>();
        private readonly Dictionary<uint, DateTime> _lastReceivedLog = new Dictionary<uint, DateTime>();//it saves last log datetime even device is disconnect so its not a disposable object
        private readonly Dictionary<uint, int> _zeroLog = new Dictionary<uint, int>();// Saves how many times device sends zero log consecutive
        private const int ZeroLogRestart = 4000; //After ZeroLogRestart times that device sends zero log we have to restart it
        //private readonly EosCodeMappings EosCodeMappings;
        //private readonly DateTime _startDateTimeThreshold;
        //private readonly DateTime _endDateTimeThreshold;

        internal HanvonBase(DeviceBasicInfo deviceInfo, LogService logService, LogEvents logEvents,
            LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, FaceTemplateTypes faceTemplateTypes,
            RestClient restClient, Dictionary<uint, Device> onlineDevices, TaskService taskService, DeviceBrands deviceBrands) : base(deviceInfo,
            logEvents, logSubEvents, eosCodeMappings)
        {
            _restClient = restClient;
            _logService = logService;
            _onlineDevices = onlineDevices;
            _taskService = taskService;
            _deviceBrands = deviceBrands;
            _faceTemplateTypes = faceTemplateTypes;
            _stFace = new StFace(new TCPIPConnection
            {
                IP = DeviceInfo.IpAddress,
                Port = DeviceInfo.Port,
                ReadTimeout = 100,
                WriteTimeout = 100,
                WaitBeforeRead = 100,
                ReadInCompleteTimeOut = 10,
                RetryCount = 1
            });

            Valid = true;
        }

        public override bool Connect()
        {
            lock (_onlineDevices)
            {
                if (_onlineDevices.ContainsKey(DeviceInfo.Code))
                {
                    _onlineDevices[DeviceInfo.Code].Disconnect();
                    _onlineDevices.Remove(DeviceInfo.Code);

                    var connectionStatus = new ConnectionStatus
                    {
                        DeviceId = DeviceInfo.DeviceId,
                        IsConnected = false
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
                            EventLog = LogEvents.Disconnect
                        });
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            }
            Valid = true;
            var isConnect = IsConnected();
            if (!isConnect) return false;

            var setDateTimeResult = SetDateTime();
            if (!setDateTimeResult)
                Logger.Log($"Could not set the time of device {DeviceInfo.Code}");

            TimeZone(); //It should be called for the Format DateTime Func. (Knows work with Georgian or persian calender)

            try
            {
                //var daylightSaving = DateTime.Now.DayOfYear <= 81 || DateTime.Now.DayOfYear > 265 ? new DateTime(DateTime.Now.Year, 3, 22, 0, 2, 0) : new DateTime(DateTime.Now.Year, 9, 22, 0, 2, 0);
                //var dueTime = (daylightSaving.Ticks - DateTime.Now.Ticks) / 10000;
                var dueTime = (DateTime.Today.AddDays(1).AddMinutes(1) - DateTime.Now).TotalMilliseconds;
                _fixDaylightSavingTimer = new System.Threading.Timer(FixDaylightSavingTimer_Elapsed, null, (long)dueTime, (long)TimeSpan.FromHours(24).TotalMilliseconds);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, exception.Message);
            }

            _taskService.ProcessQueue(_deviceBrands.Eos, DeviceInfo.DeviceId).ConfigureAwait(false);
            if (!_onlineDevices.ContainsKey(DeviceInfo.Code))
            {
                _onlineDevices.Add(DeviceInfo.Code, this);
            }

            if (!_zeroLog.ContainsKey(DeviceInfo.Code))
            {
                _zeroLog.Add(DeviceInfo.Code, 0);
            }
            else
            {
                _zeroLog[DeviceInfo.Code] = 0;
            }

            if (!_lastReceivedLog.ContainsKey(DeviceInfo.Code))
            {
                _lastReceivedLog.Add(DeviceInfo.Code, new DateTime(2017, 1, 1));
            }
            if (!_readOnlineLogTimer.ContainsKey(DeviceInfo.Code))
                SetTimer(DeviceInfo.Code);
            else
            {
                _readOnlineLogTimer[DeviceInfo.Code]?.Stop();
                _readOnlineLogTimer[DeviceInfo.Code]?.Start();
                _readOnlineLogTimer[DeviceInfo.Code].Enabled = false;
                _readOnlineLogTimer[DeviceInfo.Code].Enabled = true;

            }

            //Task.Run(() => { ReadOnlineLog(Token); }, Token).ConfigureAwait(false);
            return true;
        }

        private bool IsConnected()
        {
            try
            {
                lock (_stFace)
                {
                    try
                    {
                        ((TCPIPConnection)_stFace.Connection).IsProtected = false;
                        _stFace.Connect();
                        if (!string.IsNullOrWhiteSpace(DeviceInfo.DeviceLockPassword))
                        {
                            ((TCPIPConnection)_stFace.Connection).IsProtected = true;
                            ((TCPIPConnection)_stFace.Connection).Password = DeviceInfo.DeviceLockPassword;
                        }

                        if (_stFace.TestConnection())
                        {
                            Logger.Log(
                                $"Successfully connected to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}",
                                logType: LogType.Information);

                            return true;
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception, "", LogType.Verbose);
                    }

                }

                while (Valid)
                {
                    Logger.Log($"Could not connect to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}");

                    Thread.Sleep(10000);
                    Logger.Log($"Retrying connect to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}");
                    lock (_stFace)
                        try
                        {
                            ((TCPIPConnection)_stFace.Connection).IsProtected = false;
                            _stFace.Connect();
                            if (!string.IsNullOrWhiteSpace(DeviceInfo.DeviceLockPassword))
                            {
                                ((TCPIPConnection)_stFace.Connection).IsProtected = true;
                                ((TCPIPConnection)_stFace.Connection).Password = DeviceInfo.DeviceLockPassword;
                            }

                            if (!_stFace.TestConnection()) continue;
                            Logger.Log(
                                $"Successfully connected to device {DeviceInfo.Code} --> IP: {DeviceInfo.IpAddress}",
                                logType: LogType.Information);
                            return true;
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, "", LogType.Verbose);
                        }
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error in define and connect to STFace" + e.Message);
            }

            return false;
        }

        public bool SetDateTime()
        {
            if (!DeviceInfo.TimeSync)
                return true;

            for (var i = 0; i < 5; i++)
            {
                try
                {
                    lock (_stFace)
                        _stFace.SetDateTime(DateTime.Now);

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

        private void FixDaylightSavingTimer_Elapsed(object state)
        {
            SetDateTime();
        }

        internal override User GetUser(uint userId)
        {
            try
            {
                StFaceUserInfo terminalUserData;
                lock (_stFace)
                    terminalUserData = _stFace.GetUserInfo(userId);

                if (terminalUserData is null) return new User();
                var user = new User
                {
                    UserName = terminalUserData.UserName,
                    Password = terminalUserData.Password,
                    Code = terminalUserData.Id,
                    IsActive = true,
                    // UniqueId = (- long.Parse(terminalUserData.PersonalNumber)),
                    IsAdmin = terminalUserData.Privilege == 1,
                    SurName = terminalUserData.UserName?.Split(' ').LastOrDefault(),
                    FirstName = terminalUserData.UserName?.Split(' ').FirstOrDefault(),
                    StartDate = DateTime.Parse("1970/01/01"),
                    EndDate = DateTime.Parse("2050/01/01")
                };

                var parseResult = long.TryParse(terminalUserData.PersonalNumber, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out var uniqueId);
                if (parseResult)
                    user.UniqueId = -uniqueId;

                if (!(terminalUserData.CardNumber is null || string.Equals(terminalUserData.CardNumber, "0xffffffff",
                    StringComparison.InvariantCultureIgnoreCase)))
                {
                    user.IdentityCard = new IdentityCard
                    {
                        Id = (int)terminalUserData.Id,
                        Number = terminalUserData.CardNumber,
                        DataCheck = 0,
                        IsActive = !string.Equals(terminalUserData.CardNumber, "0xffffffff",
                            StringComparison.InvariantCultureIgnoreCase)
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

            try
            {
                if (_zeroLog.ContainsKey(DeviceInfo.Code))
                {
                    _zeroLog.Remove(DeviceInfo.Code);
                }
                //if (_readOnlineLogTimer.ContainsKey(DeviceInfo.Code))
                //{
                //    _readOnlineLogTimer[DeviceInfo.Code].Dispose();
                //    _readOnlineLogTimer.Remove(DeviceInfo.Code);
                //}
            }
            catch (Exception e)
            {
                Logger.Log($@"The timer of device {DeviceInfo.Code} can't stop: " + e);
            }


            Valid = false;
            return true;
        }

        public override bool DeleteUser(uint sUserId)
        {
            lock (_stFace)
                if (!_stFace.TestConnection())
                    return false;

            try
            {
                bool deletion;
                lock (_stFace)
                {
                    var user = _stFace.GetUserInfo((int)sUserId);
                    if (user == null) return true;
                }

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
                    Privilege = user.IsAdmin ? 1 : 0,
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


                    var userCard = user.IdentityCard;
                    if (userCard != null)
                    {
                        hasCard = true;
                        transfereeUser.CardNumber = userCard.Number;
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
                    if (user.FaceTemplates?.Count > 0)
                    {
                        var userFace = user.FaceTemplates?.First();
                        transfereeUser.FaceData = Encoding.ASCII
                            .GetString(userFace.Template, 0, userFace.Template.Length).Split('=').SkipLast(1).ToList();
                        hasFace = true;
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

        public override List<User> GetAllUsers(bool embedTemplate = false)
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
                        IsAdmin = retrievedUser.Privilege == 1,
                        IsActive = true,
                        // UniqueId = (- long.Parse(terminalUserData.PersonalNumber)),
                        SurName = retrievedUser.UserName?.Split(' ').LastOrDefault(),
                        FirstName = retrievedUser.UserName?.Split(' ').FirstOrDefault(),
                        StartDate = DateTime.Parse("1970/01/01"),
                        EndDate = DateTime.Parse("2050/01/01")
                    };

                    if (embedTemplate)
                    {


                        if (!(retrievedUser.CardNumber is null || string.Equals(retrievedUser.CardNumber, "0xffffffff",
                            StringComparison.InvariantCultureIgnoreCase)))
                        {
                            user.IdentityCard = new IdentityCard
                            {
                                Id = (int)retrievedUser.Id,
                                Number = retrievedUser.CardNumber,
                                DataCheck = 0,
                                IsActive = !string.Equals(retrievedUser.CardNumber, "0xffffffff",
                                    StringComparison.InvariantCultureIgnoreCase)
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
                    }

                    usersList.Add(user);
                }
            }
            catch (Exception e)
            {
                Logger.Log((e));
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

        public virtual ResultViewModel ReadOnlineLog(object token)
        {

            try
            {
                string eosDeviceType;
                lock (_stFace)
                    eosDeviceType = _stFace.GetModel();

                Logger.Log($"--> Retrieving Log from Terminal : {DeviceInfo.Code} Device type: {eosDeviceType}");

                bool deviceConnected;

                lock (_stFace)
                    deviceConnected = _stFace.Connected;

                if (deviceConnected && Valid && _onlineDevices.ContainsKey(DeviceInfo.Code))
                {
                    try
                    {
                        if (_lastReceivedLog.ContainsKey(DeviceInfo.Code))
                        {
                            var lastLogReceivedDateTime = _lastReceivedLog[DeviceInfo.Code];
                            var startDateTime = lastLogReceivedDateTime;
                            var difference = DateTime.Now.Subtract(lastLogReceivedDateTime);


                            if (difference < new TimeSpan(0, 0, 2, 0))
                            {
                                startDateTime = new DateTime(lastLogReceivedDateTime.Year, lastLogReceivedDateTime.Month, lastLogReceivedDateTime.Day);
                                startDateTime += new TimeSpan(lastLogReceivedDateTime.Hour, lastLogReceivedDateTime.Minute - 5, 0);
                            }
                            else if (difference < new TimeSpan(0, 1, 0, 0))
                            {
                                startDateTime = new DateTime(lastLogReceivedDateTime.Year, lastLogReceivedDateTime.Month, lastLogReceivedDateTime.Day);
                                startDateTime += new TimeSpan(lastLogReceivedDateTime.Hour - 1, lastLogReceivedDateTime.Minute - 30, 0);
                            }
                            else if (difference < new TimeSpan(7, 0, 0, 0))
                            {
                                startDateTime = new DateTime(lastLogReceivedDateTime.Year, lastLogReceivedDateTime.Month, lastLogReceivedDateTime.Day);
                                startDateTime += new TimeSpan(-1, 0, 0, 0);
                            }
                            else
                            {
                                Valid = false;
                            }

                            var res = ReadOfflineLogInPeriod(null, startDateTime, DateTime.Now);
                            if (res.Success)
                            {
                                _lastReceivedLog[DeviceInfo.Code] = DateTime.Now;
                            }
                            Logger.Log(res.Message);
                        }
                        else
                        {
                            Valid = false;
                        }

                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

                //_stFace?.Disconnect();
                // _stFace?.Dispose();
                //Disconnect();
                if (!Valid)
                    Connect();

                return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 1, Message = "0" };
            }
            catch (Exception exception)
            {
                Logger.Log(exception, "Clock " + DeviceInfo.Code);
            }

            Logger.Log("Connection fail. Cannot connect to device: " + DeviceInfo.Code + ", IP: " +
                       DeviceInfo.IpAddress);

            if (!Valid)
                Connect();

            //EosServer.IsRunning[(uint)DeviceInfo.Code] = false;
            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
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

                return Math.Abs(changedTimeZone.Hour - dateTime.Hour) == 0 &&
                       changedTimeZone.Minute - dateTime.Minute < 1;
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

        public override ResultViewModel ReadOfflineLogInPeriod(object cancellationToken, DateTime? startTime,
            DateTime? endTime,
            bool saveFile = false)
        {
            var logs = new List<string>();
            var eosLogs = new List<Log>();
            var invalidTime = false;
            if (startTime is null || startTime < new DateTime(2017, 1, 1) || startTime > DateTime.Now)
            {
                startTime = new DateTime(2017, 1, 1);
                invalidTime = true;
            }

            if (endTime is null || endTime > DateTime.Now || endTime < new DateTime(2017, 1, 1))
            {
                endTime = DateTime.Now;
                invalidTime = true;
            }

            if (invalidTime)
                Logger.Log("The chosen Time Period is wrong.");

            string text;
            bool flag;

            lock (_stFace)
            {
                var command =
                    $"GetRecord(start_time= \"{_stFace.FormatDateTime((DateTime)startTime)}\" end_time=\"{_stFace.FormatDateTime((DateTime)endTime)}\" )";
                Logger.Log(command);
                flag = _stFace.SendCommandAndGetResult(command, out text);
                //Logger.Log("OutText : " + text);
            }

            if (!flag)
                return new ResultViewModel
                { Success = false, Message = "Can't communicate with device with message" + text, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
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

            if (logs.Count <= 0)
            {
                if (_zeroLog.ContainsKey(DeviceInfo.Code))
                {
                    _zeroLog[DeviceInfo.Code]++;
                    if (_zeroLog[DeviceInfo.Code] > ZeroLogRestart)
                    {
                        Logger.Log($@"Device {DeviceInfo.Code} have to restart");
                        Valid = false;//for restart
                        _zeroLog[DeviceInfo.Code] = 0;
                    }
                }
                else
                {
                    Valid = false; //for restart
                }
                return new ResultViewModel
                {
                    Success = false,
                    Message = logs.Count < 0 ? "Can't communicate with device" + text : "The Log count is zero: " + text,
                    Code = Convert.ToInt64(TaskStatuses.FailedCode)
                };
            }

            if (_zeroLog.ContainsKey(DeviceInfo.Code))
            {
                _zeroLog[DeviceInfo.Code] = 0;
            }
            else
            {
                Valid = false;
            }


            //Logger.Log($@"THe Log Count is {logs.Count} At the time {DateTime.Now}");
            var records = logs.Select(FaceIdRecord.Parse).ToList();

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
                            DeviceId = DeviceInfo.DeviceId,
                            DeviceCode = DeviceInfo.Code,
                            InOutMode = DeviceInfo.DeviceTypeId,
                            SubEvent = LogSubEvents.Normal,
                            MatchingType = EosCodeMappings.GetMatchingTypeGenericLookup((int)record.FaceIdCheckmethodType),
                            //RawData = new string(record.RawData.Where(c => !char.IsControl(c)).ToArray()),
                            EventLog = LogEvents.Authorized,
                            TnaEvent = 0,
                        };
                        eosLogs.Add(receivedLog);
                        Logger.Log($@"<--
   +TerminalID:{DeviceInfo.Code}
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
                    Logger.Log(ex, "Clock " + DeviceInfo.Code + ": " +
                                   "Error while Inserting Data to Attendance . record: " + record);
                }
            }

            _logService.AddLog(eosLogs);
            return new ResultViewModel
            { Success = true, Message = $"{eosLogs.Count} Logs retrieved from device {DeviceInfo.Code}", Code = Convert.ToInt64(TaskStatuses.DoneCode) };
        }

        private void SetTimer(uint deviceCode)
        {
            _readOnlineLogTimer.Add(deviceCode, new Timer(10000));
            _readOnlineLogTimer[deviceCode].Elapsed += (sender, e) =>
            {
                var timer = sender as Timer;
                timer?.Stop();
                ReadOnlineLog(Token);
                timer?.Start();
            };
            _readOnlineLogTimer[deviceCode].Enabled = true;
            _readOnlineLogTimer[deviceCode].AutoReset = true;
        }


        public void Dispose()
        {
            try
            {
                _stFace?.Dispose();
                _fixDaylightSavingTimer?.Dispose();
            }
            catch (Exception exception)
            {
                Logger.Log(exception, exception.Message);
            }
        }
    }
}
