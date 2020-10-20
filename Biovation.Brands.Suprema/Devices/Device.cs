using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace Biovation.Brands.Suprema.Devices
{
    /// <summary>
    /// کلاس پدر ساعت ها که تمامی انواع ساعت ها از آن ارث بری می کنند.
    /// </summary>
    /// <seealso cref="IDevices" />
    public abstract class Device : IDevices
    {
        protected SupremaDeviceModel DeviceInfo;
        //public Mutex UserTransferMutex = new Mutex();
        public Semaphore DeviceAccessSemaphore;
        protected CancellationToken Token;
      
        private readonly UserCardService _userCardService;
        private readonly AccessGroupService _accessGroupService;
        protected Device(SupremaDeviceModel info, AccessGroupService accessGroupService, UserCardService userCardService)
        {
            DeviceInfo = info;
            _accessGroupService = accessGroupService;
            _userCardService = userCardService;
        }

        public void UpdateDeviceInfo(SupremaDeviceModel deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }

        public SupremaDeviceModel GetDeviceInfo()
        {
            return DeviceInfo;
        }

        /// <summary>
        /// <En>Check if the user is on the device or not.</En>
        /// <Fa>وجود یک کاربر بر روی یک ساعت را بررسی می کند.</Fa>
        /// </summary>
        /// <param name="id">شماره کاربر</param>
        /// <returns></returns>
        public abstract bool ExistOnDevice(uint id);

        /// <summary>
        /// <En>Transfer user to device.</En>
        /// <Fa>کاربر را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="nUserIdn">شماره کاربر</param>
        /// <returns></returns>
        public abstract bool TransferUser(User nUserIdn);

        /// <summary>
        /// <En>Transfer Accessgroup to device.</En>
        /// <Fa>دسترسی را به دستگاه انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="nAccessIdn">شماره دسترسی</param>
        /// <returns></returns>
        public abstract bool TransferAccessGroup(int nAccessIdn);

        /// <summary>
        /// <En>Transfer TimeZone to device.</En>
        /// <Fa>گروه های دسترسی موجود برای یک ساعت را از بانک به روی ساعت انتقال می دهد.</Fa>
        /// </summary>
        /// <param name="nTimeZone">شماره تایم زون</param>
        /// <returns></returns>
        public abstract bool TransferTimeZone(int nTimeZone);

        /// <summary>
        /// <En>Add new device to database.</En>
        /// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public abstract int AddDeviceToDataBase();

        /// <summary>
        /// <En>Read all log data from device, since last disconnect.</En>
        /// <Fa>داده های اتفاقات در طول زمان قطعی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
        public abstract ResultViewModel ReadOfflineLog(object token, bool fileSave = false);

        /// <summary>
        /// <En>Read all events that occured when device was offline and do the required works (Transfering User, Transfering Accessgroup, ...).</En>
        /// <Fa>داده های رویداد ها در طول زمان قطعی دستگاه از سرور را، از دیتابیس دریافت می کند و کار های مورد نیاز ( انتقال کاربر، انتقال دسترسی و ... ) را انجام می دهد.</Fa>
        /// </summary>
        public virtual void ReadOfflineEvent()
        {
            //var offlineUserEventService = new OfflineUserEventServices();


            //  var offlineEventService = new OfflineEventService();
            //var userOfflineEvents = offlineEventService.GetUserOfflineEvent(Convert.ToInt32(DeviceInfo.Code), "PersonnelConnectionString");
            // var offlineEvents = offlineEventService.GetOfflineEvents(DeviceInfo.DeviceId);

            //var accessAndTimeOfflineEvents =
            //    offlineAccessAndTimeService.GetAccessAndTimeOfflineEvent(DeviceInfo.DeviceId, "PersonnelConnectionString");

            //var accessGroups = _accessGroupService.GetAccessGroups();

            //if (/*offlineEvents.Count(offlineEvent => offlineEvent.Type == OfflineEventType.AccessGroupChanged) > 0 &&*/
            //    accessGroups.Count ==
            //    offlineEvents.Count(offlineEvent => offlineEvent.Type == OfflineEventType.AccessGroupChanged))
            //{

            DeleteAllTimeZones();
            DeleteAllAccessGroups();
        }
        //}

        //foreach (var offlineEvent in offlineEvents)
        // {
        //if (offlineEvent.Type == OfflineEventType.UserInserted)
        //{
        //    //var userData = userService.GetUser(userOfflineEvent.NUserIdn, "PersonnelConnectionString");
        //    var userData = _userService.GetUsers(offlineEvent.Data, false).FirstOrDefault();

        //    if (userData != null)
        //    {
        //        if (TransferUser(userData))
        //        {
        //           // offlineEventService.DeleteOfflineEvent(offlineEvent.Id);
        //        }
        //    }

        //    else
        //    {
        //        Logger.Log("The user with id: " + offlineEvent.Data +
        //                          " has been deleted before from database! \n");
        //        //offlineEventService.DeleteOfflineEvent(offlineEvent.Id);
        //    }
        //}

        //else if (offlineEvent.Type == OfflineEventType.UserDeleted)
        //{
        //    var userData = _userService.GetUsers(offlineEvent.Data, false);

        //    if (userData != null)
        //    {
        //        //BSSDK.BS_DeleteUser(GetDeviceInfo().Handle,
        //        //    Convert.ToUInt32(userData.SUserId));

        //        //DeleteUser(Convert.ToUInt32(userData.SUserId));
        //        if (DeleteUser(Convert.ToUInt32(userData.Code)))
        //        {
        //            offlineEventService.DeleteOfflineEvent(offlineEvent.Id);
        //        }
        //    }

        //    else
        //    {
        //        Logger.Log("The user with id: " + offlineEvent.Data +
        //                          " has been deleted before from database! \n");
        //    }
        //}

        //else if (offlineEvent.Type == OfflineEventType.AccessGroupChanged)
        //{
        //    if (!TransferAccessGroup(Convert.ToInt32(offlineEvent.Data))) continue;

        //    Logger.Log($"AccessGroup {offlineEvent.Data} transferred to device {offlineEvent.DeviceCode} successfully.");

        //    offlineEventService.DeleteOfflineEvent(offlineEvent.Id);
        //}

        //else if (offlineEvent.Type == OfflineEventType.TimeZoneChanged)
        //{
        //    if (!TransferTimeZone(Convert.ToInt32(offlineEvent.Data))) continue;

        //    Logger.Log($"Timezone {offlineEvent.Data} transferred to device {offlineEvent.DeviceCode} successfully.");

        //    offlineEventService.DeleteOfflineEvent(offlineEvent.Id);
        //}



        /// <summary>
        /// <En>Read all log data from device, since last disconnect.</En>
        /// <Fa>داده های اتفاقات در طول زمان قطعی دستگاه از سرور را، از دستگاه دریافت می کند.</Fa>
        /// </summary>
        public abstract List<object> ReadLogOfPeriod(int startTime, int endTime);

        /// <summary>
        /// <En>Recieves all users from a device.</En>
        /// <Fa>تمامی یوزر ها را از دستگاه دریافت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public abstract List<User> GetAllUsers();

        /// <summary>
        /// <En>Recieves all users from a device.</En>
        /// <Fa>یک یوزر را از دستگاه دریافت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public abstract User GetUser(uint id);

        /// <summary>
        /// <En>Check if the card has been assigned to someone one device before, or not, and remove it from devices.</En>
        /// <Fa>کنترل می کند که یک کارت روی ساعت به شخص دیگری اختصاص داشته یا نه، و درصورت وجود کاربر قبلی را از ساعت حذف می کند.</Fa>
        /// </summary>
        /// <param name="cardNumber">شماره کارت مورد نظر</param>
        public void CardValidation(string cardNumber)
        {

            var cardDevice = _userCardService.FindUserByCardNumber(cardNumber);
            //var rowcount = cardDevice.Count;

            //var rowcount = cardDevice.Count;

            //Deletes User From Device
            if (cardDevice != null) return;
            //todo: for row number>2
            // var id = Convert.ToUInt32(cardDevice[rowcount - 2].Id);
            //var id = Convert.ToUInt32(cardDevice.Id);
            //// checks if this id is still on device , then delete from device, maybe user deleted before.

            //var code = (uint)_userService.GetUsers(userId: id).FirstOrDefault().Code;
            //if (!ExistOnDevice(code)) return;
            //var result = DeleteUser(code);
            ////var result = BSSDK.BS_DeleteUser(DeviceInfo.Handle, id);

            //if (result != true)
            //{
            //    Logger.Log($"Cannot delete user :{cardDevice.Id} which had have card number : {cardNumber}");
            //}
        }

        /// <summary>
        /// <En>Convert Security Level factor from DB form to devices form.</En>
        /// <Fa>مقدار سطح امنیت را از حالت ثبت شده در دیتابیس به حالت مورد پذیرش ساعت ها تبدیل می کند.</Fa>
        /// </summary>
        /// <param name="selector">مقدار سطح امنیت ثبت شده در دیتابیس</param>
        /// <returns></returns>
        public int GetSecurityLevelForDevice(int selector)
        {
            switch (selector)
            {
                case 260:
                    return 0;
                case 261:
                    return 1;
                case 262:
                    return 2;
                case 263:
                    return 3;
                case 264:
                    return 4;
                case 265:
                    return 5;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// <En>Get the user's access group from database.</En>
        /// <Fa>اطلاعات گروه دسترسی کاربر را از دیتابیس دریافت می کند.</Fa>
        /// </summary>
        /// <param name="userId">شماره کاربر</param>
        /// <returns></returns>
        public string GetAccessGroup(long userId)
        {

            var accessGroups = _accessGroupService.GetAccessGroups(userId);

            if (accessGroups.Count > 4 || accessGroups.Count < 0)
            {
                Logger.Log(
                    $@" The user {userId} does have {accessGroups.Count} access groups which is not compatible. The user will be transferred to device with full access.");
                throw new DataException(" The user {userId} does have {accessGroups.Count} access groups which is not compatible. The user will be transferred to device with full access.");
            }

            var accessGroupCount = accessGroups.Count;
            var userAccessGroup = new int[4];
            if (accessGroupCount < 4 && accessGroupCount > 0)
                for (var i = 0; i < accessGroupCount; i++)
                    userAccessGroup[i] = accessGroups[i].Id;

            var hexValue = new string[5];
            for (var i = 0; i < 4; i++)
            {
                if (userAccessGroup[i] == 0)
                    hexValue[i] = "ff";

                else if (userAccessGroup[i] < 10)
                    hexValue[i] = "0" + userAccessGroup[i];

                else
                {
                    hexValue[i] = userAccessGroup[i].ToString("X");
                }
            }

            hexValue[4] = "0x";
            var accessGroupMask = hexValue[4] + hexValue[3] + hexValue[2] + hexValue[1] + hexValue[0];
            return accessGroupMask;
        }


        /// <summary>
        /// <En>Delete user from device.</En>
        /// <Fa>کاربر را از دستکاه حذف می کند.</Fa>
        /// </summary>
        /// <param name="userId">شماره کاربر</param>
        /// <returns></returns>
        public virtual bool DeleteUser(uint userId)
        {
            var result = 0;

            try
            {
                DeviceAccessSemaphore.WaitOne();
                result = BSSDK.BS_DeleteUser(DeviceInfo.Handle, userId);
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

            if (result == BSSDK.BS_SUCCESS || result == BSSDK.BS_ERR_NOT_FOUND)
            {
                Logger.Log("User (" + userId + ") deleted .");
                return true;
            }

            return false;
        }

        /// <summary>
        /// <En>Deletes an access group from a device.</En>
        /// <Fa>یک گروه دسترسی را از ساعت پاک می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public bool DeleteAccessGroup(int nAccessIdn)
        {
            var result = BSSDK.BS_DeleteAccessGroupEx(DeviceInfo.Handle, nAccessIdn);

            if (result != BSSDK.BS_SUCCESS && result != BSSDK.BS_ERR_NOT_FOUND) return false;
            Logger.Log("AccessGroup (" + nAccessIdn.ToString() + ") deleted .");
            return true;
        }

        /// <summary>
        /// <En>Deletes an time zone from a device.</En>
        /// <Fa>یک تایم زون را از ساعت پاک می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public bool DeleteTimeZone(int nTimeZone)
        {
            var result = BSSDK.BS_DeleteTimeScheduleEx(DeviceInfo.Handle, nTimeZone);

            if (result == BSSDK.BS_SUCCESS || result == BSSDK.BS_ERR_NOT_FOUND)
            {
                Logger.Log("Timezone (" + nTimeZone.ToString() + ") deleted .");
                return true;
            }

            return false;
        }

        /// <summary>
        /// <En>Deletes all time zones from a device.</En>
        /// <Fa>تمامی تایم زون ها را از ساعت پاک می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllTimeZones()
        {
            var result = BSSDK.BS_DeleteAllTimeScheduleEx(DeviceInfo.Handle);

            if (result == BSSDK.BS_SUCCESS || result == BSSDK.BS_ERR_NOT_FOUND)
            {
                Logger.Log($"All Timezones deleted from Device {DeviceInfo.Code}.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// <En>Deletes all accessgroups from a device.</En>
        /// <Fa>تمامی گروه های دسترسی را از ساعت پاک می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllAccessGroups()
        {
            var result = BSSDK.BS_DeleteAllAccessGroupEx(DeviceInfo.Handle);

            if (result == BSSDK.BS_SUCCESS || result == BSSDK.BS_ERR_NOT_FOUND)
            {
                Logger.Log($"All AccessGroups deleted from Device {DeviceInfo.Code}.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// <En>Receives all time zones from a device.</En>
        /// <Fa>تمامی تایم زون ها را از ساعت دریافت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        //public bool GetAllTimeZones(int timeZone)
        //{
        //    var numOfTime = 0;

        //    var timeList = new List<SupremaTimeZoneModel>();

        //    var timeInfo = Marshal.AllocHGlobal(numOfTime * Marshal.SizeOf(typeof(BSSDK.BSTimeScheduleEx)));

        //    var result = BSSDK.BS_GetAllTimeScheduleEx(DeviceInfo.Handle, ref numOfTime, timeInfo);

        //    if (result != BSSDK.BS_SUCCESS /*&& result != BSSDK.BS_ERR_NOT_FOUND*/)
        //    {
        //        Logger.Log("Cannot get timezones info");
        //        Marshal.FreeHGlobal(timeInfo);
        //        return false;
        //    }

        //    else
        //    {
        //        var timeZones = new BSSDK.BSTimeScheduleEx[numOfTime];

        //        for (var i = 0; i < numOfTime; i++)
        //        {
        //            timeZones[i] =
        //                (BSSDK.BSTimeScheduleEx)
        //                Marshal.PtrToStructure(
        //                    new IntPtr(timeInfo.ToInt32() + i * Marshal.SizeOf(typeof(BSSDK.BSTimeScheduleEx))),
        //                    typeof(BSSDK.BSTimeScheduleEx));
        //        }


        //        Marshal.FreeHGlobal(timeInfo);

        //        foreach (var time in timeZones)
        //        {
        //            if (time.scheduleID == timeZone)
        //            {
        //                timeList.Add(new SupremaTimeZoneModel
        //                {
        //                    ScheduleId = time.scheduleID
        //                });
        //            }
        //        }

        //        if (timeList.Count != 0)
        //            return true;

        //        return false;
        //    }
        //}

        public bool SetTime(int timeToSet)
        {
            //var timeInTicks = timeToSet.Ticks / 10000000 - new DateTime(1970, 1, 1).Ticks / 10000000;
            DeviceAccessSemaphore.WaitOne();
            var res = BSSDK.BS_SetTime(DeviceInfo.Handle, timeToSet);
            DeviceAccessSemaphore.Release();

            if (res == 0)
            {
                var correctedSentTime = new DateTime(1970, 1, 1).AddTicks((long)timeToSet * 10000000);
                var deviceTime = 0;
                BSSDK.BS_GetTime(DeviceInfo.Handle, ref deviceTime);
                var correctedDeviceTime = new DateTime(1970, 1, 1).AddTicks((long)deviceTime * 10000000);
                Logger.Log($"\nTime of device: {DeviceInfo.Code}, has been set to: {correctedSentTime}.\nNow the time on device is: {correctedDeviceTime}\n");
            }

            return res == BSSDK.BS_SUCCESS;
        }

        public bool LockDevice()
        {
            try
            {
                Logger.Log($" Locking the Device: {DeviceInfo.Code}.\n");
                var result = BSSDK.BS_Disable(DeviceInfo.Handle, 60);//disable the device
                if (result == BSSDK.BS_SUCCESS)
                {
                    Logger.Log($"Successfully Lock Device {DeviceInfo.Code}.");
                    return true;
                }
                if (result == BSSDK.BS_ERR_BUSY)
                {
                    Logger.Log($"Terminal is processing another command. UnSuccessfully Lock Device {DeviceInfo.Code}.");
                    return false;
                }

                return false;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $"Error On LockDevice {DeviceInfo.Code}");
                return false;
            }
        }

        public bool UnLockDevice()
        {
            try
            {
                Logger.Log($"UnLocking the Device: {DeviceInfo.Code}.\n");
                var result = BSSDK.BS_Enable(DeviceInfo.Handle);//enable the device
                if (result == BSSDK.BS_SUCCESS)
                {
                    Logger.Log($"Successfully UnLock Device {DeviceInfo.Code}.");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $"Error On UnLockDevice {DeviceInfo.Code}");
                return false;
            }
        }
    }
}
