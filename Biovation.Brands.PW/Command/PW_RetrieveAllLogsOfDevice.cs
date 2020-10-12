using Biovation.Brands.PW.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.PW.Command
{
    public class PwRetrieveAllLogsOfDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int Code { get; }
        private int DeviceId { get; }

        private int TaskItemId { get; }

        public PwRetrieveAllLogsOfDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService)
        {
            if (items.Count == 1)
            { DeviceId = Convert.ToInt32(items[0]); }
            else
            {
                DeviceId = Convert.ToInt32(items[0]);
                TaskItemId = Convert.ToInt32(items[1]);
            }

            Code = (int)(deviceService.GetDevices(code: (uint)DeviceId, brandId: DeviceBrands.ProcessingWorldCode)?.FirstOrDefault()?.Code ?? 0);
            OnlineDevices = devices;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");

                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"Device {Code} is disconnected", Validate = 0 };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                device.ReadOfflineLog(new object());

                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = DeviceId, Message = $@"تخلیه دستگاه {device.GetDeviceInfo().Code} شروع شد", Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            return false;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Read All Log of Device";
        }

        public string GetDescription()
        {
            return $"Read All Log of Device: {Code}.";
        }
    }
}
