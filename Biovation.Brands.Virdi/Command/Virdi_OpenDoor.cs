using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiOpenDoor : ICommand
    {
        private int DeviceId { get; }
        private uint Code { get; }
        private int TaskItemId { get; }
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private readonly VirdiServer _virdiServer;

        public VirdiOpenDoor(IReadOnlyList<object> items, VirdiServer virdiServer, DeviceService deviceService)
        {
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;

            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"Opendoor command,The device: {Code} is not connected.");
                //return false;
                return new ResultViewModel { Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 0, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };

            }

            try
            {
                _virdiServer.UcsApi.OpenDoorToTerminal(TaskItemId, (int)Code);
                // return true;
                return new ResultViewModel { Id = DeviceId, Message = $"Opendoor command,Error free", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };

            }
            catch (Exception)
            {
                //return false;
                return new ResultViewModel { Id = DeviceId, Message = $"Opendoor command,Error ", Validate = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode) };

            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            throw new NotImplementedException();
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
