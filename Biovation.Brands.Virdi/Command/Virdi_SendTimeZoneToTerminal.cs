using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using UCSAPICOMLib;
using TimeZone = Biovation.Domain.TimeZone;

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

        private readonly IAccessControlData _accessControlData;

        public VirdiSendTimeZoneToTerminal(uint code, int timeZoneId, VirdiServer virdiServer, TimeZoneService timeZoneService, IAccessControlData accessControlData)
        {
            Code = code;
            TimeZoneId = timeZoneId;
            _accessControlData = accessControlData;
            TimeZoneObj = timeZoneService.TimeZones(timeZoneId).GetAwaiter().GetResult()?.Data;
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
                lock (_accessControlData)
                {

                    foreach (var timeZoneDetail in TimeZoneObj.Details)
                    {
                        _accessControlData.SetTimeZone(timeZoneDetail.Id.ToString("D4"), timeZoneDetail.DayNumber,
                            timeZoneDetail.FromTime.Hours, timeZoneDetail.FromTime.Minutes, timeZoneDetail.ToTime.Hours, timeZoneDetail.ToTime.Minutes);
                    }

                    _accessControlData.SetAccessTime(TimeZoneObj.Id.ToString("D4"),
                        TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 1)?.Id.ToString("D4") ?? "****",
                        TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 2)?.Id.ToString("D4") ?? "****",
                        TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 3)?.Id.ToString("D4") ?? "****",
                        TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 4)?.Id.ToString("D4") ?? "****",
                        TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 5)?.Id.ToString("D4") ?? "****",
                        TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 6)?.Id.ToString("D4") ?? "****",
                        TimeZoneObj.Details.FirstOrDefault(tz => tz.DayNumber == 0)?.Id.ToString("D4") ?? "****", "", "");

                    // 0: TimeZone information
                    _accessControlData.SetAccessControlDataToTerminal(0, (int)Code, 0);

                    //var accessGroups = _accessGroupService.GetAccessGroups(timeZoneId: TimeZoneId).GetAwaiter().GetResult()?.Data?.Data;
                    //if (accessGroups != null)
                    //    foreach (var accessGroup in accessGroups)
                    //    {
                    //        _virdiServer.AccessControlData.InitData();

                    //        foreach (var timeZoneDetail in TimeZoneObj.Details)
                    //        {
                    //            _virdiServer.AccessControlData.SetTimeZone(timeZoneDetail.Id.ToString("D4"),
                    //                timeZoneDetail.DayNumber,
                    //                timeZoneDetail.FromTime.Hours, timeZoneDetail.FromTime.Minutes,
                    //                timeZoneDetail.ToTime.Hours, timeZoneDetail.ToTime.Minutes);
                    //        }

                    //        //_callbacks.AccessControlData.SetAuthProperty("0001", 1, 1, 0, 0, 1, 0, 0, 0);
                    //        //_callbacks.AccessControlData.SetTimeZoneToAuthProperty("0001", 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    //        //_callbackInstance.AccessControlData.SetHoliday("1000", 0, 1, 1);

                    //        _virdiServer.AccessControlData.SetAccessGroup(accessGroup.Id.ToString("D4"), 0,
                    //            TimeZoneObj.Id.ToString("D4"));

                    //        // 1: Holiday information
                    //        //_callbacks.AccessControlData.SetAccessControlDataToTerminal(0, terminalID, 1);
                    //        // 2: AccessTime information
                    //        _virdiServer.AccessControlData.SetAccessControlDataToTerminal(0, (int) Code, 2);
                    //    }
                }
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
