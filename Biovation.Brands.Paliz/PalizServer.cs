using Biovation.CommonClasses;
using Biovation.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Brands.Paliz
{
    public class PalizServer
    {
        private static Dictionary<uint, DeviceBasicInfo> _onlineDevices;

        /// <summary>
        /// <En>Make or return the unique instance of Paliz Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>
        public Task StartServer()
        {
            Logger.Log("Service started.");
            var connectToDeviceTasks = new List<Task>();
            //Parallel.ForEach(_palizDevices, device => connectToDeviceTasks.Add(ConnectToDevice(device, cancellationToken)));
            //var connectToDeviceTasks = _palizDevices.Select(ConnectToDevice).ToList();
            if (connectToDeviceTasks.Count == 0)
                return Task.CompletedTask;

            return Task.WhenAny(connectToDeviceTasks);
        }
    }
}
