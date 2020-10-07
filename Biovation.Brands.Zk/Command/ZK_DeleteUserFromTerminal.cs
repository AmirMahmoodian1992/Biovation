using Biovation.Brands.ZK.Devices;
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

namespace Biovation.Brands.ZK.Command
{
    public class ZkDeleteUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private uint UserId { get; }
        private int TaskItemId { get; }

        private readonly LogService _logService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;

        public ZkDeleteUserFromTerminal(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService, LogService logService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes, TaskService taskService)
        {
            OnlineDevices = devices;
            _logService = logService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = (deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (uint)(data["userId"]);
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"The device: {Code} is not connected.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            try
            {
                Logger.Log("-->Delete user from terminal");

                var device = OnlineDevices.FirstOrDefault(x => x.Key == Code).Value;
                var result = device.DeleteUser(UserId);
                if (result)
                {
                    Logger.Log($"  +User {UserId} successfuly deleted from device: {Code}.\n");
                    var log = new Log
                    {
                        DeviceId = DeviceId,
                        LogDateTime = DateTime.Now,
                        //EventLog = Event.USERREMOVEDFROMDEVICE,
                        EventLog = _logEvents.RemoveUserFromDevice,
                        UserId = UserId,
                        MatchingType = _matchingTypes.Finger,
                        SubEvent = _logSubEvents.Normal,
                        TnaEvent = 0,
                        SuccessTransfer = true
                    };

                    _logService.AddLog(log);

                    return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"+User {UserId} successfuly deleted from device: {Code}.\n", Code = Convert.ToInt64(TaskStatuses.DoneCode) };

                }
                Logger.Log($"  +Cannot delete user {UserId} from device: {Code} \n");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"  +Cannot delete user {UserId} from device: {Code} \n", Code = Convert.ToInt64(TaskStatuses.FailedCode) };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = exception.Message, Code = Convert.ToInt64(TaskStatuses.FailedCode) };

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
