using System;
using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.Constants;
using Biovation.Domain;
using DeviceBrands = Biovation.CommonClasses.Models.ConstantValues.DeviceBrands;

namespace Biovation.Brands.ZK
{
    public class ZKTecoServer
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private static ZKTecoServer _zkServerObj;
        private static readonly object FactoryLockObject = new object();
        private static readonly Dictionary<uint, Device> OnlineDevices = new Dictionary<uint, Device>();
        //private static readonly CommunicationManager<bool> CommunicationManager = new CommunicationManager<bool>();

        private readonly List<DeviceBasicInfo> _zkDevices;
        private readonly DeviceService _commonDeviceService = new DeviceService();
        private static bool _readingLogsInProgress;

        public static Queue<Task> LogReaderQueue = new Queue<Task>();


        ////
        ///Task And Queue
        private static List<TaskInfo> _tasks = new List<TaskInfo>();
        private static readonly TaskService TaskService = new TaskService();
        private static bool _processingQueueInProgress;
        /// 

        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>
        public static ZKTecoServer FactoryZKServer()
        {
            lock (FactoryLockObject)
            {
                return _zkServerObj ?? (_zkServerObj = new ZKTecoServer());
            }
        }

        private ZKTecoServer()
        {
            _zkDevices = _commonDeviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.ZkTecoCode).Where(x => x.Active).ToList();
        }

        public void StartServer()
        {
            Logger.Log("Service started.");

            foreach (var zkDevice in _zkDevices)
            {
                ConnectToDevice(zkDevice);
            }
        }

        public async void ConnectToDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                lock (OnlineDevices)
                {
                    if (OnlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        try
                        {
                            OnlineDevices[deviceInfo.Code].Disconnect();
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                        }
                    }
                }

                if (!deviceInfo.Active) return;

                var device = DeviceFactory.Factory(deviceInfo);
                var connectResult = device.Connect();
                if (!connectResult)
                    Logger.Log($"Cannot connect to device {deviceInfo.Code}.", logType: LogType.Warning);
            });
        }

        public async void DisconnectFromDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                lock (OnlineDevices)
                    if (!OnlineDevices.ContainsKey(deviceInfo.Code)) return;

                OnlineDevices[deviceInfo.Code].Disconnect();
                //lock (OnlineDevices)
                //    OnlineDevices.Remove(deviceInfo.Code);
            });
        }

        public void StopServer()
        {
            lock (OnlineDevices)
            {
                foreach (var onlineDevice in OnlineDevices)
                {
                    onlineDevice.Value.Disconnect();
                }
            }
        }

        public static Dictionary<uint, Device> GetOnlineDevices()
        {
            lock (OnlineDevices)
            {
                return OnlineDevices;
            }
        }
        /*
        public static void StartReadLogs()
        {
            if (_readingLogsInProgress)
                return;

            _readingLogsInProgress = true;
            while (true)
            {
                Task logReader;
                lock (LogReaderQueue)
                {
                    if (LogReaderQueue.Count <= 0)
                    {
                        _readingLogsInProgress = false;
                        return;
                    }

                    logReader = LogReaderQueue.Dequeue();
                }

                logReader.Start();
                Task.WaitAll(logReader);
            }
        }
        */
        public static void ProcessQueue()
        {
            lock (_tasks)
                _tasks = TaskService.GetTasks(brandCode: DeviceBrands.ZkTecoCode,
                    excludedTaskStatusCodes: new List<string> { TaskStatuses.Done.Code, TaskStatuses.Failed.Code }).Result;

            if (_processingQueueInProgress)
                return;

            _processingQueueInProgress = true;
            while (true)
            {
                TaskInfo taskInfo;
                lock (_tasks)
                {
                    if (_tasks.Count <= 0)
                    {
                        _processingQueueInProgress = false;
                        return;
                    }

                    taskInfo = _tasks.First();
                }

                TaskManager.ExecuteTask(taskInfo);
                lock (_tasks)
                    _tasks.Remove(taskInfo);
            }
        }

    }
}
