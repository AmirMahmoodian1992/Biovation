using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetTrafficLogsInPeriod
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private long StartDate { get; }
        private long EndDate { get; }
        private uint Code { get; }

        private readonly PalizServer _palizServer;

        public PalizGetTrafficLogsInPeriod(IReadOnlyList<object> items, PalizServer palizServer, DeviceService deviceService)
        {
            _palizServer = palizServer;
            if (items.Count == 4)
            {
                TerminalName = Convert.ToString(items[0]);
                TerminalId = Convert.ToInt32(items[1]);
                StartDate = long.Parse(items[2].ToString());
                EndDate = long.Parse(items[3].ToString());
            }
            else
            {
                // TODO - Do something or delete this block.
            }

            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
            Code = devices?.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Code ?? 7;
            OnlineDevices = palizServer.GetOnlineDevices();
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
                var request = new DeviceLogRequestModel
                {
                    StartDate = StartDate,
                    EndDate = EndDate
                };
                _palizServer.NextLogPageNumber = 1;
                _palizServer._serverManager.GetDeviceLogAsyncTask(TerminalName, request);
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
