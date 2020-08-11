using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;

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

        private readonly VirdiServer _virdiServer;
        public VirdiUnlockDevice(IReadOnlyList<object> items, VirdiServer virdiServer, DeviceService deviceService)
        {
            _virdiServer = virdiServer;

            OnlineDevices = virdiServer.GetOnlineDevices();
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            var deviceObject = deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.VirdiCode);
            Code = deviceObject.Code;
        }

        public object Execute()
        {
            if (OnlineDevices.Any(w => w.Key == Code))
            {
                _virdiServer.UcsApi.SendTerminalControl(TaskItemId, (int)Code, 0, 1); // Release
                Logger.Log("");
                Logger.Log("   +ErrorCode :" + _virdiServer.UcsApi.ErrorCode.ToString("X4") + "\n");
                if (_virdiServer.UcsApi.ErrorCode == 0)
                {
                    return new ResultViewModel { Id = DeviceId, Message = "Error free", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode)};
                }
                else
                {
                    return new ResultViewModel { Id = DeviceId, Message = $"Error { _virdiServer.UcsApi.ErrorCode} ", Validate = 0, Code =Convert.ToInt64(TaskStatuses.FailedCode) };

                }
                //return virdiServer.UcsApi.ErrorCode;
            }

            return new ResultViewModel { Id = DeviceId, Message = "Error ", Validate = 0,Code=Convert.ToInt64(TaskStatuses.FailedCode) };

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
