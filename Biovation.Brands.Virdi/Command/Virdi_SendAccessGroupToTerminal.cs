﻿using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UCSAPICOMLib;
using AccessGroup = Biovation.Domain.AccessGroup;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiSendAccessGroupToTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private uint Code { get; }
        private int AccessGroupId { get; }
        private AccessGroup AccessGroupObj { get; }
        private int TaskItemId { get; }
        private int DeviceId { get; }

        private readonly IAccessControlData _accessControlData;

        public VirdiSendAccessGroupToTerminal(IReadOnlyList<object> items, VirdiServer virdiServer, TaskService taskService, DeviceService deviceService, AccessGroupService accessGroupService, IAccessControlData accessControlData)
        {
            _accessControlData = accessControlData;
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;

            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            AccessGroupId = (int)(data?["accessGroupId"] ?? -1);

            AccessGroupObj = accessGroupService.GetAccessGroup(AccessGroupId);
            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"SendAccessGroup,The device: {Code} is not connected.");
                return new ResultViewModel { Code = 10006, Id = DeviceId, Message = $"The device: {Code} is not connected." };
            }

            if (AccessGroupObj == null)
            {
                Logger.Log($"Access Group {AccessGroupId} does not exist.");
                return new ResultViewModel { Code = 10003, Id = DeviceId, Message = $"Access Group {AccessGroupId} does not exist." };
            }

            try
            {
                lock (_accessControlData)
                {
                    _accessControlData.InitData();

                    foreach (var timeZoneDetail in AccessGroupObj.TimeZone.Details)
                    {
                        _accessControlData.SetTimeZone(timeZoneDetail.Id.ToString("D4"), timeZoneDetail.DayNumber,
                                    timeZoneDetail.FromTime.Hours, timeZoneDetail.FromTime.Minutes, timeZoneDetail.ToTime.Hours, timeZoneDetail.ToTime.Minutes);
                    }

                    //_callbacks.AccessControlData.SetAuthProperty(AccessGroupObj.Id.ToString("D4"), 1, 1, 0, 0, 1, 0, 0, 0);

                    //_callbacks.AccessControlData.SetTimeZoneToAuthProperty(AccessGroupObj.Id.ToString("D4"), 
                    //    AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 2)?.Id ?? 0,
                    //    AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 3)?.Id ?? 0,
                    //    AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 4)?.Id ?? 0,
                    //    AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 5)?.Id ?? 0,
                    //    AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 6)?.Id ?? 0,
                    //    AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 7)?.Id ?? 0,
                    //    AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 1)?.Id ?? 0,
                    //    0, 0, 0, 0, 0, 0, 0, 0, 0);

                    //_callbackInstance.AccessControlData.SetHoliday("1000", 0, 1, 1);
                    _accessControlData.SetAccessTime(AccessGroupObj.TimeZone.Id.ToString("D4"),
                        AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 2)?.Id.ToString("D4") ?? "****",
                        AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 3)?.Id.ToString("D4") ?? "****",
                        AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 4)?.Id.ToString("D4") ?? "****",
                        AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 5)?.Id.ToString("D4") ?? "****",
                        AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 6)?.Id.ToString("D4") ?? "****",
                        AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 7)?.Id.ToString("D4") ?? "****",
                        AccessGroupObj.TimeZone.Details.FirstOrDefault(tz => tz.DayNumber == 1)?.Id.ToString("D4") ?? "****", "", "");

                    _accessControlData.SetAccessGroup(AccessGroupObj.Id.ToString("D4"), 0, AccessGroupObj.TimeZone.Id.ToString("D4"));

                    // 0: TimeZone information
                    _accessControlData.SetAccessControlDataToTerminal(TaskItemId, (int)Code, 0);
                    // 1: Holiday information
                    //_callbacks.AccessControlData.SetAccessControlDataToTerminal(0, terminalID, 1);
                    // 2: AccessTime information
                    _accessControlData.SetAccessControlDataToTerminal(TaskItemId, (int)Code, 2);
                    // 3: AccessGroup information
                    _accessControlData.SetAccessControlDataToTerminal(TaskItemId, (int)Code, 3);
                }

                Logger.Log($"The access group: {AccessGroupId} transferred to device: {Code} successfully.");
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            return new ResultViewModel { Code = 10003, Id = DeviceId, Message = $"The access group: {AccessGroupId} transferred to device: {Code} successfuly." };
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Send access group to terminal";
        }

        public string GetDescription()
        {
            return $"Sending access group: {AccessGroupId} to device: {Code}.";
        }
    }
}
