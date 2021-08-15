using Biovation.Brands.Suprema.Manager;
using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Biovation.Brands.Suprema.Devices.Suprema_Version_1
{
    /// <summary>
    /// برای ساعت های  BioLiteNet , BIOENTRY_PLUS ,BIOENTRY_W  , XPASS, XPASS_SLIM, XPASS_SLIM2
    /// </summary>
    /// <seealso cref="Device" />
    internal class OtherDevices : Device
    {
        private readonly object _deviceAccessObject = new object();

        private readonly DeviceService _deviceService;
        private readonly LogService _logService;
        private readonly AccessGroupService _accessGroupService;
        private readonly UserCardService _userCardService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;

        private readonly BiometricTemplateManager _biometricTemplateManager;

        private readonly TimeZoneService _timeZoneService;
        private readonly SupremaCodeMappings _supremaCodeMappings;

        private readonly DeviceBrands _deviceBrands;
        private readonly MatchingTypes _matchingTypes;
        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;

        public OtherDevices(SupremaDeviceModel info, DeviceService deviceService, LogService logService, AccessGroupService accessGroupService, UserCardService userCardService, FingerTemplateService fingerTemplateService, SupremaCodeMappings supremaCodeMappings, FingerTemplateTypes fingerTemplateTypes, DeviceBrands deviceBrands, TimeZoneService timeZoneService, BiometricTemplateManager biometricTemplateManager, MatchingTypes matchingTypes, LogEvents logEvents, LogSubEvents logSubEvents)
            : base(info, accessGroupService, userCardService)
        {
            _deviceService = deviceService;
            _logService = logService;
            _accessGroupService = accessGroupService;
            _userCardService = userCardService;
            _fingerTemplateService = fingerTemplateService;
            _supremaCodeMappings = supremaCodeMappings;
            _fingerTemplateTypes = fingerTemplateTypes;
            _deviceBrands = deviceBrands;
            _timeZoneService = timeZoneService;
            _biometricTemplateManager = biometricTemplateManager;
            _matchingTypes = matchingTypes;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            DeviceAccessSemaphore = new Semaphore(1, 1);
        }

        private byte[] MTemplateData { get; set; }

        /// <summary>
        /// <En>Transfer user with all data (finger template , card, Id , Access Group ,....) on validated FaceStation devices.</En>
        /// <Fa>کاربر را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="userData">کاربر</param>
        /// <returns></returns>
        public override bool TransferUser(User userData)
        {
            MTemplateData = new byte[BSSDK.TEMPLATE_SIZE * 2 * 2];
            var userHdr = new BSSDK.BEUserHdr
            {
                fingerChecksum = new ushort[2],
                isDuress = new byte[2]
            };

            if (userData == null) return false;


            // pwd
            //if (!(userData.PasswordBytes is null))
            //{
            //    var pwd = userData.Password;
            //    userHdr.password = new byte[17];
            //    var tmppw = Encoding.ASCII.GetBytes(pwd);
            //    Buffer.BlockCopy(tmppw, 0, userHdr.password, 0, tmppw.Length);

            //}
            if (!(userData.Password is null))
            {
                if (userData.PasswordBytes is null)
                {
                    // pwd
                    var pwd = userData.Password;
                    //                var encoding = new ASCIIEncoding();
                    var pwdOut = new byte[32];
                    //                byte[] pwdBytes = encoding.GetBytes(pwd);
                    var pwdBytes = Encoding.ASCII.GetBytes(pwd);
                    BSSDK.BS_EncryptSHA256(pwdBytes, pwdBytes.Length, pwdOut);
                    Buffer.BlockCopy(pwdOut, 0, userHdr.password, 0, pwdOut.Length);
                }
                else
                {
                    Buffer.BlockCopy(userData.PasswordBytes, 0, userHdr.password, 0, userData.PasswordBytes.Length);
                }
            }


            //userHdr.userID = Convert.ToUInt32(userData.SUserId);
            userHdr.userID = Convert.ToUInt32(userData.Code);
            userHdr.adminLevel = Convert.ToUInt16(userData.AdminLevel == 240 ? 1 : 0);
            userHdr.startTime = userData.GetStartDateInTicks();
            userHdr.expiryTime = userData.GetEndDateInTicks();
            userHdr.opMode = userData.AuthMode;

            if (userData.AuthMode == 0)
            {
                userHdr.opMode = 0;
            }

            else
                userHdr.opMode = BSSDK.BS_AUTH_FINGER_ONLY - 1;


            //var userTemplate = (_fingerTemplateService.FingerTemplates(userId: (int)userData.Id, fingerTemplateType: FingerTemplateTypes.SU384Code).Where(x => x.FingerTemplateType.Code == _fingerTemplateTypes.SU384.Code)).ToList();
            var userTemplate = userData.FingerTemplates;
            var rowCount = userTemplate.Count;
            var numberOfFingers = 0;
            int fingerChecksum1 = new ushort();
            int fingerChecksum2 = new ushort();

            switch (rowCount)
            {
                case 0:
                    break;
                case 2:
                    numberOfFingers = 1;
                    userHdr.securityLevel = (ushort)GetSecurityLevelForDevice(userTemplate[0].SecurityLevel);
                    userTemplate[0].Template.CopyTo(MTemplateData, 0);
                    userTemplate[1].Template.CopyTo(MTemplateData, 384);
                    fingerChecksum1 = userTemplate[0].CheckSum;
                    break;
                case 4:
                    numberOfFingers = 2;
                    userHdr.securityLevel = (ushort)GetSecurityLevelForDevice(userTemplate[0].SecurityLevel);
                    userTemplate[0].Template.CopyTo(MTemplateData, 0);
                    userTemplate[1].Template.CopyTo(MTemplateData, 384);
                    userTemplate[2].Template.CopyTo(MTemplateData, 768);
                    userTemplate[3].Template.CopyTo(MTemplateData, 1152);
                    fingerChecksum1 = userTemplate[0].CheckSum;
                    fingerChecksum2 = userTemplate[2].CheckSum;
                    break;

                default:
                    if (rowCount > 4)
                    {
                        numberOfFingers = 2;
                        userHdr.securityLevel = (ushort)GetSecurityLevelForDevice(userTemplate[0].SecurityLevel);
                        userTemplate[0].Template.CopyTo(MTemplateData, 0);
                        userTemplate[1].Template.CopyTo(MTemplateData, 384);
                        userTemplate[2].Template.CopyTo(MTemplateData, 768);
                        userTemplate[3].Template.CopyTo(MTemplateData, 1152);
                        fingerChecksum1 = userTemplate[0].CheckSum;
                        fingerChecksum2 = userTemplate[2].CheckSum;
                    }
                    break;

            }

            userHdr.numOfFinger = (ushort)numberOfFingers;
            userHdr.fingerChecksum[0] = (ushort)fingerChecksum1;
            userHdr.fingerChecksum[1] = (ushort)fingerChecksum2;
            userHdr.isDuress[0] = 0;
            userHdr.isDuress[1] = 0;
            userHdr.cardFlag = 0;


            var hasCard = false;

            try
            {
                var userCards = _userCardService.GetCardsByFilter(userData.Id, true).Result;
                if (userCards != null)
                {
                    var userCard = userCards.FirstOrDefault();
                    if (userCard != null)
                    {
                        userHdr.cardID = Convert.ToUInt32(userCard.CardNum);
                        userHdr.cardCustomID = 0;
                        userHdr.cardFlag = 1;
                        CardValidation(userCard.CardNum);
                        hasCard = true;
                    }
                }

                else
                {
                    userHdr.cardID = 0;
                    Logger.Log($"The User : {userData.Id} Do not have card or query do not return any thing");
                }

            }
            catch (Exception)
            {
                userHdr.cardID = 0;
                Logger.Log($"The User : {userData.Id} Do not have card or query do not return any thing");
            }

            userHdr.cardVersion = BSSDK.BE_CARD_VERSION_1;
            userHdr.disabled = 0;
            userHdr.dualMode = 0;

            try
            {
                var accessGroupMaskString = GetAccessGroup(userData.Code);
                if (!string.IsNullOrWhiteSpace(accessGroupMaskString))
                {
                    userHdr.accessGroupMask = Convert.ToUInt32(accessGroupMaskString, 16);
                }

                userHdr.accessGroupMask = 0xffffffff;
            }
            catch (Exception)
            {
                userHdr.accessGroupMask = 0xffffffff;
            }

            if (hasCard || numberOfFingers > 0)
            {

                var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BEUserHdr)));
                Marshal.StructureToPtr(userHdr, userInfo, true);


                //var res = BSSDK.BS_EnrollUserBEPlus(DeviceInfo.Handle, userInfo, MTemplateData);
                var res = 0;

                try
                {
                    lock (_deviceAccessObject)
                    {
                        DeviceAccessSemaphore.WaitOne();
                        res = BSSDK.BS_EnrollUserBEPlus(DeviceInfo.Handle, userInfo, MTemplateData);
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
                        lock (_deviceAccessObject)
                        {
                            DeviceAccessSemaphore.Release();
                        }
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

                Marshal.FreeHGlobal(userInfo);

                if (res != BSSDK.BS_SUCCESS)
                {
                    Logger.Log($"Cannot enroll User : {userData.Id} on BioLite or other devices.");

                    return false;
                }
            }

            else
            {
                Logger.Log(
                    $"Not transferred to BioLite or other devices, Due no finger template and no card were available for User : {userData.Id}.");
                //var res = BSSDK.BS_DeleteUser(DeviceInfo.Handle, Convert.ToUInt32(userData.SUserId));
                //var res = 0;
                //var res = DeleteUser(Convert.ToUInt32(userData.SUserId));
                var res = DeleteUser(Convert.ToUInt32(userData.Code));
                //try
                //{
                //    lock (_deviceAccessObject)
                //    {
                //        DeviceAccessSemaphore.WaitOne();
                //        res = BSSDK.BS_DeleteUser(DeviceInfo.Handle, Convert.ToUInt32(userData.SUserId));
                //        DeviceAccessSemaphore.Release();
                //    }
                //}
                //catch (Exception)
                //{
                //    //ignore
                //}

                //return res == BSSDK.BS_SUCCESS || res == BSSDK.BS_ERR_NO_USER || res == BSSDK.BS_ERR_NOT_FOUND;
                return res;
            }

            return true;

            //var result = BSSDK.BS_DeleteUser(DeviceInfo.Handle, Convert.ToUInt32(userData.SUserId));
            //return result == BSSDK.BS_SUCCESS || result == BSSDK.BS_ERR_NO_USER || result == BSSDK.BS_ERR_NOT_FOUND;
        }

        /// <summary>
        /// <En>Transfer Accessgroup to device.</En>
        /// <Fa>دسترسی را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="nAccessIdn">شماره دسترسی</param>
        /// <returns></returns>
        public override bool TransferAccessGroup(int nAccessIdn)
        {
            //BSSDK.BSAccessGroupEx[] accessGroupEx = new BSSDK.BSAccessGroupEx[BSSDK.DF_MAX_ACCESSGROUP];
            //for (int i = 0; i < BSSDK.DF_MAX_ACCESSGROUP; i++)
            //{
            //    accessGroupEx[i].name = new byte[32];
            //    accessGroupEx[i].readerID = new uint[32];
            //    accessGroupEx[i].scheduleID = new int[32];
            //    accessGroupEx[i].reserved = new int[2];
            //}


            var accessGroup = _accessGroupService.GetAccessGroup(nAccessIdn);

            var accessGroupEx = new BSSDK.BSAccessGroupEx
            {
                name = new byte[(int)BSSDK.BS2UserHdr.ENUM.DS_MAX_NAME_LEN],
                readerID = new uint[32],
                scheduleID = new int[32],
                reserved = new int[2]
            };


            if (accessGroup != null)
            {
                accessGroupEx.groupID = accessGroup.Id;

                var aNameBytes = Encoding.Unicode.GetBytes(accessGroup.Name);
                Buffer.BlockCopy(aNameBytes, 0, accessGroupEx.name, 0, aNameBytes.Length);

                accessGroupEx.numOfReader = 1;
                accessGroupEx.readerID[0] = (uint)DeviceInfo.DeviceId;
                accessGroupEx.reserved[0] = 0;

                if (accessGroup.TimeZone.Id != 0)
                {
                    accessGroupEx.scheduleID[0] = accessGroup.TimeZone.Id;
                }

                else
                {
                    accessGroupEx.scheduleID[0] = 254;
                }
            }

            else
            {
                Logger.Log($"Cannot enroll AccessGroup : {nAccessIdn} on Device {DeviceInfo.DeviceId}");
                return false;
            }

            var data = Marshal.AllocHGlobal(BSSDK.DF_MAX_ACCESSGROUP * Marshal.SizeOf(typeof(BSSDK.BSAccessGroupEx)));
            Marshal.StructureToPtr(accessGroupEx, data, true);

            var result = BSSDK.BS_AddAccessGroupEx(DeviceInfo.Handle, data);

            if (result != BSSDK.BS_SUCCESS)
            {
                Logger.Log($"Cannot enroll AccessGroup : {nAccessIdn} on Device {DeviceInfo.DeviceId}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <En>Transfer Timezone to device.</En>
        /// <Fa>تایم زون را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="nTimeZone">شماره تایم زون</param>
        /// <returns></returns>
        public override bool TransferTimeZone(int nTimeZone)
        {
            var timezoneEx = new BSSDK.BSTimeScheduleEx[1];

            for (var i = 0; i < BSSDK.DF_MAX_TIMESCHEDULE; i++)
            {
                timezoneEx[0].name = new byte[32];
                timezoneEx[0].holiday = new int[2];
                timezoneEx[0].timeCode = new BSSDK.BSTimeCodeEx[9];
                for (var k = 0; k < 9; k++)
                {
                    timezoneEx[0].timeCode[k].codeElement = new BSSDK.BSTimeCodeElemEx[5];
                }

                timezoneEx[0].reserved = new int[2];
            }

            //int numOfSchedule = 1;  //support upto 128


            var timezone = _timeZoneService.TimeZones(nTimeZone);

            if (timezone != null)
            {
                //TODO
                //timezoneEx[0].scheduleID = timezone[0].ScheduleId;  //Timezone
                timezoneEx[0].scheduleID = timezone.Id;  //Timezone

                var aNameBytes = Encoding.Unicode.GetBytes(timezone.Name);
                Buffer.BlockCopy(aNameBytes, 0, timezoneEx[0].name, 0, aNameBytes.Length);

                //TODO
                //timezoneEx[0].holiday[0] = timezone[0].HolidayIdn;
                timezoneEx[0].holiday[0] = 0;

                // codeElement support upto 5
                for (var i = 0; i < timezone.Details.Count; i++)
                {
                    timezoneEx[0].timeCode[i].codeElement[0].startTime = (ushort)timezone.Details[i].FromTime.TotalSeconds;
                    timezoneEx[0].timeCode[i].codeElement[0].endTime = (ushort)timezone.Details[i].ToTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[1].startTime = (ushort)timezone.Details[i].FromTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[1].endTime = (ushort)timezone.Details[i].ToTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[2].startTime = (ushort)timezone.Details[i].FromTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[2].endTime = (ushort)timezone.Details[i].ToTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[3].startTime = (ushort)timezone.Details[i].FromTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[3].endTime = (ushort)timezone.Details[i].ToTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[4].startTime = (ushort)timezone.Details[i].FromTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[4].endTime = (ushort)timezone.Details[i].ToTime.TotalSeconds;
                }

                timezoneEx[0].reserved[0] = 0;
                timezoneEx[0].reserved[1] = 0;

                var data = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BSTimeScheduleEx)));
                Marshal.StructureToPtr(timezoneEx[0], data, true);

                var result = BSSDK.BS_AddTimeScheduleEx(DeviceInfo.Handle, data);

                if (result != BSSDK.BS_SUCCESS)
                {
                    Logger.Log($"Cannot enroll Timezone : {nTimeZone} on device : {DeviceInfo.DeviceId.ToString()} .");
                    return false;
                }

                return true;
            }

            else
            {
                Logger.Log($"Cannot enroll Timezone : {nTimeZone} on device : {DeviceInfo.DeviceId.ToString()} Because no time code is defined in database for this timezone.");
                return false;
            }
        }


        /// <summary>
        /// <En>Check if the user is on the device or not.</En>
        /// <Fa>وجود یک کاربر بر روی یک ساعت را بررسی می کند.</Fa>
        /// </summary>
        /// <param name="id">شماره کاربر</param>
        /// <returns></returns>
        public override bool ExistOnDevice(uint id)
        {
            var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BEUserHdr)));

            var result = BSSDK.BS_GetUserBEPlus(DeviceInfo.Handle, id, userInfo, MTemplateData);
            if (result != BSSDK.BS_SUCCESS)
            {
                Marshal.FreeHGlobal(userInfo);
                Logger.Log($"User : {id} do not exist on BioLite or other devices for delete");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <En>Read all log data from device, since last disconnect, For Biolite, BIOENTRY_PLUS , BIOENTRY_W , XPASS , XPASS_SLIM , XPASS_SLIM2 and BioStation.</En>
        /// <Fa>داده های اتفاقات در طول زمان قطعی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
        public override ResultViewModel ReadOfflineLog(object cancellationToken, bool fileSave = false)
        {
            var objectLock = new object();
            lock (objectLock)
            {
                var mNumOfLog = 0;
                //BSSDK.BS_GetLogCount(DeviceInfo.Handle, ref mNumOfLog);

                try
                {
                    if (Token.IsCancellationRequested)
                    {
                        Logger.Log("Thread canceled.");
                        return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = "عملیات تخلیه متوقف شد.", Validate = 0 };
                    }

                    DeviceAccessSemaphore.WaitOne();
                    BSSDK.BS_GetLogCount(DeviceInfo.Handle, ref mNumOfLog);
                }
                catch (Exception exception)
                {
                    Logger.Log($"Cannot get count of logs of device {DeviceInfo.DeviceId}");
                    Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                }
                finally
                {
                    try
                    {
                        DeviceAccessSemaphore.Release();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

                if (mNumOfLog == 0)
                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = "عملیات تخلیه متوقف شد.", Validate = 0 };

                Logger.Log($"Getting {mNumOfLog} logs of device {DeviceInfo.Code} started.");

                var logRecord = Marshal.AllocHGlobal(mNumOfLog * Marshal.SizeOf(typeof(BSSDK.BSLogRecord)));

                var logTotalCount = 0;
                var logCount = 0;

                var nMaxLogPerTrial = 8192;
                // var logService = new LogServices();

                do
                {
                    var result = 0;

                    var buf = new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSSDK.BSLogRecord)));


                    if (logTotalCount == 0)
                    {
                        var lastConnectedTime = _deviceService.GetLastConnectedTime(DeviceInfo.DeviceId)?.Data ?? new DateTime(1970, 1, 1);

                        if (lastConnectedTime.DayOfYear < DateTime.Now.DayOfYear)
                        {
                            //result = BSSDK.BS_ReadLog(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                            try
                            {
                                DeviceAccessSemaphore.WaitOne();
                                result = BSSDK.BS_ReadLog(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                            }
                            catch (Exception exception)
                            {
                                Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                                Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                            }
                            finally
                            {
                                try
                                {
                                    DeviceAccessSemaphore.Release();
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }
                            }
                        }

                        else
                        {

                            var logData = _logService.GetLastLogsOfDevice((uint)DeviceInfo.DeviceId).Result;
                            if (logData.Count == 0)
                            {
                                //result = BSSDK.BS_ReadLog(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                                try
                                {
                                    DeviceAccessSemaphore.WaitOne();
                                    result = BSSDK.BS_ReadLog(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                                    Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                                }
                                finally
                                {
                                    try
                                    {
                                        DeviceAccessSemaphore.Release();
                                    }
                                    catch (Exception)
                                    {
                                        //ignore
                                    }
                                }
                            }
                            else
                            {
                                //int startDischargingTime = 0;
                                //int deviceTime = 0;
                                //BSSDK.BS_GetTime(DeviceInfo.Handle, ref deviceTime);

                                //if (deviceTime - 24 * 60 * 60 < logData[0].nDateTime)
                                //{
                                //    startDischargingTime = deviceTime - 24 * 60 * 60;
                                //}

                                //result = BSSDK.BS_ReadLog(DeviceInfo.Handle, startDischargingTime, 0, ref logCount, buf);
                                try
                                {
                                    DeviceAccessSemaphore.WaitOne();
                                    result = BSSDK.BS_ReadLog(DeviceInfo.Handle, (int)logData[logData.Count - 1].DateTimeTicks, 0, ref logCount, buf);
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                                    Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                                }
                                finally
                                {
                                    try
                                    {
                                        DeviceAccessSemaphore.Release();
                                    }
                                    catch (Exception)
                                    {
                                        //ignore
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //result = BSSDK.BS_ReadNextLog(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                        try
                        {
                            DeviceAccessSemaphore.WaitOne();
                            result = BSSDK.BS_ReadNextLog(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                        }
                        catch (Exception exception)
                        {
                            Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                            Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                        }
                        finally
                        {
                            try
                            {
                                DeviceAccessSemaphore.Release();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }
                        }
                    }


                    if (result != BSSDK.BS_SUCCESS)
                    {
                        Marshal.FreeHGlobal(logRecord);
                        Logger.Log($"Cannot read log records Or the Device does not have new log. Result code = {result}");
                        return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = "تخلیه با مشکل مواجه شد یا دستگاه تردد تخلیه نشده ای ندارد.", Validate = 0 };
                    }

                    logTotalCount += logCount;

                } while (logCount == nMaxLogPerTrial);

                var allLogList = new List<Log>();

                for (var i = 0; i < logTotalCount; i++)
                {
                    var record = (BSSDK.BSLogRecord)Marshal.PtrToStructure(new IntPtr(logRecord.ToInt32() + i * Marshal.SizeOf(typeof(BSSDK.BSLogRecord))), typeof(BSSDK.BSLogRecord));
                    var receivedLog = new SupremaLog();

                    receivedLog.DateTimeTicks = (uint)record.eventTime;
                    receivedLog.DeviceId = DeviceInfo.DeviceId;
                    receivedLog.DeviceCode = DeviceInfo.Code;
                    receivedLog.InOutMode = DeviceInfo.DeviceTypeId;
                    receivedLog.EventLog = _supremaCodeMappings.GetLogEventGenericLookup(record.eventType) ?? new Lookup
                    {
                        Code = Convert.ToInt32(record.eventType).ToString()
                    };
                    receivedLog.TnaEvent = record.tnaEvent;
                    receivedLog.SubEvent = _supremaCodeMappings.GetLogSubEventGenericLookup(record.subEvent) ?? new Lookup
                    {
                        Code = Convert.ToInt32(record.subEvent).ToString()
                    };
                    receivedLog.UserId = (int)record.userID;
                    if (record.eventType == 40 || receivedLog.EventLog.Code == LogEvents.ConnectCode || receivedLog.EventLog.Code == LogEvents.DisconnectCode || receivedLog.EventLog.Code == LogEvents.DeviceEnabledCode)
                        record.userID = 0;

                    receivedLog.UserId = receivedLog.EventLog.Code == LogEvents.UnAuthorizedCode ? -1 : receivedLog.UserId;

                    receivedLog.MatchingType = _supremaCodeMappings.GetMatchingTypeGenericLookup(record.subEvent);


                    receivedLog.MatchingType = _supremaCodeMappings.GetMatchingTypeGenericLookup(record.subEvent);

                    allLogList.Add(receivedLog);
                }

                _logService.AddLog(allLogList);
                Logger.Log($"{logTotalCount} offline logs retrieved from device {DeviceInfo.Code}");

                Marshal.FreeHGlobal(logRecord);
                return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = $"تخلیه {logTotalCount} تردد با موفقیت انجام شد.", Validate = 1 };
            }
        }

        /// <summary>
        /// <En>Read all log data from device, since last disconnect, For FaceStation , D-Station, X-Station.</En>
        /// <Fa>داده های اتفاقات در طول مدت زمان مشخصی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
        public override ResultViewModel<List<Log>> ReadLogOfPeriod(int startTime, int endTime)
        {
            var objectLock = new object();
            lock (objectLock)
            {
                var mNumOfLog = 0;

                //BSSDK.BS_GetLogCount(DeviceInfo.Handle, ref mNumOfLog);

                try
                {
                    if (Token.IsCancellationRequested)
                    {
                        Logger.Log("Thread canceled.");
                        return new ResultViewModel<List<Log>> { Success = false, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = "Thread canceled" };
                    }

                    DeviceAccessSemaphore.WaitOne();
                    BSSDK.BS_GetLogCount(DeviceInfo.Handle, ref mNumOfLog);
                }
                catch (Exception exception)
                {
                    Logger.Log($"Cannot get count of logs of device {DeviceInfo.DeviceId}");
                    Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                }
                finally
                {
                    try
                    {
                        DeviceAccessSemaphore.Release();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

                if (mNumOfLog == 0)
                    return new ResultViewModel<List<Log>> { Success = false, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Can't get logs of device {DeviceInfo.DeviceId}" };

                Logger.Log($"Getting {mNumOfLog} logs of device {DeviceInfo.Code} started.");

                var logRecord = Marshal.AllocHGlobal(mNumOfLog * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx)));

                var logTotalCount = 0;
                var logCount = 0;

                var nMaxLogPerTrial = 8192;

                do
                {
                    var result = 0;

                    var buf = new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx)));
                    if (logTotalCount == 0)
                    {
                        try
                        {
                            DeviceAccessSemaphore.WaitOne();
                            result = BSSDK.BS_ReadLog(DeviceInfo.Handle, startTime, endTime, ref logCount, buf);
                        }
                        catch (Exception exception)
                        {
                            Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                            Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                        }
                        finally
                        {
                            try
                            {
                                DeviceAccessSemaphore.Release();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }
                        }
                        /*var logData = logService.GetLastLog(DeviceInfo.DeviceId, ConnectionType);
                        if (logData.Count == 0)
                        {
                            result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                        }
                        else
                        {
                            int startDischargingTime = 0;
                            int deviceTime = 0;
                            BSSDK.BS_GetTime(DeviceInfo.Handle, ref deviceTime);

                            if (deviceTime - 24 * 60 * 60 < logData[0].nDateTime)
                            {
                                startDischargingTime = deviceTime - 24 * 60 * 60;
                            }

                            result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, startDischargingTime, 0, ref logCount, buf);
                        }*/
                    }
                    else
                    {
                        //result = BSSDK.BS_ReadNextLog(DeviceInfo.Handle, 0, endTime, ref logCount, buf);
                        try
                        {
                            DeviceAccessSemaphore.WaitOne();
                            result = BSSDK.BS_ReadNextLog(DeviceInfo.Handle, 0, endTime, ref logCount, buf);
                        }
                        catch (Exception exception)
                        {
                            Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                            Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                        }
                        finally
                        {
                            try
                            {
                                DeviceAccessSemaphore.Release();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }
                        }
                    }

                    if (result != BSSDK.BS_SUCCESS)
                    {
                        Marshal.FreeHGlobal(logRecord);
                        Logger.Log($"Cannot read log records Or the Device does not have new log. Result code = {result}");
                        return null;
                    }

                    logTotalCount += logCount;

                } while (logCount == nMaxLogPerTrial);

                var allLogList = new List<Log>();


                for (var i = 0; i < logTotalCount; i++)
                {
                    var record = (BSSDK.BSLogRecordEx)Marshal.PtrToStructure(new IntPtr(logRecord.ToInt32() + i * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx))), typeof(BSSDK.BSLogRecordEx));

                    var receivedLog = new SupremaLog
                    {
                        DateTimeTicks = (uint)record.eventTime,
                        DeviceId = DeviceInfo.DeviceId,
                        InOutMode = DeviceInfo.DeviceTypeId,
                        EventLog = _supremaCodeMappings.GetLogEventGenericLookup(record.eventType) ?? new Lookup
                        {
                            Code = Convert.ToInt32(record.eventType).ToString()
                        },
                        Reserved = record.reserved1,
                        TnaEvent = record.tnaEvent,
                        SubEvent = _supremaCodeMappings.GetLogSubEventGenericLookup(record.subEvent) ?? new Lookup
                        {
                            Code = Convert.ToInt32(record.subEvent).ToString()
                        },
                        MatchingType = _supremaCodeMappings.GetMatchingTypeGenericLookup(record.subEvent),
                        UserId = (int)record.userID
                    };

                    if (record.eventType == 40 || receivedLog.EventLog.Code == LogEvents.ConnectCode || receivedLog.EventLog.Code == LogEvents.DisconnectCode || receivedLog.EventLog.Code == LogEvents.DeviceEnabledCode)
                        record.userID = 0;

                    receivedLog.UserId = receivedLog.EventLog.Code == LogEvents.UnAuthorizedCode ? -1 : receivedLog.UserId;


                    allLogList.Add(receivedLog);
                    //logService.InsertLog(recivedLog, ConnectionType);
                }

                //logService.BulkInsertLog(allLogList, ConnectionType);
                _logService.AddLog(allLogList);
                Logger.Log($"{logTotalCount} offline logs retrieved from device {DeviceInfo.Code}");

                Marshal.FreeHGlobal(logRecord);
                return new ResultViewModel<List<Log>> { Success = true, Code = Convert.ToInt64(TaskStatuses.DoneCode), Data = allLogList, Message = $"{logTotalCount} offline logs retrieved from device {DeviceInfo.DeviceId}" };
            }
        }

        /// <summary>
        /// <En>Add new device to database.</En>
        /// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public override int AddDeviceToDataBase()
        {
            var bLSysInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BESysInfoDataBLN)));

            var size = Marshal.SizeOf(typeof(BESysInfoDataBLN));

            var result = BSSDK.BS_ReadConfig(DeviceInfo.Handle, BSSDK.BLN_CONFIG_SYS_INFO, ref size,
                bLSysInfo);
            if (result != BSSDK.BS_SUCCESS)
            {
                Logger.Log($"Cannot read the sys info of Device : {DeviceInfo.DeviceId}");
                Marshal.FreeHGlobal(bLSysInfo);
                return -1;
            }

            var bLSysInfoObj =
                (BESysInfoDataBLN)Marshal.PtrToStructure(bLSysInfo, typeof(BESysInfoDataBLN));

            var existingDevice = _deviceService.GetDevices(code: DeviceInfo.Code, brandId: _deviceBrands.Suprema.Code).FirstOrDefault();

            if (existingDevice is null)
            {
                DeviceInfo.Active = true;
                DeviceInfo.TimeSync = true;
            }
            else
            {
                DeviceInfo.Name = existingDevice.Name;
                DeviceInfo.Active = existingDevice.Active;
                DeviceInfo.DeviceId = existingDevice.DeviceId;
                DeviceInfo.TimeSync = existingDevice.TimeSync;
            }

            DeviceInfo.MacAddress = Encoding.UTF8.GetString(bLSysInfoObj.macAddr);
            DeviceInfo.FirmwareVersion = Encoding.UTF8.GetString(bLSysInfoObj.firmwareVer);
            DeviceInfo.HardwareVersion = Encoding.UTF8.GetString(bLSysInfoObj.boardVer);
            DeviceInfo.Brand = _deviceBrands.Suprema;
            //DeviceInfo.Model.Id = 3003;
            //DeviceInfo.ModelId = 3003;

            return Convert.ToInt32(_deviceService.ModifyDevice(DeviceInfo).Id);
        }

        /// <summary>
        /// <En>Recieves all users from a device.</En>
        /// <Fa>تمامی یوزر ها را از دستگاه دریافت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public override List<User> GetAllUsers()
        {
            var numOfUser = 0;
            var mNumOfTemplate = 0;

            var usersList = new List<User>();

            //var result = BSSDK.BS_GetUserDBInfo(DeviceInfo.Handle, ref numOfUser, ref mNumOfTemplate);
            var result = 0;

            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    result = BSSDK.BS_GetUserDBInfo(DeviceInfo.Handle, ref numOfUser, ref mNumOfTemplate);
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
                    lock (_deviceAccessObject)
                    {
                        DeviceAccessSemaphore.Release();

                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            if (result != BSSDK.BS_SUCCESS)
            {
                Logger.Log("Cannot get user DB info");
                return null;
            }

            var userInfo = Marshal.AllocHGlobal(numOfUser * Marshal.SizeOf(typeof(BSSDK.BEUserHdr)));

            //result = BSSDK.BS_GetAllUserInfoBEPlus(DeviceInfo.Handle, userInfo, ref numOfUser);

            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    result = BSSDK.BS_GetAllUserInfoBEPlus(DeviceInfo.Handle, userInfo, ref numOfUser);
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
                    lock (_deviceAccessObject)
                    {
                        DeviceAccessSemaphore.Release();
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            if (result != BSSDK.BS_SUCCESS /*&& result != BSSDK.BS_ERR_NOT_FOUND*/)
            {
                Logger.Log("Cannot get user header info");
                Marshal.FreeHGlobal(userInfo);
                return null;
            }

            var users = new BSSDK.BEUserHdr[numOfUser];

            for (var i = 0; i < numOfUser; i++)
            {
                users[i] =
                    (BSSDK.BEUserHdr)
                    Marshal.PtrToStructure(
                        new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(BSSDK.BEUserHdr))),
                        typeof(BSSDK.BEUserHdr));
            }

            Marshal.FreeHGlobal(userInfo);

            foreach (var user in users)
            {
                var tempUser = new User
                {
                    Code = Convert.ToInt32(user.userID),
                    UserName = Convert.ToString(user.userID),
                    IsActive = !Convert.ToBoolean(user.disabled),
                    AdminLevel = user.adminLevel,
                    PasswordBytes = user.password,
                    FingerTemplatesCount = user.numOfFinger,
                    IdentityCardsCount = user.cardID != 0 ? 1 : 0,
                };
                tempUser.SetStartDateFromTicks(user.startTime);
                tempUser.SetEndDateFromTicks(user.expiryTime);

                //usersList.Add(new SupremaUserModel
                //{
                //    SUserId = Convert.ToString(user.ID),
                //    StartDate = Convert.ToInt32(user.startDateTime),
                //    EndDate = Convert.ToInt32(user.expireDateTime)
                //});

                usersList.Add(tempUser);
            }

            return usersList;
        }
        public override User GetUser(uint id)
        {
            var result = 0;
            var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BEUserHdr)));
            var templateData = new byte[BSSDK.TEMPLATE_SIZE * 2 * 2];

            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    //result = BSSDK.BS_GetUserDBIbnfo(DeviceInfo.Handle, ref numOfUser, ref mNumOfTemplate);
                    result = BSSDK.BS_GetUserBEPlus(DeviceInfo.Handle, id, userInfo, templateData);
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
                    lock (_deviceAccessObject)
                    {
                        DeviceAccessSemaphore.Release();

                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            if (result != BSSDK.BS_SUCCESS)
            {
                Logger.Log("Cannot get user DB info");
                Marshal.FreeHGlobal(userInfo);
                return null;
            }


            var userHdr = (BSSDK.BEUserHdr)Marshal.PtrToStructure(userInfo, typeof(BSSDK.BEUserHdr));
            Marshal.FreeHGlobal(userInfo);

            var numOfFinger = userHdr.numOfFinger;

            //var userInfo = Marshal.AllocHGlobal(numOfUser * Marshal.SizeOf(typeof(BSSDK.BS2UserHdr)));

            //result = BSSDK.BS_GetAllUserInfoBEPlus(DeviceInfo.Handle, userInfo, ref numOfUser);


            var tempUser = new User
            {
                Code = Convert.ToInt32(userHdr.userID),
                IsActive = !Convert.ToBoolean(userHdr.disabled),
                AdminLevel = userHdr.adminLevel
            };

            if (!(userHdr.password is null))
            {
                tempUser.PasswordBytes = new byte[userHdr.password.Length * 2];
                Buffer.BlockCopy(userHdr.password, 0, tempUser.PasswordBytes, 0, tempUser.PasswordBytes.Length);
                tempUser.Password = Encoding.ASCII.GetString(tempUser.PasswordBytes);
            }

            tempUser.FingerTemplates = new List<FingerTemplate>();

            //card
            tempUser.IdentityCard = new IdentityCard
            {
                //Id = (int)userHdr.userID,
                Number = userHdr.cardID.ToString(),
                DataCheck = 1,
                IsActive = userHdr.cardID != 0
            };

            ////face
            //for (var i = 0; i < userHdr.numOfFace; i++)
            //{
            // var tempFace = new FaceTemplate
            // {
            //     UserId = userHdr.ID,
            //     Template =  = 
            // };   
            //}


            if (numOfFinger > 0)
            {
                tempUser.FingerTemplates = new List<FingerTemplate>();
                for (var i = 0; i < numOfFinger; i++)
                {
                    //user.FingerTemplates.Add(new FingerTemplate
                    //{
                    //    FingerIndex = BiometricTemplateManager.GetFingerIndex(TerminalUserData.FingerID[i]),
                    //    Index = _fingerTemplateService.GetFingerTemplateByUserId(existUser.Id)?.Count(ft => ft.FingerIndex.Code == BiometricTemplateManager.GetFingerIndex(TerminalUserData.FingerID[i]).Code) ?? 0 + 1,
                    //    TemplateIndex = 0,
                    //    Size = TerminalUserData.FPSampleDataLength[fingerIndex, 0],
                    //    Template = firstTemplateSample,
                    //    CheckSum = firstSampleCheckSum,
                    //    UserId = user.Id,
                    //    FingerTemplateType = FingerTemplateTypes.V400
                    //});

                    var firstTemplateBytes = templateData.Skip(384 * 2 * i).Take(384).ToArray();

                    var fingerTemplate = new FingerTemplate
                    {
                        FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                        FingerTemplateType = _fingerTemplateTypes.SU384,
                        UserId = tempUser.Id,
                        Template = firstTemplateBytes,
                        //CheckSum = (int)userHdr.fingerChecksum[i],
                        CheckSum = firstTemplateBytes.Sum(b => b),
                        Size = firstTemplateBytes.ToList().LastIndexOf(firstTemplateBytes.LastOrDefault(b => b != 0)),
                        Index = i,
                        Duress = userHdr.isDuress[0] == 1,
                        CreateAt = DateTime.Now,
                        TemplateIndex = 0
                    };

                    tempUser.FingerTemplates.Add(fingerTemplate);

                    var secondTemplateBytes = templateData.Skip(384 * (2 * i + 1)).Take(384).ToArray();

                    var secondFingerTemplateSample = new FingerTemplate
                    {
                        FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                        FingerTemplateType = _fingerTemplateTypes.SU384,
                        UserId = tempUser.Id,
                        Template = secondTemplateBytes,
                        //CheckSum = (int)userHdr.fingerChecksum[i],
                        CheckSum = secondTemplateBytes.Sum(b => b),
                        Size = secondTemplateBytes.ToList().LastIndexOf(secondTemplateBytes.LastOrDefault(b => b != 0)),
                        Index = i,
                        Duress = userHdr.isDuress[1] == 1,
                        CreateAt = DateTime.Now,
                        TemplateIndex = 1
                    };

                    tempUser.FingerTemplates.Add(secondFingerTemplateSample);
                }
            }

            tempUser.SetStartDateFromTicks(userHdr.startTime);
            tempUser.SetEndDateFromTicks(userHdr.expiryTime);

            return tempUser;
        }

        //public List<User> GetUsers(List<uint> userIds)
        //{
        //    //var result = BSSDK.BS_GetUserDBInfo(DeviceInfo.Handle, ref numOfUser, ref mNumOfTemplate);
        //    var result = 0;
        //    var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BEUserHdr)));
        //    var templateData = new byte[BSSDK.TEMPLATE_SIZE * 2 * 2];
        //    var usersList = new List<User>();

        //    foreach (var id in userIds)
        //    {

        //        try
        //        {
        //            lock (_deviceAccessObject)
        //            {
        //                DeviceAccessSemaphore.WaitOne();
        //                //result = BSSDK.BS_GetUserDBInfo(DeviceInfo.Handle, ref numOfUser, ref mNumOfTemplate);
        //                result = BSSDK.BS_GetUserBEPlus(DeviceInfo.Handle, id, userInfo, templateData);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            //ignore
        //        }
        //        finally
        //        {
        //            try
        //            {
        //                lock (_deviceAccessObject)
        //                {
        //                    DeviceAccessSemaphore.Release();

        //                }
        //            }
        //            catch (Exception)
        //            {
        //                //ignore
        //            }
        //        }

        //        if (result != BSSDK.BS_SUCCESS)
        //        {
        //            Logger.Log("Cannot get user DB info");
        //            return null;
        //        }

        //        //var userInfo = Marshal.AllocHGlobal(numOfUser * Marshal.SizeOf(typeof(BSSDK.BEUserHdr)));

        //        //result = BSSDK.BS_GetAllUserInfoBEPlus(DeviceInfo.Handle, userInfo, ref numOfUser);

        //        try
        //        {
        //            lock (_deviceAccessObject)
        //            {
        //                DeviceAccessSemaphore.WaitOne();
        //                result = BSSDK.BS_GetUserInfoBEPlus(DeviceInfo.Handle, id, userInfo);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            //ignore
        //        }
        //        finally
        //        {
        //            try
        //            {
        //                lock (_deviceAccessObject)
        //                {
        //                    DeviceAccessSemaphore.Release();
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                //ignore
        //            }
        //        }

        //        if (result != BSSDK.BS_SUCCESS /*&& result != BSSDK.BS_ERR_NOT_FOUND*/)
        //        {
        //            Logger.Log("Cannot get user header info");
        //            Marshal.FreeHGlobal(userInfo);
        //            return null;
        //        }

        //        var user = new BSSDK.BEUserHdr[1];

        //        user[0] = (BSSDK.BEUserHdr) Marshal.PtrToStructure(new IntPtr(userInfo.ToInt32()),
        //            typeof(BSSDK.BEUserHdr));

        //        Marshal.FreeHGlobal(userInfo);

        //        var tempUser = new User
        //        {
        //            Id = Convert.ToInt32(user[0].userID),
        //            UserName = Convert.ToString(user[0].userID),
        //            IsActive = !Convert.ToBoolean(user[0].disabled),
        //            AdminLevel = user[0].adminLevel,
        //            PasswordBytes = user[0].password
        //        };

        //        tempUser.SetStartDateFromTicks(Convert.ToInt32(user[0].startTime));
        //        tempUser.SetEndDateFromTicks(Convert.ToInt32(user[0].expiryTime));

        //        usersList.Add(tempUser);
        //    }

        //    return usersList;
        //}

        public override bool DeleteUser(uint userId)
        {
            var result = 0;
            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    result = BSSDK.BS_DeleteUser(DeviceInfo.Handle, userId);
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
                    lock (_deviceAccessObject)
                    {
                        DeviceAccessSemaphore.Release();

                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            if (result == BSSDK.BS_ERR_NOT_FOUND)
            {
                Logger.Log("No user with this userID is enrolled");
                return false;
            }
            else if (result != BSSDK.BS_SUCCESS)
            {
                Logger.Log("Cannot delete user");
                return false;
            }

            var deleteLog = new SupremaLog
            {
                LogDateTime = DateTime.Now,
                DeviceId = DeviceInfo.DeviceId,
                DeviceCode = DeviceInfo.Code,
                EventLog = _logEvents.RemoveUserFromDevice,
                TnaEvent = 0,
                SubEvent = _logSubEvents.Normal,
                UserId = (int)userId,
                MatchingType = _matchingTypes.Unknown
            };
            _logService.AddLog(deleteLog);

            return true;
        }

        public bool DeleteAllUser()
        {
            var result = 0;
            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    result = BSSDK.BS_DeleteAllUser(DeviceInfo.Handle);
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
                    lock (_deviceAccessObject)
                    {
                        DeviceAccessSemaphore.Release();

                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            if (result == BSSDK.BS_SUCCESS) return true;
            Logger.Log("Cannot delete All users");
            return false;

        }

        //public bool DeleteUsers(List<uint> userIds)
        //{
        //    var totalResult = true;
        //    var result = 0;
        //    foreach (var userId in userIds)
        //    {
        //        try
        //        {
        //            lock (_deviceAccessObject)
        //            {
        //                DeviceAccessSemaphore.WaitOne();
        //                result = BSSDK.BS_DeleteUser(DeviceInfo.Handle, userId);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            //ignore
        //        }
        //        finally
        //        {
        //            try
        //            {
        //                lock (_deviceAccessObject)
        //                {
        //                    DeviceAccessSemaphore.Release();

        //                }
        //            }
        //            catch (Exception)
        //            {
        //                //ignore
        //            }
        //        }

        //        if (result == BSSDK.BS_SUCCESS) continue;
        //        totalResult = false;
        //        Logger.Log("Cannot delete All users");

        //    }

        //    return totalResult;
        //}
    }
}
