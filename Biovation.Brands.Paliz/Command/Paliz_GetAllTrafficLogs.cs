using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;
using PalizTiara.Api.CallBacks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Biovation.CommonClasses.Interface;
using Biovation.Brands.Paliz.Manager;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetAllTrafficLogs : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private LogEvents _logEvents;
        private PalizCodeMappings _palizCodeMappings;
        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private int UserId { get; }
        private uint Code { get; }//iaushdfjhsdgfjhsdgfujsdgfujs

        private readonly PalizServer _palizServer;

        public PalizGetAllTrafficLogs(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService, LogEvents logEvents, PalizCodeMappings palizCodeMappings)
        {
            TerminalId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);

            _palizCodeMappings = palizCodeMappings;
            _logEvents = logEvents;
            _palizServer = palizServer;
            var taskItem = taskService.GetTaskItem(TaskItemId)?.Data ?? new TaskItem();
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data["userId"];
            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
            Code = devices?.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Code ?? 7;
            OnlineDevices = palizServer.GetOnlineDevices();
        }


        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"RetriveAllLogsOfDevice,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = TerminalId, Message = $"The device: {Code} is not connected.", Validate = 1 };
            }

            try
            {
                var request = new LogRequestModel
                {
                    UserId = 0,
                    Page = 1
                };

                _palizServer._serverManager.TrafficLogEvent += TrafficLogEventCallBack;
                _palizServer._serverManager.GetTrafficLogAsyncTask(TerminalName, request);
                _palizServer._serverManager.FailLogEvent += FailTrafficLogEventCallBack;
                _palizServer._serverManager.GetFailLogAsyncTask(TerminalName, request);
                System.Threading.Thread.Sleep(1000);

                Logger.Log(GetDescription());

                Logger.Log($" +Retrieving logs from device: {Code} started successfully.\n");
                PalizServer.GetLogTaskFinished = true;
                return PalizServer.GetLogTaskFinished 
                    ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = TerminalId, Message = 0.ToString(), Validate = 1 }
                    : new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = TerminalId, Message = 0.ToString(), Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = TerminalId, Message = "Error in command execute", Validate = 0 };
            }
        }
        private async void FailTrafficLogEventCallBack(object sender, LogRequestEventArgs logs)
        {
            if (TerminalId != TaskItemId)
            {
                return;
            }

            if (logs.Result == false)
            {
                return;
            }

            var logList = new List<Log>();
            foreach(var log in logs.TrafficLogModel.Logs)
            {
                logList.Add(new Log
                {
                    DeviceCode = (uint)TerminalId,
                    EventLog = _logEvents.UnAuthorized,
                    UserId = log.UserId,
                    LogDateTime = new DateTime(log.Time),
                    MatchingType = _palizCodeMappings.GetMatchingTypeGenericLookup(log.TrafficType),
                    //SubEvent = _palizCodeMappings.GetLogSubEventGenericLookup(AccessLogData.AuthMode),
                    PicByte = log.Image,
                });
            }

            await _palizServer._logService.AddLog(logList);

            var request = new LogRequestModel
            {
                UserId = 0,
                Page = ++logs.TrafficLogModel.Page
            };
            await _palizServer._serverManager.GetFailLogAsyncTask(TerminalName, request);

            _palizServer._serverManager.FailLogEvent -= FailTrafficLogEventCallBack;
        }
        private async void TrafficLogEventCallBack(object sender, LogRequestEventArgs logs)
        {
            if (logs.Result == false)
            {
                return;
            }

            var logList = new List<Log>();
            foreach (var log in logs.TrafficLogModel.Logs)
            {
                logList.Add(new Log
                {
                    DeviceCode = (uint)TerminalId,
                    EventLog = _logEvents.Authorized,
                    UserId = log.UserId,
                    LogDateTime = new DateTime(log.Time),
                    MatchingType = _palizCodeMappings.GetMatchingTypeGenericLookup(log.TrafficType),
                    //SubEvent = _palizCodeMappings.GetLogSubEventGenericLookup(AccessLogData.AuthMode),
                    PicByte = log.Image,
                });
            }

            await _palizServer._logService.AddLog(logList);

            var request = new LogRequestModel
            {
                UserId = 0,
                Page = ++logs.TrafficLogModel.Page
            };
            await _palizServer._serverManager.GetTrafficLogAsyncTask(TerminalName, request);

            _palizServer._serverManager.TrafficLogEvent -= TrafficLogEventCallBack;
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
