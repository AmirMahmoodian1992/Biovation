using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Biovation.Brands.ZK.Devices
{
    // ReSharper disable once InconsistentNaming
    public class IFace : Device
    {
        private readonly AccessGroupService _accessGroupService;
        private readonly UserService _userService;
        private readonly UserCardService _userCardService;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FaceTemplateTypes _faceTemplateTypes;

        private readonly List<char> _persianLetters = new List<char>
        {
            'آ', 'ا', 'ب', 'پ', 'ت', 'ث', 'ج', 'چ', 'ح', 'خ', 'د', 'ذ', 'ر', 'ز', 'ژ', 'س', 'ش', 'ص', 'ض', 'ط', 'ظ',
            'ع', 'غ', 'ف', 'ق', 'ک', 'گ', 'ل', 'م', 'ن', 'و', 'ه', 'ی', 'ي', 'ء', 'إ', 'أ', 'ؤ', 'ئ', 'ة', 'ك'
        };

        internal IFace(DeviceBasicInfo info, TaskService taskService, UserService userService, DeviceService deviceService, LogService logService, AccessGroupService accessGroupService, FingerTemplateService fingerTemplateService, UserCardService userCardService, FaceTemplateService faceTemplateService, RestClient restClient, Dictionary<uint, Device> onlineDevices, BiovationConfigurationManager biovationConfigurationManager, LogEvents logEvents, ZkCodeMappings zkCodeMappings, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands, MatchingTypes matchingTypes, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, FaceTemplateTypes faceTemplateTypes, ILogger logger)
            : base(info, taskService, userService, deviceService, logService, accessGroupService, fingerTemplateService, userCardService, faceTemplateService, restClient, onlineDevices, biovationConfigurationManager, logEvents, zkCodeMappings, taskTypes, taskPriorities, taskStatuses, taskItemTypes, deviceBrands, matchingTypes, biometricTemplateManager, fingerTemplateTypes, faceTemplateTypes, logger)
        {
            _accessGroupService = accessGroupService;
            _userService = userService;
            _userCardService = userCardService;
            _biometricTemplateManager = biometricTemplateManager;
            _fingerTemplateTypes = fingerTemplateTypes;
            _faceTemplateTypes = faceTemplateTypes;
        }
        public override bool GetAndSaveUser(long userId)
        {
            lock (ZkTecoSdk)
            {
                try
                {
                    Logger.Log("<--EventGetUserData - iFace devices");

                    //var name = string.Empty;
                    //var password = string.Empty;
                    //var privilege = 0;
                    //var enabled = true;

                    //if (ZKTecoSdk.GetUserInfo((int)DeviceInfo.Code, (int)userId, ref name,
                    //    ref password, ref privilege, ref enabled))
                    if (ZkTecoSdk.SSR_GetUserInfo((int)DeviceInfo.Code, userId.ToString(), out var name,
                        out var password, out var privilege, out var enabled))
                    {
                        var user = new User
                        {
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
                        var existUser = _userService.GetUsers(userId).FirstOrDefault();
                        if (existUser != null)
                        {
                            user = new User
                            {
                                Id = existUser.Id,
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

                        var userInsertionResult = UserService.ModifyUser(user);
                        if (!userInsertionResult.Success)
                            return false;

                        Logger.Log("<--User is Modified");
                        user.Id = userInsertionResult.Id;

                        user.FingerTemplates = new List<FingerTemplate>();
                        user.FaceTemplates = new List<FaceTemplate>();

                        try
                        {
                            if (ZkTecoSdk.GetStrCardNumber(out var cardNumber) && cardNumber != "0")
                            {
                                var card = new UserCard
                                {
                                    CardNum = cardNumber,
                                    IsActive = true,
                                    UserId = user.Id
                                };

                                _userCardService.ModifyUserCard(card);
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
                            if (ZkTecoSdk.ReadAllTemplate((int)DeviceInfo.Code))
                            {
                                for (var i = 0; i <= 9; i++)
                                {
                                    if (!ZkTecoSdk.GetUserTmpExStr((int)DeviceInfo.Code, user.Code.ToString(), i,
                                        out _, out var tempData, out var tempLength))
                                    {
                                        Thread.Sleep(50);
                                        continue;
                                    }

                                    var fingerTemplate = new FingerTemplate
                                    {
                                        FingerIndex = _biometricTemplateManager.GetFingerIndex(i),
                                        FingerTemplateType = _fingerTemplateTypes.VX10,
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
                                            fp.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(i).Code &&
                                            fp.FingerTemplateType == _fingerTemplateTypes.VX10))
                                        {
                                            user.FingerTemplates.Add(fingerTemplate);
                                            Logger.Log(
                                                $"A finger print with index: {i} is retrieved for user: {user.Code}");
                                        }
                                        else
                                        {
                                            Logger.Log($"The User: {user.Code} has a finger print with index: {i}");
                                        }
                                    }
                                    else
                                    {
                                        user.FingerTemplates.Add(fingerTemplate);
                                        Logger.Log($"A finger print with index: {i} is retrieved for user: {user.Code}");
                                    }
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
                                if (!ZkTecoSdk.GetUserFaceStr((int)DeviceInfo.Code, userId.ToString(), 50,
                                    ref faceStr, ref faceLen))
                                {
                                    Thread.Sleep(50);
                                    continue;
                                }
                                var faceTemplate = new FaceTemplate
                                {
                                    Index = 50,
                                    FaceTemplateType = _faceTemplateTypes.ZKVX7,
                                    UserId = user.Id,
                                    Template = Encoding.ASCII.GetBytes(faceStr),
                                    CheckSum = Encoding.ASCII.GetBytes(faceStr).Sum(x => x),
                                    Size = faceLen,
                                };

                                retrievedFaceTemplates.Add(faceTemplate);

                                if (existUser != null)
                                {
                                    if (existUser.FaceTemplates.Any(fp => fp.Index == 50 && fp.CheckSum == faceTemplate.CheckSum)) break;
                                    faceTemplate.Id = existUser.FaceTemplates.FirstOrDefault()?.Id ?? 0;
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

        public override bool TransferUser(User user)
        {
            lock (ZkTecoSdk)
            {
                var errorCode = 0;
                // _zkTecoSdk.EnableDevice((int)_deviceInfo.Code, false);
                //var card = UserCardService.GetCardsByFilter(user.Id).FirstOrDefault(c => c.IsActive);
                if (user.IdentityCard != null && user.IdentityCard.IsActive)
                {
                    if (ZkTecoSdk.SetStrCardNumber(user.IdentityCard.Number))
                        Logger.Log($"Successfully set card for UserId {user.Code} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                    else
                    {
                        ZkTecoSdk.GetLastError(ref errorCode);
                        Logger.Log($"Cannot set card for UserId {user.Code} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                    }
                }

                if (!user.IsActive || user.StartDate != user.EndDate && user.StartDate != default &&
                    user.StartDate != DateTime.MinValue && user.StartDate != DateTime.MaxValue &&
                    user.EndDate != default && user.EndDate != DateTime.MinValue &&
                    (user.StartDate > DateTime.Now || user.EndDate < DateTime.Now))
                {
                    lock (ZkTecoSdk)
                    {
                        ZkTecoSdk.DeleteEnrollData((int)DeviceInfo.Code, (int)user.Code, (int)DeviceInfo.Code, 12);
                        ZkTecoSdk.RefreshData((int)DeviceInfo.Code);
                        return true;
                    }
                }

                var isoEncoding = Encoding.GetEncoding(28591);
                var windowsEncoding = Encoding.GetEncoding(1256);
                var userName = (string.IsNullOrWhiteSpace(user.UserName) ? user.FirstName + " " + user.SurName : user.UserName).Trim();
                var convertedUserName = string.IsNullOrEmpty(userName) ? null : isoEncoding.GetString(windowsEncoding.GetBytes(userName));

                if (userName.Count(c => _persianLetters.Contains(c)) > 3)
                {
                    var replacements = new Dictionary<string, string> { { "\u0098", "˜" }, { "\u008e", "Ž" } };
                    convertedUserName = replacements.Aggregate(convertedUserName, (current, replacement) => current.Replace(replacement.Key, replacement.Value));
                }

                if (ZkTecoSdk.SSR_SetUserInfo((int)DeviceInfo.Code, user.Code.ToString(), convertedUserName?.Trim() ?? string.Empty, user.Password,
                    user.IsAdmin ? 3 : 0, true))
                {
                    Logger.Log($"UserId {user.Code} successfully added to DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                    try
                    {
                        if (user.FingerTemplates.Any())
                        {
                            var zkFinger = user.FingerTemplates
                                .Where(x => x.FingerTemplateType.Code == FingerTemplateTypes.VX10Code)
                                .ToList();
                            foreach (var finger in zkFinger)
                            {
                                for (var i = 0; i < 9; i++)
                                {
                                    if (ZkTecoSdk.SetUserTmpExStr((int)DeviceInfo.Code, user.Code.ToString(), finger.Index,
                                        1,
                                        Encoding.ASCII.GetString(finger.Template)))
                                    {
                                        //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                                        Logger.Log(
                                            $"Successfully set template for UserId {user.Code} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                                        break;
                                    }

                                    ZkTecoSdk.GetLastError(ref errorCode);
                                    Thread.Sleep(50);
                                    Logger.Log(
                                        $"Cannot set template for UserId {user.Code} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                                }
                            }
                        }

                        //var faceZk = FaceTemplateService.FaceTemplates(userId: user.Id, index: 50);
                        if (user.FaceTemplates.Any(template => template.FaceTemplateType.Code == FaceTemplateTypes.ZKVX7Code))
                        {
                            var faceTemplate = user.FaceTemplates.First(template =>
                                template.FaceTemplateType.Code == FaceTemplateTypes.ZKVX7Code);
                            //foreach (var face in user.FaceTemplates.Where(template => template.FaceTemplateType.Code == FaceTemplateTypes.ZKVX7Code))
                            //{
                            for (var i = 0; i < 9; i++)
                            {
                                if (ZkTecoSdk.SetUserFaceStr((int)DeviceInfo.Code, user.Code.ToString(), 50,
                                    Encoding.ASCII.GetString(faceTemplate.Template), faceTemplate.Size))
                                {
                                    //_zkTecoSdk.RefreshData((int)_deviceInfo.Code);
                                    Logger.Log(
                                        $"Successfully set face template for UserId {user.Code} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                                    break;
                                }

                                ZkTecoSdk.GetLastError(ref errorCode);
                                Thread.Sleep(50);
                                Logger.Log(
                                    $"Cannot set face template for UserId {user.Code} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                            }
                            //}
                        }

                        var userAccessGroups = user.Id == default ? null : _accessGroupService.GetAccessGroups(user.Id);
                        var validAccessGroup =
                            userAccessGroups?.FirstOrDefault(ag =>
                                ag.DeviceGroup.Any(dg => dg.Devices.Any(d => d.DeviceId == DeviceInfo.DeviceId)));
                        if (ZkTecoSdk.SetUserGroup((int)DeviceInfo.Code, (int)user.Code,
                            validAccessGroup?.Id ?? 1))
                        {
                            ZkTecoSdk.RefreshData((int)DeviceInfo.Code);
                            Logger.Log(
                                $"Successfully set access group for UserId {user.Code} in DeviceId {DeviceInfo.Code}.", logType: LogType.Information);
                            //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);
                            return true;
                        }

                        //ZkTecoSdk.SetUserValidDate((int) DeviceInfo.Code, user.Code.ToString(), 0, 0,
                        //    user.StartDate.ToString(CultureInfo.InvariantCulture), user.EndDate.ToString(CultureInfo.InvariantCulture));
                        //ZkTecoSdk.SSR_EnableUser((int) DeviceInfo.Code, user.Code.ToString(), user.IsActive);
                        ZkTecoSdk.RefreshData((int)DeviceInfo.Code);
                        ZkTecoSdk.GetLastError(ref errorCode);
                        //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                        Logger.Log($"Cannot set access group for UserId {user.Code} in DeviceId {DeviceInfo.Code}.", logType: LogType.Warning);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception, exception.Message);
                    }
                    // _zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                    return true;
                }

                errorCode = 0;
                ZkTecoSdk.GetLastError(ref errorCode);
                //_zkTecoSdk.EnableDevice((int)_deviceInfo.Code, true);

                Logger.Log($"Cannot add user {user.Code} to device {DeviceInfo.Code}. ErrorCode={errorCode}",
                    logType: LogType.Warning);
                return false;
            }
        }

        /*public override ResultViewModel ReadOfflineLog(object cancelationToken, bool saveFile = false)
        {
            //lock (_zkTecoSdk)
            //{
            try
            {
                var iLogCount = 0;
                Logger.Log($"Retrieving offline logs of DeviceId: {DeviceInfo.Code}.");

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
                            Logger.Log(
                                $"Could not retrieve offline logs from DeviceId:{DeviceInfo.Code} General Log Data Count:0 ErrorCode={idwErrorCode}",
                                logType: LogType.Warning);
                            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
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
                                EventId = Event.ATHORIZED,
                                UserId = userId,
                                MatchingType = iVerifyMethod,
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
        }*/
    }
}
