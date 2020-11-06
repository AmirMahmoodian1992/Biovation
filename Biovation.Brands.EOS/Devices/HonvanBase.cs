using Biovation.Brands.EOS.Manager;
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
using System.Windows.Automation;
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


        internal HonvanBase(DeviceBasicInfo deviceInfo, EosLogService eosLogService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, FaceTemplateTypes faceTemplateTypes, FaceTemplateService faceTemplateService,  UserCardService userCardService) : base(deviceInfo, eosLogService, logEvents, logSubEvents, eosCodeMappings)
        {
            _valid = true;
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
                    _stFace.SetDateTime(DateTime.Now);
            }
            catch (Exception)
            {
                //CommonClasses.Logger.Log(exception);
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
            _stFace = new StFace(new TCPIPConnection { IP = _deviceInfo.IpAddress, Port = _deviceInfo.Port, ReadTimeout = 100, WriteTimeout = 100 });
            _stFace.Connect();
            if (_stFace.Connected)
            //if (_stFace.Connected)
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
                var terminalUserData = _stFace.GetUserInfo((long)userId);
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
            //_valid = false;
            _stFace?.Disconnect();
            _stFace?.Dispose();
            return true;
        }

        public override bool DeleteUser(uint sUserId)
        {
            if (_stFace.TestConnection())
            {
                try
                {
                    _stFace?.Connect();

                    // var userFaceTemplates = _stFace.GetUserInfo((int)sUserId);
                    //_stFace.DeleteUserFaceTemplate(userFaceTemplates.)
                    return _stFace.DeleteUser((int)sUserId);

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
                    Console.WriteLine(e);
                    throw;
                }

                if (hasFace && hasCard)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.BothFaceAndCard;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.BothFaceAndCard;
                    return _stFace.SetUserInfo(transfereeUser);
                }
                else if (hasFace && !hasCard)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.Face;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.Face;
                    return _stFace.SetUserInfo(transfereeUser);
                }
                else if (hasCard && !hasFace)
                {
                    transfereeUser.CheckmethodType = FaceIdCheckmethodType.Card;
                    transfereeUser.OpenDoorType = FaceIdCheckmethodType.Card;
                    return _stFace.SetUserInfo(transfereeUser);
                }
                return false;
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
                return _stFace.DeleteAllUsers();
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

            try
            {
                lock (_stFace)
                {

                    var users = _stFace.GetUserList();
                    foreach (var user in users)
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
            }
            catch (Exception)
            {
                //ignore
            }

            return usersList;
        }

        public bool ExistOnDevice(uint id)
        {
            var deviceUser = _stFace.GetUserInfo(id);
            if (!(deviceUser is null))
            {
                return true;
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
                    var eosDeviceType = _stFace.GetModel();
                    Logger.Log($"--> Retrieving Log from Terminal : {_deviceInfo.Code} Device type: {eosDeviceType}");

                    while (_stFace.Connected && _valid)
                    {
                        try
                        {
                            while (!_stFace.IsEmpty() && _valid)
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
                                            record = (Record)_stFace.GetRecord();
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
                                                //SubEvent = _eosCodeMappings.GetLogSubEventGenericLookup(record.RawData),
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

                                            _stFace.NextRecord();
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
            return _stFace.GetDateTime();
        }

        public string FirmwareVersion() 
        {
            return _stFace.GetFirmwareVersion();
        }
        public string Model()
        {
            return _stFace.GetModel();
        }

        public int DeviceCapacity()
        {
            return _stFace.GetDeviceCapacity();
        }

        public string Serial()
        {
            return _stFace.GetSerial();
        }

        public bool TransferTimeZone(DateTime dateTime)
        {
            try
            {
                _stFace.SetDateTime(dateTime);
                var changedTimeZone = _stFace.GetDateTime().Minute;
                return Math.Abs(changedTimeZone - dateTime.Minute)<1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateFirmware(string filePath)
        {
            return _stFace.UpdateFirmware(filePath);
        }










    }

}
