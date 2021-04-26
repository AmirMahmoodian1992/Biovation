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
    public class PalizGetUserFromTerminal
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
        public PalizGetUserFromTerminal(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService, LogEvents logEvents, PalizCodeMappings palizCodeMappings)
        {
            TerminalId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);

            var taskItem = taskService.GetTaskItem(TaskItemId)?.Data ?? new TaskItem();
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data["userId"];
            try
            {
                var date = (DateTime)data["fromDate"];
                StartDate = date.Ticks;
            }
            catch (Exception)
            {
                StartDate = new DateTime(1970, 1, 1).Ticks;
            }
            try
            {
                var date = (DateTime)data["toDate"];
                EndDate = date.Ticks;
            }
            catch (Exception)
            {
                EndDate = DateTime.Now.AddYears(5).Ticks;
            }
            _palizCodeMappings = palizCodeMappings;
            _logEvents = logEvents;
            _palizServer = palizServer;
            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
            Code = devices?.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Code ?? 7;
            OnlineDevices = palizServer.GetOnlineDevices();
        }
        //public object Execute()
        //{
        //    if (OnlineDevices.All(device => device.Key != Code))
        //    {
        //        Logger.Log($"RetriveUser,The device: {Code} is not connected.");
        //        return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 1 };
        //    }

        //    try
        //    {
        //        //Callbacks.ModifyUserData = true;
        //        _palizServer._serverManager.UserInfoEvent += GetUserInfoEventCallBack;
        //        var userIdModel = new UserIdModel(UserId);
        //        _palizServer._serverManager.GetUserInfoAsyncTask(userIdModel, TerminalName);
        //        System.Threading.Thread.Sleep(1000);
        //        Logger.Log(GetDescription());


        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Log(exception);
        //        return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"Exeption: {exception}", Validate = 0 };

        //    }
        //}
        private void GetUserInfoEventCallBack(object sender, UserInfoEventArgs args)
        {
            if (TerminalId != TaskItemId)
            {
                return;
            }

            if (args.Result == false)
            {
                return;
            }


            _palizServer._serverManager.UserInfoEvent -= GetUserInfoEventCallBack;
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
