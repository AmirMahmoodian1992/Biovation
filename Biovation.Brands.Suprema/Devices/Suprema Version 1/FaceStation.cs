using Biovation.Brands.Suprema.Manager;
using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Brands.Suprema.Services;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Suprema.Devices.Suprema_Version_1
{
    /// <summary>
    /// برای ساعت FaceStation
    /// </summary>
    /// <seealso cref="Device" />
    internal class FaceStation : Device
    {
        private readonly object _deviceAccessObject = new object();
        private readonly DeviceService _deviceService ;
        private readonly SupremaLogService _supremaLogService ;
        private readonly UserService _userService;
        private readonly AccessGroupService _accessGroupService;
        private readonly UserCardService _userCardService;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly LogService _logService;


        private readonly TimeZoneService _timeZoneService;
        private readonly SupremaCodeMappings _supremaCodeMappings;

        private readonly DeviceBrands _deviceBrands;
        private readonly MatchingTypes _matchingTypes;
        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        public FaceStation(SupremaDeviceModel info, DeviceService deviceService, SupremaLogService supremaLogService, UserCardService userCardService, AccessGroupService accessGroupService, UserService userService, FaceTemplateService faceTemplateService, FaceTemplateTypes faceTemplateTypes, MatchingTypes matchingTypes, LogEvents logEvents, LogService logService, TimeZoneService timeZoneService, SupremaCodeMappings supremaCodeMappings, DeviceBrands deviceBrands, LogSubEvents logSubEvents)
            : base(info,accessGroupService,userCardService)
        {
            _deviceService = deviceService;
            _supremaLogService = supremaLogService;
            _userCardService = userCardService;
            _accessGroupService = accessGroupService;
            _userService = userService;
            _faceTemplateService = faceTemplateService;
            _faceTemplateTypes = faceTemplateTypes;
            _matchingTypes = matchingTypes;
            _logEvents = logEvents;
            _logService = logService;
            _timeZoneService = timeZoneService;
            _supremaCodeMappings = supremaCodeMappings;
            _deviceBrands = deviceBrands;
            _logSubEvents = logSubEvents;
            DeviceAccessSemaphore = new Semaphore(1, 1);
        }

        /// <summary>
        /// <En>Transfer user with all data (face template , card, Id , Access Group ,....) on validated FaceStation devices.</En>
        /// <Fa>کاربر را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="userData">کاربر</param>
        /// <returns></returns>
        public override bool TransferUser(User userData)
        {
            //var user = new UserServices();
            //var userData = user.GetUser(nUserIdn, ConnectionType);


            if (userData == null) return false;


            var stillCutData = new byte[BSSDK.BS_MAX_IMAGE_SIZE * BSSDK.BS_FST_MAX_FACE_TYPE];
            var faceTemplateData = new byte[BSSDK.BS_FST_FACETEMPLATE_SIZE * BSSDK.BS_FST_MAX_FACE_TEMPLATE * BSSDK.BS_FST_MAX_FACE_TYPE];
            var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.FSUserHdrEx)));
            var nSize = Marshal.SizeOf(typeof(BSSDK.FSUserHdrEx));
            var bytes = new byte[nSize];
            Marshal.Copy(userInfo, bytes, 0, nSize);
            Array.Clear(bytes, 0, nSize);
            Marshal.Copy(bytes, 0, userInfo, nSize);
            var userHdr = (BSSDK.FSUserHdrEx)Marshal.PtrToStructure(userInfo, typeof(BSSDK.FSUserHdrEx));

            // name 
            if (!(userData.UserName is null))
            {
                var username = userData.UserName;
                var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
            }

            if (!(userData.Password is null))
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

            //userHdr.ID = Convert.ToUInt32(userData.SUserId);
            userHdr.ID = Convert.ToUInt32(userData.Code);

            var adminL = userData.AdminLevel == 240 ? 1 : 0;
            userHdr.adminLevel =
                (ushort)
                (adminL == 1
                    ? BSSDK.FSUserHdrEx.ENUM.USER_ADMIN
                    : BSSDK.FSUserHdrEx.ENUM.USER_NORMAL);

            userHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;

            userHdr.startDateTime = Convert.ToUInt32(userData.GetStartDateInTicks());
            userHdr.expireDateTime = Convert.ToUInt32(userData.GetEndDateInTicks());
            userHdr.authMode = Convert.ToUInt16(userData.AuthMode);
            //(ushort)(BSSDK.BS_AUTH_MODE_DISABLED) ;//yani tebghe har tanzimi ke rooye dastgah hast amal konad//BS_AUTH_CARD_ONLY


            try
            {
                // AccessGroup 
                //var t = GetAccessGroup(nUserIdn);
                var t = GetAccessGroup(userData.Code);
                userHdr.accessGroupMask = Convert.ToUInt32(t, 16);
            }
            catch (Exception)
            {
                userHdr.accessGroupMask = 0xffffffff;
            }

            var hasCard = false;

            try
            {
                //card
              
                var userCards = _userCardService.GetCardsByFilter(userData.Id,true);
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
                    Logger.Log($"The User : {userData.Id} Do not have card or query do not return any thing");
                }
            }
            catch (Exception e)
            {
                userHdr.cardID = 0;
                Logger.Log(e.ToString());
            }

            // fill stillcutData
            //var hasFaceStillCut = false;
            var hasFaceTemplate = false;
            var nIndexMax = new int[5];
           

            // TODO Important: Add still cut image to face template
            //var userFace = faceService.GetImage(nUserIdn, ConnectionType);
            //if (userFace.Count > 0)
            //{
            //    hasFaceStillCut = true;
            //}

            //if (hasFaceStillCut)
            //{
            //    var index = 0;
            //    for (var i = 0; i < userFace.Count; i++)
            //    {
            //        userFace[i].Image.CopyTo(stillCutData, index);
            //        index += userFace[i].ImageLenght;
            //        userHdr.faceStillcutLen[i] = Convert.ToUInt16(userFace[i].ImageLenght);
            //    }
            //}

            //var faceTemplate = faceService.GetFaceTemplate(nUserIdn, ConnectionType);
            var faceTemplate = (_faceTemplateService.FaceTemplates(userId:userData.Id).Where(x => x.FaceTemplateType.Code == _faceTemplateTypes.SFACE.Code)).ToList();
            var rowCount = faceTemplate.Count;

            if (rowCount > 0) hasFaceTemplate = true;

            //if (hasFaceTemplate && !(faceTemplate.Any(x => x.FaceTemplateType.Code == FaceTemplateTypes.SFACE.Code)))
            if (hasFaceTemplate)
            {
                var numberOfFace = 1;

                userHdr.numOfFace[0] = Convert.ToUInt16(rowCount);
                for (var i = 0; i < rowCount; i++)
                {
                    faceTemplate[i].Template.CopyTo(faceTemplateData, i * BSSDK.BS_FST_FACETEMPLATE_SIZE);
                    //face template's length
                    userHdr.faceLen[i] = Convert.ToUInt16(faceTemplate[i].Size);
                    userHdr.faceChecksum[i] = Convert.ToUInt32(faceTemplate[i].CheckSum);
                    numberOfFace = faceTemplate[i].Index > numberOfFace
                        ? faceTemplate[i].Index
                        : numberOfFace;
                    switch (faceTemplate[i].Index)
                    {
                        case 1:
                            nIndexMax[0] = faceTemplate[i].Index > nIndexMax[0]
                                ? faceTemplate[i].Index
                                : nIndexMax[0];
                            break;
                        case 2:
                            nIndexMax[1] = faceTemplate[i].Index > nIndexMax[1]
                                ? faceTemplate[i].Index
                                : nIndexMax[1];
                            break;
                        case 3:
                            nIndexMax[2] = faceTemplate[i].Index > nIndexMax[2]
                                ? faceTemplate[i].Index
                                : nIndexMax[2];
                            break;
                        case 4:
                            nIndexMax[3] = faceTemplate[i].Index > nIndexMax[3]
                                ? faceTemplate[i].Index
                                : nIndexMax[3];
                            break;
                        case 5:
                            nIndexMax[4] = faceTemplate[i].Index > nIndexMax[4]
                                ? faceTemplate[i].Index
                                : nIndexMax[4];
                            break;
                    }
                }


                userHdr.numOfFaceType = Convert.ToUInt16(numberOfFace); //1;// number of face that define for one person
                for (var i = 0; i < numberOfFace; i++)
                {
                    userHdr.numOfFace[i] = Convert.ToUInt16(nIndexMax[i]);

                }
            }

            if (hasFaceTemplate || hasCard)
            {
                Thread.Sleep(5000);

                userHdr.faceUpdatedIndex[0] = 0;

                Marshal.StructureToPtr(userHdr, userInfo, true);
                //var res = BSSDK.BS_EnrollUserFStationEx(DeviceInfo.Handle, userInfo, stillcutData, faceTemplateData);
                var res = 0;

                try
                {
                    lock (_deviceAccessObject)
                    {
                        DeviceAccessSemaphore.WaitOne();
                        res = BSSDK.BS_EnrollUserFStationEx(DeviceInfo.Handle, userInfo, stillCutData,
                            faceTemplateData);
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
                    Logger.Log($"Cannot enroll User : {userData.Id} on FaceStation Device");
                    return false;
                }
            }

            else
            {
                Logger.Log($"Not transferred to Facestation Due no face template and no card were available for User : {userData.Id}.");
                //var res = BSSDK.BS_DeleteUser(DeviceInfo.Handle, Convert.ToUInt32(userData.SUserId));
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
                //        DeviceAccessSemaphore.Release();
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
            //var accessGroupZoneService = new TimeZoneService();
            //var accessGroupZone = accessGroupZoneService.GetAccessGroupZone(nAccessIdn, ConnectionType);

            //if (accessGroupZone.Count != 0)
            //{
            //    BSSDK.BSTimeScheduleEx[] timezoneEx = new BSSDK.BSTimeScheduleEx[1];

            //    for (int i = 0; i < BSSDK.DF_MAX_TIMESCHEDULE; i++)
            //    {
            //        timezoneEx[0].name = new byte[32];
            //        timezoneEx[0].holiday = new int[2];
            //        timezoneEx[0].timeCode = new BSSDK.BSTimeCodeEx[9];
            //        for (int k = 0; k < 9; k++)
            //        {
            //            timezoneEx[0].timeCode[k].codeElement = new BSSDK.BSTimeCodeElemEx[5];
            //        }

            //        timezoneEx[0].reserved = new int[2];
            //    }

            //    //int numOfSchedule = 1;  //support upto 128

            //    var timezoneService = new TimeZoneService();
            //    var nTimeZone = accessGroupZone.FirstOrDefault();
            //    var timezone = timezoneService.GetTimeZone(nTimeZone.TimeCode, ConnectionType).ToList();

            //    timezoneEx[0].scheduleID = timezone[0].ScheduleId;  //Timezone

            //    byte[] aNameBytesZone = Encoding.Unicode.GetBytes(timezone[0].Name);
            //    Buffer.BlockCopy(aNameBytesZone, 0, timezoneEx[0].name, 0, aNameBytesZone.Length);

            //    timezoneEx[0].holiday[0] = timezone[0].HolidayIdn;

            //    // codeElement support upto 5
            //    for (int i = 0; i < timezone.Count; i++)
            //    {
            //        timezoneEx[0].timeCode[i].codeElement[0].startTime = (ushort)timezone[i].StartTime1;
            //        timezoneEx[0].timeCode[i].codeElement[0].endTime = (ushort)timezone[i].EndTime1;
            //        timezoneEx[0].timeCode[i].codeElement[1].startTime = (ushort)timezone[i].StartTime2;
            //        timezoneEx[0].timeCode[i].codeElement[1].endTime = (ushort)timezone[i].EndTime2;
            //        timezoneEx[0].timeCode[i].codeElement[2].startTime = (ushort)timezone[i].StartTime3;
            //        timezoneEx[0].timeCode[i].codeElement[2].endTime = (ushort)timezone[i].EndTime3;
            //        timezoneEx[0].timeCode[i].codeElement[3].startTime = (ushort)timezone[i].StartTime4;
            //        timezoneEx[0].timeCode[i].codeElement[3].endTime = (ushort)timezone[i].EndTime4;
            //        timezoneEx[0].timeCode[i].codeElement[4].startTime = (ushort)timezone[i].StartTime5;
            //        timezoneEx[0].timeCode[i].codeElement[4].endTime = (ushort)timezone[i].EndTime5;
            //    }

            //    timezoneEx[0].reserved[0] = 0;
            //    timezoneEx[0].reserved[1] = 0;

            //    IntPtr dataZone = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BSTimeScheduleEx)));
            //    Marshal.StructureToPtr(timezoneEx[0], dataZone, true);

            //    int resultZone = BSSDK.BS_AddTimeScheduleEx(DeviceInfo.Handle, dataZone);
            //    while (resultZone != BSSDK.BS_SUCCESS && resultZone == BSSDK.BS_ERR_NOT_FOUND)
            //    {
            //        resultZone = BSSDK.BS_AddTimeScheduleEx(DeviceInfo.Handle, dataZone);
            //    }

            //    if (resultZone == BSSDK.BS_SUCCESS || resultZone == BSSDK.BS_ERR_NOT_FOUND)
            //    {
            //        Logger.Log("Timezone {0} transferred to device {1} successfully.", nTimeZone.TimeCode, DeviceInfo.DeviceId.ToString());
            //        //return true;
            //    }

            //    Logger.Log("Cannot enroll Timezone : {0} on Biostation Device", nTimeZone.TimeCode);
            //    //return false;
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
                accessGroupEx.readerID[0] = DeviceInfo.Code;
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
                Logger.Log($"Cannot enroll AccessGroup : {nAccessIdn} on FaceStation Device {DeviceInfo.Code}");
                return false;
            }

            var data = Marshal.AllocHGlobal(BSSDK.DF_MAX_ACCESSGROUP * Marshal.SizeOf(typeof(BSSDK.BSAccessGroupEx)));
            Marshal.StructureToPtr(accessGroupEx, data, true);

            var result = BSSDK.BS_AddAccessGroupEx(DeviceInfo.Handle, data);

            if (result != BSSDK.BS_SUCCESS)
            {
                Logger.Log($"Cannot enroll AccessGroup : {nAccessIdn} on FaceStation Device {DeviceInfo.Code}");
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
            var imageData = new byte[BSSDK.BS_MAX_IMAGE_SIZE * BSSDK.BS_FST_MAX_FACE_TYPE];
            var faceTemplate =
                new byte[BSSDK.BS_FST_FACETEMPLATE_SIZE * BSSDK.BS_FST_MAX_FACE_TEMPLATE * BSSDK.BS_FST_MAX_FACE_TYPE];

            var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.FSUserHdrEx)));

            var result = BSSDK.BS_GetUserFStationEx(DeviceInfo.Handle, id, userInfo, imageData, faceTemplate);

            if (result != BSSDK.BS_SUCCESS)
            {

                Marshal.FreeHGlobal(userInfo);
                Logger.Log($"User : {id} do not exist on FaceStation for delete");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <En>Read all log data from device, since last disconnect, For FaceStation , D-Station, X-Station.</En>
        /// <Fa>داده های اتفاقات در طول زمان قطعی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
        public override /*async*/ ResultViewModel ReadOfflineLog(object cancellationToken, bool fileSave = false)
        {
            Token = (CancellationToken)cancellationToken;
            //await Task.Run(() =>
            //{
            var objectLock = new object();
            lock (objectLock)
            {
                var mNumOfLog = 0;

                try
                {
                    DeviceAccessSemaphore.WaitOne();

                    if (Token.IsCancellationRequested)
                    {
                        Logger.Log("Thread canceled.");
                        return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = "عملیات تخلیه متوقف شد.", Validate = 0 };
                    }

                    //Thread.Sleep(500);
                    BSSDK.BS_GetLogCount(DeviceInfo.Handle, ref mNumOfLog);
                }
                catch (Exception exception)
                {
                    //Logger.Log($"Cannot get count of logs of device {DeviceInfo.DeviceId}");
                    //Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                    Logger.Log(exception, $"Cannot get count of logs of device {DeviceInfo.DeviceId}");
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
                    return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = "عملیات تخلیه متوقف شد، مشکل در ارتباط با دستگاه.", Validate = 0 };

                Logger.Log($"Getting {mNumOfLog} logs of device {DeviceInfo.Code} started.");

                var logRecord = Marshal.AllocHGlobal(mNumOfLog * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx)));

                var logTotalCount = 0;
                var logCount = 0;

                const int nMaxLogPerTrial = 21845;
                // var logService = new LogServices();

                do
                {
                    var result = 0;

                    var buf =
                        new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx)));
                    if (logTotalCount == 0)
                    {
                        var lastConnectedTime = _deviceService.GetLastConnectedTime(DeviceInfo.DeviceId)?.Data?? new DateTime(1970, 1, 1);
                        var logData = _supremaLogService.GetLastLogsOfDevice(DeviceInfo.Code).Result;

                        if (lastConnectedTime.DayOfYear < DateTime.Now.DayOfYear)
                        {
                            //result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                            if (logData.Count == 0)
                            {
                                try
                                {
                                    DeviceAccessSemaphore.WaitOne();

                                    if (Token.IsCancellationRequested)
                                    {
                                        Marshal.FreeHGlobal(logRecord);

                                        Logger.Log("Thread canceled.");
                                        return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = "عملیات تخلیه متوقف شد.", Validate = 0 };
                                    }

                                    result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                                }
                                catch (Exception)
                                {
                                    //ignore
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
                                try
                                {
                                    DeviceAccessSemaphore.WaitOne();

                                    if (Token.IsCancellationRequested)
                                    {
                                        Marshal.FreeHGlobal(logRecord);

                                        Logger.Log("Thread canceled.");
                                        return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = "عملیات تخلیه متوقف شد.", Validate = 0 };
                                    }

                                    var timeInTicks = logData[logData.Count - 1].DateTimeTicks - 7 * 24 * 60 * 60;
                                    result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, (int)timeInTicks, 0, ref logCount, buf);
                                }
                                catch (Exception exception)
                                {
                                    //Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                                    //Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                                    Logger.Log(exception, $"Cannot get logs of device {DeviceInfo.DeviceId}");
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

                        else
                        {
                            if (logData.Count == 0)
                            {
                                //result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                                try
                                {
                                    DeviceAccessSemaphore.WaitOne();

                                    if (Token.IsCancellationRequested)
                                    {
                                        Marshal.FreeHGlobal(logRecord);

                                        Logger.Log("Thread canceled.");
                                        break;
                                    }

                                    result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                                }
                                catch (Exception exception)
                                {
                                    //Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                                    //Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                                    Logger.Log(exception, $"Cannot get logs of device {DeviceInfo.DeviceId}");
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

                                //result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, logData[logData.Count - 1].nDateTime, 0, ref logCount, buf);
                                try
                                {
                                    DeviceAccessSemaphore.WaitOne();

                                    if (Token.IsCancellationRequested)
                                    {
                                        Marshal.FreeHGlobal(logRecord);

                                        Logger.Log("Thread canceled.");
                                        break;
                                    }

                                    result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle,
                                        (int)logData[logData.Count - 1].DateTimeTicks, 0, ref logCount, buf);
                                }
                                catch (Exception exception)
                                {
                                    //Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                                    //Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                                    Logger.Log(exception, $"Cannot get logs of device {DeviceInfo.DeviceId}");
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
                        //result = BSSDK.BS_ReadNextLogEx(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                        try
                        {
                            DeviceAccessSemaphore.WaitOne();

                            if (Token.IsCancellationRequested)
                            {
                                Marshal.FreeHGlobal(logRecord);

                                Logger.Log("Thread canceled.");
                                break;
                            }

                            result = BSSDK.BS_ReadNextLogEx(DeviceInfo.Handle, 0, 0, ref logCount, buf);
                        }
                        catch (Exception exception)
                        {
                            //Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                            //Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                            Logger.Log(exception, $"Cannot get logs of device {DeviceInfo.DeviceId}");
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
                    var record =
                        (BSSDK.BSLogRecordEx)
                        Marshal.PtrToStructure(
                            new IntPtr(logRecord.ToInt32() + i * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx))),
                            typeof(BSSDK.BSLogRecordEx));

                    var receivedLog = new SupremaLog
                    {
                        DateTimeTicks = (uint)record.eventTime,
                        DeviceId = DeviceInfo.DeviceId,
                        DeviceCode = DeviceInfo.Code,
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
                        UserId = (int)record.userID
                    };

                    if (record.eventType == 40)
                        record.userID = 0;

                    if (receivedLog.EventLog.Code == "16001" || receivedLog.EventLog.Code == "16002" || receivedLog.EventLog.Code == "16007")
                    {
                        receivedLog.UserId = 0;
                    }

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

                //logService.BulkInsertLogAsync(allLogList, ConnectionType)/*.ConfigureAwait(true)*/;
                _supremaLogService.AddLog(allLogList);

                Task.Run(() => GetAllImageOfFaceLogs(allLogList));

                //GetAllImageOfFaceLogs();

                //foreach (var log in allLogList)
                //{
                //    if (log.EventId == 55 || log.EventId == 61)
                //    {
                //        GetImageOfFaceLog(log);
                //    }
                //}

                Logger.Log($"{logTotalCount} offline logs retrieved from device {DeviceInfo.DeviceId}");

                Marshal.FreeHGlobal(logRecord);
                return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = $"تخلیه {logTotalCount} تردد با موفقیت انجام شد.", Validate = 1 };
            }
            //});
        }

        private void GetAllImageOfFaceLogs(List<Log> logList)
        {
            foreach (var log in logList)
            {
                if (log.EventLog.Code == _logEvents.Authorized.Code || log.EventLog.Code == _logEvents.IdentifySuccessFace.Code)
                {
                    GetImageOfFaceLog(log);
                }
            }
        }

        //private void GetAllImageOfFaceLogs()
        //{
        //    try
        //    {
        //        //int dataLen = 0;
        //        var numOfImageLog = 0;


        //        BSSDK.BS_GetImageLogCount(DeviceInfo.Handle, ref numOfImageLog);
        //        var imageLogs =
        //            new byte[numOfImageLog][];

        //        for (var i = 0; i < imageLogs.Length; i++)
        //        {
        //            imageLogs[i] = new byte[Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)) + BSSDK.BS_MAX_IMAGE_SIZE];
        //        }


        //        var imageLogMemStartPtr = Marshal.AllocHGlobal(numOfImageLog * (Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)) + BSSDK.BS_MAX_IMAGE_SIZE));
        //        //var imageLogMemStartPtr = new byte[numOfImageLog * (Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)) + BSSDK.BS_MAX_IMAGE_SIZE)];
        //        //var logRecord = (mNumOfLog * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx)));

        //        //byte[][] imageLog =
        //        //    new byte[][Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)) + BSSDK.DF_LEN_MAX_IMAGE];
        //        var logCountOfPeriod = 0;
        //        var result = BSSDK.BS_ReadImageLog(DeviceInfo.Handle, 0, 0, ref numOfImageLog, imageLogMemStartPtr);

        //        for (var i = 0; i < logCountOfPeriod; i++)
        //        {
        //            var packet = GCHandle.Alloc(imageLogMemStartPtr, GCHandleType.Pinned);
        //            var imageHdr =
        //                (BSSDK.BSImageLogHdr)
        //                Marshal.PtrToStructure(packet.AddrOfPinnedObject() + i * (Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)) + BSSDK.BS_MAX_IMAGE_SIZE), typeof(BSSDK.BSImageLogHdr));

        //            //var record =
        //            //    (BSSDK.BSImageLogHdr)
        //            //    Marshal.PtrToStructure(
        //            //        new IntPtr(imageLogMemStartPtr.ToInt32() + i * (Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)) + BSSDK.DF_LEN_MAX_IMAGE)),
        //            //        typeof(BSSDK.BSImageLogHdr));
        //        }

        //        Marshal.FreeHGlobal(imageLogMemStartPtr);

        //        //if (dataLen > Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)))
        //        //{
        //        //    GCHandle packet = GCHandle.Alloc(imageLog, GCHandleType.Pinned);
        //        //    BSSDK.BSImageLogHdr imageHdr =
        //        //        (BSSDK.BSImageLogHdr)
        //        //        Marshal.PtrToStructure(packet.AddrOfPinnedObject(), typeof(BSSDK.BSImageLogHdr));
        //        //    var imageSize = imageHdr.imageSize;
        //        //    packet.Free();

        //        //    if (imageSize > 0 && imageSize < BSSDK.DF_LEN_MAX_IMAGE)
        //        //    {
        //        //        var imageBytes = new byte[imageSize];
        //        //        Buffer.BlockCopy(imageLog, Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)), imageBytes,
        //        //            0, imageBytes.Length);

        //        //        var faceLog = new FaceLogModel
        //        //        {
        //        //            DateTime = log.nDateTime,
        //        //            EventIdn = log.EventId,
        //        //            ReaderIdn = DeviceInfo.DeviceId,
        //        //            UserID = log.UserID == 0 ? (uint)log.nUserID : log.UserID,
        //        //            FaceImageLen = imageSize,
        //        //            Type = 1,
        //        //            Image = imageBytes
        //        //        };

        //        //        var logService = new LogServices();
        //        //        logService.InsertFaceLog(faceLog, ConnectionType);
        //        //    }
        //        //}

        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log(e.ToString());
        //    }
        //}

        private void GetImageOfFaceLog(Log log)
        {
            try
            {
                var imageLog =
                    new byte[Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)) + BSSDK.DF_LEN_MAX_IMAGE];

                var dataLen = 0;

                var eventType = Convert.ToInt32(log.EventLog.Code);
                var nEventTime = log.DateTimeTicks;
                //int numOfLog = 0;

                //BSSDK.BS_GetImageLogCount(DeviceInfo.Handle, ref numOfLog);
                BSSDK.BS_ReadSpecificImageLog(DeviceInfo.Handle, (int)nEventTime, eventType, ref dataLen, imageLog);

                if (dataLen > Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)))
                {
                    var packet = GCHandle.Alloc(imageLog, GCHandleType.Pinned);
                    var imageHdr =
                        (BSSDK.BSImageLogHdr)
                        Marshal.PtrToStructure(packet.AddrOfPinnedObject(), typeof(BSSDK.BSImageLogHdr));
                    var imageSize = imageHdr.imageSize;
                    packet.Free();

                    if (imageSize > 0 && imageSize < BSSDK.DF_LEN_MAX_IMAGE)
                    {
                        var imageBytes = new byte[imageSize];
                        Buffer.BlockCopy(imageLog, Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)), imageBytes,
                            0, imageBytes.Length);

                        //var faceLog = new FaceLogModel
                        //{
                        //    DateTime = (int)log.DateTimeTicks,
                        //    EventIdn = log.EventId,
                        //    ReaderIdn = DeviceInfo.Code,
                        //    UserID = (uint)log.UserId,
                        //    FaceImageLen = imageSize,
                        //    Type = 1,
                        //    Image = imageBytes
                        //};

                        log.PicByte = imageBytes;

                     var listLog=new List<Log>{log};
                        //logService.InsertFaceLog(faceLog, ConnectionType);
                        _logService.UpdateLog(listLog);
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
            }
        }

        /// <summary>
        /// <En>Read all log data from device, since last disconnect, For FaceStation , D-Station, X-Station.</En>
        /// <Fa>داده های اتفاقات در طول مدت زمان مشخصی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
        public override List<object> ReadLogOfPeriod(int startTime, int endTime)
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
                        return new List<object>();
                    }
                    DeviceAccessSemaphore.WaitOne();
                    BSSDK.BS_GetLogCount(DeviceInfo.Handle, ref mNumOfLog);
                }
                catch (Exception exception)
                {
                    Logger.Log($"Cannot get number of logs of device {DeviceInfo.DeviceId}");
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
                    return new List<object>();

                Logger.Log($"Getting {mNumOfLog} logs of device {DeviceInfo.Code} started.");

                var logRecord = Marshal.AllocHGlobal(mNumOfLog * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx)));

                var logTotalCount = 0;
                var logCount = 0;

                var nMaxLogPerTrial = 21845;
            

                do
                {
                    var result = 0;

                    var buf = new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSSDK.BSLogRecordEx)));
                    if (logTotalCount == 0)
                    {
                        //result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, startTime, endTime, ref logCount, buf);
                        try
                        {
                            DeviceAccessSemaphore.WaitOne();
                            result = BSSDK.BS_ReadLogEx(DeviceInfo.Handle, startTime, endTime, ref logCount, buf);
                        }
                        catch (Exception exception)
                        {
                            //Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}", logType:LogType.Warning);
                            //Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                            Logger.Log(exception, $"Cannot get logs of device {DeviceInfo.DeviceId}");
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
                        //result = BSSDK.BS_ReadNextLogEx(DeviceInfo.Handle, 0, endTime, ref logCount, buf);
                        try
                        {
                            DeviceAccessSemaphore.WaitOne();
                            result = BSSDK.BS_ReadNextLogEx(DeviceInfo.Handle, 0, endTime, ref logCount, buf);
                        }
                        catch (Exception exception)
                        {
                            //Logger.Log($"Cannot get logs of device {DeviceInfo.DeviceId}");
                            //Logger.Log($"Exception: {exception.Message} \n\nStackTrace: {exception.StackTrace}");
                            Logger.Log(exception, $"Cannot get logs of device {DeviceInfo.DeviceId}");
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
                        DeviceCode = DeviceInfo.Code,
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
                        UserId = (int)record.userID
                    };

                    if (record.eventType == 40)
                        record.userID = 0;

                    if (receivedLog.EventLog.Code == "16001" || receivedLog.EventLog.Code == "16002" || receivedLog.EventLog.Code == "16007")
                    {
                        receivedLog.UserId = 0;
                    }

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
                Logger.Log($"{logTotalCount} offline logs retrieved from device {logTotalCount}");

                Marshal.FreeHGlobal(logRecord);

                var logListOfObject = new List<object>(allLogList);
                return logListOfObject;
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
                Logger.Log($"Cannot read the sys info of Device : {DeviceInfo.Code}");
                Marshal.FreeHGlobal(bsSysInfoConfig);
                return -1;
            }

            var bsSysInfoConfigObj =
                (BSSysInfoConfig)Marshal.PtrToStructure(bsSysInfoConfig, typeof(BSSysInfoConfig));
            Marshal.FreeHGlobal(bsSysInfoConfig);

            var existingDevice = _deviceService.GetDevices(code:DeviceInfo.Code,brandId: _deviceBrands.Suprema.Code).FirstOrDefault();

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

            var userInfo = Marshal.AllocHGlobal(numOfUser * Marshal.SizeOf(typeof(BSSDK.FSUserHdr)));

            //result = BSSDK.BS_GetAllUserInfoFStation(DeviceInfo.Handle, userInfo, ref numOfUser);

            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    result = BSSDK.BS_GetAllUserInfoFStation(DeviceInfo.Handle, userInfo, ref numOfUser);
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

            var users = new BSSDK.FSUserHdr[numOfUser];

            for (var i = 0; i < numOfUser; i++)
            {
                users[i] =
                    (BSSDK.FSUserHdr)
                    Marshal.PtrToStructure(
                        new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(BSSDK.FSUserHdr))),
                        typeof(BSSDK.FSUserHdr));
            }

            Marshal.FreeHGlobal(userInfo);



            foreach (var user in users)
            {
                var nameAsBytes = new byte[user.name.Length * sizeof(ushort)];
                Buffer.BlockCopy(user.name, 0, nameAsBytes, 0, nameAsBytes.Length);

                var tempUser = new User
                {
                    Id = Convert.ToInt32(user.ID),
                    UserName = Encoding.Unicode.GetString(nameAsBytes).Replace("\0", string.Empty),
                    IsActive = !Convert.ToBoolean(user.disabled),
                    AdminLevel = user.adminLevel

                };

                tempUser.SetStartDateFromTicks(Convert.ToInt32(user.startDateTime));
                tempUser.SetEndDateFromTicks(Convert.ToInt32(user.expireDateTime));

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

            const int bsFstMaxFaceType = 5;
            const int bsFstMaxFaceTemplate = 25;
            const int bsFstFaceTemplateSize = 2000;
            const int bsMaxImageSize = (100 * 1024);

            var mStillCutData = new byte[bsFstMaxFaceType * bsMaxImageSize];
            var mFaceTemplate = new byte[bsFstMaxFaceType * bsFstFaceTemplateSize * bsFstMaxFaceTemplate];

            var imageData = new byte[bsMaxImageSize * bsFstMaxFaceType];
            var faceTemplate = new byte[bsFstFaceTemplateSize * bsFstMaxFaceTemplate * bsFstMaxFaceType];
            var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.FSUserHdrEx)));
            var result = 0;

            try
            {
                lock (_deviceAccessObject)
                {
                    DeviceAccessSemaphore.WaitOne();
                    //result = BSSDK.BS_GetUserDBIbnfo(DeviceInfo.Handle, ref numOfUser, ref mNumOfTemplate);
                    result = BSSDK.BS_GetUserFStationEx(DeviceInfo.Handle, id, userInfo, imageData, faceTemplate);
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



            var userHdr = (BSSDK.FSUserHdrEx)Marshal.PtrToStructure(userInfo, typeof(BSSDK.FSUserHdrEx));
            Marshal.FreeHGlobal(userInfo);

            var nameAsBytes = new byte[userHdr.name.Length * sizeof(ushort)];
            Buffer.BlockCopy(userHdr.name, 0, nameAsBytes, 0, nameAsBytes.Length);
            var tempUser = new User
            {
                Id = Convert.ToInt32(userHdr.ID),
                UserName = Encoding.Unicode.GetString(nameAsBytes).Replace("\0", string.Empty),
                IsActive = !Convert.ToBoolean(userHdr.disabled),
                AdminLevel = userHdr.adminLevel
            };

            if (!(userHdr.password is null))
            {
                tempUser.PasswordBytes = userHdr.password.Select(Convert.ToByte).ToArray();
                tempUser.Password = userHdr.password.ToString();
            }

            tempUser.SetStartDateFromTicks((int)(new DateTime(1970, 1, 1).AddSeconds(userHdr.startDateTime)).Ticks);
            tempUser.SetEndDateFromTicks((int)(new DateTime(1970, 1, 1).AddSeconds(userHdr.expireDateTime)).Ticks);

            //card
            tempUser.IdentityCard = new IdentityCard
            {
                Id = (int)userHdr.ID,
                Number = userHdr.cardID.ToString(),
                DataCheck = 0,
                IsActive = userHdr.cardID != 0
            };

            //face
            if (userHdr.numOfFaceType > 0)
            {
                var imagePos = 0;
                var templatePos = 0;
                for (var i = 0; i < userHdr.numOfFaceType; i++)
                {
                    Buffer.BlockCopy(imageData, imagePos, mStillCutData, i * bsMaxImageSize,
                        userHdr.faceStillcutLen[i]);
                    imagePos += userHdr.faceStillcutLen[i];

                    var nTemplateLen = 0;
                    for (var k = 0; k < userHdr.numOfFace[i]; k++)
                        nTemplateLen += userHdr.faceLen[i * 25 + k];

                    Buffer.BlockCopy(faceTemplate, templatePos, mFaceTemplate,
                        i * bsFstMaxFaceTemplate * bsFstFaceTemplateSize, nTemplateLen);
                    templatePos += nTemplateLen;


                    //jeddan az imageData nmikhaim estefade konim?????

                    var tempFaceTemplate = new FaceTemplate
                    {
                        Id = i,
                        UserId = userHdr.ID,
                        Index = userHdr.numOfFaceType,
                        FaceTemplateType = _faceTemplateTypes.SFACE,
                        Template = mFaceTemplate,
                        CheckSum = mFaceTemplate.Sum(x => x),
                        Size = mFaceTemplate.Length
                        //CreateAt = new DateTime(1970, 1, 1).AddSeconds(userHdr.startDateTime) in mitoone dorost nabashe chon momkene baadan behesh face ezafe shode bashe

                    };
                    tempUser.FaceTemplates.Add(tempFaceTemplate);
                }
            }
            Marshal.FreeHGlobal(userInfo);
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
                DateTimeTicks = (uint)(DateTime.Now.Ticks / 100000),
                DeviceId = DeviceInfo.DeviceId,
                DeviceCode = DeviceInfo.Code,
                EventLog = _logEvents.RemoveUserFromDevice,
                TnaEvent = 0,
                SubEvent = _logSubEvents.Normal,
                UserId = (int)userId,
                MatchingType = _matchingTypes.Unknown
            };
            _supremaLogService.AddLog(deleteLog);

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
