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
using System.Threading;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiUpgradeDeviceFirmware : ICommand
    {
        private int DeviceId { get; }
        private int DeviceCode { get; }
        private string FirmwareFilePath { get; }
        private int TaskItemId { get; }

        private readonly VirdiServer _virdiServer;

        public VirdiUpgradeDeviceFirmware(IReadOnlyList<object> items, VirdiServer virdiServer, TaskService taskService, DeviceService deviceService)
        {
            _virdiServer = virdiServer;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            DeviceCode = (int)(deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
            var taskItem = taskService.GetTaskItem(TaskItemId);
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
                VirdiServer.UpgradeFirmwareTaskFinished = true;
                VirdiServer.UploadFirmwareFileTaskFinished = true;

                DeviceBasicInfo deviceInfo = null;

                try
                {
                    deviceInfo = _virdiServer.GetOnlineDevices()[(uint)DeviceCode];
                }
                catch (Exception)
                {
                    //ignore
                }

                _virdiServer.UcsApi.UpgradeFirmwareToTerminal(TaskItemId, DeviceCode, FirmwareFilePath);
                Thread.Sleep(1000);

                Logger.Log(GetDescription());

                while (!VirdiServer.UploadFirmwareFileTaskFinished)
                    Thread.Sleep(2000);

                for (var i = 0; i < 4; i++)
                {
                    if (VirdiServer.UpgradeFirmwareTaskFinished)
                        break;
                    Thread.Sleep(500);
                }

                if (VirdiServer.UpgradeFirmwareTaskFinished)
                {
                    return new ResultViewModel
                    {

                        Id = DeviceId,
                        Code = Convert.ToInt64(TaskStatuses.DoneCode),
                        Validate = VirdiServer.UpgradeFirmwareTaskFinished ? 1 : 0,
                        Message = VirdiServer.UpgradeFirmwareTaskFinished
                            ? $"Firmware upgraded successfully on device {DeviceCode}"
                            : $"Uploaded firmware file was not compatible with the device: Device code: {DeviceCode}, Device Model: {deviceInfo?.Model?.Name}"
                    };
                }
                return new ResultViewModel
                {

                    Id = DeviceId,
                    Code = Convert.ToInt64(TaskStatuses.FailedCode),
                    Validate = VirdiServer.UpgradeFirmwareTaskFinished ? 1 : 0,
                    Message = VirdiServer.UpgradeFirmwareTaskFinished
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
                VirdiServer.UpgradeFirmwareTaskFinished = true;
                VirdiServer.UploadFirmwareFileTaskFinished = true;
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
