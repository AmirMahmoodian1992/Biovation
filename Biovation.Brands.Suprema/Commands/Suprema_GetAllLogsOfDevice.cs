using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    class SupremaGetAllLogsOfDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;

        private readonly BioStarServer _bioStarServer;
        private readonly DeviceService _deviceService;

        private uint DeviceId { get; set; }
        private TaskItem TaskItem { get; }

        public SupremaGetAllLogsOfDevice(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, BioStarServer bioStarServer, DeviceService deviceService)
        {
            _onlineDevices = onlineDevices;
            _bioStarServer = bioStarServer;
            _deviceService = deviceService;
            TaskItem = taskItem;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item.{Environment.NewLine}", Validate = 0 };

            DeviceId = (uint)TaskItem.DeviceId;

            var device = _deviceService.GetDevice(DeviceId).Result?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!_onlineDevices.ContainsKey(device.Code))
            {
                Logger.Log($"The device: {device.DeviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }


            //_onlineDevices[DeviceId].ReadOfflineLog(new CancellationToken());
            _bioStarServer.LogReaderQueue.Enqueue(new KeyValuePair<uint, Task>(DeviceId, new Task(() => _onlineDevices[DeviceId].ReadOfflineLog(new CancellationToken()))));
            _bioStarServer.StartReadLogs();
            return true;

        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return " Get all logs of a device command";
        }

        public string GetDescription()
        {
            return " Getting all logs of a device (id: " + DeviceId + ") command";
        }
    }
}
