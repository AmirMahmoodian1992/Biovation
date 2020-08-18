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
using System.Threading;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiUpgradeDeviceFirmware : ICommand
    {
        private int DeviceId { get; }
        private int DeviceCode { get; }
        private string FirmwareFilePath { get; }
        private int TaskItemId { get; }

        private readonly Callbacks _callbacks;
        private readonly VirdiServer _virdiServer;

        public VirdiUpgradeDeviceFirmware(IReadOnlyList<object> items, VirdiServer virdiServer, Callbacks callbacks, TaskService taskService, DeviceService deviceService)
        {
            _virdiServer = virdiServer;
            _callbacks = callbacks;


            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            DeviceCode = (int)(deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.VirdiCode)?.Code ?? 0);
            var taskItem = taskService.GetTaskItem(TaskItemId).Result;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            if (data.HasValues)
                FirmwareFilePath = data["LocalFileName"]?.ToString();
        }

        public object Execute()
        {
            if (_virdiServer.GetOnlineDevices().All(device => device.Key != DeviceCode))
            {
                Logger.Log($"The device: {DeviceCode} is not online.");
                return new ResultViewModel { Id = DeviceId, Message = $"The device: {DeviceCode} is not online.", Validate = 0, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            try
            {
                Callbacks.UpgradeFirmwareTaskFinished = true;
                Callbacks.UploadFirmwareFileTaskFinished = true;

                DeviceBasicInfo deviceInfo = null;

                try
                {
                    deviceInfo = _virdiServer.GetOnlineDevices()[(uint)DeviceCode];
                }
                catch (Exception)
                {
                    //ignore
                }

                _callbacks.UcsApi.UpgradeFirmwareToTerminal(TaskItemId, DeviceCode, FirmwareFilePath);
                Thread.Sleep(1000);

                Logger.Log(GetDescription());

                while (!Callbacks.UploadFirmwareFileTaskFinished)
                    Thread.Sleep(2000);

                for (var i = 0; i < 4; i++)
                {
                    if (Callbacks.UpgradeFirmwareTaskFinished)
                        break;
                    Thread.Sleep(500);
                }

                if (Callbacks.UpgradeFirmwareTaskFinished)
                {
                    return new ResultViewModel
                    {

                        Id = DeviceId,
                        Code = Convert.ToInt64(TaskStatuses.DoneCode),
                        Validate = Callbacks.UpgradeFirmwareTaskFinished ? 1 : 0,
                        Message = Callbacks.UpgradeFirmwareTaskFinished
                            ? $"Firmware upgraded successfully on device {DeviceCode}"
                            : $"Uploaded firmware file was not compatible with the device: Device code: {DeviceCode}, Device Model: {deviceInfo?.Model?.Name}"
                    };
                }
                return new ResultViewModel
                {

                    Id = DeviceId,
                    Code = Convert.ToInt64(TaskStatuses.FailedCode),
                    Validate = Callbacks.UpgradeFirmwareTaskFinished ? 1 : 0,
                    Message = Callbacks.UpgradeFirmwareTaskFinished
                        ? $"Firmware upgraded successfully on device {DeviceCode}"
                        : $"Uploaded firmware file was not compatible with the device: Device code: {DeviceCode}, Device Model: {deviceInfo?.Model?.Name}"
                };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Id = DeviceId, Message = "An error occured", Validate = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }
            finally
            {
                Callbacks.UpgradeFirmwareTaskFinished = true;
                Callbacks.UploadFirmwareFileTaskFinished = true;
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Upgrade device firmware";
        }

        public string GetDescription()
        {
            return $"Upgrading firmware of device {DeviceCode}.....";
        }
    }
}
