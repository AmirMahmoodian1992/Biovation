using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiDeleteUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private int UserId { get; }

        private int TaskItemId { get; }
        private readonly TaskService _taskService;


        private readonly Callbacks _callbacks;
        private readonly LogService _logService;
        private readonly DeviceService _deviceService;

        public VirdiDeleteUserFromTerminal(IReadOnlyList<object> items, VirdiServer virdiServer, Callbacks callbacks, TaskService taskService, LogService logService, DeviceService deviceService)
        {
            DeviceId = Convert.ToInt32(items[0]);

           
          
            TaskItemId = Convert.ToInt32(items[1]);
            var taskItem = _taskService.GetTaskItem(TaskItemId).Result;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);

            UserId = (int) data["userId"];
            Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.VirdiCode)?.Code ?? 0;
            OnlineDevices = virdiServer.GetOnlineDevices();
            
            _callbacks = callbacks;
            _logService = logService;
            _taskService = taskService;
            _deviceService = deviceService;
        }
        public object Execute()
        {


            if (OnlineDevices.All(device => device.Key != Code))
            {
               
                Logger.Log($"DeleteUser,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 1 };

            }


            try
            {
                _callbacks.TerminalUserData.DeleteUserFromTerminal(TaskItemId, (int)Code, UserId);

                Logger.Log("-->Delete user from terminal");

                Log log;

                if (_callbacks.TerminalUserData.ErrorCode == 0)
                {
                    Logger.Log($"  +User {UserId} successfuly deleted from device: {Code}.\n");

                    log = new Log
                    {
                        DeviceId = DeviceId,
                        LogDateTime = DateTime.Now,
                        //EventLog = Event.USERREMOVEDFROMDEVICE,
                        EventLog = LogEvents.RemoveUserFromDevie,
                        UserId = UserId,
                        MatchingType = MatchingTypes.Unknown,
                        SubEvent = LogSubEvents.Normal,
                        TnaEvent = 0,
                        SuccessTransfer = true
                    };

                    _logService.AddLog(log);

                    return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = DeviceId, Message = $"  +User {UserId} successfuly deleted from device: {Code}.\n", Validate = 1 };

                }

                Logger.Log($"  +Cannot delete user {UserId} from device: {Code}. Error code = {_callbacks.TerminalUserData.ErrorCode}\n");

                return new ResultViewModel { Code =Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot delete user {UserId} from device: {Code}. Error code = {_callbacks.TerminalUserData.ErrorCode}\n", Validate = 0 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot delete user {UserId} from device: {Code}. Error code = {_callbacks.TerminalUserData.ErrorCode}\n", Validate = 0 };

            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Delete user from terminal";
        }

        public string GetDescription()
        {
            return $"Deleting user: {UserId} from device: {DeviceId}.";
        }
    }
}
