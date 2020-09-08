using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Service;
using Biovation.Service.SQL.v1;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiLockDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private uint Code { get; }
        private int TaskItemId { get; }
        private int DeviceId { get; }
        private readonly VirdiServer _virdiServer;

        public VirdiLockDevice(IReadOnlyList<object> items, VirdiServer virdiServer, DeviceService deviceService)
        {
            _virdiServer = virdiServer;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.VirdiCode)?.Code ?? 0;

            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(x => x.Key != Code))
                return new ResultViewModel
                { Id = DeviceId, Message = "Error in lock device execute,device Code not found", Validate = 0, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            _virdiServer.UcsApi.SendTerminalControl(TaskItemId, (int)Code, 1, 1); // Shutdown
            Logger.Log("");
            Logger.Log($"-->Terminal:{Code} Locked(Shutdown)");
            Logger.Log("   +ErrorCode :" + _virdiServer.UcsApi.ErrorCode.ToString("X4") + "\n");
            return _virdiServer.UcsApi.ErrorCode == 0 ? new ResultViewModel { Id = DeviceId, Message = "Error free", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) } : new ResultViewModel { Id = DeviceId, Message = "Error ", Validate = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode) };

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
