﻿using Biovation.Brands.EOS.Manager;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.EOS.Devices
{
    /// <summary>
    /// کلاس پدر ساعت ها که تمامی انواع ساعت ها از آن ارث بری می کنند.
    /// </summary>
    /// <seealso cref="IDevices" />
    public class Device : IDevices
    {
        protected CancellationToken Token;

        //protected Clock _clock;
        protected readonly DeviceBasicInfo DeviceInfo;
        protected CancellationToken ServiceCancellationToken;
        //public Semaphore DeviceAccessSemaphore;
        protected bool Valid;

        protected int TotalLogCount;
        //private readonly LogService _commonLogService = new LogService();

        // private readonly EosServer _eosServer;

        protected readonly LogEvents LogEvents;
        protected readonly LogSubEvents LogSubEvents;
        protected readonly EosCodeMappings EosCodeMappings;

        internal Device(DeviceBasicInfo deviceInfo, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings)
        {
            Valid = true;
            DeviceInfo = deviceInfo;
            //  _eosServer = eosServer;
            LogEvents = logEvents;
            LogSubEvents = logSubEvents;
            EosCodeMappings = eosCodeMappings;
            TotalLogCount = 0;
        }
        public virtual bool Connect(CancellationToken cancellationToken)
        {
            /*   var isConnect = IsConnected();
               if (!isConnect) return false;

               try
               {
                   if (_deviceInfo.TimeSync)
                       _clock.SetDateTime(DateTime.Now);
               }
               catch (Exception exception)
               {
                   Logger.Log(exception);
                   Task.Delay(TimeSpan.FromMilliseconds(500), ServiceCancellationToken).Wait(ServiceCancellationToken);
                   try
                   {
                       if (_deviceInfo.TimeSync)
                           _clock.SetDateTime(DateTime.Now);
                   }
                   catch (Exception innerException)
                   {
                       Logger.Log(innerException);
                   }
               }

               //_valid = true;
               Task.Run(() => { ReadOnlineLog(Token); }, Token);
               return true;*/
            return true;
        }

        internal virtual User GetUser(uint userId)
        {
            throw new NotImplementedException();
        }

        public virtual List<User> GetAllUsers(bool embedTemplate = false)
        {

            var usersList = new List<User>();


            return usersList;
        }
        public virtual bool Disconnect()
        {
            Valid = false;
            //_clock?.Disconnect();
            //_clock?.Dispose();
            return true;
        }

        public DeviceBasicInfo GetDeviceInfo()
        {
            return DeviceInfo;
        }


        /// <summary>
        /// <En>Add new device to database.</En>
        /// <Fa>دستگاه جدید را در دیتابیس ثبت می کند.</Fa>
        /// </summary>
        /// <returns></returns>
        public int AddDeviceToDataBase()
        {
            return 0;
        }

        public virtual ResultViewModel ReadOfflineLog(object token, bool fileSave = false)
        {
            return new ResultViewModel { Id = DeviceInfo.DeviceId, Validate = 0, Message = "0" };
        }

        /// <summary>
        /// <En>Delete user from device.</En>
        /// <Fa>کاربر را از دستکاه حذف می کند.</Fa>
        /// </summary>
        /// <param name="sUserId">شماره کاربر</param>
        /// <returns></returns>
        public virtual bool DeleteUser(uint sUserId)
        {
            return false;

        }

        public virtual bool TransferUser(User user)
        {
            return true;
        }

        public virtual ResultViewModel ReadOfflineLogInPeriod(object cancellationToken, DateTime? startTime, DateTime? endTime, bool saveFile = false)
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<string, string> GetAdditionalData(int code)
        {
            throw new NotImplementedException();
        }
    }
}
