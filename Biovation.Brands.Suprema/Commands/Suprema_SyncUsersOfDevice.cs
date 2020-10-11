using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class SupremaSyncUsersOfDevice : ICommand
    {
        private DeviceBasicInfo DeviceInfo { get; }
        //private readonly DeviceServices _deviceService = new DeviceServices();
        private readonly AccessGroupService _accessGroupService;
        private readonly DeviceService _deviceService;
        private readonly UserService _userService;
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> Devices { get; }

        public SupremaSyncUsersOfDevice(uint deviceId, Dictionary<uint, Device> devices, AccessGroupService accessGroupService, DeviceService deviceService, DeviceBrands deviceBrands, UserService userService)
        {
            DeviceInfo = _deviceService.GetDevices(deviceId, brandId:deviceBrands.Suprema.Code).FirstOrDefault();
            Devices = devices;
            _accessGroupService = accessGroupService;
            _deviceService = deviceService;
            _userService = userService;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            
            //var forceUpdate = new ForceUpdateService();
            var allUserEvents = _userService.GetUsers(getTemplatesData: false);
            //forceUpdate.DeleteAllEvents(ConnectionType);

            if (DeviceInfo == null)
            {
                Logger.Log("Wrong DeviceId. Device does not exists.");
                return false;
            }

            var targetDevice = Devices.FirstOrDefault(x => x.Key == DeviceInfo.DeviceId);

            if (targetDevice.Equals(default(KeyValuePair<uint, Device>)))
            {
                Logger.Log($"The device {DeviceInfo.DeviceId} is not connected to service now, Please try again.");
                return false;
            }

            var usersOnDevices = new List<User>();

            #region sync users

            var usersOnDevice = targetDevice.Value.GetAllUsers();

            if (usersOnDevice != null)
            {
                foreach (var userModel in usersOnDevice)
                {
                    if (usersOnDevices.All(x => x.Id != userModel.Id))
                    {
                        usersOnDevices.Add(userModel);
                    }
                }
            }

            foreach (var item in usersOnDevices)
            {
                if (allUserEvents.SingleOrDefault(x => x.Id == item.Id) == null)
                {
                    allUserEvents.Add(item);
                }
            }

            #endregion

            foreach (var @event in allUserEvents)
            {
                //todo:usercode
                var user = _userService.GetUsers( @event.Id).FirstOrDefault();

                var userAccess = _accessGroupService.GetAccessGroups(user.Id);

                var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
                var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
                var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

                //var validDevice = _deviceService.GetUserValidDevices(user.Id, ConnectionType);

                var validDevice = new List<DeviceBasicInfo>();
                var accessGroups = _accessGroupService.GetAccessGroups(user.Id);
                if (!accessGroups.Any())
                {
                    continue;
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
               /* var offlineEventService = new OfflineEventService();*/

                /*#region manageOfflineDevices*/

                /*if (fullAccess != null)
                {
                    offlineEventService.AddOfflineEvent(new OfflineEvent
                    {
                        DeviceCode = DeviceInfo.Code,
                        Data = user.Id.ToString(),
                        Type = OfflineEventType.UserInserted
                    });
                }

                else if (noAccess != null)
                {
                    offlineEventService.AddOfflineEvent(new OfflineEvent
                    {
                        DeviceCode = DeviceInfo.Code,
                        Data = user.Id.ToString(),
                        Type = OfflineEventType.UserInserted
                    });
                }

                else if (disable != null)
                {
                    offlineEventService.AddOfflineEvent(new OfflineEvent
                    {
                        DeviceCode = DeviceInfo.Code,
                        Data = user.Id.ToString(),
                        Type = OfflineEventType.UserDeleted
                    });
                }

                else
                {
                    if (validDevice.Count == 0)
                    {
                        if (user.Id != 0)
                        {
                            offlineEventService.AddOfflineEvent(new OfflineEvent
                            {
                                DeviceCode = DeviceInfo.Code,
                                Data = user.Id.ToString(),
                                Type = OfflineEventType.UserDeleted
                            });
                        }
                    }

                    else
                    {
                        if (validDevice.Any(x => x.DeviceId == DeviceInfo.DeviceId))
                            offlineEventService.AddOfflineEvent(new OfflineEvent
                            {
                                DeviceCode = DeviceInfo.Code,
                                Data = user.Id.ToString(),
                                Type = OfflineEventType.UserInserted
                            });
                        else
                        {
                            offlineEventService.AddOfflineEvent(new OfflineEvent
                            {
                                DeviceCode = DeviceInfo.Code,
                                Data = user.Id.ToString(),
                                Type = OfflineEventType.UserDeleted
                            });
                        }
                    }
                }

                #endregion manageOfflineDevices*/

                #region transferUserToDevices

                var deviceFind =
                    validDevice.FirstOrDefault(d => DeviceInfo.DeviceId == d.DeviceId);

                if (deviceFind != null)
                {
                    if (targetDevice.Value.TransferUser(user))
                    {
                        /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                        {
                            DeviceCode = targetDevice.Value.GetDeviceInfo().Code,
                            Data = user.Id.ToString(),
                            Type = OfflineEventType.UserInserted
                        });*/

                        Logger.Log($"User {user.Id} transferred to device {targetDevice.Value.GetDeviceInfo().DeviceId} successfully.");
                    }

                    else
                    {
                        Logger.Log($"User {user.Id} is added to offline events of device {targetDevice.Value.GetDeviceInfo().DeviceId}.");
                    }
                }

                else
                {
                    if (fullAccess != null)
                    {
                        if (targetDevice.Value.TransferUser(user))
                        {
                           /* offlineEventService.DeleteOfflineEvent(new OfflineEvent
                            {
                                DeviceCode = targetDevice.Value.GetDeviceInfo().Code,
                                Data = user.Id.ToString(),
                                Type = OfflineEventType.UserInserted
                            });*/

                            Logger.Log(
                                $"User {@event.Id} with FullAccess, transferred to device {targetDevice.Value.GetDeviceInfo().DeviceId} successfully.");
                        }

                        else
                        {
                            Logger.Log($"User {user.Id} is added to offline events of device {targetDevice.Value.GetDeviceInfo().DeviceId} with FullAccess.");
                        }
                    }
                    else if (noAccess != null)
                    {
                        if (targetDevice.Value.TransferUser(user))
                        {
                            /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                            {
                                DeviceCode = targetDevice.Value.GetDeviceInfo().Code,
                                Data = user.Id.ToString(),
                                Type = OfflineEventType.UserInserted
                            });*/

                            Logger.Log(
                                $"User {user.Id} with NoAccess transferred to device {targetDevice.Value.GetDeviceInfo().DeviceId} successfully.");
                        }

                        else
                        {
                            Logger.Log($"User {user.Id} is added to offline events of device {targetDevice.Value.GetDeviceInfo().DeviceId}.");
                        }
                    }
                    else if (disable != null)
                    {
                        if (targetDevice.Value.DeleteUser(Convert.ToUInt32(@event.Id)))
                        {
                            /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                            {
                                DeviceCode = targetDevice.Value.GetDeviceInfo().Code,
                                Data = user.Id.ToString(),
                                Type = OfflineEventType.UserDeleted
                            });*/

                            Logger.Log(
                                $"User {user.Id} removed from device {targetDevice.Value.GetDeviceInfo().DeviceId} successfully, due disabled.");
                        }

                        else
                        {
                            Logger.Log($"User {user.Id} is added to offline events to delete from device {targetDevice.Value.GetDeviceInfo().DeviceId}.");
                        }
                    }

                    else
                    {
                        if (user.Id != 0)
                        {
                            var result = targetDevice.Value.DeleteUser(Convert.ToUInt32(@event.Id));

                            if (result)
                            {
                                /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                {
                                    DeviceCode = targetDevice.Value.GetDeviceInfo().Code,
                                    Data = user.Id.ToString(),
                                    Type = OfflineEventType.UserDeleted
                                });*/

                                Logger.Log(
                                    $"User {user.Id} removed from device {targetDevice.Value.GetDeviceInfo().DeviceId} successfully, due no access group found.");
                            }
                        }

                        else
                        {
                            var result = targetDevice.Value.DeleteUser(Convert.ToUInt32(@event.Id));

                            if (result)
                            {
                                /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                {
                                    DeviceCode = targetDevice.Value.GetDeviceInfo().Code,
                                    Data = user.Id.ToString(),
                                    Type = OfflineEventType.UserDeleted
                                });*/

                                Logger.Log(
                                    $"User {user.Id} removed from device {targetDevice.Value.GetDeviceInfo().DeviceId} successfully, due no access group found.");
                            }
                        }
                    }
                }

                #endregion transferUserToDevices
            }

            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Sync all users of a device command";
        }

        public string GetDescription()
        {
            return "Syncing all users of a device (id: " + DeviceInfo.DeviceId + " IP: " + DeviceInfo.IpAddress + ") command";
        }
    }

}