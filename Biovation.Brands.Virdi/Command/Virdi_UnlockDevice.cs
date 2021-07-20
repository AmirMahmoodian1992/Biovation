using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiUnlockDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private uint Code { get; }
        private int TaskItemId { get; }
        private int DeviceId { get; }

        private readonly UCSAPI _ucsApi;
        public VirdiUnlockDevice(IReadOnlyList<object> items, VirdiServer virdiServer, DeviceService deviceService, UCSAPI ucsApi)
        {
            _ucsApi = ucsApi;

            OnlineDevices = virdiServer.GetOnlineDevices();
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            var deviceObject = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId);
            Code = deviceObject?.Code ?? 0;
        }

        public object Execute()
        {
            if (OnlineDevices.All(w => w.Key != Code))
                return new ResultViewModel
                { Id = DeviceId, Message = "Error ", Validate = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode) };

            lock (_ucsApi)
            {
                _ucsApi.SendTerminalControl(TaskItemId, (int)Code, 0, 1); // Release
                Logger.Log("");
                Logger.Log("   +ErrorCode :" + _ucsApi.ErrorCode.ToString("X4") + "\n");
                return _ucsApi.ErrorCode == 0 ? new ResultViewModel { Id = DeviceId, Message = "Error free", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) } : new ResultViewModel { Id = DeviceId, Message = $"Error { _ucsApi.ErrorCode} ", Validate = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Lock device";
        }

        public string GetDescription()
        {
            return "Locking given device.";
        }
    }
}
