using Biovation.Brands.Suprema.Devices;
using Biovation.Brands.Suprema.Service;
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
    public class SupremaSyncUserOfDevice : ICommand
    {
        private uint DeviceCode { get; }
        private int UserId { get; }

        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> Devices { get; }

        public SupremaSyncUserOfDevice(uint deviceCode, int userId, Dictionary<uint, Device> devices)
        {
            DeviceCode = deviceCode;
            Devices = devices?? BioStarServer.GetOnlineDevices();
            UserId = userId;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            //var eventID = Convert.ToInt32(items.First());


            var userService = new UserService();
            var user = userService.GetUser(userCode:UserId, withPicture:false);

            if (user == null)
            {
                return false;
            }

            var accessGroup = new AccessGroupService();
            var userAccess = accessGroup.GetAccessGroupsOfUser(user.Id);

            var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
            var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
            var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

            var offlineEventService = new OfflineEventService();

            //#region manageOfflineDevices

            //if (fullAccess != null)
            //{
            //    offlineEventService.AddOfflineEvent(new OfflineEvent
            //    {
            //        DeviceCode = DeviceCode,
            //        Data = user.Id.ToString(),
            //        Type = OfflineEventType.UserInserted
            //    });
            //}

            //else if (noAccess != null)
            //{
            //    offlineEventService.AddOfflineEvent(new OfflineEvent
            //    {
            //        DeviceCode = DeviceCode,
            //        Data = user.Id.ToString(),
            //        Type = OfflineEventType.UserDeleted
            //    });
            //}

            //else if (disable != null)
            //{
            //    offlineEventService.AddOfflineEvent(new OfflineEvent
            //    {
            //        DeviceCode = DeviceCode,
            //        Data = user.Id.ToString(),
            //        Type = OfflineEventType.UserDeleted
            //    });
            //}

            //else
            //{
            //    offlineEventService.AddOfflineEvent(new OfflineEvent
            //    {
            //        DeviceCode = DeviceCode,
            //        Data = user.Id.ToString(),
            //        Type = OfflineEventType.UserInserted
            //    });
            //}

            //#endregion manageOfflineDevices


                #region transferUserToDevice

                try
                {
                    var device = Devices[Convert.ToUInt32(DeviceCode)];

                    Task.Run(() =>
                    {
                        if (device == null) return;
                        if (!device.TransferUser(user)) return;
                        //offlineEventService.DeleteOfflineEvent(new OfflineEvent
                        //{
                        //    DeviceCode = device.GetDeviceInfo().Code,
                        //    Data = user.Id.ToString(),
                        //    Type = OfflineEventType.UserInserted
                        //});

                        Logger.Log($"User {user.Id} transferred to device {device.GetDeviceInfo().DeviceId} successfully.");
                    });
                }
                catch (Exception)
                {
                    return false;
                }

            #endregion transferUserToDevices

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
            return "Syncing a users of a device command";
        }
    }
}