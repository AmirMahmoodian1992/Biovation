using Biovation.Brands.Virdi.UniComAPI;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiRetrieveAllLogsOfDeviceInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int TaskItemId { get; }
        private int DeviceId { get; }
        private DeviceBasicInfo Device { get; }
        private DateTime FromDate { get; }
        private DateTime ToDate { get; }

        private readonly VirdiServer _virdiServer;
        private readonly IAccessLogData _accessLogData;

        public VirdiRetrieveAllLogsOfDeviceInPeriod(IReadOnlyList<object> items, VirdiServer virdiServer, TaskService taskService, DeviceService deviceService, IAccessLogData accessLogData)
        {
            _virdiServer = virdiServer;
            _accessLogData = accessLogData;
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Device = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId);


            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);

            FromDate = Convert.ToDateTime(data?["fromDate"] ?? "1970/01/01");
            ToDate = Convert.ToDateTime(data?["toDate"] ?? "2050/01/01");
            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Device?.Code))
            {
                Logger.Log($"RetrieveLogInPeriod,The device: {Device?.Code} is not connected.");
                return new ResultViewModel { Id = DeviceId, Message = $"The device: {Device?.Code} is not connected.", Validate = 0, Code = 10006 };
            }

            try
            {
                //Callbacks.GetLogTaskFinished = true;
                //Callbacks.RetrieveLogs = new List<Log>();

                //try
                //{
                //    Callbacks.SetLogPeriodSemaphore.WaitOne(240000);
                //}
                //catch (Exception exception)
                //{
                //    Logger.Log(exception);
                //}

                _virdiServer.AccessLogPeriodFromDateTime = FromDate;
                _virdiServer.AccessLogPeriodToDateTime = ToDate;

                if (FromDate == default || ToDate == default || FromDate == new DateTime(1970, 1, 1))
                {
                    _virdiServer.GetAccessLogType = (int)VirdiDeviceLogType.All;
                    _accessLogData.GetAccessLogCountFromTerminal(TaskItemId, (int)(Device?.Code ?? 0), (int)VirdiDeviceLogType.All);
                }
                else
                {
                    _virdiServer.GetAccessLogType = (int)VirdiDeviceLogType.Period;
                    _accessLogData.SetPeriod(FromDate.Year, FromDate.Month, FromDate.Day, ToDate.Year, ToDate.Month, ToDate.Day);
                    _accessLogData.GetAccessLogCountFromTerminal(TaskItemId, (int)(Device?.Code ?? 0), (int)VirdiDeviceLogType.Period);
                }
                //_callbacks.AccessLogData.GetAccessLogFromTerminal(0, (int)Code, (int)VirdiDeviceLogType.Period);
                //System.Threading.Thread.Sleep(1000);
                Logger.Log(GetDescription());

                Logger.Log($" +Retrieving logs from device: {Device?.Code} started successfully.");

                //while (!Callbacks.GetLogTaskFinished)
                //{
                //    System.Threading.Thread.Sleep(3000);
                //}
                //var result = Callbacks.RetrieveLogs;
                //var count = result.Count;
                //Task.Run(() =>
                //{
                //    _logService.AddLog(result);
                //});

                //Callbacks.RetrieveLogs = new List<Log>();

                return VirdiServer.GetLogTaskFinished ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = DeviceId, Message = 0.ToString(), Validate = 1 } : new ResultViewModel { Id = DeviceId, Message = 0.ToString(), Validate = 1, Code = Convert.ToInt64(TaskStatuses.InProgressCode) };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Id = DeviceId, Message = "0", Validate = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get all logs of a device in a period of time command";
        }

        public string GetDescription()
        {
            return $"Getting all logs of a device (id: {Device?.Code} from: {FromDate} To: {ToDate}) command";
        }
    }
}
