using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.Command
{
    public class ZkGetAdditionalData : ICommand
    {
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private readonly UCSAPI _ucsApi;
        private readonly ITerminalUserData _terminalUserData;
        private readonly ManualResetEventSlim _doneEvent;
        private int DeviceId { get; }
        private uint Code { get; }
        private int TaskItemId { get; }
        public ZkGetAdditionalData(IReadOnlyList<object> items, UCSAPI ucsApi, VirdiServer virdiServer, Callbacks callbacks, TaskService taskService, DeviceService deviceService)
        {
            _ucsApi = ucsApi;
            _terminalUserData = ucsApi.TerminalUserData as ITerminalUserData;

            _doneEvent = new ManualResetEventSlim(false);

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;

            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"RetrieveUser,The device: {Code} is not connected.");
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 0, Data = new List<User>() };
            }
            try
            {
                //Callbacks.GetUserTaskFinished = true;
                //Callbacks.RetrieveUsers = new List<User>();
                lock (_ucsApi)
                    _ucsApi.EventGetUserCount += GetUserCount;
                lock (_terminalUserData)
                    _terminalUserData.GetUserInfoListFromTerminal(TaskItemId, (int)Code);

                Logger.Log(GetDescription());
                if (_terminalUserData.ErrorCode == 0)
                {
                    Logger.Log($"  +Retrieving users from device: {Code} started successful.\n");
                    _doneEvent.Wait(TimeSpan.FromMinutes(2));

                    return new ResultViewModel<Dictionary<string,string>> { Data = {}, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
                }

                Logger.Log($"  +Cannot retrieve users from device: {Code}. Error code = {_terminalUserData.ErrorCode}\n");
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.FailedCode), Data = new List<User>(), Id = DeviceId, Message = "0", Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = "0", Validate = 0 };
            }
        }

        private void GetUserCount(int clientId, int terminalId, int AdminNumber, int UserNumber)
        {
            
        }

        public string GetDescription()
        {
            return $"GetAdditionalData from Device {Code}";
        }

        public string GetTitle()
        {
            return $"GetAdditionalData from Device {Code}";
        }

        public void Rollback()
        {
            
        }
    }
}
