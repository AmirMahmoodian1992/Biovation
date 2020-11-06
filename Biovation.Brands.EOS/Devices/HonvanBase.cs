﻿using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Service;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using EosClocks;
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
    public class HonvanBase : Device
    {
        private StFace _stFace;
        private readonly DeviceBasicInfo _deviceInfo;
        private readonly EosLogService _eosLogService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly EosCodeMappings _eosCodeMappings;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly UserCardService _userCardService;
        private bool _valid;
        private int _counter;


        internal HonvanBase(DeviceBasicInfo deviceInfo, EosLogService eosLogService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, FaceTemplateTypes faceTemplateTypes, FaceTemplateService faceTemplateService, UserCardService userCardService) : base(deviceInfo, eosLogService, logEvents, logSubEvents, eosCodeMappings)
        {
            _valid = false;
            _deviceInfo = deviceInfo;
            _eosLogService = eosLogService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _eosCodeMappings = eosCodeMappings;
            _faceTemplateTypes = faceTemplateTypes;
            _userCardService = userCardService;
            _faceTemplateService = faceTemplateService;
        }




        public override bool Connect()
        {

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

            _valid = true;
            Task.Run(() => { ReadOnlineLog(Token); }, Token);
            return true;
        }
        private bool IsConnected()
        {
            _stFace = new StFace(new TCPIPConnection { IP = _deviceInfo.IpAddress, Port = _deviceInfo.Port, ReadTimeout = 100, WriteTimeout = 100 });
            lock (_stFace)
            {
                _stFace.Connect();
            }
            if (_stFace.Connected)
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

        internal override User GetUser(uint userId)
        {
            try
            {
                var terminalUserData = new StFaceUserInfo();
                lock (_stFace)
                {
                    terminalUserData = _stFace.GetUserInfo((long)userId);
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

                if (!(terminalUserData.CardNumber is null || terminalUserData.CardNumber == "0Xffffffff"))
                {
                    user.IdentityCard = new IdentityCard()
                    {
                        Id = (int)terminalUserData.Id,
                        Number = terminalUserData.CardNumber,
                        DataCheck = 0,
                        IsActive = terminalUserData.CardNumber != "0Xffffffff"
                    };
                }

                //Face
                try
                {
                    user.FaceTemplates ??= new List<FaceTemplate>();
                    var faceData = terminalUserData.FaceData.SelectMany(s =>
                            System.Text.Encoding.ASCII.GetBytes(s)).ToArray();
                    var faceTemplate = new FaceTemplate
                    {
                        Index = 1,
                        FaceTemplateType = _faceTemplateTypes.EOSHonvan,
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
            if (_stFace.TestConnection())
            {
                try
                {
                    var user = new StFaceUserInfo();
                    bool deletion = false;
                    lock (_stFace)
                    {
                        user = _stFace.GetUserInfo((int)sUserId);
                    }
                    if (user != null)
                    {
                        lock (_stFace)
                        {
                            deletion = _stFace.DeleteUser((int)sUserId);
                        }
                    }
                    return deletion;
                }
                catch (Exception)
                {
                    //var message = ex.Message;

                }
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
                        var userCard = userCards.FirstOrDefault();
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
                    var userFaces = _faceTemplateService.FaceTemplates(userId: user.Id);
                    if (userFaces != null)
                    {
                        var userFace = userFaces.FirstOrDefault();
                        userFace = new FaceTemplate { Template = user.FaceTemplates[0].Template };
                        if (userFace != null)
                        {
                            hasFace = true;
                            transfereeUser.FaceData = Encoding.ASCII.GetString(userFace.Template, 0, userFace.Template.Length).Split('=').SkipLast(1).ToList();
                        }
                    }

                }
                catch (Exception e)
                {
                    Logger.Log("Error in Face part of Transfer User" + e.ToString());
                    return false;
                }

                if (hasFace && hasCard)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.BothFaceAndCard;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.BothFaceAndCard;
                }
                else if (hasFace && !hasCard)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.Face;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.Face;
                }
                else if (hasCard && !hasFace)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.Card;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.Card;
                }
                else
                {
                    return false;
                }
                bool result = false;
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
            bool deleted = false;
            try
            {
                lock (_stFace)
                {
                    deleted = _stFace.DeleteAllUsers();
                }
                return deleted;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            return false;

        }

        public override List<User> GetAllUsers()
        {

            var usersList = new List<User>();
            var sTUsers = new List<StFaceUserInfo>();
            try
            {
                lock (_stFace)
                {
                    sTUsers = _stFace.GetUserList();
                }
                foreach (var user in sTUsers)
                {

                    var tempUser = new User
                    {
                        UserName = user.UserName,
                        Password = user.Password,
                        Code = user.Id,
                        // UniqueId = (- long.Parse(user.PersonalNumber)),
                        IsAdmin = user.Privilege == 1
                    };
                    if (!(user.CardNumber is null || user.CardNumber == "0Xffffffff"))
                    {
                        tempUser.IdentityCard = new IdentityCard()
                        {
                            Id = (int)user.Id,
                            Number = user.CardNumber,
                            DataCheck = 0,
                            IsActive = user.CardNumber != "0Xffffffff"
                        };
                    }

                    //Face
                    try
                    {
                        tempUser.FaceTemplates ??= new List<FaceTemplate>();
                        var faceData = user.FaceData.SelectMany(s =>
                                System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
                        var faceTemplate = new FaceTemplate
                        {
                            Index = 1,
                            FaceTemplateType = _faceTemplateTypes.EOSHonvan,
                            UserId = user.Id,
                            Template = faceData,
                            CheckSum = faceData.Sum(x => x),
                            Size = faceData.Length
                        };
                        tempUser.FaceTemplates.Add(faceTemplate);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e);
                    }
                    usersList.Add(tempUser);

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
                var terminalUser = new StFaceUserInfo();
                lock (_stFace)
                {
                    terminalUser = _stFace.GetUserInfo(id);
                }
                if (!(terminalUser is null))
                {
                    return true;
                }
            }
            catch (Exception)
            {

                Logger.Log("Failed to getUser");
            }

            return false;
        }

        public override ResultViewModel ReadOnlineLog(object token)
        {
            var Object = new object();
            Thread.Sleep(1000);
            lock (Object)
            {
                try
                {
                    string eosDeviceType = "";
                    lock (_stFace)
                    {
                        eosDeviceType = _stFace.GetModel();
                    }
                    Logger.Log($"--> Retrieving Log from Terminal : {_deviceInfo.Code} Device type: {eosDeviceType}");

                    while (_stFace.Connected && _valid)
                    {
                        try
                        {
                            bool empty = false;
                            lock (_stFace)
                            {
                                empty = _stFace.IsEmpty();
                            }
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
                                                //////Danger: maybe inifinite loop
                                                //TODO: Fix it
                                            }
                                        }
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
                                {
                                    empty = _stFace.IsEmpty();
                                }
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

            if (_valid)
            {
                Connect();
            }

            //EosServer.IsRunning[(uint)_deviceInfo.Code] = false;
            return new ResultViewModel { Id = _deviceInfo.DeviceId, Validate = 0, Message = "0" };

        }

        public DateTime TimeZone()
        {
            DateTime timezone = new DateTime();
            lock (_stFace)
            {
                timezone = _stFace.GetDateTime();
            }
            return timezone;
        }

        public string FirmwareVersion()
        {
            string firmwareVersion = "";
            lock (_stFace)
            {
                firmwareVersion = _stFace.GetFirmwareVersion();
            }
            return firmwareVersion;
        }
        public string Model()
        {
            string model = "";
            lock (_stFace)
            {
                model = _stFace.GetModel();
            }
            return model;
        }

        public int DeviceCapacity()
        {
            int capacity = -1;
            lock (_stFace)
            {
                capacity = _stFace.GetDeviceCapacity();
            }
            return capacity;
        }

        public string Serial()
        {
            string serial = "";
            lock (_stFace)
            {
                serial = _stFace.GetSerial();
            }
            return serial;
        }

        public bool TransferTimeZone(DateTime dateTime)
        {
            try
            {
                DateTime changedTimeZone = new DateTime();
                lock (_stFace)
                {
                    _stFace.SetDateTime(dateTime);
                    changedTimeZone = _stFace.GetDateTime();
                }
                return ((Math.Abs(changedTimeZone.Hour - dateTime.Hour) == 0) && (changedTimeZone.Minute - dateTime.Minute) < 1);
            }
            catch (Exception)
            {
                Logger.Log("can't change TimeZone");
                return false;
            }
        }

        public bool UpdateFirmware(string filePath)
        {
            bool updateFirm = false;
            lock (_stFace)
            {
                updateFirm = _stFace.UpdateFirmware(filePath);
            }
            return updateFirm;
        }










    }

}
