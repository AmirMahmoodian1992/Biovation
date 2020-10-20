using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Suprema.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class SupremaSyncUserOfDevice : ICommand
    {
        private uint DeviceCode { get; }
        private int UserId { get; }

        /// <summary>
        /// All connected devices
        /// </summary>
        ///
        /// 
        private readonly Dictionary<uint, Device> _onlineDevices;

        private readonly BioStarServer _bioStarServer;
        private readonly AccessGroupService _accessGroupService;
        private readonly UserService _userService;

        public SupremaSyncUserOfDevice(uint deviceCode, int userId, Dictionary<uint, Device> onlineDevices, BioStarServer bioStarServer, AccessGroupService accessGroupService, UserService userService)
        {
            DeviceCode = deviceCode;
            _onlineDevices = onlineDevices;
            UserId = userId;

            _bioStarServer = bioStarServer;
            _accessGroupService = accessGroupService;
            _userService = userService;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            //var eventID = Convert.ToInt32(items.First());



            var user = _userService.GetUsers(UserId).FirstOrDefault();

            if (user == null)
            {
                return false;
            }


            var userAccess = _accessGroupService.GetAccessGroups(user.Id);

            var fullAccess = userAccess.FirstOrDefault(ua => ua.Id == 254);
            var noAccess = userAccess.FirstOrDefault(ua => ua.Id == 253);
            var disable = userAccess.FirstOrDefault(ua => ua.Name.ToUpper() == "DISABLE");

            /* var offlineEventService = new OfflineEventService();*/

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
                var device = _onlineDevices[Convert.ToUInt32(DeviceCode)];

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