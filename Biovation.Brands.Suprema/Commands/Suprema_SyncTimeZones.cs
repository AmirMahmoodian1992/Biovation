using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Interface;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    /// <seealso cref="Command" />
    public class SupremaSyncTimeZone : ICommand
    {
        private readonly DeviceService _deviceService = new DeviceService();

        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> Devices { get; }

        public SupremaSyncTimeZone(Dictionary<uint, Device> devices)
        {
            Devices = devices;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            var timeZoneService = new TimeZoneService();

            var timeZones = timeZoneService.GetAllTimeZones();

            //var offlineAccessAndTimeEventService = new OfflineAccessAndTimeEventService();
            var offlineEventService = new OfflineEventService();

            var tasksDevice = new List<Task>();

            foreach (var device in Devices)
            {
                tasksDevice.Add(Task.Run(() =>
                {
                    device.Value.DeleteAllTimeZones();
                }));
            }

            Task.WaitAll(tasksDevice.ToArray());

            foreach (var time in timeZones)
            {
                #region manageOfflineDevices

                var offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

                foreach (var device in offlineCheckerDevices)
                {
                    offlineEventService.AddOfflineEvent(new OfflineEvent
                    {
                        Data = time.Id.ToString(),
                        DeviceCode = device.Code,
                        Type = OfflineEventType.TimeZoneChanged
                    });
                }

                #endregion

                #region transferAGToDevices

                var tasks = new List<Task>();

                foreach (var device in Devices)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        if (device.Value.TransferTimeZone(time.Id))
                        {
                            Logger.Log($"Timezone {time.Id} transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");

                            offlineEventService.DeleteOfflineEvent(new OfflineEvent
                            {
                                Data = time.Id.ToString(),
                                DeviceCode = device.Value.GetDeviceInfo().Code,
                                Type = OfflineEventType.TimeZoneChanged
                            });
                        }

                    }));
                }

                Task.WaitAll(tasks.ToArray());

                #endregion transferAGToDevices
            }

            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Sync all time zones with all devices command";
        }

        public string GetDescription()
        {
            return "Syncing all time zones with all devices command";
        }
    }
}