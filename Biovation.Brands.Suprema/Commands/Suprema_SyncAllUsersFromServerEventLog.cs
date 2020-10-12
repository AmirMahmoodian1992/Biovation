using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Suprema.Commands
{
    public class SupremaSyncAllUsersFromServerEventLog : ICommand
    {
        //private readonly object _object = new object();
   
        private readonly UserService _userService;
        private readonly AccessGroupService _accessGroupService;

        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;

        public SupremaSyncAllUsersFromServerEventLog(Dictionary<uint, Device> onlineDevices, AccessGroupService accessGroupService, UserService userService)
        {
           
          
            _accessGroupService = accessGroupService;
            _onlineDevices = onlineDevices;
            _userService = userService;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {


            //var forceUpdate = new ServerEventLogForceUpdateService();
            var allUserEvents = _userService.GetUsers(getTemplatesData: false);
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

            foreach (var @event in allUserEvents)
            {
                //todo:usercode
                //var user = _userService.GetUsers(userCode: Event.Id, withPicture: false);
                var user = _userService.GetUsers( @event.Id).FirstOrDefault();

                //if (Event.SUserId != null)
                //{

                if (user != null)
                {
                    var userAccess = _accessGroupService.GetAccessGroups(user.Id);

                    var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
                    var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
                    var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

                    //List<DeviceBasicInfo> offlineCheckerDevices;

                    //var validDevice = deviceService.GetUserValidDevices(user.Id, ConnectionType);

                    var validDevice = new List<DeviceBasicInfo>();
                    var accessGroups = _accessGroupService.GetAccessGroups(@event.Id);
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

                    // var offlineEventService = new OfflineEventService();

                    if (fullAccess != null)
                    {
                        //offlineCheckerDevices = _deviceServic.GetDevices();

                        /*foreach (var device in offlineCheckerDevices)*/
                        /* offlineEventService.AddOfflineEvent(new OfflineEvent
                        {
                            DeviceCode = device.Code,
                            Data = user.Id.ToString(),
                            Type = OfflineEventType.UserInserted
                        });
                }

                else if (noAccess != null)
                {
                    offlineCheckerDevices = _deviceServic.GetDevices();

                    foreach (var device in offlineCheckerDevices
                        offlineEventService.AddOfflineEvent(new OfflineEvent
                        {
                            DeviceCode = device.Code,
                            Data = user.Id.ToString(),
                            Type = OfflineEventType.UserInserted
                        });
                }

                else if (disable != null)
                {
                    offlineCheckerDevices = _deviceServic.GetDevices();

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
                        //var user = new _userServices();
                        //var userWithSUserId = user.GetUserBySUserId(Convert.ToInt32(Event.SUserId), ConnectionType).FirstOrDefault();
                        if (user.Id != 0)
                        {
                            offlineCheckerDevices = _deviceServic.GetDevices();

                            foreach (var device in offlineCheckerDevices)
                                offlineEventService.AddOfflineEvent(new OfflineEvent
                                {
                                    DeviceCode = device.Code,
                                    Data = user.Id.ToString(),
                                    Type = OfflineEventType.UserInserted
                                });
                        }
                    }*/
                    }
                }
            }

            foreach (var @event in allUserEvents)
            {
                //if (Event.SUserId != null)
                //{
                //todo
                var user = _userService.GetUsers(@event.Id).FirstOrDefault();

               
                var userAccess = _accessGroupService.GetAccessGroups(@event.Id);

                var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
                var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
                var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

                //IList<SupremaDeviceModel> offlineCheckerDevices;
                //var validDevice = deviceService.GetUserValidDevices(Event.Id, ConnectionType);

                var validDevice = new List<DeviceBasicInfo>();
                var accessGroups = _accessGroupService.GetAccessGroups(@event.Id);
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
                //        //var user = new _userServices();
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

                foreach (var device in _onlineDevices)
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
                                /* offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                {
                                    DeviceCode = device.Value.GetDeviceInfo().Code,
                                    Data = user.Id.ToString(),
                                    Type = OfflineEventType.UserInserted
                                });*/

                                if (user != null)
                                    Logger.Log(
                                        $"User {user.Id} transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                            }

                            else
                            {
                                if (user != null)
                                    Logger.Log(
                                        $"User {user.Id} is added to offline events of device {device.Value.GetDeviceInfo().DeviceId}.");
                            }
                        }

                        else
                        {
                            if (fullAccess != null)
                            {
                                if (device.Value.TransferUser(user))
                                {
                                    /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserInserted
                                    });*/

                                    if (user != null)
                                        Logger.Log(
                                            $"User {user.Id} with FullAccess, transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                                }

                                else
                                {
                                    if (user != null)
                                        Logger.Log(
                                            $"User {user.Id} is added to offline events of device {device.Value.GetDeviceInfo().DeviceId} with FullAccess.");
                                }
                            }
                            else if (noAccess != null)
                            {
                                if (device.Value.TransferUser(user))
                                {
                                    /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserInserted
                                    });*/

                                    if (user != null)
                                        Logger.Log(
                                            $"User {user.Id} with NoAccess transferred to device {device.Value.GetDeviceInfo().DeviceId} successfully.");
                                }

                                else
                                {
                                    if (user != null)
                                        Logger.Log(
                                            $"User {user.Id} is added to offline events of device {device.Value.GetDeviceInfo().DeviceId}.");
                                }
                            }
                            else if (disable != null)
                            {
                                //var _userService = new _userServices();
                                //var userData = _userService.GetUser(Event.NUserIdn, ConnectionType);

                                if (device.Value.DeleteUser(Convert.ToUInt32(@event.Id)))
                                {
                                    /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserDeleted
                                    });*/

                                    if (user != null)
                                        Logger.Log(
                                            $"User {user.Id} removed from device {device.Value.GetDeviceInfo().DeviceId} successfully, due disabled.");
                                }

                                else
                                {
                                    if (user != null)
                                        Logger.Log(
                                            $"User {user.Id} is added to offline events to delete from device {device.Value.GetDeviceInfo().DeviceId}.");
                                }
                            }

                            else
                            {
                                var result = device.Value.DeleteUser(Convert.ToUInt32(@event.Id));

                                if (result)
                                {
                                    /*offlineEventService.DeleteOfflineEvent(new OfflineEvent
                                    {
                                        DeviceCode = device.Value.GetDeviceInfo().Code,
                                        Data = user.Id.ToString(),
                                        Type = OfflineEventType.UserDeleted
                                    });*/

                                    if (user != null)
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
