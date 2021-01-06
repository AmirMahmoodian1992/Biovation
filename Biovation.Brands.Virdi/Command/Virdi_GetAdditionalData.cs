using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiGetAdditionalData : ICommand
    {
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private readonly UCSAPI _ucsApi;
        private readonly ITerminalUserData _terminalUserData;
        private readonly AutoResetEvent _doneEvent;
        private int DeviceId { get; }
        private uint Code { get; }
        private int TaskItemId { get; }

        private Dictionary<string, string> InfoDictionary { get; set; }
        private int UserCount { get; set; }
        private int AdminCount { get; set; }

        public VirdiGetAdditionalData(IReadOnlyList<object> items, UCSAPI ucsApi, VirdiServer virdiServer, Callbacks callbacks, TaskService taskService, DeviceService deviceService)
        {
            _ucsApi = ucsApi;
            _terminalUserData = ucsApi.TerminalUserData as ITerminalUserData;

            _doneEvent = new AutoResetEvent(false);

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;

            OnlineDevices = virdiServer.GetOnlineDevices();

            InfoDictionary = new Dictionary<string, string>();
            UserCount = 0;
            AdminCount = 0;

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

                //Logger.Log(GetDescription());
                InfoDictionary.Add("AdminCount", AdminCount.ToString());
                InfoDictionary.Add("UserCount", UserCount.ToString());
                lock (_ucsApi)
                {
                    if (_terminalUserData.ErrorCode == 0)
                    {
                        Logger.Log($"  +Retrieving Infos from device: {Code} started successful.\n");
                        _doneEvent.WaitOne(1);

                        return new ResultViewModel<Dictionary<string, string>> { Data = InfoDictionary, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
                    }
                }

                lock (_ucsApi)
                {
                    Logger.Log($"  +Cannot retrieve Infos from device: {Code}. Error code = {_terminalUserData.ErrorCode}\n");
                }
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.FailedCode), Data = new List<User>(), Id = DeviceId, Message = "0", Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = "0", Validate = 0 };
            }
        }

        private void GetUserCount(int clientId, int terminalId, int adminNumber, int userNumber)
        {
            if (_terminalUserData.UserID == 0 || clientId != TaskItemId)
                return;
            lock (_ucsApi)
            {
                UserCount = userNumber;
                AdminCount = adminNumber;
            }
            if (_terminalUserData.CurrentIndex != _terminalUserData.TotalNumber) return;
            _doneEvent.Set();
            _ucsApi.EventGetUserCount += GetUserCount;
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
