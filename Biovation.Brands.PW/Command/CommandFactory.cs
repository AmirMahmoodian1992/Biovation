using Biovation.Brands.PW.Devices;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;

namespace Biovation.Brands.PW.Command
{
    /// <summary>
    /// ایجاد و بازگردانی یک نمونه از اتفاق با توجه به نوع آن
    /// </summary>
    public class CommandFactory
    {
        private readonly DeviceService _deviceService;
        private readonly Dictionary<uint, Device> _onlineDevices;
        public CommandFactory(DeviceService deviceService, Dictionary<uint, Device> onlineDevices)
        {
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
        }

        public ICommand Factory(int eventId, List<object> items)
        {
            return Factory(new DataTransferModel { EventId = eventId, Items = items });
        }

        /// <summary>
        /// <En>Create and return an instance of requested event handler.</En>
        /// <Fa>بر اساس نوع درخواست، یک نمونه از کنترل کننده درخواست می سازد.</Fa>
        /// </summary>
        /// <param name="transferModelData">داده ی دریافتی از کلاینت، بانک، sdk و یا...</param>
        /// <returns></returns>
        public ICommand Factory(DataTransferModel transferModelData)
        {
            var taskItem = (TaskItem)transferModelData.Items[0];

            switch (transferModelData.EventId)
            {
                #region DatabaseRequests(NoResponces)
                case CommandType.PersonnelEvent:
                    //Change in Personnel
                    throw new NotImplementedException();

                case CommandType.GuestEvent:
                    //Guest request
                    throw new NotImplementedException();

                case CommandType.ServerEventLogForceUpdate:
                    //Force Update from Server_Event_Log request
                    throw new NotImplementedException();

                case CommandType.SendAccessGroupToDevice:
                    //Transfer Access Group request
                    throw new NotImplementedException();

                case CommandType.SendTimeZoneToDevice:
                    //Transfer Time Zone request
                    throw new NotImplementedException();

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    throw new NotImplementedException();

                case CommandType.SendUserToDevice:
                    //Transfer Specific User to Specific Device request
                    throw new NotImplementedException();

                case CommandType.SyncAllUsers:
                    //Sync Update request
                    throw new NotImplementedException();

                case CommandType.SetTime:
                    //Update time in all devices
                    //var timeToSet = Convert.ToInt32(transferModelData.Items.FirstOrDefault());
                    throw new NotImplementedException();

                case CommandType.RetrieveAllLogsOfDevice:
                    //Gets and updates all logs from device
                    {
                        return new PwRetrieveAllLogsOfDevice(taskItem, _onlineDevices, _deviceService);
                    }

                case CommandType.RetrieveLogsOfDeviceInPeriod:
                    {
                        //Gets and updates all log in a period of time from device
                        return new RetrieveLogsOfDeviceInPeriod(taskItem, _onlineDevices, _deviceService);
                    }

                case CommandType.LockDevice:
                    //Locks the device
                    throw new NotImplementedException();

                case CommandType.UnlockDevice:
                    //Unlocks the device
                    throw new NotImplementedException();

                case CommandType.EnrollFromTerminal:
                    //Unlocks the device
                    throw new NotImplementedException();

                case CommandType.AddUserToTerminal:
                    //Unlocks the device
                    throw new NotImplementedException();

                case CommandType.DeleteUserFromTerminal:
                    //Unlocks the device
                    throw new NotImplementedException();

                #endregion

                #region WebClientRequests(WithResponse)
                case CommandType.GetUsersOfDevice:
                    //Gets users of devices
                    throw new NotImplementedException();

                case CommandType.GetOnlineDevices:
                    //Gets online devices
                    throw new NotImplementedException();


                #endregion

                default:
                    return null;
            }
        }
    }
}
