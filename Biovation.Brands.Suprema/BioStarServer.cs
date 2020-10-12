using Biovation.Brands.Suprema.Devices;
using Biovation.Brands.Suprema.Manager;
using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Brands.Suprema.Services;
using Biovation.CommonClasses.Manager;

namespace Biovation.Brands.Suprema
{
    /// <summary>
    /// کلاس کنترل کننده سرور بایواستار برای اتصال ساعت ها
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantDelegateCreation")]
    public class BioStarServer
    {
        /// <summary>
        /// پورت سرور برای اتصال ساعت ها
        /// </summary>
        private int _mPort;
        /// <summary>
        /// ماکزیمم تعداد کانکشن مجاز
        /// </summary>
        private int _mConnections;
        /// <summary>
        /// تعداد ساعت های متصل
        /// </summary>
        private int _mCount;
        private readonly bool _mUseFunctionLock;
        private readonly bool _mUseAutoResponse;
        private readonly bool _mUseLock;
        private readonly bool _mMatchingFail;

        /// <summary>
        /// لیست کامل ساعت های متصل
        /// </summary>

        private readonly Dictionary<uint, Device> _onlineDevices;

        private readonly Dictionary<int, string> _deviceTypes;
        private readonly SupremaCodeMappings _supremaCodeMappings;
        private readonly DeviceFactory _deviceFactory;
        private readonly Dictionary<uint, CancellationTokenSource> _deviceCancellationTokens =
                    new Dictionary<uint, CancellationTokenSource>();

        private readonly RestClient _monitoringRestClient;

        /// <summary>
        /// استفاده برای قفل روی دسترسی ترد ها
        /// </summary>
        private  readonly object Object = new object();
        private  readonly object _logObject = new object();
        //private static readonly object Log1Object = new object();

        public  readonly Semaphore DeviceConnectionSemaphore = new Semaphore(1, 1);
        public  readonly CancellationToken ServiceCancellationToken = new CancellationToken(false);

        /// 

        private BSSDK.BS_ConnectionProc _fnCallbackConnected;
        private BSSDK.BS_DisconnectedProc _fnCallbackDisconnected;
        private BSSDK.BS_RequestStartProc _fnCallbackRequestStart;
        private BSSDK.BS_LogProc _fnCallbackLog;
        private BSSDK.BS_ImageLogProc _fnCallbackImageLog;
        private BSSDK.BS_RequestMatchingProc _fnCallbackRequestMatching;
        private BSSDK.BS_RequestUserInfoProc _fnCallbackRequestUserInfo;

        private readonly DeviceService _deviceService;
        private readonly LogService _logService;
        private readonly SupremaLogService _supremaLogService;

      
        private bool _handlingOfflineEventsInProgress;

        private readonly LogEvents _logEvents;
        private readonly MatchingTypes _matchingTypes;


        public Queue<KeyValuePair<uint, Task>> LogReaderQueue = new Queue<KeyValuePair<uint, Task>>();
        public Queue<KeyValuePair<uint, Task>> OfflineEventHandlersQueue = new Queue<KeyValuePair<uint, Task>>();

        private bool _readingLogsInProgress;

        public BioStarServer(DeviceService deviceService, Dictionary<uint, Device> onlineDevices, Dictionary<int, string> deviceTypes, LogEvents logEvents, MatchingTypes matchingTypes, LogService logService, SupremaLogService supremaLogService, RestClient monitoringRestClient, SupremaCodeMappings supremaCodeMappings, BiovationConfigurationManager biovationConfigurationManager, DeviceFactory deviceFactory)
        {
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _deviceTypes = deviceTypes;
            _logEvents = logEvents;
            _matchingTypes = matchingTypes;
            _logService = logService;
            _supremaLogService = supremaLogService;
            _monitoringRestClient = monitoringRestClient;
            _supremaCodeMappings = supremaCodeMappings;
            _deviceFactory = deviceFactory;

            _mPort = biovationConfigurationManager.SupremaDevicesConnectionPort;
            BSSDK.BS_InitSDK();

            _mUseFunctionLock = false;
            _mUseAutoResponse = true;
            _mUseLock = false;
            _mCount = 0;
            _mMatchingFail = false;

            //_mPort = ConfigurationManager.SupremaDevicesConnectionPort;

            Logger.Log(" BioStar Server Started!");

            //StartService();
        }



