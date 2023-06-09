﻿using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;
using PalizTiara.Api.CallBacks;
using Biovation.CommonClasses.Interface;
using Biovation.Brands.Paliz.Manager;
using System.Threading;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetAllTrafficLogs : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> _onlineDevices { get; }
        //private int taskItemId { get; }
        private string _terminalName { get; }
        private int _terminalId { get; }
        private uint _code { get; }
        private DeviceBasicInfo _onlineDevice { get; set; }

        private LogEvents _logEvents;
        private PalizCodeMappings _palizCodeMappings;
        private readonly PalizServer _palizServer;
        private int _totalOfflineLogs;
        /// <summary>
        /// Paliz sdk has a limit of maximum number of 100 logs per page
        /// </summary>
        private const int MaxLogCountPerPage = 100;
        private static Queue<AutoResetEvent> _autoResetEvents = new Queue<AutoResetEvent>();

        public PalizGetAllTrafficLogs(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService,
            DeviceService deviceService, LogEvents logEvents, PalizCodeMappings palizCodeMappings)
        {
            _terminalId = Convert.ToInt32(items[0]);
            //taskItemId = Convert.ToInt32(items[1]);

            _palizCodeMappings = palizCodeMappings;
            _logEvents = logEvents;
            _palizServer = palizServer;
            //var taskItem = taskService.GetTaskItem(taskItemId)?.Data ?? new TaskItem();
            //var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            //UserId = (int)data["userId"];
            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode).GetAwaiter().GetResult();
            if (devices is null)
            {
                _onlineDevices = new Dictionary<uint, DeviceBasicInfo>();
                return;
            }

            _code = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == _terminalId)?.Code ?? 0;
            _terminalName = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == _terminalId)?.Name ?? string.Empty;
            _onlineDevices = _palizServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (_onlineDevices.All(device => device.Key != _code))
            {
                Logger.Log($"RetriveAllLogsOfDevice,The device: {_code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = _terminalId, Message = $"The device: {_code} is not connected.", Validate = 1 };
            }

            _onlineDevice = _onlineDevices.FirstOrDefault(device => device.Key == _code).Value;

            try
            {
                var request = new LogRequestModel
                {
                    UserId = 0,
                    Page = 1,
                    StartDate = 0,
                    EndDate = 0
                };

                Logger.Log($"Retrieving logs from device: {_code} started successfully.\n");

                for (int i = 0; i < 2; i++)
                {
                    _autoResetEvents.Enqueue(new AutoResetEvent(false));
                }

                _palizServer._serverManager.TrafficLogEvent += TrafficLogEventCallBack;
                _palizServer._serverManager.GetTrafficLogAsyncTask(_terminalName, request);
                _palizServer._serverManager.FailLogEvent += FailTrafficLogEventCallBack;
                _palizServer._serverManager.GetFailLogAsyncTask(_terminalName, request);

                //System.Threading.Thread.Sleep(1000);
                //while (!succeedLogsRetrieved || !failedLogsRetrieved)
                //{
                //    System.Threading.Thread.Sleep(1000);
                //}

                WaitHandle.WaitAll(_autoResetEvents.ToArray());

                _palizServer._serverManager.TrafficLogEvent -= TrafficLogEventCallBack;
                _palizServer._serverManager.FailLogEvent -= FailTrafficLogEventCallBack;

                //Logger.Log(GetDescription());
                Logger.Log($"{_totalOfflineLogs} Offline log retrieved from DeviceId: {_code}.");

                PalizServer.GetLogTaskFinished = true;
                return PalizServer.GetLogTaskFinished
                    ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = _terminalId, Message = 0.ToString(), Validate = 1 }
                    : new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = _terminalId, Message = 0.ToString(), Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = _terminalId, Message = "Error in command execute", Validate = 0 };
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
                lock (_autoResetEvents)
                {
                    var waitHandle = _autoResetEvents.Dequeue();
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
                    DeviceId = _onlineDevice.DeviceId,
                    DeviceCode = _onlineDevice.Code,
                    EventLog = _logEvents.UnAuthorized,
                    UserId = -1,
                    //UserId = log.UserId,
                    //LogDateTime = new DateTime(log.Time),
                    DateTimeTicks = (ulong)(log.Time / 1000),
                    MatchingType = _palizCodeMappings.GetMatchingTypeGenericLookup(log.TrafficType),
                    //SubEvent = _palizCodeMappings.GetLogSubEventGenericLookup(AccessLogData.AuthMode),
                    PicByte = log.Image,
                    InOutMode = _onlineDevice.DeviceTypeId
                });
            }

            await _palizServer._logService.AddLog(logList);
            _totalOfflineLogs += logs.TrafficLogModel.Logs.Length;

            if (logs.TrafficLogModel.Logs.Length >= MaxLogCountPerPage)
            {
                var request = new LogRequestModel
                {
                    UserId = 0,
                    Page = ++logs.TrafficLogModel.Page,
                    StartDate = 0,
                    EndDate = 0
                };

                await _palizServer._serverManager.GetFailLogAsyncTask(_terminalName, request);
            }
            else
            {
                lock (_autoResetEvents)
                {
                    var waitHandle = _autoResetEvents.Dequeue();
                    waitHandle.Set();
                }
                //failedLogsRetrieved = true;
            }
        }

        private async void TrafficLogEventCallBack(object sender, LogRequestEventArgs logs)
        {
            if (logs.Result == false || logs.TrafficLogModel?.Logs is null)
            {
                lock (_autoResetEvents)
                {
                    var waitHandle = _autoResetEvents.Dequeue();
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
                    DeviceId = _onlineDevice.DeviceId,
                    DeviceCode = _onlineDevice.Code,
                    EventLog = _logEvents.Authorized,
                    UserId = log.UserId,
                    DateTimeTicks = (ulong)(log.Time / 1000),
                    //LogDateTime = new DateTime(log.Time),
                    MatchingType = _palizCodeMappings.GetMatchingTypeGenericLookup(log.TrafficType),
                    //SubEvent = _palizCodeMappings.GetLogSubEventGenericLookup(AccessLogData.AuthMode),
                    PicByte = log.Image,
                    InOutMode = _onlineDevice.DeviceTypeId
                });
            }

            await _palizServer._logService.AddLog(logList);
            _totalOfflineLogs += logs.TrafficLogModel.Logs.Length;

            if (logs.TrafficLogModel.Logs.Length >= MaxLogCountPerPage)
            {
                var request = new LogRequestModel
                {
                    UserId = 0,
                    Page = ++logs.TrafficLogModel.Page,
                    StartDate = 0,
                    EndDate = 0
                };

                await _palizServer._serverManager.GetTrafficLogAsyncTask(_terminalName, request);
            }
            else
            {
                lock (_autoResetEvents)
                {
                    var waitHandle = _autoResetEvents.Dequeue();
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
