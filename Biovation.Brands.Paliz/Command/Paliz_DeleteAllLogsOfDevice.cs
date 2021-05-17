using Biovation.Brands.Paliz.UniComAPI;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;
using PalizTiara.Api.CallBacks;
using Biovation.Brands.Paliz.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizDeleteAllLogsOfDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        //private PalizCodeMappings _palizCodeMappings;
        //private LogEvents _logEvents;
        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private uint Code { get; }

        private readonly PalizServer _palizServer;

        public PalizDeleteAllLogsOfDevice(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService)
        {
            TerminalId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            //_palizCodeMappings = palizCodeMappings;
            //_logEvents = logEvents;
            var taskItem = taskService.GetTaskItem(TaskItemId)?.GetAwaiter().GetResult().Data ?? new TaskItem();
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            _palizServer = palizServer;
            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode).GetAwaiter().GetResult();
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
                var request = new DeviceLogRequestModel();
                _palizServer._serverManager.DeviceLogDeleteEvent += DeleteDeviceLogEventCallBack;
                _palizServer._serverManager.DeleteDeviceLogAsyncTask(TerminalName);
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
        private void DeleteDeviceLogEventCallBack(object sender, SetActionEventArgs acttion)
        {
            if (TerminalId != TaskItemId)
            {
                return;
            }

            if (acttion.Result == false)
            {
                return;
            }
            _palizServer._serverManager.DeviceLogDeleteEvent -= DeleteDeviceLogEventCallBack;
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
