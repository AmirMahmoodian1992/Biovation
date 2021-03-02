using Biovation.Brands.Virdi.UniComAPI;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiRetrieveAllLogsOfDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int TaskItemId { get; }

        private int DeviceId { get; }

        private uint Code { get; }


        /*
        public VirdiRetrieveAllLogsOfDevice(uint code, Dictionary<uint, DeviceBasicInfo> devices)
        {
            Code = code;
            DeviceId = devices.FirstOrDefault(dev => dev.Key == code).Value.DeviceId;
            OnlineDevices = devices;
        }
        */
        private readonly VirdiServer _virdiServer;

        public VirdiRetrieveAllLogsOfDevice(IReadOnlyList<object> items, VirdiServer virdiServer, DeviceService deviceService)
        {
            if (items.Count == 1)
            { DeviceId = Convert.ToInt32(items[0]); }
            else
            {
                DeviceId = Convert.ToInt32(items[0]);
                TaskItemId = Convert.ToInt32(items[1]);
            }

            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;
            OnlineDevices = virdiServer.GetOnlineDevices();
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
                //Callbacks.GetLogTaskFinished = true;
                //Callbacks.RetrieveLogs = new List<Log>();



                _virdiServer.GetAccessLogType = (int)VirdiDeviceLogType.All;
                _virdiServer.AccessLogData.GetAccessLogCountFromTerminal(TaskItemId, (int)Code, (int)VirdiDeviceLogType.All);
                //System.Threading.Thread.Sleep(1000);
                Logger.Log(GetDescription());

                Logger.Log($" +Retrieving logs from device: {Code} started successfully.\n");

                //while (!Callbacks.GetLogTaskFinished)
                //{
                //    System.Threading.Thread.Sleep(3000);
                //}
                //var result = Callbacks.RetrieveLogs;
                //var count = result.Count;



                //Callbacks.RetrieveLogs = new List<Log>();
                VirdiServer.GetLogTaskFinished = true;
                return VirdiServer.GetLogTaskFinished ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = DeviceId, Message = 0.ToString(), Validate = 1 } : new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = DeviceId, Message = 0.ToString(), Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = "Error in command execute", Validate = 0 };
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