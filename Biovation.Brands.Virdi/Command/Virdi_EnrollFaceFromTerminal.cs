using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System;
using System.Collections.Generic;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models.ConstantValues;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiEnrollFaceFromTerminal : ICommand
    {
        private int DeviceId { get; }
        private int TaskItemId { get; }
        public Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private readonly Callbacks _callbacks;
        private readonly VirdiServer _virdiServer;
        private readonly DeviceService _deviceService;

        public VirdiEnrollFaceFromTerminal(IReadOnlyList<object> items, Callbacks callbacks, VirdiServer virdiServer, DeviceService deviceService)
        {
            _callbacks = callbacks;
            _virdiServer = virdiServer;
            _deviceService = deviceService;
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            OnlineDevices = _virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            var device = _deviceService.GetDeviceInfo(DeviceId);

            if (!OnlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };

            //_virdiServer._ucsApi.EnrollFromTerminal(0, DeviceId);

            _callbacks.TerminalUserData.RegistFaceFromTerminal(TaskItemId, (int)device.Code, 0);


            if (_virdiServer.UcsApi.ErrorCode == 0)
            {
                Logger.Log($"-->Terminal:{device.Code} Face Template enrollment started.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  Enroll User face from device: {device.Code} started.", Validate = 1 };
            }

            Logger.Log($@"-->Terminal:{device.Code} Face Template enrollment failed.
                +ErrorCode : {_virdiServer.UcsApi.ErrorCode:X4}{Environment.NewLine}");

            return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  Enroll User face from device: {device.Code} failed. Error code = {_virdiServer.UcsApi.ErrorCode}{Environment.NewLine}", Validate = 0 };
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
