using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeZone = Biovation.CommonClasses.Models.TimeZone;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiSendTimeZoneToTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private uint Code { get; }
        private int TimeZoneId { get; set; }
        private TimeZone TimeZoneObj { get; }

        private readonly Callbacks _callbacks;

        public VirdiSendTimeZoneToTerminal(uint code, int timeZoneId, VirdiServer virdiServer, Callbacks callbacks, TimeZoneService timeZoneService)
        {
            Code = code;

            TimeZoneId = timeZoneId;
            _callbacks = callbacks;
            TimeZoneObj = timeZoneService.GetTimeZoneById(timeZoneId);
            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = Code, Message = $"The device: {Code} is not connected." };
            }

            if (TimeZoneObj == null)
            {
                Logger.Log($"Time Zone {TimeZoneId} does not exist.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = Code, Message = $"Time Zone {TimeZoneId} does not exist." };

            }

            try
            {
                _callbacks.AccessControlData.InitData();

                foreach (var timeZoneDetail in TimeZoneObj.Details)
                {
                    _callbacks.AccessControlData.SetTimeZone(timeZoneDetail.Id.ToString("D4"), timeZoneDetail.DayNumber,
                                timeZoneDetail.FromTime.Hours, timeZoneDetail.FromTime.Minutes, timeZoneDetail.ToTime.Hours, timeZoneDetail.ToTime.Minutes);
                }

                //_callbacks.AccessControlData.SetAuthProperty("0001", 1, 1, 0, 0, 1, 0, 0, 0);
                //_callbacks.AccessControlData.SetTimeZoneToAuthProperty("0001", 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                //_callbackInstance.AccessControlData.SetHoliday("1000", 0, 1, 1);
                _callbacks.AccessControlData.SetAccessTime(TimeZoneObj.Id.ToString("D4"),
                    TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 1)?.Id.ToString("D4"),
                    TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 2)?.Id.ToString("D4"),
                    TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 3)?.Id.ToString("D4"),
                    TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 4)?.Id.ToString("D4"),
                    TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 5)?.Id.ToString("D4"),
                    TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 6)?.Id.ToString("D4"),
                    TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 0)?.Id.ToString("D4"), "", "");

                _callbacks.AccessControlData.SetAccessGroup(TimeZoneObj.Id.ToString("D4"), 0, TimeZoneObj.Id.ToString("D4"));

                // 0: TimeZone information
                _callbacks.AccessControlData.SetAccessControlDataToTerminal(0, (int)Code, 0);
                // 1: Holiday information
                //_callbacks.AccessControlData.SetAccessControlDataToTerminal(0, terminalID, 1);
                // 2: AccessTime information
                _callbacks.AccessControlData.SetAccessControlDataToTerminal(0, (int)Code, 2);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Send time zone to terminal";
        }

        public string GetDescription()
        {
            return $"Sending time zone: {TimeZoneId} to device: {Code}.";
        }
    }
}
