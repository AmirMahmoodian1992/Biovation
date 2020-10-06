using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Interface;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    /// <seealso cref="Command" />
    public class SupremaSyncUser : ICommand
    {
        private User User { get; }
        private readonly UserService _userService = new UserService();
        private readonly AccessGroupService _accessGroupService = new AccessGroupService();
        private readonly DeviceService _deviceService = new DeviceService();
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        public SupremaSyncUser(int userId, Dictionary<uint, Device> onlineDevices)
        {
            User = _userService.GetUser(userCode:userId, withPicture:false);
            OnlineDevices = onlineDevices;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            var accessGroupService = new AccessGroupService();
            var userAccess = accessGroupService.GetAccessGroupsOfUser(User.Id);

            var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
            var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
            var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

            List<DeviceBasicInfo> offlineCheckerDevices;
            //var validDevice = deviceService.GetUserValidDevices(User.Id, ConnectionType);

            var validDevice = new List<DeviceBasicInfo>();
            var accessGroups = _accessGroupService.GetAccessGroupsOfUser(User.Id);
            if (!accessGroups.Any())
            {
                return true;
            }

            foreach (var accessGroup in accessGroups)
            {
                foreach (var deviceGroup in accessGroup.DeviceGroup)
                {
                    foreach (var deviceGroupMember in deviceGroup.Devices)
                    {
                        validDevice.Add(deviceGroupMember);
                    }
                }
            }
            var offlineEventService = new OfflineEventService();


            #region manageOfflineDevices

            if (fullAccess != null)
            {
                offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

                foreach (var device in offlineCheckerDevices)
                    offlineEventService.AddOfflineEvent(new OfflineEvent
                    {
                        DeviceCode = device.Code,
                        Data = User.Id.ToString(),
                        Type = OfflineEventType.UserInserted
                    });
            }

            else if (noAccess != null)
            {
                offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

                foreach (var device in offlineCheckerDevices)
                    offlineEventService.AddOfflineEvent(new OfflineEvent
                    {
                        DeviceCode = device.Code,
                        Data = User.Id.ToString(),
                        Type = OfflineEventType.UserInserted
                    });
            }

            else if (disable != null)
            {
                offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

                foreach (var device in offlineCheckerDevices)
                    offlineEventService.AddOfflineEvent(new OfflineEvent
                    {
                        DeviceCode = device.Code,
                        Data = User.Id.ToString(),
                        Type = OfflineEventType.UserDeleted
                    });
            }

            else
            {
                var allDevices = _deviceService.GetAllDevicesBasicInfos();
                offlineCheckerDevices = validDevice;

                foreach (var device in allDevices)
                {
                    offlineEventService.AddOfflineEvent(new OfflineEvent
                    {
                        DeviceCode = device.Code,
                        Data = User.Id.ToString(),
                        Type = offlineCheckerDevices.Any(offlineCheckerDevice => offlineCheckerDevice.DeviceId == device.DeviceId)
                            ? OfflineEventType.UserInserted
                            : OfflineEventType.UserDeleted
                    });
                }
            }

            #endregion manageOfflineDevices


            #region transferUserToDevices

            var tasks = new List<Task>();

            foreach (var device in OnlineDevices)
            {
                tasks.Add(Task.Run(() =>
                    {
                        var deviceFind =
                            validDevice.FirstOrDefault(d => device.Key == d.DeviceId);

                        if (deviceFind != null)
                        {
                            if (device.Value.TransferUser(User))
                            {
                                offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                {
                                    DeviceCode = device.Value.GetDeviceInfo().Code,
                                    Data = User.Id.ToString(),
                                    Type = OfflineEventType.UserInserted
                                });

                                Logger.Log($"User {User.Id} transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                            }
                        }

                        else
                        {
                            if (fullAccess != null)
                            {
                                if (device.Value.TransferUser(User))
                                {
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = User.Id.ToString(),
                                        Type = OfflineEventType.UserInserted
                                    });

                                    Logger.Log(
                                        $"User {User.Id} with FullAccess, transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                                }
                            }

                            else if (noAccess != null)
                            {
                                if (device.Value.TransferUser(User))
                                {
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = User.Id.ToString(),
                                        Type = OfflineEventType.UserInserted
                                    });

                                    Logger.Log(
                                        $"User {User.Id} with NoAccess transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                                }
                            }

                            else if (disable != null)
                            {
                                if (device.Value.DeleteUser(Convert.ToUInt32(User.Code)))
                                {
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = User.Id.ToString(),
                                        Type = OfflineEventType.UserDeleted
                                    });

                                    Logger.Log(
                                        $"User {User.Id} removed from device {device.Value.GetDeviceInfo().DeviceId} successfully, due disabled.");
                                }

                            }

                            else
                            {
                                var result = device.Value.DeleteUser(Convert.ToUInt32(User.Code));

                                if (result)
                                {
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = User.Id.ToString(),
                                        Type = OfflineEventType.UserDeleted
                                    });

                                    Logger.Log(
                                        $"User {User.Id} removed from device {device.Value.GetDeviceInfo().DeviceId} successfully, due no access group found.");
                                }
                            }
                        }
                    })
                );
            }

            Task.WaitAll(tasks.ToArray());

            #endregion transferUserToDevices

            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Sync a user with all devices command";
        }

        public string GetDescription()
        {
            return "Syncing User (id: " + User.Id + " name: " + User.UserName + ") command";
        }
    }
}