        /// <summary>
        /// <En>Starts Biostar server to connect to the devices</En>
        /// <Fa>سرور مربوط به ساعت هارا برای اتصال به ساعت ها راه اندازی می کند.</Fa>
        /// </summary>
        public void StartService()
        {
            //Set event procedure. 

            _fnCallbackConnected = new BSSDK.BS_ConnectionProc(ConnectedProc);
            //var gch = GCHandle.Alloc(_fnCallbackConnected);
            BSSDK.BS_SetConnectedCallback(Marshal.GetFunctionPointerForDelegate(_fnCallbackConnected), _mUseFunctionLock, _mUseAutoResponse);
            //gch.Free();

            _fnCallbackDisconnected = new BSSDK.BS_DisconnectedProc(DisconnectedProc);
            BSSDK.BS_SetDisconnectedCallback(Marshal.GetFunctionPointerForDelegate(_fnCallbackDisconnected), _mUseFunctionLock);

            _fnCallbackRequestStart = new BSSDK.BS_RequestStartProc(RequestStartProc);
            BSSDK.BS_SetRequestStartedCallback(Marshal.GetFunctionPointerForDelegate(_fnCallbackRequestStart), _mUseFunctionLock, _mUseAutoResponse);

            _fnCallbackLog = new BSSDK.BS_LogProc(LogProc);
            BSSDK.BS_SetLogCallback(Marshal.GetFunctionPointerForDelegate(_fnCallbackLog), _mUseFunctionLock, _mUseAutoResponse);

            _fnCallbackImageLog = new BSSDK.BS_ImageLogProc(ImageLogProc);
            BSSDK.BS_SetImageLogCallback(_fnCallbackImageLog, _mUseFunctionLock, _mUseAutoResponse);

            _fnCallbackRequestUserInfo = new BSSDK.BS_RequestUserInfoProc(RequestUserInfoProc);
            BSSDK.BS_SetRequestUserInfoCallback(_fnCallbackRequestUserInfo, _mUseFunctionLock);

            _fnCallbackRequestMatching = new BSSDK.BS_RequestMatchingProc(RequestMatchingProc);
            BSSDK.BS_SetRequestMatchingCallback(_fnCallbackRequestMatching, _mUseFunctionLock);

            BSSDK.BS_SetSynchronousOperation(_mUseLock);

            //Connection approved count
            _mConnections = 64;

            _deviceTypes.Add(0, "BioStation");
            _deviceTypes.Add(2, "BioLite");
            _deviceTypes.Add(6, "BioStation_T2");
            _deviceTypes.Add(10, "FaceStation");
            _deviceTypes.Add(3001, "BiominiClient");

            var result = BSSDK.BS_StartServerApp(_mPort, _mConnections, "C:\\OpenSSL\\bin\\openssl.exe", "12345678", BSSDK.KEEP_ALIVE_INTERVAL);
            //var result = BSSDK.BS_StartServerApp(_mPort, _mConnections, "C:\\OpenSSL\\bin\\openssl.exe", "12345678", 45);
            Logger.Log(result == BSSDK.BS_SUCCESS
                ? $" Biostar Server SDK Started on port {_mPort} ... "
                : " BS_StartServerApp Fail!"
            , logType: result == BSSDK.BS_SUCCESS ? LogType.Debug : LogType.Warning);

            _mCount = 0;


            //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}"); ;


            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(28800000);

                    if (ServiceCancellationToken.IsCancellationRequested)
                    {
                        Logger.Log("Read log of devices function is canceled.");
                        return;
                    }

                 
                    lock (_onlineDevices)
                    {
                        foreach (var onlineDevice in _onlineDevices)
                        {
                            _onlineDevices.Add(onlineDevice.Key, onlineDevice.Value);
                        }
                    }

                    foreach (var onlineDevice in _onlineDevices)
                    {
                        LogReaderQueue.Enqueue(new KeyValuePair<uint, Task>(onlineDevice.Key, new Task(() => onlineDevice.Value.ReadOfflineLog(ServiceCancellationToken), ServiceCancellationToken)));
                        StartReadLogs();
                    }
                }
            }, ServiceCancellationToken);

            #region Mock

            //_deviceList.Add(40463,new SupremaDeviceInfoModel {ConnectionType = "", DeviceId = 40463,DeviceType = 2, Handle = 1, IpAddress = "192.168.30.96"});

            #endregion
        }

        /// <summary>
        /// <En>Stops Biostar server to connect to the devices</En>
        /// <Fa>سرور مربوط به ساعت هارا خاموش می کند.</Fa>
        /// </summary>
        public void StopService()
        {
            lock (_onlineDevices)
            {
                foreach (var onlineDevice in _onlineDevices)
                {
                    if (onlineDevice.Value.GetDeviceInfo().DeviceTypeId != BSSDK.BS_DEVICE_BIOMINI_CLIENT)
                    {
                        BSSDK.BS_CloseConnection(onlineDevice.Value.GetDeviceInfo().Code);
                    }
                }
            }

            BSSDK.BS_StopServerApp();
        }

        public int ConnectedProc(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress)
        {
            try
            {
                ConnectedProcMethod(handle, deviceId, deviceType, connectionType, functionType, ipAddress);
                return BSSDK.BS_SUCCESS;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return BSSDK.BS_ERR_CHANNEL_CLOSED;
            }
        }

        /// <summary>
        /// <En>Add new device to the dictionary and apply offline changes for users on device on connect.</En>
        /// <Fa>در لحظه ی اتصال، ساعت های جدید را به لیست ساعت های متصل اضافه کرده و تغییرات کاربر ها در زمان عدم اتصال ساعت را اعمال می کند.</Fa>
        /// </summary>
        /// <param name="handle">شماره هر ساعت در برنامه پس از اتصال</param>
        /// <param name="deviceId">شناسه ی ساعت</param>
        /// <param name="deviceType">نوع ساعت</param>
        /// <param name="connectionType">نوع اتصال</param>
        /// <param name="functionType"></param>
        /// <param name="ipAddress">آدرس آی پی ساعت</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int ConnectedProcMethod(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress, string name = default)
        {
            //var _deviceModel = _deviceService.GetDeviceBasicInfoWithCode(deviceId, _deviceBrands.Suprema);
            var deviceInfo = new SupremaDeviceModel
            {
                Code = deviceId,
                DeviceId = 0,
                Handle = handle,
                Model = _supremaCodeMappings.GetGenericDeviceModel(deviceType),
                IpAddress = ipAddress,
                Port = _mPort,
                DeviceTypeId = deviceType,
                Name = string.IsNullOrWhiteSpace(name) ? $"{deviceId} ({ipAddress})" : name,
                ConnectionType = "Normal"
            };



            //var result = DeviceService.ModifyDeviceBasicInfoByID(deviceInfo);
            //deviceInfo.DeviceId = (int)result.Id;
            var device = _deviceFactory.Factory(deviceInfo);
            deviceInfo.DeviceId = device.AddDeviceToDataBase();
            device.UpdateDeviceInfo(deviceInfo);

            var deviceResult = _deviceService.GetDevices(code: deviceId, brandId: Convert.ToInt32(DeviceBrands.SupremaCode).ToString()).FirstOrDefault();

            if (deviceResult != null && deviceResult.TimeSync)
            {

                var timeFrom1970 = new DateTime(1970, 1, 1, 0, 0, 00);
                var resultTimeSync = BSSDK.BS_SetTime(handle, (int)((DateTime.Now - timeFrom1970).TotalSeconds));
                if (resultTimeSync != BSSDK.BS_SUCCESS)
                {
                    Logger.Log("Unfortunately, Set Time of the device is canceled.");
                }
            }

            try
            {
                DeviceConnectionSemaphore.WaitOne(10000);
            }
            catch (Exception)
            {
                //ignore
            }

            Device connectedDevice;
            lock (_onlineDevices)
            {
                //connectedDevice = _onlineDevices.Values.FirstOrDefault(x => x.GetDeviceInfo().Handle == handle);
                connectedDevice = _onlineDevices.FirstOrDefault(x => x.Key == deviceInfo.Code).Value;
            }

            try
            {
                DeviceConnectionSemaphore.Release();
            }
            catch (Exception)
            {
                //ignore
            }

            if (connectedDevice != null)
            {
                _deviceCancellationTokens[connectedDevice.GetDeviceInfo().Code].Cancel();
                try
                {
                    try
                    {
                        DeviceConnectionSemaphore.WaitOne(10000);
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    lock (_onlineDevices)
                    {
                        _onlineDevices.Remove(connectedDevice.GetDeviceInfo().Code);
                    }

                    try
                    {
                        DeviceConnectionSemaphore.Release();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
            }

            lock (_onlineDevices)
            {
                if (_onlineDevices.ContainsKey(deviceInfo.Code))
                {

                    _onlineDevices[deviceId].UpdateDeviceInfo(deviceInfo);



                }

                else // if not exist in list, then insert new one.
                {
                    try
                    {
                        DeviceConnectionSemaphore.WaitOne(10000);
                    }
                    catch (Exception)
                    {
                        //ignore
                    }


                    _onlineDevices.Add(deviceInfo.Code, device);

                    try
                    {
                        DeviceConnectionSemaphore.Release();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    _mCount++;
                    //var fetchedDevice = DeviceService.GetDeviceBasicInfoWithCode(deviceId, _deviceBrands.Suprema);

                    //if (fetchedDevice == null)
                    //{
                    //    _onlineDevices[deviceId].AddDeviceToDataBase();
                    //}
                }
            }

            Task.Run(async () =>
            {

                var connectionStatus = new ConnectionStatus
                {
                    DeviceId = deviceInfo.DeviceId,
                    IsConnected = true
                };

                try
                {
                    var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                    restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                    await _monitoringRestClient.ExecuteAsync<ResultViewModel>(restRequest, ServiceCancellationToken);

                    await _supremaLogService.AddLog(new Log
                    {
                        DeviceId = (int)deviceId,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.Connect
                    });

                }
                catch (Exception)
                {
                    //ignore
                }
            }, ServiceCancellationToken);

            Logger.Log($"Connected : {_mCount}, Device Brand: Suprema , Device Type: {_deviceTypes[deviceType]}, DeviceID: {deviceId}.", logType: LogType.Information);

            if (deviceType != DeviceModels.Biomini)
            {
                Task.Run(() =>
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            Thread.Sleep(5000);
                            var requestStartResult = BSSDK.BS_StartRequest(handle, deviceType, _mPort);
                            Logger.Log($"Device {deviceId} StartRequest result: {requestStartResult} Steps_SourceDatabase_");

                            if (requestStartResult == 0)
                            {
                                break;
                            }
                        }

                    });
            }

            return BSSDK.BS_SUCCESS;
        }

        public int RequestStartProc(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress)
        {
            Logger.Log("RequestStartProc callback! " + DateTime.Now);

            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            // this if statement is a modified by Amin (due to generated exception)
            try
            {
                if (_deviceCancellationTokens.Keys.Any(x => x == deviceId))
                {
                    _deviceCancellationTokens[deviceId].Cancel();
                    _deviceCancellationTokens[deviceId] = cancellationTokenSource;
                }
                else
                {
                    _deviceCancellationTokens.Add(deviceId, cancellationTokenSource);
                }


            }
            catch (Exception)
            {
                //Exist... ignore
            }
            Task.Run(() => RequestStartProcMethod(deviceId, token), token);

            //Thread.Sleep(2000);
            //cancelationTokenSource.Cancel();
            //RequestStartProcMethod(handle, deviceId, deviceType, connectionType, functionType, ipAddress);
            return BSSDK.BS_SUCCESS;
        }

        //private int RequestStartProcMethod(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress, CancellationToken token)
        private int RequestStartProcMethod(uint deviceId, CancellationToken token)
        {
            #region OfflineLog

            //--------------------------------------------------------------------------------------------------------------------------


            Logger.Log($"Reading offline logs of Device : {deviceId} Started.");

            //
            //Thread.Sleep(1950);

            if (token.IsCancellationRequested)
            {
                Logger.Log("Thread canceled.");
                return 0;
            }

            if (!_onlineDevices.ContainsKey(deviceId))
            {
                return BSSDK.BS_ERR_CHANNEL_CLOSED;
            }

            //_onlineDevices[deviceId].ReadOfflineLog(token);
            LogReaderQueue.Enqueue(new KeyValuePair<uint, Task>(deviceId, new Task(() => _onlineDevices[deviceId].ReadOfflineLog(token), token)));
            StartReadLogs();

            Logger.Log($"Finished reading offline logs of Device : {deviceId} .");

            #endregion OfflineLog

            if (!_onlineDevices.ContainsKey(deviceId))
            {
                return BSSDK.BS_ERR_CHANNEL_CLOSED;
            }

            if (token.IsCancellationRequested)
            {
                Logger.Log("Thread canceled.");
                return 0;
            }

            //_deviceService.UpdateDeviceNetworkToDataBase(_onlineDevices[deviceId].GetDeviceInfo(), "Personnel");

            #region OfflineEvents

            //if (!_onlineDevices.ContainsKey(deviceId))
            //{
            Logger.Log($"Handling Offline events of Device : {deviceId} Started.");


            try
            {
                if (token.IsCancellationRequested)
                {
                    Logger.Log("Thread canceled.");
                    return 0;
                }

                //_onlineDevices[deviceId].ReadOfflineEvent();
                OfflineEventHandlersQueue.Enqueue(new KeyValuePair<uint, Task>(deviceId, new Task(() => _onlineDevices[deviceId].ReadOfflineEvent(), token)));
                //StartHandleOfflineEvents();
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            Logger.Log($"Finished handling offline events of Device : {deviceId} .Steps_SourceDatabase_");
            //}


            //--------------------------------------------------------------------------------------------------------------------------
            #endregion offlineEvent
            return BSSDK.BS_SUCCESS;
        }

        public  int DisconnectedProc(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress)
        {
            //return DisconnectedProcMethod(handle, deviceId, deviceType, connectionType, functionType, ipAddress);
            try
            {
                Logger.Log("DisconnectedProc callback. " + DateTime.Now);
                //Task.Run(() => 
                DisconnectedProcMethod(handle, deviceId, deviceType, connectionType, functionType, ipAddress);
                return BSSDK.BS_SUCCESS;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return BSSDK.BS_ERR_CHANNEL_CLOSED;
            }
        }

        /// <summary>
        /// <En>Delete device from devices dictionary after disconnect.</En>
        /// <Fa>پس از قظع شدن دستگاه، ساعت را از لیست موجود حذف می کند.</Fa>
        /// </summary>
        /// <param name="handle">شماره هر ساعت در برنامه پس از اتصال</param>
        /// <param name="deviceId">شناسه ی ساعت</param>
        /// <param name="deviceType">نوع ساعت</param>
        /// <param name="connectionType">نوع اتصال</param>
        /// <param name="functionType"></param>
        /// <param name="ipAddress">آدرس آی پی ساعت</param>
        /// <returns></returns>
        public int DisconnectedProcMethod(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress)
        {
            var deviceModel = _deviceService.GetDevices(code:deviceId).FirstOrDefault();

            lock (_onlineDevices)
            {
                foreach (var device in _onlineDevices)
                {
                    if (deviceModel is null
                        ? device.Key == deviceId
                        : device.Value.GetDeviceInfo().Code == deviceModel.Code &&
                          device.Value.GetDeviceInfo().Handle == handle)
                    {
                        try
                        {
                            DeviceConnectionSemaphore.WaitOne(10000);
                        }
                        catch (Exception)
                        {
                            //ignore
                        }

                        _onlineDevices.Remove(device.Key);

                        try
                        {
                            DeviceConnectionSemaphore.Release();
                        }
                        catch (Exception)
                        {
                            //ignore
                        }




                        Logger.Log("A device has been disconnected");
                        _mCount--;
                        Logger.Log(
                            $"Connected : {_mCount}, disconnected Device type: {_deviceTypes[deviceType]}, DeviceID: {deviceId}.",
                            logType: LogType.Information);
                        break;
                    }
                }
            }

            Task.Run(async () =>
            {
                var connectionStatus = new ConnectionStatus
                {
                    DeviceId = deviceModel?.DeviceId ?? (int)deviceId,
                    IsConnected = false
                };

                try
                {
                    var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                    restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                    await _monitoringRestClient.ExecuteAsync<ResultViewModel>(restRequest, ServiceCancellationToken);

                    await _supremaLogService.AddLog(new Log
                    {
                        DeviceId = (int)deviceId,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.Disconnect
                    });
                }
                catch (Exception)
                {
                    //ignore
                }
            });

            return BSSDK.BS_SUCCESS;
        }

        public int LogProc(int inputHandle, uint inputDeviceId, int deviceType, int connectionType, IntPtr data)
        {
            var handle = Convert.ToInt32(inputHandle);
            var deviceId = Convert.ToUInt32(inputDeviceId);

            if (deviceType == BSSDK.BS_DEVICE_FSTATION ||
                deviceType == BSSDK.BS_DEVICE_BIOSTATION2 ||
                deviceType == BSSDK.BS_DEVICE_DSTATION ||
                deviceType == BSSDK.BS_DEVICE_XSTATION)
            {
                try
                {
                    var packet = new byte[Marshal.SizeOf(typeof(BSLogRecordEx))];
                    var tempPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSLogRecordEx)));
                    Marshal.Copy(data, packet, 0, Marshal.SizeOf(typeof(BSLogRecordEx)));

                    Marshal.Copy(packet, 0, tempPtr, Marshal.SizeOf(typeof(BSLogRecordEx)));

                    var logRecord = (BSLogRecordEx)Marshal.PtrToStructure(tempPtr, typeof(BSLogRecordEx));
                    Task.Run(() => LogProcMethodEx(handle, deviceId, deviceType, logRecord));

                    Marshal.FreeHGlobal(tempPtr);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Error with parsing data, ExStructure");
                }
            }

            else
            {
                try
                {
                    var packet = new byte[Marshal.SizeOf(typeof(BSLogRecord))];
                    var tempPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSLogRecord)));
                    Marshal.Copy(data, packet, 0, Marshal.SizeOf(typeof(BSLogRecord)));

                    Marshal.Copy(packet, 0, tempPtr, Marshal.SizeOf(typeof(BSLogRecord)));

                    var logRecord = (BSLogRecord)Marshal.PtrToStructure(data, typeof(BSLogRecord));
                    Task.Run(() => LogProcMethod(handle, deviceId, deviceType, logRecord));
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Error with parsing data");
                }
            }

            try
            {
                //Logger.Log("LogProc callback! " + DateTime.Now);
                return BSSDK.BS_SUCCESS;
            }
            catch (Exception)
            {
                return BSSDK.BS_ERR_CHANNEL_CLOSED;
            }
        }

        /// <summary>
        /// <En>Callback function on receiving log from devices. Store the log data in database.</En>
        /// <Fa>در هنگام دریافت گزارش از دستگاه ها، داده های دریافتی را در بانک ذخیره می کند.</Fa>
        /// </summary>
        /// <param name="handle">شماره هر ساعت در برنامه پس از اتصال</param>
        /// <param name="deviceId">شناسه ی ساعت</param>
        /// <param name="deviceType">نوع ساعت</param>
        /// <param name="logRecord"></param>
        /// <returns></returns>
        public int LogProcMethodEx(int handle, uint deviceId, int deviceType, BSLogRecordEx logRecord)
        {
            //var receivedTime = DateTime.Now;
            //var device =
            //    _onlineDevices.FirstOrDefault(
            //        dev =>
            //            dev.Value.GetDeviceInfo().DeviceId == deviceId && dev.Value.GetDeviceInfo().Handle == handle);

            var receivedLog = new SupremaLog();

            var device = _deviceService.GetDevices(code:deviceId, brandId: DeviceBrands.SupremaCode).FirstOrDefault();

            lock (_logObject)
            {
                if (logRecord.Event == 40)
                    logRecord.userID = 0;
                if (device != null)
                {
                    receivedLog.DeviceId = device.DeviceId;
                    // receivedLog.EventLog = logRecord.Event;
                    receivedLog.EventLog = _supremaCodeMappings.GetLogEventGenericLookup(logRecord.Event) ??
                                           new Lookup {Code = logRecord.Event.ToString()};
                    receivedLog.DateTimeTicks = (uint) logRecord.eventTime;
                    receivedLog.Reserved = logRecord.reserved1;
                    receivedLog.TnaEvent = logRecord.tnaKey;
                    receivedLog.SubEvent = _supremaCodeMappings.GetLogSubEventGenericLookup(logRecord.subEvent) ??
                                           new Lookup {Code = logRecord.subEvent.ToString()};
                    receivedLog.UserId = (int) logRecord.userID;

                    if (receivedLog.EventLog.Code == "16001" || receivedLog.EventLog.Code == "16002" ||
                        receivedLog.EventLog.Code == "16007")
                    {
                        receivedLog.UserId = 0;
                    }

                    switch (logRecord.subEvent)
                    {
                        case 0x3A:
                            receivedLog.MatchingType = _matchingTypes.Finger;
                            break;
                        case 0x3B:
                            //User has been verified by(Finger + PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x3D:
                            receivedLog.MatchingType = _matchingTypes.Face;
                            break;

                        case 0x3E:
                            //User has been verified by(Face + PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x2B:
                            //User has been verified by(ID + Finger)
                            receivedLog.MatchingType = _matchingTypes.Finger;
                            break;
                        case 0x2C:
                            //User has been verified by (ID+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x2D:
                            //User has been verified by (Card+Finger)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x2E:
                            //User has been verified by (Card+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x2F:
                            receivedLog.MatchingType = _matchingTypes.Card;
                            break;
                        case 0x30:
                            //User has been verified by (Card+Finger+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x31:
                            //User has been verified by (ID+Finger+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x32:
                            //User has been verified by (ID+Face)
                            receivedLog.MatchingType = _matchingTypes.Face;
                            break;
                        case 0x33:
                            //User has been verified by (Card+Face)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x34:
                            //User has been verified by (Card+Face+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                        case 0x35:
                            //User has been verified by (FACE+PIN)
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                            break;
                    }


                    if (receivedLog.MatchingType is null)
                    {
                        if (logRecord.Event == 55 || logRecord.Event == 56 || logRecord.Event == 109 ||
                            logRecord.Event == 99)
                        {
                            receivedLog.MatchingType = _matchingTypes.Unknown;
                        }
                        else
                        {
                            receivedLog.MatchingType = _matchingTypes.UnIdentify;
                        }
                    }

                    Logger.Log($@"nReaderIdn : {device.Code}
    EventId : {receivedLog.EventLog.Code}
    nDateTime : {receivedLog.DateTimeTicks}
    DateTime : {receivedLog.LogDateTime}
    nReserved : {receivedLog.Reserved}
    TnaEvent : {receivedLog.TnaEvent}
    SubEvent : {receivedLog.SubEvent.Code}
    sUserID : {receivedLog.UserId}
    _matchingTypes:{receivedLog.MatchingType.Code}", logType: LogType.Information);
                }

                //--------------------------------------------------------------------------------------------------------------------------
                //logService.InsertLog(receivedLog, "Personnel");
                //--------------------------------------------------------------------------------------------------------------------------

                //if ((receivedLog.EventId == 55 || receivedLog.EventId == 58 || receivedLog.EventId == 61) && (deviceId == 542183962 || deviceId == 542183337 || deviceId == 20140))
                //{
                //Environment.SetEnvironmentVariable("BIOVATION_HOME", targetDir, EnvironmentVariableTarget.Machine);


                //if (receivedLog.EventId == 55 || receivedLog.EventId == 58 || receivedLog.EventId == 61)
                //{
                //    var biovationHomeDir = Environment.GetEnvironmentVariable("BIOVATION_HOME");

                //    if (string.IsNullOrEmpty(biovationHomeDir))
                //    {
                //        biovationHomeDir = @".\";
                //    }
                //    var eventTime = new DateTime(1970, 1, 1).AddTicks((long)receivedLog.DateTimeTicks * 10000000);
                //    Logger.Log($"Event Time: {eventTime}Steps_SourceDatabase_");

                //    File.AppendAllText(biovationHomeDir + deviceId + "-Logs" + ".txt",
                //                                Environment.NewLine + "User id: " + receivedLog.UserId
                //                                + Environment.NewLine + "Log received time: " + receivedTime + " :" + receivedTime.Millisecond
                //                                + Environment.NewLine + "Log converted time: " + convertedTime + " :" + convertedTime.Millisecond
                //                                + Environment.NewLine + "Log inserted time: " + insertedTime + " :" + insertedTime.Millisecond
                //                                + Environment.NewLine + "Event time: " + eventTime + ":" + eventTime.Millisecond
                //                                + Environment.NewLine);
                //}

                if (deviceType == BSSDK.BS_DEVICE_FSTATION && (logRecord.Event == 55 || logRecord.Event == 61))
                {
                    try
                    {
                        var imageLog =
                            new byte[Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)) + BSSDK.DF_LEN_MAX_IMAGE];

                        var dataLen = 0;

                        int eventType = logRecord.Event;
                        var nEventTime = logRecord.eventTime;
                        var numOfLog = 0;

                        BSSDK.BS_GetImageLogCount(handle, ref numOfLog);
                        BSSDK.BS_ReadSpecificImageLog(handle, nEventTime, eventType, ref dataLen, imageLog);

                        if (dataLen > Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)))
                        {
                            var packet = GCHandle.Alloc(imageLog, GCHandleType.Pinned);
                            var imageHdr =
                                (BSSDK.BSImageLogHdr)
                                Marshal.PtrToStructure(packet.AddrOfPinnedObject(), typeof(BSSDK.BSImageLogHdr));
                            var imageSize = (int)imageHdr.imageSize;
                            packet.Free();

                            if (imageSize > 0 && imageSize < BSSDK.DF_LEN_MAX_IMAGE)
                            {
                                var imageBytes = new byte[imageSize];
                                Buffer.BlockCopy(imageLog, Marshal.SizeOf(typeof(BSSDK.BSImageLogHdr)), imageBytes,
                                    0, imageBytes.Length);

                                //var faceLog = new FaceLogModel
                                //{
                                //    DateTime = logRecord.eventTime,
                                //    EventIdn = Convert.ToInt32(logRecord.Event),
                                //    ReaderIdn = device.Code,
                                //    UserID = logRecord.userID,
                                //    FaceImageLen = (uint)imageSize,
                                //    Type = 1,
                                //    Image = imageBytes
                                //};

                                //var faceLog = new Log
                                //{
                                //    DateTimeTicks = (uint)logRecord.eventTime,
                                //    EventId = Convert.ToInt32(logRecord.Event),
                                //    DeviceId = device.DeviceId,
                                //    UserId = logRecord.userID,
                                //    // FaceImageLen = (uint)imageSize,
                                //    // Type = 1,
                                //    PicByte = imageBytes
                                //};

                                receivedLog.PicByte = imageBytes;

                                Logger.Log("In LogProcMethod");

                                //var logService = new LogServices();
                                //--------------------------------------------------------------------------------------------------------------------------
                                //logService.InsertFaceLog(faceLog, "Personnel");
                                _supremaLogService.AddLog(receivedLog);
                                //--------------------------------------------------------------------------------------------------------------------------

                                //try
                                //{
                                //    String filename = "SupremaImageLog\\" + deviceId + "-" + logRecord.userID + "-" + DateTime.Now.ToLongDateString() + "-" + DateTime.Now.ToLongTimeString() + ".jpg";
                                //    filename = filename.Replace(@":", @"-");
                                //    filename = @"C:\" + filename;

                                //    FileStream fs = new FileStream(filename, FileMode.Create,
                                //        FileAccess.ReadWrite);
                                //    BinaryWriter writeFs = new BinaryWriter(fs);
                                //    writeFs.Write(imageBytes);
                                //    writeFs.Close();
                                //    fs.Close();
                                //}
                                //catch (Exception exception)
                                //{
                                //    Logger.Log(exception);
                                //}
                            }
                        }

                    }
                    catch (IOException exception)
                    {
                        Logger.Log(exception);
                    }
                }

                else
                {
                    _supremaLogService.AddLog(receivedLog);
                }
            }

            return BSSDK.BS_SUCCESS;
        }

        public int LogProcMethod(int handle, uint deviceId, int deviceType, BSLogRecord logRecord)
        {


            //var device =
            //    _onlineDevices.FirstOrDefault(
            //        dev =>
            //            dev.Value.GetDeviceInfo().DeviceId == deviceId && dev.Value.GetDeviceInfo().Handle == handle);

            var device = _deviceService.GetDevices(code:deviceId, brandId: DeviceBrands.SupremaCode).FirstOrDefault();


            if (device != null)
            {
                var receivedLog = new SupremaLog
                {
                    DeviceId = device.DeviceId,
                    EventLog = _supremaCodeMappings.GetLogEventGenericLookup(logRecord.Event) ?? new Lookup { Code = logRecord.Event.ToString() },
                    DateTimeTicks = (uint)logRecord.eventTime,
                    Reserved = logRecord.reserved,
                    TnaEvent = logRecord.tnaEvent,
                    SubEvent = _supremaCodeMappings.GetLogSubEventGenericLookup(logRecord.subEvent) ?? new Lookup { Code = logRecord.subEvent.ToString() },
                    UserId = (int)logRecord.userID
                };
                if (receivedLog.EventLog.Code == "16001" || receivedLog.EventLog.Code == "16002" || receivedLog.EventLog.Code == "16007")
                {
                    receivedLog.UserId = 0;
                }
                switch (logRecord.subEvent)
                {
                    case 0x3A:
                        receivedLog.MatchingType = _matchingTypes.Finger;
                        break;
                    case 0x3B:
                        //User has been verified by(Finger + PIN)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x3D:
                        receivedLog.MatchingType = _matchingTypes.Face;
                        break;

                    case 0x3E:
                        //User has been verified by(Face + PIN)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x2B:
                        //User has been verified by(ID + Finger)
                        receivedLog.MatchingType = _matchingTypes.Finger;
                        break;
                    case 0x2C:
                        //User has been verified by (ID+PIN)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x2D:
                        //User has been verified by (Card+Finger)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x2E:
                        //User has been verified by (Card+PIN)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x2F:
                        receivedLog.MatchingType = _matchingTypes.Card;
                        break;
                    case 0x30:
                        //User has been verified by (Card+Finger+PIN)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x31:
                        //User has been verified by (ID+Finger+PIN)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x32:
                        //User has been verified by (ID+Face)
                        receivedLog.MatchingType = _matchingTypes.Face;
                        break;
                    case 0x33:
                        //User has been verified by (Card+Face)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x34:
                        //User has been verified by (Card+Face+PIN)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                    case 0x35:
                        //User has been verified by (FACE+PIN)
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                        break;
                }

                if (receivedLog.MatchingType is null)
                {
                    if (logRecord.Event == 55 || logRecord.Event == 56 || logRecord.Event == 109 || logRecord.Event == 99)
                    {
                        receivedLog.MatchingType = _matchingTypes.Unknown;
                    }
                    else
                    {
                        receivedLog.MatchingType = _matchingTypes.UnIdentify;
                    }
                }


                //var convertedTime = DateTime.Now;

                Logger.Log($@"nReaderIdn : {deviceId}
    EventId : {receivedLog.EventLog.Code}
    nDateTime : {receivedLog.DateTimeTicks}
    DateTime : {receivedLog.LogDateTime}
    nReserved : {receivedLog.Reserved}
    TnaEvent : {receivedLog.TnaEvent}
    SubEvent : {receivedLog.SubEvent.Code}
    sUserID : {receivedLog.UserId}
    _matchingTypes:{receivedLog.MatchingType.Code}", logType: LogType.Information);

                //--------------------------------------------------------------------------------------------------------------------------
                lock (_logObject)
                {
                    _supremaLogService.AddLog(receivedLog);
                }
            }

            //--------------------------------------------------------------------------------------------------------------------------
            //var insertedTime = DateTime.Now;

            //if ((receivedLog.EventId == 55 || receivedLog.EventId == 58 || receivedLog.EventId == 61) && (deviceId == 542183962 || deviceId == 542183337 || deviceId == 20140))
            //{
            //if (receivedLog.EventId == 55 || receivedLog.EventId == 58 || receivedLog.EventId == 61)
            //{
            //    var biovationHomeDir = Environment.GetEnvironmentVariable("BIOVATION_HOME");
            //    if (string.IsNullOrEmpty(biovationHomeDir))
            //    {
            //        biovationHomeDir = @".\";
            //    }
            //    var eventTime = new DateTime(1970, 1, 1).AddTicks((long)receivedLog.nDateTime * 10000000);
            //    File.AppendAllText(biovationHomeDir + deviceId + "-Logs" + ".txt",
            //                                Environment.NewLine + "User id: " + receivedLog.sUserID
            //                                + Environment.NewLine + "Log received time: " + receivedTime + " :" + receivedTime.Millisecond
            //                                + Environment.NewLine + "Log converted time: " + convertedTime + " :" + convertedTime.Millisecond
            //                                + Environment.NewLine + "Log inserted time: " + insertedTime + " :" + insertedTime.Millisecond
            //                                + Environment.NewLine + "Event time: " + eventTime + ":" + eventTime.Millisecond
            //                                + Environment.NewLine);
            //}

            return BSSDK.BS_SUCCESS;
        }

        public object RawDeserialize(byte[] rawData, int position, Type anyType)
        {
            var rawSize = Marshal.SizeOf(anyType);
            if (rawSize > rawData.Length)
                return null;

            var buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.Copy(rawData, position, buffer, rawSize);
            var returnedObj = Marshal.PtrToStructure(buffer, anyType);
            Marshal.FreeHGlobal(buffer);

            return returnedObj;
        }

        public static byte[] RawSerialize(object anything)
        {
            var rawSize = Marshal.SizeOf(anything);
            var buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(anything, buffer, false);
            var rawDatas = new byte[rawSize];
            Marshal.Copy(buffer, rawDatas, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return rawDatas;
        }

        public int ImageLogProc(int handle, uint deviceId, int deviceType, int connectionType, IntPtr data, int dataLen)
        {
            //return ImageLogProcMethod(handle, deviceId, deviceType, connectionType, data, dataLen);
            return BSSDK.BS_SUCCESS;
        }

        public int ImageLogProcMethod(int handle, uint deviceId, int deviceType, int connectionType, IntPtr data, int dataLen)
        {
            if (deviceType == BSSDK.BS_DEVICE_FSTATION ||
                deviceType == BSSDK.BS_DEVICE_BIOSTATION2 ||
                deviceType == BSSDK.BS_DEVICE_DSTATION ||
                deviceType == BSSDK.BS_DEVICE_XSTATION)
            {
                try
                {
                    var packet = new byte[dataLen];
                    Marshal.Copy(data, packet, 0, dataLen);

                    // copy header
                    var ptrHeader = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSImageLogHdr)));
                    Marshal.Copy(packet, 0, ptrHeader, Marshal.SizeOf(typeof(BSImageLogHdr)));
                    var imageLogHdr = (BSImageLogHdr)Marshal.PtrToStructure(ptrHeader, typeof(BSImageLogHdr));

                    // copy image data
                    var ptrImageData = Marshal.AllocHGlobal((int)imageLogHdr.imageSize);
                    Marshal.Copy(packet, Marshal.SizeOf(typeof(BSImageLogHdr)), ptrImageData, (int)imageLogHdr.imageSize);

                    //String filename = "SupremaImageLog\\" + deviceId + "-" + imageLogHdr.userID + "-" + DateTime.Now.ToLongDateString() + "-" + DateTime.Now.ToLongTimeString() + ".jpg";
                    //filename = filename.Replace(@":", @"-");
                    //filename = @"C:\" + filename;
                    //filename += imageLogHdr.deviceID + "_" + imageLogHdr.Event + "_" + imageLogHdr.eventTime + ".jpg";

                    //FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                    var byteArray = new byte[imageLogHdr.imageSize];
                    Marshal.Copy(ptrImageData, byteArray, 0, (int)imageLogHdr.imageSize);

                    //var faceLog = new FaceLogModel
                    //{
                    //    DateTime = imageLogHdr.eventTime,
                    //    EventIdn = Convert.ToInt32(imageLogHdr.Event),
                    //    ReaderIdn = deviceId,
                    //    UserID = imageLogHdr.userID,
                    //    FaceImageLen = imageLogHdr.imageSize,
                    //    Type = 1,
                    //    Image = byteArray
                    //};

                    var faceLog = new Log
                    {
                        DateTimeTicks = (uint)imageLogHdr.eventTime,
                        //EventLog =SupremaLogService Convert.ToInt32(imageLogHdr.Event),
                        EventLog = _supremaCodeMappings.GetLogEventGenericLookup(imageLogHdr.Event),
                        DeviceId = (int)deviceId,
                        UserId = imageLogHdr.userID,
                        //FaceImageLen = imageLogHdr.imageSize,
                        //Type = 1,
                        PicByte = byteArray
                    };

                    Logger.Log($@"ImageLog Callback
    nReaderIdn : {faceLog.DeviceCode}
    nSubEventIdn : {imageLogHdr.subEvent}
    nDateTime : {faceLog.LogDateTime}
    sUserID : {faceLog.UserId}", logType: LogType.Information);

                    //var logService = new LogService();

                    //logService.InsertFaceLog(faceLog, "Personnel");
                    _logService.AddLog(faceLog);
                    //fs.Write(byteArray, 0, (int)imageLogHdr.imageSize);
                    //fs.Close();

                    Marshal.FreeHGlobal(ptrHeader);
                    Marshal.FreeHGlobal(ptrImageData);

                }
                catch (IOException exception)
                {
                    Logger.Log(exception);
                }
            }

            return BSSDK.BS_SUCCESS;
        }

        public static byte[] StrToByteArray(string str)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public int RequestMatchingProc(int handle, uint deviceId, int deviceType, int connectionType, int matchingType, uint id, IntPtr templateData, IntPtr userHdr, ref int isDuress)
        {
            //RequestMatchingProcMethod(handle, deviceId, deviceType, connectionType, matchingType,
            //    id, templateData, userHdr, ref isDuress);
            Logger.Log("RequestMatchingProc callback! " + DateTime.Now);
            return BSSDK.BS_SUCCESS;
        }

        public int RequestMatchingProcMethod(int handle, uint deviceId, int deviceType, int connectionType, int matchingType, uint id, IntPtr templateData, IntPtr userHdr, ref int isDuress)
        {
            uint userId = 0;

            lock (_onlineDevices)
            {
                foreach (var device in _onlineDevices)
                {
                    if (device.Value.GetDeviceInfo().Code == deviceId && device.Value.GetDeviceInfo().Handle == handle)
                    {
                        userId = matchingType == BSSDK.REQUEST_IDENTIFY ? 200 : id;
                        break;
                    }
                }
            }

            if (_mMatchingFail)
            {
                return BSSDK.BS_ERR_NOT_FOUND;
            }

            if (deviceType == BSSDK.BS_DEVICE_BIOENTRY_PLUS || deviceType == BSSDK.BS_DEVICE_BIOENTRY_W)
            {
                var beUserHdr = (BEUserHdr)Marshal.PtrToStructure(userHdr, typeof(BEUserHdr));

                beUserHdr.version = 0;
                beUserHdr.userID = userId;
                beUserHdr.startTime = 0;
                beUserHdr.expiryTime = 0;
                beUserHdr.cardID = 0;

                beUserHdr.cardCustomID = 0;
                beUserHdr.commandCardFlag = 0;
                beUserHdr.cardFlag = 0;
                beUserHdr.cardVersion = BSSDK.BE_CARD_VERSION_1;

                beUserHdr.adminLevel = BSSDK.BE_USER_LEVEL_NORMAL;
                beUserHdr.securityLevel = BSSDK.BE_USER_SECURITY_DEFAULT;

                beUserHdr.accessGroupMask = 0xFFFFFFFF;

                beUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(beUserHdr, userHdr, true);
            }
            else if (deviceType == BSSDK.BS_DEVICE_BIOLITE)
            {
                var beUserHdr = (BEUserHdr)Marshal.PtrToStructure(userHdr, typeof(BEUserHdr));

                beUserHdr.version = 0;
                beUserHdr.userID = userId;
                beUserHdr.startTime = 0;
                beUserHdr.expiryTime = 0;
                beUserHdr.cardID = 0;

                beUserHdr.cardCustomID = 0;
                beUserHdr.commandCardFlag = 0;
                beUserHdr.cardFlag = 0;
                beUserHdr.cardVersion = BSSDK.BE_CARD_VERSION_1;

                beUserHdr.adminLevel = BSSDK.BE_USER_LEVEL_NORMAL;
                beUserHdr.securityLevel = BSSDK.BE_USER_SECURITY_DEFAULT;

                beUserHdr.accessGroupMask = 0xFFFFFFFF;

                beUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(beUserHdr, userHdr, true);

            }
            else if (deviceType == BSSDK.BS_DEVICE_BIOSTATION)
            {
                var bsUserHdr = (BSUserHdrEx)Marshal.PtrToStructure(userHdr, typeof(BSUserHdrEx));

                bsUserHdr.ID = userId;
                bsUserHdr.accessGroupMask = 0xffffffff; // no access group

                // name 
                var username = "Test User";
                var nameBytes = Encoding.ASCII.GetBytes(username);             // UTF8
                Buffer.BlockCopy(nameBytes, 0, bsUserHdr.name, 0, nameBytes.Length);


                bsUserHdr.adminLevel = BSSDK.BS_USER_NORMAL;
                bsUserHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;
                bsUserHdr.duressMask = 0x00; // no duress finger
                bsUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(bsUserHdr, userHdr, true);
            }
            else if (deviceType == BSSDK.BS_DEVICE_DSTATION)
            {
                var bsUserHdr = (DSUserHdr)Marshal.PtrToStructure(userHdr, typeof(DSUserHdr));

                bsUserHdr.ID = userId;
                bsUserHdr.accessGroupMask = 0xffffffff; // no access group


                var username = "Test User";
                var nameBytes = Encoding.Unicode.GetBytes(username);             // UTF16
                Buffer.BlockCopy(nameBytes, 0, bsUserHdr.name, 0, nameBytes.Length);


                bsUserHdr.adminLevel = BSSDK.BS_USER_NORMAL;
                bsUserHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;
                bsUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(bsUserHdr, userHdr, true);
            }
            else if (deviceType == BSSDK.BS_DEVICE_XSTATION)
            {
                var bsUserHdr = (XSUserHdr)Marshal.PtrToStructure(userHdr, typeof(XSUserHdr));

                bsUserHdr.ID = userId;
                bsUserHdr.accessGroupMask = 0xffffffff; // no access group


                var username = "Test User";
                var nameBytes = Encoding.Unicode.GetBytes(username);             // UTF16
                Buffer.BlockCopy(nameBytes, 0, bsUserHdr.name, 0, nameBytes.Length);

                bsUserHdr.adminLevel = BSSDK.BS_USER_NORMAL;
                bsUserHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;

                Marshal.StructureToPtr(bsUserHdr, userHdr, true);
            }
            else if (deviceType == BSSDK.BS_DEVICE_BIOSTATION2)
            {

                var bsUserHdr = (BS2UserHdr)Marshal.PtrToStructure(userHdr, typeof(BS2UserHdr));

                bsUserHdr.ID = userId;
                bsUserHdr.accessGroupMask = 0xffffffff; // no access group


                var username = "Test User";
                var nameBytes = Encoding.Unicode.GetBytes(username);             // UTF16
                Buffer.BlockCopy(nameBytes, 0, bsUserHdr.name, 0, nameBytes.Length);

                bsUserHdr.adminLevel = BSSDK.BS_USER_NORMAL;
                bsUserHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;
                bsUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(bsUserHdr, userHdr, true);
            }

            isDuress = BSSDK.NORMAL_FINGER;

            return BSSDK.BS_SUCCESS;
        }

        public  int RequestUserInfoProc(int handle, uint deviceId, int deviceType, int connectionType, int idType, uint id, uint customId, IntPtr userHdr)
        {
            return RequestUserInfoProcMethod(handle, deviceId, deviceType, connectionType, idType, id, customId, userHdr);

        }
        public int RequestUserInfoProcMethod(int handle, uint deviceId, int deviceType, int connectionType, int idType, uint id, uint customId, IntPtr userHdr)
        {
            lock (_onlineDevices)
            {
                foreach (var device in _onlineDevices)
                {
                    if (device.Value.GetDeviceInfo().Code == deviceId && device.Value.GetDeviceInfo().Handle == handle)
                    {
                        if (idType == BSSDK.ID_USER)
                            //                        itemString = "ID Request";
                            //                    else
                            //                        itemString = "Card ID Request";

                            //deviceList[deviceId].log = itemString;
                            break;
                    }
                }
            }


            if (_mMatchingFail)
            {
                return BSSDK.BS_ERR_NOT_FOUND;
            }

            if (deviceType == BSSDK.BS_DEVICE_BIOENTRY_PLUS || deviceType == BSSDK.BS_DEVICE_BIOENTRY_W)
            {
                var beUserHdr = (BEUserHdr)Marshal.PtrToStructure(userHdr, typeof(BEUserHdr));

                beUserHdr.version = 0;
                beUserHdr.userID = id;
                beUserHdr.startTime = 0;
                beUserHdr.expiryTime = 0;
                beUserHdr.cardID = 0;

                beUserHdr.cardCustomID = 0;
                beUserHdr.commandCardFlag = 0;
                beUserHdr.cardFlag = 0;
                beUserHdr.cardVersion = BSSDK.BE_CARD_VERSION_1;

                beUserHdr.adminLevel = BSSDK.BE_USER_LEVEL_NORMAL;
                beUserHdr.securityLevel = BSSDK.BE_USER_SECURITY_DEFAULT;

                beUserHdr.accessGroupMask = 0xFFFFFFFF;

                beUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(beUserHdr, userHdr, true);

            }
            else if (deviceType == BSSDK.BS_DEVICE_XPASS || deviceType == BSSDK.BS_DEVICE_XPASS_SLIM)
            {
                var beUserHdr = (BEUserHdr)Marshal.PtrToStructure(userHdr, typeof(BEUserHdr));

                beUserHdr.version = 0;
                beUserHdr.userID = id;
                beUserHdr.startTime = 0;
                beUserHdr.expiryTime = 0;
                beUserHdr.cardID = 0;

                beUserHdr.cardCustomID = 0;
                beUserHdr.commandCardFlag = 0;
                beUserHdr.cardFlag = 0;
                beUserHdr.cardVersion = BSSDK.BE_CARD_VERSION_1;

                beUserHdr.adminLevel = BSSDK.BE_USER_LEVEL_NORMAL;
                beUserHdr.securityLevel = BSSDK.BE_USER_SECURITY_DEFAULT;

                beUserHdr.accessGroupMask = 0xFFFFFFFF;

                beUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(beUserHdr, userHdr, true);

            }
            else if (deviceType == BSSDK.BS_DEVICE_BIOLITE)
            {
                var beUserHdr = (BEUserHdr)Marshal.PtrToStructure(userHdr, typeof(BEUserHdr));

                beUserHdr.version = 0;
                beUserHdr.userID = id;
                beUserHdr.startTime = 0;
                beUserHdr.expiryTime = 0;
                beUserHdr.cardID = 0;

                beUserHdr.cardCustomID = 0;
                beUserHdr.commandCardFlag = 0;
                beUserHdr.cardFlag = 0;
                beUserHdr.cardVersion = BSSDK.BE_CARD_VERSION_1;

                beUserHdr.adminLevel = BSSDK.BE_USER_LEVEL_NORMAL;
                beUserHdr.securityLevel = BSSDK.BE_USER_SECURITY_DEFAULT;

                beUserHdr.accessGroupMask = 0xFFFFFFFF;

                beUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(beUserHdr, userHdr, true);
            }
            else if (deviceType == BSSDK.BS_DEVICE_BIOSTATION)
            {
                var bsUserHdr = (BSUserHdrEx)Marshal.PtrToStructure(userHdr, typeof(BSUserHdrEx));

                bsUserHdr.ID = id;
                bsUserHdr.accessGroupMask = 0xffffffff; // no access group

                // name 
                var username = "Test User";
                var nameBytes = Encoding.ASCII.GetBytes(username);             // UTF8
                Buffer.BlockCopy(nameBytes, 0, bsUserHdr.name, 0, nameBytes.Length);

                bsUserHdr.adminLevel = BSSDK.BS_USER_NORMAL;
                bsUserHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;
                bsUserHdr.duressMask = 0x00; // no duress finger
                bsUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(bsUserHdr, userHdr, true);
            }
            else if (deviceType == BSSDK.BS_DEVICE_DSTATION)
            {
                var bsUserHdr = (DSUserHdr)Marshal.PtrToStructure(userHdr, typeof(DSUserHdr));

                bsUserHdr.ID = id;
                bsUserHdr.accessGroupMask = 0xffffffff; // no access group

                var username = "Test User";
                var nameBytes = Encoding.Unicode.GetBytes(username);             // UTF16
                Buffer.BlockCopy(nameBytes, 0, bsUserHdr.name, 0, nameBytes.Length);


                bsUserHdr.adminLevel = BSSDK.BS_USER_NORMAL;
                bsUserHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;
                bsUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(bsUserHdr, userHdr, true);
            }
            else if (deviceType == BSSDK.BS_DEVICE_XSTATION)
            {
                var bsUserHdr = (XSUserHdr)Marshal.PtrToStructure(userHdr, typeof(XSUserHdr));

                bsUserHdr.ID = id;
                bsUserHdr.accessGroupMask = 0xffffffff; // no access group

                var username = "Test User";
                var nameBytes = Encoding.Unicode.GetBytes(username);             // UTF16
                Buffer.BlockCopy(nameBytes, 0, bsUserHdr.name, 0, nameBytes.Length);


                bsUserHdr.adminLevel = BSSDK.BS_USER_NORMAL;
                bsUserHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;

                Marshal.StructureToPtr(bsUserHdr, userHdr, true);
            }
            else if (deviceType == BSSDK.BS_DEVICE_BIOSTATION2)
            {
                var bsUserHdr = (BS2UserHdr)Marshal.PtrToStructure(userHdr, typeof(BS2UserHdr));

                bsUserHdr.ID = id;
                bsUserHdr.accessGroupMask = 0xffffffff; // no access group

                var username = "Test User";
                var nameBytes = Encoding.Unicode.GetBytes(username);             // UTF16
                Buffer.BlockCopy(nameBytes, 0, bsUserHdr.name, 0, nameBytes.Length);


                bsUserHdr.adminLevel = BSSDK.BS_USER_NORMAL;
                bsUserHdr.securityLevel = BSSDK.BS_USER_SECURITY_DEFAULT;
                bsUserHdr.numOfFinger = 1;

                Marshal.StructureToPtr(bsUserHdr, userHdr, true);

            }

            return BSSDK.BS_SUCCESS;
        }

        public  void StartReadLogs()
        {
            if (_readingLogsInProgress)
                return;

            _readingLogsInProgress = true;
            while (true)
            {
                KeyValuePair<uint, Task> logReader;
                lock (LogReaderQueue)
                {
                    if (LogReaderQueue.Count <= 0)
                    {
                        _readingLogsInProgress = false;
                        return;
                    }

                    logReader = LogReaderQueue.Dequeue();
                }

                lock (_onlineDevices)
                {
                    if (!_onlineDevices.ContainsKey(logReader.Key)) continue;

                    logReader.Value.Start();
                    Task.WaitAll(logReader.Value);
                }
            }
        }

        public void StartHandleOfflineEvents()
        {
            if (_handlingOfflineEventsInProgress)
                return;

            _handlingOfflineEventsInProgress = true;
            while (true)
            {
                KeyValuePair<uint, Task> offlineEventHandler;
                lock (OfflineEventHandlersQueue)
                {
                    if (OfflineEventHandlersQueue.Count <= 0)
                    {
                        _handlingOfflineEventsInProgress = false;
                        return;
                    }

                    offlineEventHandler = OfflineEventHandlersQueue.Dequeue();
                }

                lock (_onlineDevices)
                {
                    if (!_onlineDevices.ContainsKey(offlineEventHandler.Key)) continue;

                    offlineEventHandler.Value.Start();
                    Task.WaitAll(offlineEventHandler.Value);
                }
            }
        }

       
    }
}
