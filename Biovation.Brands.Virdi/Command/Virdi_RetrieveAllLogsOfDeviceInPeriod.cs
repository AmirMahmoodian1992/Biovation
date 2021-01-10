﻿using Biovation.Brands.Virdi.UniComAPI;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiRetrieveAllLogsOfDeviceInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int TaskItemId { get; }
        private int DeviceId { get; }
        private DeviceBasicInfo Device { get; }
        private DateTime FromDate { get; }
        private DateTime ToDate { get; }

        private readonly Callbacks _callbacks;

        public VirdiRetrieveAllLogsOfDeviceInPeriod(IReadOnlyList<object> items, VirdiServer virdiServer, Callbacks callbacks, TaskService taskService, DeviceService deviceService)
        {
            _callbacks = callbacks;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Device = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId);


            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);

            FromDate = Convert.ToDateTime(data["fromDate"]);
            ToDate = Convert.ToDateTime(data["toDate"]);
            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Device?.Code))
            {
                Logger.Log($"RetrieveLogInPeriod,The device: {Device?.Code} is not connected.");
                return new ResultViewModel { Id = DeviceId, Message = $"The device: {Device?.Code} is not connected.", Validate = 0, Code = 10006 };
            }

            try
            {
                //Callbacks.GetLogTaskFinished = true;
                //Callbacks.RetrieveLogs = new List<Log>();

                //try
                //{
                //    Callbacks.SetLogPeriodSemaphore.WaitOne(240000);
                //}
                //catch (Exception exception)
                //{
                //    Logger.Log(exception);
                //}

                _callbacks.AccessLogPeriodFromDateTime = FromDate;
                _callbacks.AccessLogPeriodToDateTime = ToDate;

                if (FromDate == default || ToDate == default || FromDate == new DateTime(1970, 1, 1))
                {
                    _callbacks.GetAccessLogType = (int)VirdiDeviceLogType.All;
                    _callbacks.AccessLogData.GetAccessLogCountFromTerminal(TaskItemId, (int)(Device?.Code ?? 0), (int)VirdiDeviceLogType.All);
                }
                else
                {
                    _callbacks.GetAccessLogType = (int)VirdiDeviceLogType.Period;
                    _callbacks.AccessLogData.SetPeriod(FromDate.Year, FromDate.Month, FromDate.Day, ToDate.Year, ToDate.Month, ToDate.Day);
                    _callbacks.AccessLogData.GetAccessLogCountFromTerminal(TaskItemId, (int)(Device?.Code ?? 0), (int)VirdiDeviceLogType.Period);
                }
                //_callbacks.AccessLogData.GetAccessLogFromTerminal(0, (int)Code, (int)VirdiDeviceLogType.Period);
                //System.Threading.Thread.Sleep(1000);
                Logger.Log(GetDescription());

                Logger.Log($" +Retrieving logs from device: {Device?.Code} started successfully.");

                //while (!Callbacks.GetLogTaskFinished)
                //{
                //    System.Threading.Thread.Sleep(3000);
                //}
                //var result = Callbacks.RetrieveLogs;
                //var count = result.Count;
                //Task.Run(() =>
                //{
                //    _logService.AddLog(result);
                //});

                //Callbacks.RetrieveLogs = new List<Log>();

                return Callbacks.GetLogTaskFinished ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = DeviceId, Message = 0.ToString(), Validate = 1 } : new ResultViewModel { Id = DeviceId, Message = 0.ToString(), Validate = 1, Code = Convert.ToInt64(TaskStatuses.InProgressCode) };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Id = DeviceId, Message = "0", Validate = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get all logs of a device in a period of time command";
        }

        public string GetDescription()
        {
            return $"Getting all logs of a device (id: {Device?.Code} from: {FromDate} To: {ToDate}) command";
        }
    }
}
