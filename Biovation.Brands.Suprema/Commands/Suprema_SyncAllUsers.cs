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
    public class SupremaSyncAllUsers : ICommand
    {
        private readonly object _lockObject = new object();

        private readonly AccessGroupService _accessGroupService = new AccessGroupService();
        private readonly DeviceService _deviceService = new DeviceService();
        private readonly UserService _userService = new UserService();

        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        public SupremaSyncAllUsers(Dictionary<uint, Device> devices)
        {
            OnlineDevices = devices;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            //var forceUpdate = new ForceUpdateService();
            var userService = new UserService();
            var allUserEvents = userService.GetUsers(getTemplatesData: false).Result;
            //forceUpdate.DeleteAllEvents(ConnectionType);

            #region sync users

            var userCheckingTasks = new List<Task>();
            var usersOnDevices = new List<User>();

            foreach (var device in OnlineDevices)
            {
                userCheckingTasks.Add(Task.Run(() =>
                {
                    var usersOnDevice = device.Value.GetAllUsers();

                    lock (_lockObject)
                    {
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
                    }
                }));
            }

            Task.WaitAll(userCheckingTasks.ToArray());

            foreach (var item in usersOnDevices)
            {
                if (allUserEvents.SingleOrDefault(x => x.Id == item.Id) == null)
                {
                    allUserEvents.Add(item);
                }
            }

            #endregion

            foreach (var Event in allUserEvents)
            {
                var user = _userService.GetUser(userCode:Event.Id, withPicture:false);
                var accessGroupService = new AccessGroupService();
                var userAccess = accessGroupService.GetAccessGroupsOfUser(user.Id);

                var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
                var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
                var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

                //var deviceService = new DeviceServices();

                //var validDevice = deviceService.GetUserValidDevices(user.Id, ConnectionType);

                var validDevice = new List<DeviceBasicInfo>();
                var accessGroups = _accessGroupService.GetAccessGroupsOfUser(Event.Id);
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

                var offlineEventService = new OfflineEventService();
                var offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

                if (fullAccess != null)
                {
                    foreach (var device in offlineCheckerDevices)
                        offlineEventService.AddOfflineEvent(new OfflineEvent
                        {
                            DeviceCode = device.Code,
                            Data = user.Id.ToString(),
                            Type = OfflineEventType.UserInserted
                        });
                }

                else if (noAccess != null)
                {
                    foreach (var device in offlineCheckerDevices)
                        offlineEventService.AddOfflineEvent(new OfflineEvent
                        {
                            DeviceCode = device.Code,
                            Data = user.Id.ToString(),
                            Type = OfflineEventType.UserInserted
                        });
                }

                else if (disable != null)
                {
                    foreach (var device in offlineCheckerDevices)
                        offlineEventService.AddOfflineEvent(new OfflineEvent
                        {
                            DeviceCode = device.Code,
                            Data = user.Id.ToString(),
                            Type = OfflineEventType.UserInserted
                        });
                }

                else
                {
                    if (validDevice.Count == 0)
                    {
                        if (user.Id != 0)
                        {
                            foreach (var device in offlineCheckerDevices)
                                offlineEventService.AddOfflineEvent(new OfflineEvent
                                {
                                    DeviceCode = device.Code,
                                    Data = user.Id.ToString(),
                                    Type = OfflineEventType.UserDeleted
                                });
                        }
                    }

                    else
                    {
                        offlineCheckerDevices = validDevice;

                        foreach (var device in offlineCheckerDevices)
                            offlineEventService.AddOfflineEvent(new OfflineEvent
                            {
                                DeviceCode = device.Code,
                                Data = user.Id.ToString(),
                                Type = OfflineEventType.UserInserted
                            });
                    }
                }
            }

            foreach (var Event in allUserEvents)
            {
                var userE = _userService.GetUser(userCode: Event.Id, withPicture: false);
                var accessGroupService = new AccessGroupService();
                var userAccess = accessGroupService.GetAccessGroupsOfUser(userE.Id);

                var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
                var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
                var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

                //var validDevice = deviceService.GetUserValidDevices(Event.Id, ConnectionType);
                var validDevice = new List<DeviceBasicInfo>();
                var accessGroups = _accessGroupService.GetAccessGroupsOfUser(userE.Id);
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
                            //var addUserToTerminalCommand = CommandFactory.Factory(CommandType.SendUserToDevice,
                            //    new List<object> { deviceGroupMember.DeviceId, user.Id });
                            validDevice.Add(deviceGroupMember);
                            //addUserToTerminalCommand.Execute();
                        }
                    }
                }

                var offlineEventService = new OfflineEventService();
                //var offlineCheckerDevices = deviceService.GetAllDevices(ConnectionType);

                #region manageOfflineDevices

                //if (fullAccess != null)
                //{
                //    foreach (var device in offlineCheckerDevices)
                //        offlineEventService.InsertUserOfflineEvent(Convert.ToInt32(device.DeviceId), Event.NUserIdn,
                //            SupremaOfflineUserEventModel.OfflineUserInserted, ConnectionType);
                //}

                //else if (noAccess != null)
                //{
                //    foreach (var device in offlineCheckerDevices)
                //        offlineEventService.InsertUserOfflineEvent(Convert.ToInt32(device.DeviceId), Event.NUserIdn,
                //            SupremaOfflineUserEventModel.OfflineUserInserted, ConnectionType);
                //}

                //else if (disable != null)
                //{
                //    foreach (var device in offlineCheckerDevices)
                //        offlineEventService.InsertUserOfflineEvent(Convert.ToInt32(device.DeviceId), Event.NUserIdn,
                //            SupremaOfflineUserEventModel.OfflineUserDeleted, ConnectionType);
                //}

                //else
                //{
                //    if (validDevice.Count == 0)
                //    {
                //        if (Event.NUserIdn != 0)
                //        {
                //            foreach (var device in offlineCheckerDevices)
                //                offlineEventService.InsertUserOfflineEvent(Convert.ToInt32(device.DeviceId), Event.NUserIdn,
                //                    SupremaOfflineUserEventModel.OfflineUserDeleted, ConnectionType);
                //        }
                //    }

                //    else
                //    {
                //        offlineCheckerDevices = validDevice;

                //        foreach (var device in offlineCheckerDevices)
                //            offlineEventService.InsertUserOfflineEvent(Convert.ToInt32(device.DeviceId), Event.NUserIdn,
                //                SupremaOfflineUserEventModel.OfflineUserInserted, ConnectionType);
                //    }
                //}

                #endregion manageOfflineDevices


                #region transferUserToDevices

                var tasks = new List<Task>();

                foreach (var device in OnlineDevices)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        var user = _userService.GetUser(userCode:Event.Id, withPicture:false);

                        //var localDevice = tempDevice;
                        //var device = deviceFactory.Factory(tempDevice.Value, ConnectionType);

                        var deviceFind =
                            validDevice.FirstOrDefault(d => device.Key == d.DeviceId);

                        if (deviceFind != null)
                        {
                            if (device.Value.TransferUser(user))
                                offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                {
                                    DeviceCode = device.Value.GetDeviceInfo().Code,
                                    Data = user.Id.ToString(),
                                    Type = OfflineEventType.UserInserted
                                });

                            Logger.Log($"User {user.Id} transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                        }

                        else
                        {
                            if (fullAccess != null)
                            {
                                if (device.Value.TransferUser(user))
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserInserted
                                    });

                                Logger.Log(
                                    $"User {user.Id} with FullAccess, transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                            }
                            else if (noAccess != null)
                            {
                                if (device.Value.TransferUser(user))
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserInserted
                                    });

                                Logger.Log(
                                    $"User {user.Id} with NoAccess transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                            }
                            else if (disable != null)
                            {
                                if (device.Value.DeleteUser(Convert.ToUInt32(Event.Id)))
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserDeleted
                                    });

                                Logger.Log(
                                    $"User {user.Id} removed from device {device.Value.GetDeviceInfo().DeviceId} successfully, due disabled.");
                            }

                            else
                            {
                                var result = BSSDK.BS_DeleteUser(device.Value.GetDeviceInfo().Handle,
                                    Convert.ToUInt32(Event.Id));

                                if (result == 0)
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserDeleted
                                    });

                                Logger.Log($"User {user.Id} removed from device {device.Value.GetDeviceInfo().DeviceId} successfully, due no access group found.");
                            }
                        }
                    })
                    );
                }

                Task.WaitAll(tasks.ToArray());

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
            return "Sync all users of all devices command";
        }

        public string GetDescription()
        {
            return " Syncing all users of all devices command";
        }
    }
}