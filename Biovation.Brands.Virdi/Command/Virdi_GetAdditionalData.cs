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
        private readonly ManualResetEventSlim _doneEventUserCount;
        private int DeviceId { get; }
        private uint Code { get; }
        private int TaskItemId { get; }

        private Dictionary<string, string> InfoDictionary { get; set; }
        private int UserCount { get; set; }
        private int AdminCount { get; set; }
        private DeviceBasicInfo deviceBasicInfo;

        public VirdiGetAdditionalData(IReadOnlyList<object> items, UCSAPI ucsApi, VirdiServer virdiServer, Callbacks callbacks, TaskService taskService, DeviceService deviceService)
        {
            _ucsApi = ucsApi;
            _terminalUserData = ucsApi.TerminalUserData as ITerminalUserData;

            _doneEventUserCount = new ManualResetEventSlim(false);

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            deviceBasicInfo = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId);
            Code = deviceBasicInfo?.Code ?? 0;
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
                return InfoDictionary;
            }
            try
            {
                //Callbacks.GetUserTaskFinished = true;
                //Callbacks.RetrieveUsers = new List<User>();
                lock (_ucsApi)
                    _ucsApi.EventGetUserCount += GetUserCount;
                lock (_terminalUserData)
                {
                    _terminalUserData.GetUserCountFromTerminal(TaskItemId, (int)Code);
                }
                //Logger.Log(GetDescription());
                lock (_terminalUserData)
                {

                    if (_terminalUserData.ErrorCode == 0)
                    {
                        Logger.Log($"  +Retrieving Infos from device: {Code} started successful.\n");
                        _doneEventUserCount.Wait(TimeSpan.FromSeconds(5));
                        _ucsApi.EventGetUserCount -= GetUserCount;
                    }
                    else
                    {
                        Logger.Log($"  +Cannot retrieve Infos from device: {Code}. Error code = {_terminalUserData.ErrorCode}\n");
                    }
                }
                _ucsApi.EventGetUserCount -= GetUserCount;


                if (deviceBasicInfo.FirmwareVersion != null)
                {
                    InfoDictionary.Add("Firmware Version", deviceBasicInfo.FirmwareVersion);
                }

                return InfoDictionary;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                _ucsApi.EventGetUserCount -= GetUserCount;
                return InfoDictionary;
            }
        }

        private void GetUserCount(int clientId, int terminalId, int adminNumber, int userNumber)
        {
            if (clientId != TaskItemId)
                return;

            UserCount = userNumber;
            AdminCount = adminNumber;
            InfoDictionary.Add("AdminCount", AdminCount.ToString());
            InfoDictionary.Add("UserCount", UserCount.ToString());
            _doneEventUserCount.Set();
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
