﻿using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class SupremaSyncAccessGroups : ICommand
    {
  
        private readonly TimeZoneService _timeZoneService;

        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> Devices { get; }

        private readonly AccessGroupService _accessGroupService;
     
        public SupremaSyncAccessGroups(Dictionary<uint, Device> devices,AccessGroupService accessGroupService, TimeZoneService timeZoneService)
        {
            Devices = devices;
            _accessGroupService = accessGroupService;
            _timeZoneService = timeZoneService;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {


            var accessGroups = _accessGroupService.GetAccessGroups().Result?.Data?.Data;

            //var offlineAccessAndTimeEventService = new OfflineAccessAndTimeEventService();
            //var o fflineEventService = new OfflineEventService();
            // var timeZoneService = new TimeZoneService();

            var timeZones = _timeZoneService.GetTimeZones().Result?.Data?.Data;

            Task.WaitAll(Devices.Select(device => Task.Run(() =>
            {
                device.Value.DeleteAllTimeZones();
                device.Value.DeleteAllAccessGroups();
            })).ToArray());

            foreach (var time in timeZones)
            {
                #region manageOfflineDevices

             /*   var offlineCheckerDevices = _deviceService.GetDevices();*/

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

                #region transferTZToDevices

                var tasks = new List<Task>();

                foreach (var device in Devices)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        //var localDevice = device;
                        //var deviceFactory = new DeviceFactory();
                        //var device = deviceFactory.Factory(tempDevice.Value, ConnectionType);

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

            foreach (var accessGroup in accessGroups)
            {
                #region manageOfflineDevices

               /* var offlineCheckerDevices = _deviceService.GetDevices();*/

                /* foreach (var device in offlineCheckerDevices)
                 {
                     offlineEventService.AddOfflineEvent(new OfflineEvent
                     {
                         Data = accessGroup.Id.ToString(),
                         DeviceCode = device.Code,
                         Type = OfflineEventType.AccessGroupChanged
                     });
                 }*/

                //var accessGroupZoneService = new TimeZoneService();
                //var accessGroupZone = accessGroupZoneService.GetAccessGroupZone(group.GroupMaskId, ConnectionType);

                //if (accessGroupZone.Count != 0)
                //{
                //    var nTimeZone = accessGroupZone.FirstOrDefault();

                //    foreach (var device in offlineCheckerDevices)
                //    {
                //        offlineAccessAndTimeEventService.InsertAccessAndTimeOfflineEvent(nTimeZone.TimeCode, Convert.ToInt32(device.DeviceId),
                //            SupremaOfflineAccessAndTimeEventModel.OfflineTimeZone, ConnectionType);
                //    }
                //}

                #endregion

                #region transferAGToDevices

                var tasks = new List<Task>();

                foreach (var device in Devices)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        //var localDevice = tempDevice;
                        //var deviceFactory = new DeviceFactory();
                        //var device = deviceFactory.Factory(tempDevice.Value, ConnectionType);

                        //if (accessGroupZone.Count != 0)
                        //{
                        //    var nTimeZone = accessGroupZone.FirstOrDefault();

                        //    if (!listTime.Contains(nTimeZone.TimeCode))
                        //    {
                        //        listTime.Add(nTimeZone.TimeCode);

                        //        if (device.TransferTimeZone(nTimeZone.TimeCode))
                        //        {
                        //            Logger.Log("TimeZone {0} transferred to device {1} successfully.", nTimeZone.TimeCode, localDevice.Value.DeviceId);

                        //            offlineAccessAndTimeEventService.DeleteAccessAndTimeOfflineEvent(nTimeZone.TimeCode, Convert.ToInt32(tempDevice.Key)
                        //                , SupremaOfflineAccessAndTimeEventModel.OfflineTimeZone, ConnectionType);
                        //        }
                        //    }

                        //    else
                        //    {
                        //        offlineAccessAndTimeEventService.DeleteAccessAndTimeOfflineEvent(nTimeZone.TimeCode, Convert.ToInt32(tempDevice.Key)
                        //                , SupremaOfflineAccessAndTimeEventModel.OfflineTimeZone, ConnectionType);
                        //    }
                        //}

                        if (device.Value.TransferAccessGroup(accessGroup.Id))
                        {
                            Logger.Log($"AccessGroup {accessGroup.Id} transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");

                            /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                            {
                                Data = accessGroup.Id.ToString(),
                                DeviceCode = device.Value.GetDeviceInfo().Code,
                                Type = OfflineEventType.AccessGroupChanged
                            });*/
                        }
                    })
                                       );
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
            return "Sync all access groups with all devices command";
        }

        public string GetDescription()
        {
            return " Syncing all access groups with all devices command";
        }
    }
}