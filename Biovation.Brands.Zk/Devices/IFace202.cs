using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;

namespace Biovation.Brands.ZK.Devices
{
    // ReSharper disable once InconsistentNaming
    public class IFace202 : Device
    {
        private readonly DeviceService _commonDeviceService = new DeviceService();

        internal IFace202(DeviceBasicInfo info) : base(info) { }
        public override bool GetAndSaveUser(long userId)
        {
            lock (ZKTecoSdk)
            {
                try
                {
                    Logger.Log("<--EventGetUserData");

                    if (ZKTecoSdk.SSR_GetUserInfo((int)DeviceInfo.Code, userId.ToString(), out var name,
                        out var password, out var privilege, out var enabled))
                    {
                        var user = new User
                        {
                            Id = 0,
                            Code = userId,
                            AdminLevel = privilege,
                            IsActive = enabled,
                            SurName = name.Split(' ').LastOrDefault(),
                            FirstName = name.Split(' ').FirstOrDefault(),
                            StartDate = DateTime.Parse("1970/01/01"),
                            EndDate = DateTime.Parse("2050/01/01"),
                            Password = password,
                            UserName = name,
                        };
                        var existUser = CommonUserService.GetUser(userCode:userId, withPicture:false);
                        if (existUser != null)
                        {
                            user = new User
                            {
                                Id = 0,
                                Code = userId,
                                AdminLevel = privilege,
                                IsActive = existUser.IsActive,
                                SurName = existUser.SurName,
                                FirstName = existUser.FirstName,
                                Email = existUser.Email,
                                EntityId = existUser.EntityId,
                                TelNumber = existUser.TelNumber,
                                UserName = name,
                                StartDate = DateTime.Parse("1970/01/01"),
                                EndDate = DateTime.Parse("2050/01/01"),
                                Password = password,
                                IsAdmin = existUser.IsAdmin,
                                Type = existUser.Type
                            };
                        }

                        CommonUserService.ModifyUser(user);
                        user.Id = (CommonUserService.GetUser(userCode: user.Code, withPicture: false)).Id;
                        Logger.Log("<--User is Modified");

                        user.FingerTemplates = new List<FingerTemplate>();
                        user.FaceTemplates = new List<FaceTemplate>();

                        try
                        {
                            if (ZKTecoSdk.GetStrCardNumber(out var cardNumber) && cardNumber != "0")
                            {
                                var card = new UserCard
                                {
                                    CardNum = cardNumber,
                                    IsActive = true,
                                    UserId = user.Id
                                };

                                CommonUserCardService.ModifyUserCard(card);
                                Logger.Log("<--User card is Modified");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Log(e);
                            //ignore
                        }

                        var retrievedFingerTemplates = new List<FingerTemplate>();
                        var retrievedFaceTemplates = new List<FaceTemplate>();

                        try
                        {
                            if (ZKTecoSdk.ReadAllTemplate((int)DeviceInfo.Code))
                                for (var i = 0; i <= 9; i++)
                                {
                                    if (!ZKTecoSdk.SSR_GetUserTmpStr((int)DeviceInfo.Code, user.Id.ToString(), i,
                                        out var tempData, out var tempLength))
                                    {
                                        Thread.Sleep(50);
                                        continue;
                                    }
                                    var fingerTemplate = new FingerTemplate
                                    {
                                        FingerIndex = BiometricTemplateManager.GetFingerIndex(i),
                                        FingerTemplateType = FingerTemplateTypes.VX10,
                                        UserId = user.Id,
                                        Template = Encoding.ASCII.GetBytes(tempData),
                                        CheckSum = Encoding.ASCII.GetBytes(tempData).Sum(x => x),
                                        Size = tempLength,
                                        Index = i
                                    };

                                    retrievedFingerTemplates.Add(fingerTemplate);

                                    if (existUser != null)
                                    {
                                        if (!existUser.FingerTemplates.Any(fp =>
                                            fp.FingerIndex.Code == BiometricTemplateManager.GetFingerIndex(i).Code && fp.FingerTemplateType == FingerTemplateTypes.VX10))
                                        {
                                            user.FingerTemplates.Add(fingerTemplate);
                                            Logger.Log($"A finger print with index: {i} is retrieved for user: {user.Id}");
                                        }
                                        else
                                        {
                                            Logger.Log($"The User: {user.Id} has a finger print with index: {i}");
                                        }
                                    }
                                    else
                                    {
                                        user.FingerTemplates.Add(fingerTemplate);
                                        Logger.Log($"A finger print with index: {i} is retrieved for user: {user.Id}");
                                    }
                                }

                            if (user.FingerTemplates.Count > 0)
                            {
                                foreach (var fingerTemplate in user.FingerTemplates)
                                {
                                    FingerTemplateService.ModifyFingerTemplate(fingerTemplate);
                                }

                                Logger.Log("<-- Finger Template is modified");
                            }
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, $"Error in getting finger template from device: {DeviceInfo.Code}");
                        }

                        try
                        {
                            var faceStr = "";
                            var faceLen = 0;
                            for (var i = 0; i < 9; i++)
                            {
                                if (!ZKTecoSdk.GetUserFaceStr((int)DeviceInfo.Code, userId.ToString(), 50,
                                    ref faceStr, ref faceLen))
                                {
                                    Thread.Sleep(50);
                                    continue;
                                }
                                var faceTemplate = new FaceTemplate
                                {
                                    Index = 50,
                                    FaceTemplateType = FaceTemplateTypes.ZKVX7,
                                    UserId = user.Id,
                                    Template = Encoding.ASCII.GetBytes(faceStr),
                                    CheckSum = Encoding.ASCII.GetBytes(faceStr).Sum(x => x),
                                    Size = faceLen,
                                };

                                retrievedFaceTemplates.Add(faceTemplate);

                                if (existUser != null)
                                {
                                    if (existUser.FaceTemplates.Any(fp => fp.Index == 50)) break;
                                    user.FaceTemplates.Add(faceTemplate);
                                    break;
                                }

                                user.FaceTemplates.Add(faceTemplate);
                                break;
                            }

                            if (user.FaceTemplates.Count > 0)
                            {
                                foreach (var faceTemplates in user.FaceTemplates)
                                {
                                    FaceTemplateService.ModifyFaceTemplate(faceTemplates);
                                }

                                Logger.Log("<-- face Template is modified");
                            }

                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, $"Error in getting face template from device: {DeviceInfo.Code}");
                        }

                        Logger.Log($@" The user: {userId} is retrieved from device:{DeviceInfo.Code}
    Info: Finger retrieved count: {retrievedFingerTemplates.Count}, inserted count: {user.FingerTemplates.Count}, 
          Face retrieved count: {retrievedFaceTemplates.Count}, inserted count: {user.FaceTemplates.Count}");
                    }


                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log($" --> Error On GetUserData {e.Message}", logType: LogType.Warning);
                    return false;
                }
            }
        }

