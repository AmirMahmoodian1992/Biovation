using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiRetrieveUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int TaskItemId { get; }
        private int DeviceId { get; }
        private int UserId { get; }
        private uint Code { get; }

        private readonly Callbacks _callbacks;

        public VirdiRetrieveUserFromTerminal(IReadOnlyList<object> items, VirdiServer virdiServer, Callbacks callbacks, TaskService taskService, DeviceService deviceService)
        {
            _callbacks = callbacks;
            DeviceId = Convert.ToInt32(items[0]);

            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;


            TaskItemId = Convert.ToInt32(items[1]);
            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data["userId"];
            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"RetriveUser,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 1 };
            }

            try
            {
                Callbacks.ModifyUserData = true;
                _callbacks.TerminalUserData.GetUserDataFromTerminal(TaskItemId, (int)Code, UserId);

                Logger.Log(GetDescription());

                if (_callbacks.TerminalUserData.ErrorCode == 0)
                {
                    Logger.Log($"  +User {UserId} successfully retrieved from device: {Code}.\n");

                    return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = DeviceId, Message = $"  +User {UserId} successfully retrieved from device: {Code}.\n", Validate = 1 };

                }

                Logger.Log($"  +Cannot retrieve user {Code} from device: {Code}. Error code = {_callbacks.TerminalUserData.ErrorCode}\n");

                Callbacks.ModifyUserData = false;

                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot retrieve user {Code} from device: {Code}. Error code = {_callbacks.TerminalUserData.ErrorCode}\n", Validate = 0 };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"Exeption: {exception}", Validate = 0 };

            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Retrieve user from terminal";
        }

        public string GetDescription()
        {
            return $"Retrieving user: {UserId} from device: {Code}.";
        }
    }
}
