using Biovation.Brands.Suprema.Manager;
using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Biovation.Brands.Suprema.Devices.Suprema_Version_1
{
    /// <summary>
    /// برای ساعت BioStation
    /// </summary>
    /// <seealso cref="Device" />
    internal class BioStation : Device
    {
        private readonly object _deviceAccessObject = new object();
        private readonly DeviceService _deviceService;
        private readonly LogService _logService;
        private readonly TimeZoneService _timeZoneService;
        private readonly AccessGroupService _accessGroupService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly UserCardService _userCardService;
        private readonly SupremaCodeMappings _supremaCodeMappings;
        private readonly MatchingTypes _matchingTypes;
        private readonly DeviceBrands _deviceBrands;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;

        public BioStation(SupremaDeviceModel info, LogService logService, DeviceService deviceService, TimeZoneService timeZoneService, AccessGroupService accessGroupService, FingerTemplateService fingerTemplateService, FingerTemplateTypes fingerTemplateTypes, UserCardService userCardService, SupremaCodeMappings supremaCodeMappings, MatchingTypes matchingTypes, DeviceBrands deviceBrands, BiometricTemplateManager biometricTemplateManager, LogEvents logEvents, LogSubEvents logSubEvents)
            : base(info, accessGroupService, userCardService)
        {
            _biometricTemplateManager = biometricTemplateManager;
            _fingerTemplateService = fingerTemplateService;
            _supremaCodeMappings = supremaCodeMappings;
            _fingerTemplateTypes = fingerTemplateTypes;
            _accessGroupService = accessGroupService;
            _timeZoneService = timeZoneService;
            _userCardService = userCardService;
            _matchingTypes = matchingTypes;
            _deviceService = deviceService;
            _logSubEvents = logSubEvents;
            _deviceBrands = deviceBrands;
            _logService = logService;
            _logEvents = logEvents;
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
            MTemplateData = new byte[BSSDK.TEMPLATE_SIZE * BSSDK.BS_MAX_TEMPLATE_PER_USER];

            //var userService = new UserServices();
            //var userData = userService.GetUser(nUserIdn, ConnectionType);

            if (userData == null) return false;


            var userHdr = new BSSDK.BSUserHdrEx
            {
                checksum = new ushort[5],
                name = new byte[33],
                department = new byte[33],
                password = new byte[17],

                authLimitCount = 0,
                timedAntiPassback = 0,
                disabled = 0
            };

            // name 

            var username = userData.UserName;
            var nameBytes = Encoding.UTF8.GetBytes(username); // UTF8
            Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);




            // pwd
            if (!(userData.Password is null))
            {
                var pwd = userData.Password;
                userHdr.password = new byte[17];
                var tmppw = Encoding.ASCII.GetBytes(pwd);
                Buffer.BlockCopy(tmppw, 0, userHdr.password, 0, tmppw.Length);
            }


            //userHdr.ID = Convert.ToUInt32(userData.SUserId);
            userHdr.ID = (uint)userData.Code;

            userHdr.adminLevel = (ushort)(userData.AdminLevel == 240 ? 1 : 0);
            userHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;


            userHdr.startDateTime = Convert.ToUInt32(userData.GetStartDateInTicks());
            userHdr.expireDateTime = Convert.ToUInt32(userData.GetEndDateInTicks());

            userHdr.version = BSSDK.BE_CARD_VERSION_1;
            try
            {
                //var userAccessGroup = GetAccessGroup(nUserIdn);
                var userAccessGroup = GetAccessGroup(userData.Code);
                userHdr.accessGroupMask = Convert.ToUInt32(userAccessGroup, 16);
            }
            catch (Exception)
            {
                userHdr.accessGroupMask = 0xffffffff;
            }

            if (userData.AuthMode == 0)
                userHdr.authMode = 0;
            else
                userHdr.authMode =
                    (ushort)(userData.AuthMode + BSSDK.BS_AUTH_FINGER_ONLY - 1);


            #region finger Print


            var userTemplate = (_fingerTemplateService.FingerTemplates(userId: (int)userData.Id).Where(x => x.FingerTemplateType.Code == _fingerTemplateTypes.SU384.Code)).ToList();
            var rowCount = userTemplate.Count;
            var nS = 0;
            var numberOfFingers = 0;
            int fingerChecksum1 = new ushort();
            int fingerChecksum2 = new ushort();
            int fingerChecksum3 = new ushort();
            int fingerChecksum4 = new ushort();
            int fingerChecksum5 = new ushort();
            userHdr.duressMask = Convert.ToUInt16(0);

            switch (rowCount)
            {
                case 0:
                    numberOfFingers = 0;
                    break;
                case 2:
                    numberOfFingers = 1;
                    nS = userTemplate[0].SecurityLevel;
                    userHdr.duressMask = Convert.ToUInt16(userTemplate[0].Duress);
                    userTemplate[0].Template.CopyTo(MTemplateData, 0);
                    userTemplate[1].Template.CopyTo(MTemplateData, 384);
                    fingerChecksum1 = userTemplate[0].CheckSum;

                    break;
                case 4:
                    numberOfFingers = 2;
                    nS = userTemplate[0].SecurityLevel;
                    userHdr.duressMask = Convert.ToUInt16(userTemplate[0].Duress);
                    userTemplate[0].Template.CopyTo(MTemplateData, 0);
                    userTemplate[1].Template.CopyTo(MTemplateData, 384);
                    userTemplate[2].Template.CopyTo(MTemplateData, 768);
                    userTemplate[3].Template.CopyTo(MTemplateData, 1152);
                    fingerChecksum1 = userTemplate[0].CheckSum;
                    fingerChecksum2 = userTemplate[2].CheckSum;
                    break;
                case 6:
                    numberOfFingers = 3;
                    nS = userTemplate[0].SecurityLevel;
                    userHdr.duressMask = Convert.ToUInt16(userTemplate[0].Duress);
                    userTemplate[0].Template.CopyTo(MTemplateData, 0);
                    userTemplate[1].Template.CopyTo(MTemplateData, 384);
                    userTemplate[2].Template.CopyTo(MTemplateData, 768);
                    userTemplate[3].Template.CopyTo(MTemplateData, 1152);
                    userTemplate[4].Template.CopyTo(MTemplateData, 1536);
                    userTemplate[5].Template.CopyTo(MTemplateData, 1920);
                    fingerChecksum1 = userTemplate[0].CheckSum;
                    fingerChecksum2 = userTemplate[2].CheckSum;
                    fingerChecksum3 = userTemplate[4].CheckSum;
                    break;
                case 8:
                    numberOfFingers = 4;
                    nS = userTemplate[0].SecurityLevel;
                    userHdr.duressMask = Convert.ToUInt16(userTemplate[0].Duress);
                    userTemplate[0].Template.CopyTo(MTemplateData, 0);
                    userTemplate[1].Template.CopyTo(MTemplateData, 384);
                    userTemplate[2].Template.CopyTo(MTemplateData, 768);
                    userTemplate[3].Template.CopyTo(MTemplateData, 1152);
                    userTemplate[4].Template.CopyTo(MTemplateData, 1536);
                    userTemplate[5].Template.CopyTo(MTemplateData, 1920);
                    userTemplate[6].Template.CopyTo(MTemplateData, 2304);
                    userTemplate[7].Template.CopyTo(MTemplateData, 2688);
                    fingerChecksum1 = userTemplate[0].CheckSum;
                    fingerChecksum2 = userTemplate[2].CheckSum;
                    fingerChecksum3 = userTemplate[4].CheckSum;
                    fingerChecksum4 = userTemplate[6].CheckSum;
                    break;
                case 10:
                    numberOfFingers = 5;
                    nS = userTemplate[0].SecurityLevel;
                    userHdr.duressMask = Convert.ToUInt16(userTemplate[0].Duress);
                    userTemplate[0].Template.CopyTo(MTemplateData, 0);
                    userTemplate[1].Template.CopyTo(MTemplateData, 384);
                    userTemplate[2].Template.CopyTo(MTemplateData, 768);
                    userTemplate[3].Template.CopyTo(MTemplateData, 1152);
                    userTemplate[4].Template.CopyTo(MTemplateData, 1536);
                    userTemplate[5].Template.CopyTo(MTemplateData, 1920);
                    userTemplate[6].Template.CopyTo(MTemplateData, 2304);
                    userTemplate[7].Template.CopyTo(MTemplateData, 2688);
                    userTemplate[8].Template.CopyTo(MTemplateData, 3072);
                    userTemplate[9].Template.CopyTo(MTemplateData, 3456);
                    fingerChecksum1 = userTemplate[0].CheckSum;
                    fingerChecksum2 = userTemplate[2].CheckSum;
                    fingerChecksum3 = userTemplate[4].CheckSum;
                    fingerChecksum4 = userTemplate[6].CheckSum;
                    fingerChecksum5 = userTemplate[8].CheckSum;
                    break;

                default:
                    if (rowCount > 10)
                    {
                        numberOfFingers = 5;
                        nS = userTemplate[0].SecurityLevel;
                        userHdr.duressMask = Convert.ToUInt16(userTemplate[0].Duress);
                        userTemplate[0].Template.CopyTo(MTemplateData, 0);
                        userTemplate[1].Template.CopyTo(MTemplateData, 384);
                        userTemplate[2].Template.CopyTo(MTemplateData, 768);
                        userTemplate[3].Template.CopyTo(MTemplateData, 1152);
                        userTemplate[4].Template.CopyTo(MTemplateData, 1536);
                        userTemplate[5].Template.CopyTo(MTemplateData, 1920);
                        userTemplate[6].Template.CopyTo(MTemplateData, 2304);
                        userTemplate[7].Template.CopyTo(MTemplateData, 2688);
                        userTemplate[8].Template.CopyTo(MTemplateData, 3072);
                        userTemplate[9].Template.CopyTo(MTemplateData, 3456);
                        fingerChecksum1 = userTemplate[0].CheckSum;
                        fingerChecksum2 = userTemplate[2].CheckSum;
                        fingerChecksum3 = userTemplate[4].CheckSum;
                        fingerChecksum4 = userTemplate[6].CheckSum;
                        fingerChecksum5 = userTemplate[8].CheckSum;
                    }
                    break;
            }
            userHdr.numOfFinger = (ushort)numberOfFingers;

            userHdr.checksum[0] = (ushort)fingerChecksum1;
            userHdr.checksum[1] = (ushort)fingerChecksum2;
            userHdr.checksum[2] = (ushort)fingerChecksum3;
            userHdr.checksum[3] = (ushort)fingerChecksum4;
            userHdr.checksum[4] = (ushort)fingerChecksum5;

            #endregion


            userHdr.securityLevel = (ushort)GetSecurityLevelForDevice(nS);


            #region card

            var hasCard = false;

            try
            {

                var userCards = _userCardService.GetCardsByFilter(userData.Id, true).Result;
                //var rowc = dtCard.Count;
                if (userCards != null)
                {
                    var userCard = userCards.FirstOrDefault();
                    if (userCard != null)
                    {
                        userHdr.cardID = Convert.ToUInt32(userCard.CardNum);
                        userHdr.customID = 0;
                        //userHdr.Card = (byte)userCardData[rowc - 1].nBypass;
                        userHdr.bypassCard = 1;
                        CardValidation(userCard.CardNum);
                        hasCard = true;
                    }
                }

                else
                {
                    userHdr.cardID = 0;
                    userHdr.customID = 0;
                    Logger.Log($"The User : {userData.Id} Do not have card or query do not return any thing");
                }

            }
            catch (Exception e)
            {
                userHdr.cardID = 0;
                Logger.Log(e.ToString());
            }

            #endregion

            if (hasCard || numberOfFingers > 0)
            {
                var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BSUserHdrEx)));
                Marshal.StructureToPtr(userHdr, userInfo, true);

                //var res = BSSDK.BS_EnrollUserEx(DeviceInfo.Handle, userInfo, MTemplateData);
                var res = 0;

                try
                {
                    lock (_deviceAccessObject)
                    {
                        DeviceAccessSemaphore.WaitOne();
                        res = BSSDK.BS_EnrollUserEx(DeviceInfo.Handle, userInfo, MTemplateData);
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
                    Logger.Log($"Cannot enroll User : {userData.Id} on Biostation Device");

                    return false;
                }
            }

            else
            {
                Logger.Log(
                    $"Not transferred to BioStation Due no finger template and no card were available for User : {userData.Id}.");
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
                //    }
                //}
                //catch (Exception)
                //{
                //    //ignore
                //}
                //finally
                //{
                //    try
                //    {
                //        lock (_deviceAccessObject)
                //        {
                //            DeviceAccessSemaphore.Release(); 
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        //ignore
                //    }
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
        /// <Fa>گروه های دسترسی را به دستگاه انتقال می دهد.</Fa>
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
                name = new byte[32],
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
                Logger.Log($"Cannot enroll AccessGroup : {nAccessIdn} on Biostation Device {DeviceInfo.DeviceId}");
                return false;
            }

            var data = Marshal.AllocHGlobal(BSSDK.DF_MAX_ACCESSGROUP * Marshal.SizeOf(typeof(BSSDK.BSAccessGroupEx)));
            Marshal.StructureToPtr(accessGroupEx, data, true);

            var result = BSSDK.BS_AddAccessGroupEx(DeviceInfo.Handle, data);

            if (result != BSSDK.BS_SUCCESS)
            {
                Logger.Log($"Cannot enroll AccessGroup : {nAccessIdn} on Biostation Device {DeviceInfo.DeviceId}");
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

            var timeZone = _timeZoneService.TimeZones(nTimeZone);

            if (timeZone != null)
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
                // TODO
                timezoneEx[0].scheduleID = timeZone.Id;  //Timezone

                var aNameBytes = Encoding.Unicode.GetBytes(timeZone.Name);
                Buffer.BlockCopy(aNameBytes, 0, timezoneEx[0].name, 0, aNameBytes.Length);

                //TODO
                timezoneEx[0].holiday[0] = 0;

                // codeElement support upto 5
                for (var i = 0; i < timeZone.Details.Count; i++)
                {
                    timezoneEx[0].timeCode[i].codeElement[0].startTime = (ushort)timeZone.Details[i].FromTime.TotalSeconds;
                    timezoneEx[0].timeCode[i].codeElement[0].endTime = (ushort)timeZone.Details[i].ToTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[1].startTime = (ushort)timeZone.Details[i].FromTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[1].endTime = (ushort)timeZone.Details[i].ToTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[2].startTime = (ushort)timeZone.Details[i].FromTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[2].endTime = (ushort)timeZone.Details[i].ToTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[3].startTime = (ushort)timeZone.Details[i].FromTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[3].endTime = (ushort)timeZone.Details[i].ToTime.TotalSeconds;
                    //timezoneEx[0].timeCode[i].codeElement[4].startTime = (ushort)timeZone.Details[i].FromTime.TicTotalSecondsks;
                    //timezoneEx[0].timeCode[i].codeElement[4].endTime = (ushort)timeZone.Details[i].ToTime.TotalSeconds;
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
            var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BSUserHdrEx)));

            var result = BSSDK.BS_GetUserEx(DeviceInfo.Handle, id, userInfo, MTemplateData);
            if (result != BSSDK.BS_SUCCESS)
            {

                Marshal.FreeHGlobal(userInfo);
                Logger.Log($"User : {id} do not exist on Biostation for delete");
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

                var nMaxLogPerTrial = 32768;
                //var logService = new LogServices();
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

                    if (record.eventType == 40)
                        record.userID = 0;
                    receivedLog.DateTimeTicks = (ulong)record.eventTime;
                    receivedLog.DeviceId = DeviceInfo.DeviceId;
                    receivedLog.DeviceCode = DeviceInfo.Code;
                    receivedLog.InOutMode = DeviceInfo.DeviceTypeId;
                    receivedLog.EventLog = _supremaCodeMappings.GetLogEventGenericLookup(record.eventType) ?? new Lookup
                    {
                        Code = Convert.ToInt32(record.eventType).ToString()
                    };
                    //receivedLog.Reserved = record.reserved;
                    receivedLog.TnaEvent = record.tnaEvent;
                    receivedLog.SubEvent = _supremaCodeMappings.GetLogSubEventGenericLookup(record.subEvent) ?? new Lookup
                    {
                        Code = Convert.ToInt32(record.subEvent).ToString()
                    };
                    receivedLog.MatchingType = _supremaCodeMappings.GetMatchingTypeGenericLookup(record.subEvent);
                    receivedLog.UserId = (int)record.userID;
                    if (receivedLog.EventLog.Code == "16001" || receivedLog.EventLog.Code == "16002" || receivedLog.EventLog.Code == "16007")
                    {
                        receivedLog.UserId = 0;
                    }
                    //receivedLog.MatchingType = _matchingTypes.Unknown;
                    switch (record.subEvent)
                    {
                        case 0x3A:
                            receivedLog.MatchingType = _matchingTypes.Finger;
                            break;
                        case 0x3B:
                            //User has been verified by(Finger + PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x3D:
                            receivedLog.MatchingType = _matchingTypes.Face;
                            break;

                        case 0x3E:
                            //User has been verified by(Face + PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x2B:
                            //User has been verified by(ID + Finger)
                            receivedLog.MatchingType = _matchingTypes.Finger;
                            break;
                        case 0x2C:
                            //User has been verified by (ID+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x2D:
                            //User has been verified by (Card+Finger)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x2E:
                            //User has been verified by (Card+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x2F:
                            receivedLog.MatchingType = _matchingTypes.Card;
                            break;
                        case 0x30:
                            //User has been verified by (Card+Finger+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x31:
                            //User has been verified by (ID+Finger+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x32:
                            //User has been verified by (ID+Face)
                            receivedLog.MatchingType = _matchingTypes.Face;
                            break;
                        case 0x33:
                            //User has been verified by (Card+Face)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x34:
                            //User has been verified by (Card+Face+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x35:
                            //User has been verified by (FACE+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                    }

                    if (receivedLog.MatchingType is null)
                    {
                        if (record.eventType == 55 || record.eventType == 56 || record.eventType == 109 || record.eventType == 99)
                        {
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                        }
                        else
                        {
                            receivedLog.MatchingType = _matchingTypes.UnIdentify;
                        }
                    }

                    allLogList.Add(receivedLog);
                    //logService.InsertLog(recivedLog, ConnectionType);
                }

                //logService.BulkInsertLog(allLogList, ConnectionType);
                _logService.AddLog(allLogList);
                Logger.Log($"{logTotalCount} offline logs retrieved from device {DeviceInfo.DeviceId}");

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

                var nMaxLogPerTrial = 32768;
                //var logService = new LogService();

                do
                {
                    var result = 0;

                    var buf = new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx)));
                    if (logTotalCount == 0)
                    {
                        //result = BSSDK.BS_ReadLog(DeviceInfo.Handle, startTime, endTime, ref logCount, buf);
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
                        DateTimeTicks = (ulong)record.eventTime,
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

                    if (record.eventType == 40)
                        record.userID = 0;

                    allLogList.Add(receivedLog);
                    //logService.InsertLog(recivedLog, ConnectionType);
                }

                //logService.BulkInsertLog(allLogList, ConnectionType);
                _logService.AddLog(allLogList).Wait();
                Logger.Log($"{logTotalCount} offline logs retrieved from device {DeviceInfo.DeviceId}");

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
            var bsSysInfoConfig = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSysInfoConfig)));

            var result = BSSDK.BS_ReadSysInfoConfig(DeviceInfo.Handle, bsSysInfoConfig);
            if (result != BSSDK.BS_SUCCESS)
            {
                Logger.Log($"Cannot read the sys info of Device : {DeviceInfo.DeviceId}");
                Marshal.FreeHGlobal(bsSysInfoConfig);
                return -1;
            }

            var bsSysInfoConfigObj =
                (BSSysInfoConfig)Marshal.PtrToStructure(bsSysInfoConfig, typeof(BSSysInfoConfig));
            Marshal.FreeHGlobal(bsSysInfoConfig);

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

            DeviceInfo.MacAddress = Encoding.UTF8.GetString(bsSysInfoConfigObj.macAddr);
            DeviceInfo.FirmwareVersion = Encoding.UTF8.GetString(bsSysInfoConfigObj.firmwareVer);
            DeviceInfo.HardwareVersion = Encoding.UTF8.GetString(bsSysInfoConfigObj.boardVer);
            DeviceInfo.Brand = _deviceBrands.Suprema;

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

            var userInfo = Marshal.AllocHGlobal(numOfUser * Marshal.SizeOf(typeof(BSSDK.BSUserHdrEx)));

            //result = BSSDK.BS_GetAllUserInfoEx(DeviceInfo.Handle, userInfo, ref numOfUser);
            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    result = BSSDK.BS_GetAllUserInfoEx(DeviceInfo.Handle, userInfo, ref numOfUser);
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
                    lock (this)
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

            var users = new BSSDK.BSUserHdrEx[numOfUser];

            for (var i = 0; i < numOfUser; i++)
            {
                users[i] =
                    (BSSDK.BSUserHdrEx)
                    Marshal.PtrToStructure(
                        new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(BSSDK.BSUserHdrEx))),
                        typeof(BSSDK.BSUserHdrEx));
            }

            Marshal.FreeHGlobal(userInfo);

            foreach (var user in users)
            {
                var nameAsBytes = new byte[user.name.Length * sizeof(ushort)];
                Buffer.BlockCopy(user.name, 0, nameAsBytes, 0, nameAsBytes.Length);

                var tempUser = new User
                {
                    Code = Convert.ToInt32(user.ID),
                    UserName = Encoding.Unicode.GetString(nameAsBytes).Replace("\0", string.Empty),
                    IsActive = !Convert.ToBoolean(user.disabled),
                    AdminLevel = user.adminLevel,
                    AuthMode = user.authMode
                };

                tempUser.SetStartDateFromTicks(user.startDateTime);
                tempUser.SetEndDateFromTicks(user.expireDateTime);

                usersList.Add(tempUser);

                //usersList.Add(new SupremaUserModel
                //{
                //    SUserId = Convert.ToString(user.ID),
                //    StartDate = Convert.ToInt32(user.startDateTime),
                //    EndDate = Convert.ToInt32(user.expireDateTime),
                //    UserName = Encoding.Unicode.GetString(nameAsBytes).Replace("\0", string.Empty)
                //});
            }

            return usersList;
        }

        public override User GetUser(uint id)
        {
            var result = 0;
            var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BSUserHdrEx)));
            var templateData = new byte[BSSDK.TEMPLATE_SIZE * 2 * 2];

            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    //result = BSSDK.BS_GetUserDBIbnfo(DeviceInfo.Handle, ref numOfUser, ref mNumOfTemplate);
                    result = BSSDK.BS_GetUserEx(DeviceInfo.Handle, id, userInfo, templateData);
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


            var userHdr = (BSSDK.BSUserHdrEx)Marshal.PtrToStructure(userInfo, typeof(BSSDK.BSUserHdrEx));

            Marshal.FreeHGlobal(userInfo);

            var numOfFinger = userHdr.numOfFinger;

            //var userInfo = Marshal.AllocHGlobal(numOfUser * Marshal.SizeOf(typeof(BSSDK.BS2UserHdr)));

            //result = BSSDK.BS_GetAllUserInfoBEPlus(DeviceInfo.Handle, userInfo, ref numOfUser);


            var nameAsBytes = new byte[userHdr.name.Length * sizeof(ushort)];
            Buffer.BlockCopy(userHdr.name, 0, nameAsBytes, 0, nameAsBytes.Length);
            var tempUser = new User
            {
                Code = Convert.ToInt32(userHdr.ID),
                UserName = Encoding.Unicode.GetString(nameAsBytes).Replace("\0", string.Empty),
                IsActive = !Convert.ToBoolean(userHdr.disabled),
                AdminLevel = userHdr.adminLevel
            };

            if (!(userHdr.password is null))
            {
                tempUser.PasswordBytes = userHdr.password.Select(Convert.ToByte).ToArray();
                tempUser.Password = userHdr.password.ToString();
            }

            tempUser.FingerTemplates = new List<FingerTemplate>();

            //card
            tempUser.IdentityCard = new IdentityCard
            {
                //Id = (int)userHdr.ID,
                Number = userHdr.cardID.ToString(),
                DataCheck = 0,
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
                        CreateAt = DateTime.Now,
                        TemplateIndex = 1
                    };

                    tempUser.FingerTemplates.Add(secondFingerTemplateSample);
                }
            }

            tempUser.SetStartDateFromTicks(userHdr.startDateTime);
            tempUser.SetEndDateFromTicks(userHdr.expireDateTime);

            return tempUser;
        }

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
                DateTimeTicks = (ulong)(DateTime.Now.Ticks / 100000),
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
    }
}
