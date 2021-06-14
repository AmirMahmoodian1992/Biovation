using Biovation.Brands.PW.Devices;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Brands.PW
{
    public class PwServer
    {
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly List<DeviceBasicInfo> _pwDevices;
        private readonly LogService _logService;

        private readonly RestClient _restClient;
        //private readonly UserService _userService = new UserService();
        protected CancellationToken CancellationToken;
        //public xLink.xLinkClass pwSdkClass;

        private readonly LogEvents _logEvents;
        private readonly DeviceFactory _deviceFactory;

        public PwServer(DeviceService commonDeviceService, LogService commonLogService, Dictionary<uint, Device> onlineDevices, RestClient restClient, LogEvents logEvents, DeviceFactory deviceFactory)
        {
            _logService = commonLogService;
            _onlineDevices = onlineDevices;
            _restClient = restClient;
            _logEvents = logEvents;
            _deviceFactory = deviceFactory;
            _pwDevices = commonDeviceService.GetDevices(brandId: DeviceBrands.ProcessingWorldCode)?.Where(x => x.Active).ToList();
        }

        public void StartServer(CancellationToken cancellationToken)
        {
            Logger.Log("Service started.");
            CancellationToken = cancellationToken;

            Task.Run(() =>
            {
                //while (!Token.IsCancellationRequested && !stopService)
                //{
                //    _pwDevices = _commonDeviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.ProcessingWorldCode).Where(x => x.Active).ToList();

                //    List<DeviceBasicInfo> disconnectDevices;
                //    lock (OnlineDevices)
                //    {
                //        disconnectDevices = _pwDevices.ExceptBy(OnlineDevices?.Values.Select(onlineDevice => onlineDevice.GetDeviceInfo()), info => info.Code).ToList();
                //    }

                foreach (var pwDevice in _pwDevices)
                {
                    ConnectToDevice(pwDevice);
                }

                //Thread.Sleep(120000);
                //}

                //Logger.Log("Reading log of devices function is canceled.");
            }, CancellationToken);

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            if (Token.IsCancellationRequested)
            //            {
            //                Logger.Log("Reading log of devices function is canceled.");
            //                return;
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            // ignore
            //        }

            //        //var onlineDevices = new Dictionary<uint, Device>();
            //        //lock (OnlineDevices)
            //        //{
            //        //    foreach (var onlineDevice in OnlineDevices)
            //        //    {
            //        //        onlineDevices.Add(onlineDevice.Key, onlineDevice.Value);
            //        //    }
            //        //}

            //        //foreach (var onlineDevice in onlineDevices)
            //        //{
            //        //    //LogReaderQueue.Enqueue(new Task(() => onlineDevice.Value.ReadOfflineLog(Token), Token));
            //        //    //StartReadLogs();

            //        //    var creatorUser = _userService.GetUser(123456789, false);
            //        //    var task = new TaskInfo
            //        //    {
            //        //        CreatedAt = DateTimeOffset.Now,
            //        //        CreatedBy = creatorUser,
            //        //        TaskType = TaskTypes.GetServeLogs,
            //        //        Priority = TaskPriorities.Medium,
            //        //        DeviceBrand = DeviceBrands.ProcessingWorld,
            //        //        TaskItems = new List<TaskItem>()
            //        //    };
            //        //    task.TaskItems.Add(new TaskItem
            //        //    {

            //        //        Status = TaskStatuses.Queued,
            //        //        TaskItemType = TaskItemTypes.GetServeLogs,
            //        //        Priority = TaskPriorities.Medium,
            //        //        DueDate = DateTime.Today,
            //        //        DeviceId = onlineDevice.Value.GetDeviceInfo().DeviceId,
            //        //        Data = JsonConvert.SerializeObject(new { onlineDevice.Value.GetDeviceInfo().DeviceId }),
            //        //        IsParallelRestricted = true,
            //        //        IsScheduled = false,
            //        //        OrderIndex = 1
            //        //    });
            //        //    TaskService.InsertTask(task).Wait(Token);
            //        //    ProcessQueue();
            //        //}

            //        Thread.Sleep(120000);
            //    }
            //}, Token);
        }

        public async void ConnectToDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                ConnectionStatus connectionStatus;

                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        _onlineDevices[deviceInfo.Code].Disconnect();
                        _onlineDevices.Remove(deviceInfo.Code);

                        connectionStatus = new ConnectionStatus
                        {
                            DeviceId = deviceInfo.DeviceId,
                            IsConnected = false
                        };

                        try
                        {
                            var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                            restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                            _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                            _logService.AddLog(new Log
                            {
                                DeviceId = deviceInfo.DeviceId,
                                LogDateTime = DateTime.Now,
                                EventLog = _logEvents.Disconnect
                            });
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }
                }

                if (!deviceInfo.Active) return;

                var device = _deviceFactory.Factory(deviceInfo);
                var connectResult = device.Connect(CancellationToken);
                if (!connectResult) return;

                lock (_onlineDevices)
                {
                    if (!_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        _onlineDevices.Add(deviceInfo.Code, device);
                    }
                }

                //Task.Run(() =>
                //{
                connectionStatus = new ConnectionStatus
                {
                    DeviceId = deviceInfo.DeviceId,
                    IsConnected = true
                };

                try
                {
                    var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                    restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                    _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                    _logService.AddLog(new Log
                    {
                        DeviceId = deviceInfo.DeviceId,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.Connect
                    });
                }
                catch (Exception)
                {
                    //ignore
                }
                //});

            }, CancellationToken);
        }
        public async void DisconnectFromDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        _onlineDevices[deviceInfo.Code].Disconnect();
                        _onlineDevices.Remove(deviceInfo.Code);
                    }
                }

                try
                {
                    var connectionStatus = new ConnectionStatus
                    {
                        DeviceId = deviceInfo.DeviceId,
                        IsConnected = false
                    };

                    var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                    restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                    _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                    _logService.AddLog(new Log
                    {
                        DeviceId = deviceInfo.DeviceId,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.Disconnect
                    });
                }
                catch (Exception)
                {
                    //ignore
                }
            }, CancellationToken);
        }

        public void StopServer()
        {
            lock (_onlineDevices)
            {
                foreach (var onlineDevice in _onlineDevices)
                {
                    onlineDevice.Value.Disconnect();
                }
            }
        }

        //public static Dictionary<uint, Device> GetOnlineDevices()
        //{
        //    lock (_onlineDevices)
        //    {
        //        return _onlineDevices;
        //    }
        //}


        //public static void StartReadLogs()
        //{
        //    if (_readingLogsInProgress)
        //        return;

        //    _readingLogsInProgress = true;
        //    while (true)
        //    {
        //        Task logReader;
        //        lock (LogReaderQueue)
        //        {
        //            if (LogReaderQueue.Count <= 0)
        //            {
        //                _readingLogsInProgress = false;
        //                return;
        //            }

        //            logReader = LogReaderQueue.Dequeue();
        //        }

        //        logReader.Start();
        //        Task.WaitAll(logReader);
        //    }
        //}
    }
}
