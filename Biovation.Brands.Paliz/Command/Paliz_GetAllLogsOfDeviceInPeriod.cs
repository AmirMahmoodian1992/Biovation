//using Biovation.Brands.Paliz.UniComAPI;
using Biovation.Brands.Paliz.UniComAPI;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetLogsOfDeviceInPeriod
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int TaskItemId { get; }

        private int TerminalId { get; }

        private uint Code { get; }


        /*
        public VirdiRetrieveAllLogsOfDevice(uint code, Dictionary<uint, DeviceBasicInfo> devices)
        {
            Code = code;
            TerminalId = devices.FirstOrDefault(dev => dev.Key == code).Value.TerminalId;
            OnlineDevices = devices;
        }
        */
        private readonly PalizServer _palizServer;

        public PalizGetLogsOfDeviceInPeriod(IReadOnlyList<object> items, PalizServer palizServer, DeviceService deviceService)
        {
            _palizServer = palizServer;
            if (items.Count == 1)
            { TerminalId = Convert.ToInt32(items[0]); }
            else
            {
                TerminalId = Convert.ToInt32(items[0]);
                TaskItemId = Convert.ToInt32(items[1]);
            }

            var devices = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode);
            Code = devices?.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Code ?? 0;
            //OnlineDevices = palizServer.GetOnlineDevices();
        }


        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"RetriveAllLogsOfDevice,The device: {Code} is not connected.");
                return new List<User>();
            }

            try
            {
                var request = new DeviceLogRequestModel();
                //var fromDate = this.FromDatePicker.SelectedDate;
                //var toDate = this.ToDatePicker.SelectedDate;

                //_palizServer.GetAccessLogType = (int)PalizDeviceLogType.All;
                //_serverManager.GetDeviceLogAsyncTask();
                System.Threading.Thread.Sleep(1000);
                Logger.Log(GetDescription());

                Logger.Log($" +Retrieving logs from device: {Code} started successfully.\n");
                PalizServer.GetLogTaskFinished = true;
                return PalizServer.GetLogTaskFinished
                    ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = TerminalId, Message = 0.ToString(), Validate = 1 }
                    : new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = TerminalId, Message = 0.ToString(), Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = TerminalId, Message = "Error in command execute", Validate = 0 };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get all logs of a device command";
        }

        public string GetDescription()
        {
            return "Getting all logs of a device and insert into database.";
        }
    }
}
