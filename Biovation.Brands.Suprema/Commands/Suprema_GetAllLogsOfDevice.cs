using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Biovation.CommonClasses.Interface;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    /// <seealso cref="Command" />
    class SupremaGetAllLogsOfDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private uint DeviceId { get; }

        public SupremaGetAllLogsOfDevice(uint deviceId, Dictionary<uint, Device> devices)
        {
            DeviceId = deviceId;
            OnlineDevices = devices;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            if (OnlineDevices.ContainsKey(DeviceId))
            {
                //OnlineDevices[DeviceId].ReadOfflineLog(new CancellationToken());
                BioStarServer.LogReaderQueue.Enqueue(new KeyValuePair<uint, Task>(DeviceId, new Task(() => OnlineDevices[DeviceId].ReadOfflineLog(new CancellationToken()))));
                BioStarServer.StartReadLogs();
                return true;
            }
            else
            {
                Logger.Log($"Device: {DeviceId} is not connected.");
                return false;
            }
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