        public override ResultViewModel ReadOfflineLog(object cancelationToken, bool saveFile = false)
        {
            //lock (_zkTecoSdk)
            //{
            try
            {
                var iLogCount = 0;
                lock (DeviceInfo)
                {
                    Logger.Log($"Retrieving offline logs of DeviceId: {DeviceInfo.Code}.");
                }

                //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);//disable the device
                var idwErrorCode = 0;

                bool result;

                lock (ZKTecoSdk)
                    result = ZKTecoSdk.ReadNewGLogData((int)DeviceInfo.Code);

                if (!result)
                {
                    lock (ZKTecoSdk)
                        ZKTecoSdk.GetLastError(ref idwErrorCode);
                    if (idwErrorCode != 0)
                    {
                        lock (ZKTecoSdk)
                            result = ZKTecoSdk.ReadGeneralLogData((int)DeviceInfo.Code);

                        if (!result)
                        {
                            lock (ZKTecoSdk)
                                ZKTecoSdk.GetLastError(ref idwErrorCode);
                            //Thread.Sleep(2000);
                            //Connect();
                            lock (DeviceInfo)
                            {
                                Logger.Log(
                                    $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}",
                                    logType: LogType.Warning);
                            }
                            lock (DeviceInfo)
                            {
                                return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
                            }
                        }
                    }
                }

                var lstLogs = new List<Log>();
                const int recordCnt = 0;

                string iUserId;
                int iVerifyMethod;
                int iInOutMode;
                int iYear;
                int iMonth;
                int iDay;
                int iHour;
                int iMinute;
                int iSecond;
                var iWorkCode = 0;

                lock (ZKTecoSdk)
                    result = ZKTecoSdk.SSR_GetGeneralLogData((int)DeviceInfo.Code, out iUserId,
                        out iVerifyMethod, out iInOutMode, out iYear, out iMonth, out iDay,
                        out iHour, out iMinute, out iSecond, ref iWorkCode);

                while (result)
                {
                    if (iLogCount > 100000)
                        break;

                    if (iLogCount % 1000 == 0)
                        Thread.Sleep(10);

                    if (iLogCount % 20000 == 0)
                        Thread.Sleep(2000);

                    iLogCount++; //increase the number of attendance records

                    lock (LockObject) //make the object exclusive 
                    {
                        try
                        {
                            var userId = Convert.ToInt32(iUserId);
                            var log = new Log
                            {
                                DeviceId = DeviceInfo.DeviceId,
                                DeviceCode = DeviceInfo.Code,
                                LogDateTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond),
                                //EventLog = Event.ATHORIZED,
                                EventLog = LogEvents.Authorized,
                                UserId = userId,
                                //MatchingType = iVerifyMethod,
                                MatchingType = ZKCodeMappings.GetMatchingTypeGenericLookup(iVerifyMethod),
                                TnaEvent = (ushort)iInOutMode
                            };

                            //_zkLogService.AddLog(log);
                            lstLogs.Add(log);
                            Logger.Log($@"<--
       +TerminalID: {DeviceInfo.Code}
       +UserID: {userId}
       +DateTime: {log.LogDateTime}
       +AuthType: {iVerifyMethod}
       +TnaEvent: {(ushort)iInOutMode}
       +Progress: {iLogCount}/{recordCnt}", logType: LogType.Verbose);
                        }
                        catch (Exception)
                        {
                            Logger.Log($"User id of log is not in a correct format. UserId : {iUserId}", logType: LogType.Warning);
                        }
                    }

                    lock (ZKTecoSdk)
                        result = ZKTecoSdk.SSR_GetGeneralLogData((int)DeviceInfo.Code, out iUserId,
                            out iVerifyMethod, out iInOutMode, out iYear, out iMonth, out iDay,
                            out iHour, out iMinute, out iSecond, ref iWorkCode);
                }

                Task.Run(() =>
                {
                    ZKLogService.AddLog(lstLogs);
                    if (!saveFile) return;

                    lock (DeviceInfo)
                    {
                        CommonLogService.SaveLogsInFile(lstLogs, "ZK", DeviceInfo.Code);
                    }
                }, TokenSource.Token);

                lock (DeviceInfo)
                {
                    Logger.Log($"{iLogCount} Offline log retrieved from DeviceId: {DeviceInfo.Code}.", logType: LogType.Information);
                }


                //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);//enable the device
                lock (DeviceInfo)
                {
                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 1, Message = iLogCount.ToString() };
                }
            }
            catch (Exception exception)
            {
                //Thread.Sleep(2000);
                //Connect();
                Logger.Log(exception, exception.Message);
                lock (DeviceInfo)
                {
                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
                }
            }
            //}
        }
    }
}
