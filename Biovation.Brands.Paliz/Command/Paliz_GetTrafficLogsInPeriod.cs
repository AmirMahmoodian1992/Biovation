﻿using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;
using Biovation.CommonClasses.Interface;
using Biovation.Brands.Paliz.Manager;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetTrafficLogsInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> onlineDevices { get; }
        private int taskItemId { get; }
        private string terminalName { get; }
        private int terminalId { get; }
        private uint code { get; }
        private DateTime fromDate { get; }
        private DateTime toDate { get; }
        private DeviceBasicInfo onlineDevice { get; set; }

        private LogEvents _logEvents;
        private PalizCodeMappings _palizCodeMappings;
        private readonly PalizServer _palizServer;
        private bool succeedLogsRetrieved;
        private bool failedLogsRetrieved;
        private int totalOfflineLogs;
        /// <summary>
        /// Paliz sdk has a limit of maximum number of 100 logs per page
        /// </summary>
        private const int MaxLogCountPerPage = 100;
        private static Queue<AutoResetEvent> autoResetEvents = new Queue<AutoResetEvent>();

        public PalizGetTrafficLogsInPeriod(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService, LogEvents logEvents, PalizCodeMappings palizCodeMappings)
        {
            terminalId = Convert.ToInt32(items[0]);
            taskItemId = Convert.ToInt32(items[1]);

            _palizCodeMappings = palizCodeMappings;
            _logEvents = logEvents;
            _palizServer = palizServer;
            var taskItem = taskService.GetTaskItem(taskItemId)?.GetAwaiter().GetResult().Data ?? new TaskItem();
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            fromDate = (DateTime)data["fromDate"];
            toDate = (DateTime)data["toDate"];

            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode).GetAwaiter().GetResult();
            if (devices is null)
            {
                onlineDevices = new Dictionary<uint, DeviceBasicInfo>();
                return;
            }

            code = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == terminalId)?.Code ?? 0;
            terminalName = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == terminalId)?.Name ?? string.Empty;
            onlineDevices = _palizServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (onlineDevices.All(device => device.Key != code))
            {
                Logger.Log($"RetriveAllLogsOfDeviceInPeriod,The device: {code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = terminalId, Message = $"The device: {code} is not connected.", Validate = 1 };
            }

            onlineDevice = onlineDevices.FirstOrDefault(device => device.Key == code).Value;

            try
            {
                var request = new LogRequestModel
                {
                    UserId = 0,
                    Page = 1,
                    StartDate = StaticHelpers.GetEpochTime(fromDate),
                    EndDate = StaticHelpers.GetEpochTime(toDate)
                };

                Logger.Log($" +Retrieving logs from device: {code} started successfully.\n");

                for (int i = 0; i < 2; i++)
                {
                    autoResetEvents.Enqueue(new AutoResetEvent(false));
                }

                _palizServer._serverManager.TrafficLogEvent += TrafficLogEventCallBack;
                _palizServer._serverManager.GetTrafficLogAsyncTask(terminalName, request).Wait();
                _palizServer._serverManager.FailLogEvent += FailTrafficLogEventCallBack;
                _palizServer._serverManager.GetFailLogAsyncTask(terminalName, request).Wait();

                //System.Threading.Thread.Sleep(100);
                //while (!succeedLogsRetrieved || !failedLogsRetrieved)
                //{
                //    System.Threading.Thread.Sleep(100);
                //}

                WaitHandle.WaitAll(autoResetEvents.ToArray());

                _palizServer._serverManager.TrafficLogEvent -= TrafficLogEventCallBack;
                _palizServer._serverManager.FailLogEvent -= FailTrafficLogEventCallBack;

                //Logger.Log(GetDescription());
                Logger.Log($"{totalOfflineLogs} Offline log retrieved from DeviceId: {code}.");

                PalizServer.GetLogInPeriodTaskFinished = true;
                return PalizServer.GetLogInPeriodTaskFinished
                    ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = terminalId, Message = 0.ToString(), Validate = 1 }
                    : new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = terminalId, Message = 0.ToString(), Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = terminalId, Message = "Error in command execute", Validate = 0 };
            }
        }

        private async void FailTrafficLogEventCallBack(object sender, LogRequestEventArgs logs)
        {
            //if (TerminalId != TaskItemId)
            //{
            //    return;
            //}

            if (logs.Result == false || logs.TrafficLogModel?.Logs is null)
            {
                lock (autoResetEvents)
                {
                    var waitHandle = autoResetEvents.Dequeue();
                    waitHandle.Set();
                }
                //failedLogsRetrieved = true;
                return;
            }

            var logList = new List<Log>();
            foreach (var log in logs.TrafficLogModel.Logs)
            {
                logList.Add(new Log
                {
                    DeviceId = onlineDevice.DeviceId,
                    DeviceCode = onlineDevice.Code,
                    EventLog = _logEvents.UnAuthorized,
                    UserId = -1,
                    //UserId = log.UserId,
                    //LogDateTime = new DateTime(log.Time),
                    DateTimeTicks = (ulong)(log.Time / 1000),
                    MatchingType = _palizCodeMappings.GetMatchingTypeGenericLookup(log.TrafficType),
                    //SubEvent = _palizCodeMappings.GetLogSubEventGenericLookup(AccessLogData.AuthMode),
                    PicByte = log.Image,
                    InOutMode = onlineDevice.DeviceTypeId
                });
            }

            await _palizServer._logService.AddLog(logList);
            totalOfflineLogs += logs.TrafficLogModel.Logs.Length;

            if (logs.TrafficLogModel.Logs.Length >= MaxLogCountPerPage)
            {
                var request = new LogRequestModel
                {
                    UserId = 0,
                    Page = ++logs.TrafficLogModel.Page,
                    StartDate = 0,
                    EndDate = 0
                };

                await _palizServer._serverManager.GetFailLogAsyncTask(terminalName, request);
            }
            else
            {
                lock (autoResetEvents)
                {
                    var waitHandle = autoResetEvents.Dequeue();
                    waitHandle.Set();
                }
                //failedLogsRetrieved = true;
            }
        }

        private async void TrafficLogEventCallBack(object sender, LogRequestEventArgs logs)
        {
            if (logs.Result == false || logs.TrafficLogModel?.Logs is null)
            {
                lock (autoResetEvents)
                {
                    var waitHandle = autoResetEvents.Dequeue();
                    waitHandle.Set();
                }
                //succeedLogsRetrieved = true;
                return;
            }

            var logList = new List<Log>();
            foreach (var log in logs.TrafficLogModel.Logs)
            {
                logList.Add(new Log
                {
                    DeviceId = onlineDevice.DeviceId,
                    DeviceCode = onlineDevice.Code,
                    EventLog = _logEvents.Authorized,
                    UserId = log.UserId,
                    DateTimeTicks = (ulong)(log.Time / 1000),
                    //LogDateTime = new DateTime(log.Time),
                    MatchingType = _palizCodeMappings.GetMatchingTypeGenericLookup(log.TrafficType),
                    //SubEvent = _palizCodeMappings.GetLogSubEventGenericLookup(AccessLogData.AuthMode),
                    PicByte = log.Image,
                    InOutMode = onlineDevice.DeviceTypeId
                });
            }

            await _palizServer._logService.AddLog(logList);
            totalOfflineLogs += logs.TrafficLogModel.Logs.Length;

            if (logs.TrafficLogModel.Logs.Length >= MaxLogCountPerPage)
            {
                var request = new LogRequestModel
                {
                    UserId = 0,
                    Page = ++logs.TrafficLogModel.Page,
                    StartDate = 0,
                    EndDate = 0
                };

                await _palizServer._serverManager.GetTrafficLogAsyncTask(terminalName, request);
            }
            else
            {
                lock (autoResetEvents)
                {
                    var waitHandle = autoResetEvents.Dequeue();
                    waitHandle.Set();
                }
                //succeedLogsRetrieved = true;
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get all logs of a device command";
        }

        public string GetDescription()
        {
            return "Getting all logs of a device and insert into database.";
        }
    }
}
