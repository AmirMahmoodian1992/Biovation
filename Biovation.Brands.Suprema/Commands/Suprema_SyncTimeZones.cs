using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class SupremaSyncTimeZone : ICommand
    {
     
        private readonly TimeZoneService _timeZoneService;

        /// <summary>
        /// All connected _onlineDevices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;

        public SupremaSyncTimeZone(Dictionary<uint, Device> onlineDevices,  TimeZoneService timeZoneService)
        {
            _onlineDevices = onlineDevices;
            _timeZoneService = timeZoneService;
        }

        /// <summary>
        /// <En>Handles the received event on _onlineDevices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {


            var timeZones = _timeZoneService.GetTimeZones().Result?.Data?.Data;

            //var offlineAccessAndTimeEventService = new OfflineAccessAndTimeEventService();
            //var offlineEventService = new OfflineEventService();

            var tasksDevice = new List<Task>();

            foreach (var device in _onlineDevices)
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

                //var offlineCheckerDevices = _deviceService.GetDevices();

                /* foreach (var device in offlineCheckerDevices)
                 {
                     offlineEventService.AddOfflineEvent(new OfflineEvent
                     {
                         Data = time.Id.ToString(),
                         DeviceCode = device.Code,
                         Type = OfflineEventType.TimeZoneChanged
                     });
                 }*/

                #endregion

                #region transferAGToDevices

                var tasks = new List<Task>();

                foreach (var device in _onlineDevices)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        if (device.Value.TransferTimeZone(time.Id))
                        {
                            Logger.Log($"Timezone {time.Id} transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");

                            /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                            {
                                Data = time.Id.ToString(),
                                DeviceCode = device.Value.GetDeviceInfo().Code,
                                Type = OfflineEventType.TimeZoneChanged
                            });*/
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
            return "Sync all time zones with all _onlineDevices command";
        }

        public string GetDescription()
        {
            return "Syncing all time zones with all _onlineDevices command";
        }
    }
}