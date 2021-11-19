using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiEnrollFaceFromTerminal : ICommand
    {
        private int DeviceId { get; }
        private int TaskItemId { get; }
        public Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private readonly DeviceService _deviceService;
        private readonly ITerminalUserData _terminalUserData;

        public VirdiEnrollFaceFromTerminal(IReadOnlyList<object> items, VirdiServer virdiServer, DeviceService deviceService, ITerminalUserData terminalUserData)
        {
            _deviceService = deviceService;
            _terminalUserData = terminalUserData;
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            var device = _deviceService.GetDevice(DeviceId);

            if (!OnlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };

            //_virdiServer._ucsApi.EnrollFromTerminal(0, DeviceId);
            lock (_terminalUserData)
            {
                _terminalUserData.RegistFaceFromTerminal(TaskItemId, (int)device.Code, 0);

                if (_terminalUserData.ErrorCode == 0)
                {
                    Logger.Log($"-->Terminal:{device.Code} Face Template enrollment started.");
                    return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  Enroll User face from device: {device.Code} started.", Validate = 1 };
                }

                Logger.Log($@"-->Terminal:{device.Code} Face Template enrollment failed.
                +ErrorCode : {_terminalUserData.ErrorCode:X4}{Environment.NewLine}");

                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  Enroll User face from device: {device.Code} failed. Error code = {_terminalUserData.ErrorCode}{Environment.NewLine}", Validate = 0 };
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
