using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Kasra.MessageBus.Domain.Enumerators;
using Kasra.MessageBus.Infrastructure;
using Kasra.MessageBus.Managers.Sinks.EventBus;
using Kasra.MessageBus.Managers.Sinks.Internal;
using PalizTiara.Api;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Models;
using PalizTiara.Protocol.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Brands.Paliz
{
    public class PalizServer
    {
        private BiovationConfigurationManager biovationConfiguration { get; }
        //private const string BiovationTopicName = "BiovationTaskStatusUpdateEvent";

        private static Dictionary<uint, DeviceBasicInfo> _onlineDevices;
        private readonly TiaraServerManager _serverManager;

        public PalizServer(Dictionary<uint, DeviceBasicInfo> onlineDevices)
        {
            _onlineDevices = onlineDevices;

            //Trace.TraceLevel = TraceLevel.Error;
            //TextBlockTraceListener traceListener = new TextBlockTraceListener(this.ServerOutputTextBlock);
            // Trace.TraceListener = (f, a) => System.Diagnostics.Trace.WriteLine(String.Format(f, a));
            //Trace.TraceListener = (f, a) => traceListener.WriteLine(string.Format(f, a));

            //Trace.TraceListener += (f, a) =>
            //{
            //    Logger.Log("Test");
            //};

            TiaraServerManager.Bootstrapper();
            _serverManager = new TiaraServerManager();

            // initialize events
            _serverManager.LiveTrafficLogEvent += OnLiveTrafficLogEvent;
        }

        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>
        public Task StartServer()
        {
            Logger.Log("Service started.");

            _serverManager.Start();

            //var connectToDeviceTasks = new List<Task>();
            ////Parallel.ForEach(_zkDevices, device => connectToDeviceTasks.Add(ConnectToDevice(device, cancellationToken)));
            ////var connectToDeviceTasks = _zkDevices.Select(ConnectToDevice).ToList();
            //if (connectToDeviceTasks.Count == 0)
            //    return Task.CompletedTask;

            //return Task.WhenAny(connectToDeviceTasks);

            //var kafkaServerAddress = biovationConfiguration.KafkaServerAddress;
            //var biovationInternalSource = InternalSourceBuilder.Start().SetPriorityLevel(PriorityLevel.Medium)
            //    .Build<DataChangeMessage<TaskInfo>>();

            //var biovationKafkaTarget = KafkaTargetBuilder.Start().SetBootstrapServer(kafkaServerAddress).SetTopicName(BiovationTopicName)
            //    .BuildTarget<DataChangeMessage<TaskInfo>>();

            //var biovationTaskConnectorNode = new ConnectorNode<DataChangeMessage<TaskInfo>>(biovationInternalSource, biovationKafkaTarget);
            //biovationTaskConnectorNode.StartProcess();

            ////DeviceStatus integration 
            //_deviceConnectionStateInternalSource = InternalSourceBuilder.Start().SetPriorityLevel(PriorityLevel.Medium)
            //    .Build<DataChangeMessage<ConnectionStatus>>();

            //var deviceConnectionStateKafkaTarget = KafkaTargetBuilder.Start().SetBootstrapServer(kafkaServerAddress).SetTopicName(DeviceConnectionStateTopicName)
            //    .BuildTarget<DataChangeMessage<ConnectionStatus>>();

            //var deviceConnectionStateConnectorNode = new ConnectorNode<DataChangeMessage<ConnectionStatus>>(_deviceConnectionStateInternalSource, deviceConnectionStateKafkaTarget);
            //deviceConnectionStateConnectorNode.StartProcess();

            //UcsApi.ServerStart(150, BiovationConfiguration.VirdiDevicesConnectionPort);

            //Logger.Log(UcsApi.ErrorCode != 0
            //        ? $"Error on starting service.\n   +ErrorCode:{UcsApi.ErrorCode} {UcsApi.EventError}"
            //        : $"Service started on port: {BiovationConfiguration.VirdiDevicesConnectionPort}"
            //    , logType: UcsApi.ErrorCode != 0 ? LogType.Error : LogType.Information);


            //try
            //{
            //    //var daylightSaving = DateTime.Now.DayOfYear <= 81 || DateTime.Now.DayOfYear > 265 ? new DateTime(DateTime.Now.Year, 3, 22, 0, 2, 0) : new DateTime(DateTime.Now.Year, 9, 22, 0, 2, 0);
            //    //var dueTime = (daylightSaving.Ticks - DateTime.Now.Ticks) / 10000;
            //    var dueTime =
            //        ((DateTime.Now.DayOfYear <= 81 || DateTime.Now.DayOfYear > 265
            //            ? new DateTime(DateTime.Now.Year, 3, 22, 0, 0, 10)
            //            : new DateTime(DateTime.Now.Year, 6, 22, 0, 0, 10)) - DateTime.Now).TotalMilliseconds;
            //    _fixDaylightSavingTimer = new Timer(FixDaylightSavingTimer_Elapsed, null, (long)dueTime, (long)TimeSpan.FromHours(24).TotalMilliseconds);
            //}
            //catch (Exception exception)
            //{
            //    Logger.Log(exception);
            //}

            return Task.CompletedTask;
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

            //this.LiveTrafficDataGrid.Dispatcher.Invoke(() =>
            //{
            //    BitmapImage bitmapImage = StaticHelpers.ToImage(log.Image);
            //    if (bitmapImage != null)
            //    {
            //        this.TestImage.Source = bitmapImage;
            //    }
            //    this.liveTraffics.Add(new LiveTrafficModel(device.TerminalName, log.Id, log.UserId, log.Valid, StaticHelpers.GetDateTimeFromEpoch(log.Time), StaticHelpers.GetDeviceNames(log.Device), StaticHelpers.GetTrafficName(log.TrafficType)));
            //});
        }
    }
}
