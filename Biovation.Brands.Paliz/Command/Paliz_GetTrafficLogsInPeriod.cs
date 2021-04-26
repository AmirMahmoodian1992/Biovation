using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Biovation.CommonClasses.Interface;
using Biovation.Brands.Paliz.Manager;
using PalizTiara.Api.CallBacks;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetTrafficLogsInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private readonly LogEvents _logEvents;
        private readonly PalizCodeMappings _palizCodeMappings;
        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private long StartDate { get; }
        private long EndDate { get; }
        private uint Code { get; }
        private int UserId { get; }

        private readonly PalizServer _palizServer;

        public PalizGetTrafficLogsInPeriod(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService, LogEvents logEvents, PalizCodeMappings palizCodeMappings)
        {
            if (items.Count == 4)
            {
                TerminalName = Convert.ToString(items[0]);
                TerminalId = Convert.ToInt32(items[1]);
                StartDate = long.Parse(items[2].ToString());
                EndDate = long.Parse(items[3].ToString());
            }
            else
            {
                // TODO - Do something or delete this block.
            }

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
                return new List<User>();
            }

            try
            {
                var request = new LogRequestModel
                {
                    UserId = 0,
                    Page = 1,
                    StartDate = StartDate,
                    EndDate = EndDate
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
        private async void FailTrafficLogEventCallBack(object sender, LogRequestEventArgs args)
        {
            if (args.Result == false)
            {
                return;
            }

            var logList = new List<Log>();
            foreach (var log in args.TrafficLogModel.Logs)
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
                Page = ++args.TrafficLogModel.Page
            };
            await _palizServer._serverManager.GetFailLogAsyncTask(TerminalName, request);
        }
        private async void TrafficLogEventCallBack(object sender, LogRequestEventArgs args)
        {
            if (args.Result == false)
            {
                return;
            }

            var logList = new List<Log>();
            foreach (var log in args.TrafficLogModel.Logs)
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
                Page = ++args.TrafficLogModel.Page
            };
            await _palizServer._serverManager.GetTrafficLogAsyncTask(TerminalName, request);
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
