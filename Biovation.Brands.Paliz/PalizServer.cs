using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using PalizTiara.Api;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Models;
using PalizTiara.Protocol.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Paliz
{
    public class PalizServer
    {
        private readonly UserService _commonUserService;
        private readonly DeviceService _commonDeviceService;

        private BiovationConfigurationManager biovationConfiguration { get; }
        //private const string BiovationTopicName = "BiovationTaskStatusUpdateEvent";

        private static Dictionary<uint, DeviceBasicInfo> _onlineDevices;
        internal readonly TiaraServerManager _serverManager;

        public PalizServer(Dictionary<uint, DeviceBasicInfo> onlineDevices, UserService commonUserService, DeviceService commonDeviceService)
        {
            _onlineDevices = onlineDevices;
            _commonDeviceService = commonDeviceService;
            _commonUserService = commonUserService;

            Trace.TraceLevel = TraceLevel.Error;
            Trace.TraceListener += WriteTrace;

            TiaraServerManager.Bootstrapper();
            _serverManager = new TiaraServerManager();

            // initialize events
            _serverManager.LiveTrafficLogEvent += OnLiveTrafficLogEvent;

            //foreach (var device in _onlineDevices)
            //{
            //    if (device.IsOnline)
            //    {
            //        await this.serverManager.GetLiveTrafficLogAsyncTask(device.TerminalName);
            //    }
            //}
        }

        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>
        public void StartServer()
        {
            Logger.Log("Service started.");

            _serverManager.Start();
        }

        public void StopServer()
        {
            _serverManager.Stop();
        }

        public Dictionary<uint, DeviceBasicInfo> GetOnlineDevices()
        {
            return _onlineDevices;
        }

        private void OnLiveTrafficLogEvent(object sender, LiveTrafficEventArgs args)
        {
            var device = (DeviceSender)sender;
            if (sender == null || args?.LiveTraffic == null)
            {
                return;
            }
            var log = args.LiveTraffic;
        }

        private async void WriteTrace(string format, params object[] args)
        {
            var terminalName = args[1].ToString();
            var existedDevice = _commonDeviceService.GetDevices(deviceName: terminalName).Data.Data.FirstOrDefault();

            //if (!_onlineDevices.ContainsKey((string)terminalName))
            //    _onlineDevices.Add((string)terminalName, new DeviceBasicInfo());

            await _serverManager.GetLiveTrafficLogAsyncTask(terminalName);
        }
    }
}
