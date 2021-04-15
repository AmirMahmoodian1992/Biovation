using Biovation.CommonClasses;
using Biovation.Domain;
using PalizTiara.Api;
using PalizTiara.Protocol.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Brands.Paliz
{
    public class PalizServer
    {
        private static Dictionary<uint, DeviceBasicInfo> _onlineDevices;
        private readonly TiaraServerManager _serverManager;

        public PalizServer()
        {
            Trace.TraceLevel = TraceLevel.Error;
            //TextBlockTraceListener traceListener = new TextBlockTraceListener(this.ServerOutputTextBlock);
            // Trace.TraceListener = (f, a) => System.Diagnostics.Trace.WriteLine(String.Format(f, a));
            //Trace.TraceListener = (f, a) => traceListener.WriteLine(string.Format(f, a));

            Trace.TraceListener += (f, a) =>
            {
                Logger.Log("Test");
            };
            TiaraServerManager.Bootstrapper();
            _serverManager = new TiaraServerManager();
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

            return Task.CompletedTask;
        }
    }
}
