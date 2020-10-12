using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
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

        private uint DeviceId { get; }

        public SupremaGetAllLogsOfDevice(uint deviceId, Dictionary<uint, Device> onlineDevices, BioStarServer bioStarServer)
        {
            DeviceId = deviceId;
            _onlineDevices = onlineDevices;
            _bioStarServer = bioStarServer;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            if (_onlineDevices.ContainsKey(DeviceId))
            {
                //_onlineDevices[DeviceId].ReadOfflineLog(new CancellationToken());
                _bioStarServer.LogReaderQueue.Enqueue(new KeyValuePair<uint, Task>(DeviceId, new Task(() => _onlineDevices[DeviceId].ReadOfflineLog(new CancellationToken()))));
                _bioStarServer.StartReadLogs();
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
