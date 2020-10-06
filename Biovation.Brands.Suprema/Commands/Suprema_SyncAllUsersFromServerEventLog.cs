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
    public class SupremaSyncAllUsersFromServerEventLog : ICommand
    {
        //private readonly object _object = new object();
        private readonly DeviceService _deviceService = new DeviceService();
        private readonly AccessGroupService _accessGroupService = new AccessGroupService();

        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        public SupremaSyncAllUsersFromServerEventLog(Dictionary<uint, Device> devices)
        {
            OnlineDevices = devices;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            var userService = new UserService();

            //var forceUpdate = new ServerEventLogForceUpdateService();
            var allUserEvents = userService.GetUsers(getTemplatesData: false).Result;
            //forceUpdate.DeleteAllEvents(ConnectionType);


            #region sync users

            //List<Task> userCheckingTasks = new List<Task>();
            //var deviceFactory = new DeviceFactory();
            /*
            var usersOnDevices = new List<SupremaUserModel>();

            foreach (var device in devices)
            {
                userCheckingTasks.Add(Task.Run(() =>
                {
                    var dev = deviceFactory.Factory(device.Value, ConnectionType);
                    var usersOnDevice = dev.GetAllUsers();

                    lock (_object)
                    {
                        if (usersOnDevice != null)
                        {
                            foreach (var userModel in usersOnDevice)
                            {
                                if (!usersOnDevices.Any(x => x.SUserId == userModel.SUserId))
                                {
                                    usersOnDevices.Add(userModel);
                                }
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(userCheckingTasks.ToArray());

            ////var allusers = allUserEvents;

            foreach (var item in usersOnDevices)
            {
                if (allUserEvents.Where(x => x.SUserId == item.SUserId).SingleOrDefault() == null)
                {
                    allUserEvents.Add(item);
                }
            } 
            */
            #endregion

            foreach (var Event in allUserEvents)
            {
                var user = userService.GetUser(userCode:Event.Id, withPicture:false);
                //if (Event.SUserId != null)
                //{
                var accessGroupService = new AccessGroupService();
                var userAccess = accessGroupService.GetAccessGroupsOfUser(user.Id);

                var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
                var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
                var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

                List<DeviceBasicInfo> offlineCheckerDevices;

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

                if (fullAccess != null)
                {
                    offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

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
                    offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

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
                    offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

                    foreach (var device in offlineCheckerDevices)
                        offlineEventService.AddOfflineEvent(new OfflineEvent
                        {
                            DeviceCode = device.Code,
                            Data = user.Id.ToString(),
                            Type = OfflineEventType.UserDeleted
                        });
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
                    if (validDevice.Count == 0)
                    {
                        //var user = new UserServices();
                        //var userWithSUserId = user.GetUserBySUserId(Convert.ToInt32(Event.SUserId), ConnectionType).FirstOrDefault();
                        if (user.Id != 0)
                        {
                            offlineCheckerDevices = _deviceService.GetAllDevicesBasicInfos();

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
            }

            foreach (var Event in allUserEvents)
            {
                //if (Event.SUserId != null)
                //{
                var user = userService.GetUser(userCode:Event.Id, withPicture:false);

                var accessGroupService = new AccessGroupService();
                var userAccess = accessGroupService.GetAccessGroupsOfUser(Event.Id);

                var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
                var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
                var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

                //IList<SupremaDeviceModel> offlineCheckerDevices;
                //var validDevice = deviceService.GetUserValidDevices(Event.Id, ConnectionType);

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

                //if (allusers.Any(x => x.SUserId == Event.SUserId))
                //{
                //    //usersOnDevices.RemoveAll(x => x.SUserId == Event.SUserId);
                //}

                //else
                //{
                //    fullAccess = null;
                //    noAccess = null;
                //    disable = new SupremaAccessGroupModel();
                //}

                #region manageOfflineDevices

                //if (fullAccess != null)
                //{
                //    offlineCheckerDevices = deviceService.GetAllDevices(ConnectionType);

                //    foreach (var device in offlineCheckerDevices)
                //        offlineEventService.InsertUserOfflineEvent(device.DeviceId, Event.NUserIdn,
                //            SupremaOfflineUserEventModel.OfflineUserInserted, ConnectionType);
                //}

                //else if (noAccess != null)
                //{
                //    offlineCheckerDevices = deviceService.GetAllDevices(ConnectionType);

                //    foreach (var device in offlineCheckerDevices)
                //        offlineEventService.InsertUserOfflineEvent(device.DeviceId, Event.NUserIdn,
                //            SupremaOfflineUserEventModel.OfflineUserInserted, ConnectionType);
                //}

                //else if (disable != null)
                //{
                //    offlineCheckerDevices = deviceService.GetAllDevices(ConnectionType);

                //    foreach (var device in offlineCheckerDevices)
                //        offlineEventService.InsertUserOfflineEvent(device.DeviceId, Event.NUserIdn,
                //            SupremaOfflineUserEventModel.OfflineUserDeleted, ConnectionType);
                //}

                //else
                //{
                //    offlineCheckerDevices = validDevice;

                //    foreach (var device in offlineCheckerDevices)
                //        offlineEventService.InsertUserOfflineEvent(device.DeviceId, Event.NUserIdn,
                //            SupremaOfflineUserEventModel.OfflineUserInserted, ConnectionType);
                //    if (validDevice.Count == 0)
                //    {
                //        //var user = new UserServices();
                //        //var userWithSUserId = user.GetUserBySUserId(Convert.ToInt32(Event.SUserId), ConnectionType).FirstOrDefault();
                //        if (Event.NUserIdn != 0)
                //        {
                //            offlineCheckerDevices = deviceService.GetAllDevices(ConnectionType);

                //            foreach (var device in offlineCheckerDevices)
                //                offlineEventService.InsertUserOfflineEvent(device.DeviceId, Event.NUserIdn,
                //                    SupremaOfflineUserEventModel.OfflineUserDeleted, ConnectionType);
                //        }
                //    }
                //}

                #endregion manageOfflineDevices


                #region transferUserToDevices

                var tasks = new List<Task>();

                foreach (var device in OnlineDevices)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        //var localDevice = tempDevice;
                        //var device = deviceFactory.Factory(device.Value.GetDeviceInfo(), ConnectionType);

                        var deviceFind =
                            validDevice.FirstOrDefault(d => device.Key == d.DeviceId);

                        if (deviceFind != null)
                        {
                            if (device.Value.TransferUser(user))
                            {
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
                                Logger.Log($"User {user.Id} is added to offline events of device {device.Value.GetDeviceInfo().DeviceId}.");
                            }
                        }

                        else
                        {
                            if (fullAccess != null)
                            {
                                if (device.Value.TransferUser(user))
                                {
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserInserted
                                    });

                                    Logger.Log(
                                        $"User {user.Id} with FullAccess, transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                                }

                                else
                                {
                                    Logger.Log($"User {user.Id} is added to offline events of device {device.Value.GetDeviceInfo().DeviceId} with FullAccess.");
                                }
                            }
                            else if (noAccess != null)
                            {
                                if (device.Value.TransferUser(user))
                                {
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserInserted
                                    });

                                    Logger.Log(
                                        $"User {user.Id} with NoAccess transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                                }

                                else
                                {
                                    Logger.Log($"User {user.Id} is added to offline events of device {device.Value.GetDeviceInfo().DeviceId}.");
                                }
                            }
                            else if (disable != null)
                            {
                                //var userService = new UserServices();
                                //var userData = userService.GetUser(Event.NUserIdn, ConnectionType);

                                if (device.Value.DeleteUser(Convert.ToUInt32(Event.Id)))
                                {
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
                                    Logger.Log($"User {user.Id} is added to offline events to delete from device {device.Value.GetDeviceInfo().DeviceId}.");
                                }
                            }

                            else
                            {
                                var result = device.Value.DeleteUser(Convert.ToUInt32(Event.Id));

                                if (result)
                                {
                                    offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserDeleted
                                    });

                                    Logger.Log(
                                        $"User {user.Id} removed from device {device.Value.GetDeviceInfo().DeviceId} successfully, due no access group found.");
                                }
                            }
                        }
                    })
                    );
                }

                Task.WaitAll(tasks.ToArray());
                //allUserEvents.Remove(Event);

                #endregion transferUserToDevices

                //}
            }

            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Sync all users that were in server event log table of all devices command";
        }

        public string GetDescription()
        {
            return " Syncing all users that were in server event log table of all devices command";
        }
    }
}
