using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Models;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizSendTimeZoneToTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private uint Code { get; }
        private int UserId { get; }
        private readonly PalizServer _palizServer;
        private readonly TaskService _taskService;
        private int TimeZoneId { get; }
        private TimeZone TimeZoneObj { get; }
        private SetActionEventArgs _setTimeZoneResult;

        public PalizSendTimeZoneToTerminal(IReadOnlyList<object> items, TaskService taskService, PalizServer palizServer, TimeZoneService timeZoneService
            , DeviceService deviceService)
        {
            TerminalId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            _taskService = taskService;

            _palizServer = palizServer;
            var taskItem = taskService.GetTaskItem(TaskItemId)?.GetAwaiter().GetResult().Data ?? new TaskItem();
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode).GetAwaiter().GetResult();
            if (devices is null)
            {
                OnlineDevices = new Dictionary<uint, DeviceBasicInfo>();
                return;
            }
            if (data != null)
            {
                TimeZoneId = (int)data["timeZoneId"];
            }
            Code = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Code ?? 7;
            TimeZoneObj = timeZoneService.TimeZones(TimeZoneId).GetAwaiter().GetResult().Data;

            OnlineDevices = palizServer.GetOnlineDevices();

        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel
                {
                    Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode),
                    Id = Code,
                    Message = $"The device: {Code} is not connected.",
                    Validate = 1
                };
            }

            if (TimeZoneObj == null)
            {
                return new ResultViewModel
                {
                    Code = Convert.ToInt64(TaskStatuses.FailedCode),
                    Id = TerminalId,
                    Message = $"  +Cannot send time zone {TimeZoneId} to device: {Code}.\n",
                    Validate = 0
                };
            }

            try
            {
                _palizServer._serverManager.SetTimeZoneEvent += SetTimeZoneEventCallBack;

                Logger.Log(GetDescription());

                foreach (var timeZoneDetail in TimeZoneObj.Details)
                {
                    var fromDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, timeZoneDetail.DayNumber,
                        timeZoneDetail.FromTime.Hours, timeZoneDetail.FromTime.Minutes,
                        timeZoneDetail.FromTime.Seconds);

                    var toDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, timeZoneDetail.DayNumber,
                        timeZoneDetail.ToTime.Hours, timeZoneDetail.ToTime.Minutes,
                        timeZoneDetail.ToTime.Seconds);

                    var interval = fromDateTime - toDateTime;

                    var timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(timeZoneDetail.Id.ToString(), interval,
                        TimeZoneObj.Name, null);

                    _palizServer._serverManager.SetTimeZoneAsyncTask(timeZoneInfo, TerminalName);
                }

                System.Threading.Thread.Sleep(500);
                while (_setTimeZoneResult == null)
                {
                    System.Threading.Thread.Sleep(500);
                }

                _palizServer._serverManager.SetTimeZoneEvent -= SetTimeZoneEventCallBack;

                if (_setTimeZoneResult.Result)
                {
                    return new ResultViewModel
                    {
                        Code = Convert.ToInt64(TaskStatuses.DoneCode),
                        Id = TerminalId,
                        Message = $"  +Time zone {TimeZoneId} successfully set to device: {Code}.\n",
                        Validate = 1
                    };
                }

                return new ResultViewModel
                {
                    Code = Convert.ToInt64(TaskStatuses.FailedCode),
                    Id = TerminalId,
                    Message = $"  +Cannot send time zone {TimeZoneId} to device: {Code}.\n",
                    Validate = 0
                };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = TerminalId, Message = "Error in command execute", Validate = 0 };
            }
        }
        private void SetTimeZoneEventCallBack(object sender, SetActionEventArgs args)
        {
            //if (TerminalId != TaskItemId)
            //{
            //    return;
            //}
            _setTimeZoneResult = args;
            if (_setTimeZoneResult.Result == false)
            {
                Logger.Log($"  +Cannot set time zone for device: {Code}.\n");
                return;
            }
            Logger.Log($"  +Time zone successfully set for device: {Code}.\n");
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
