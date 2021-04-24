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

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetAllTrafficLogs : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private int UserId { get; }
        private uint Code { get; }

        private readonly PalizServer _palizServer;

        public PalizGetAllTrafficLogs(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService)
        {
            if (items.Count == 3)
            {
                TaskItemId = Convert.ToInt32(items[0]);
                TerminalName = Convert.ToString(items[1]);
                TerminalId = Convert.ToInt32(items[2]);
            }
            else
            {
                // TODO - Do something or delete this block.
            }

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

                _palizServer.NextLogPageNumber = 1;
                _palizServer._serverManager.TrafficLogEvent += TrafficLogEventCallBack;
                _palizServer._serverManager.GetTrafficLogAsyncTask(TerminalName, request);
                //_palizServer._serverManager.FailLogEvent += FailTrafficLogEventCallBack;
                //_palizServer._serverManager.GetFailLogAsyncTask(TerminalName, request);
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
        private void FailTrafficLogEventCallBack(object sender, LogRequestEventArgs args)
        {
            if(args.Result == false)
            {
                return;
            }

            var device = (DeviceSender)sender;
            var list = new List<TrafficLogModel>();
            var request = new LogRequestModel
            {
                UserId = 0,
                Page = ++args.TrafficLogModel.Page
            };
            _palizServer._serverManager.GetFailLogAsyncTask(TerminalName, request);
            //if (args.DeviceLogModel.Logs == null || args?.DeviceLogModel?.Logs?.Length < 1)
            //{
            //    return;
            //}

            //// TODO - Send logs.
            ////AddLog(device, args.DeviceLogModel.Logs);

            //var request = new DeviceLogRequestModel
            //{
            //    StartDate = args.DeviceLogModel.StartDate,
            //    EndDate = args.DeviceLogModel.EndDate,
            //    Page = ++NextLogPageNumber
            //};
            //await _serverManager.GetDeviceLogAsyncTask(device.TerminalName, request);
        }
        private void TrafficLogEventCallBack(object sender, LogRequestEventArgs args)
        {
            if (args.Result == false)
            {
                return;
            }

            var device = (DeviceSender)sender;
            var list = new List<TrafficLogModel>();
            var request = new LogRequestModel
            {
                UserId = 0,
                Page = ++args.TrafficLogModel.Page
            };
            _palizServer._serverManager.GetTrafficLogAsyncTask(TerminalName, request);
            //if (args.DeviceLogModel.Logs == null || args?.DeviceLogModel?.Logs?.Length < 1)
            //{
            //    return;
            //}

            //// TODO - Send logs.
            ////AddLog(device, args.DeviceLogModel.Logs);

            //var request = new DeviceLogRequestModel
            //{
            //    StartDate = args.DeviceLogModel.StartDate,
            //    EndDate = args.DeviceLogModel.EndDate,
            //    Page = ++NextLogPageNumber
            //};
            //await _serverManager.GetDeviceLogAsyncTask(device.TerminalName, request);
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
