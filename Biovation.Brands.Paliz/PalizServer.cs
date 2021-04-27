﻿using Biovation.Brands.Paliz.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using PalizTiara.Api;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Models;
using PalizTiara.Protocol.Utility;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.Paliz.Command;
using System.Collections.ObjectModel;

namespace Biovation.Brands.Paliz
{
    public class PalizServer
    {
        private readonly UserService _commonUserService;
        private readonly DeviceService _commonDeviceService;
        private readonly UserCardService _commonUserCardService;
        private readonly AccessGroupService _commonAccessGroupService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly AccessGroupService _accessGroupService;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly TaskService _taskService;
        internal readonly LogService _logService;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskItemTypes _taskItemTypes;

        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly PalizCodeMappings _codeMappings;
        private readonly LogEvents _logEvents;
        private readonly DeviceBrands _deviceBrands;
        private readonly BlackListService _blackListService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly RestClient _monitoringRestClient;
        private readonly ObservableCollection<DeviceLogDataModel> _deviceLogs =
            new ObservableCollection<DeviceLogDataModel>();

        public int NextLogPageNumber { get; set; }
        private BiovationConfigurationManager biovationConfiguration { get; }
        //private const string BiovationTopicName = "BiovationTaskStatusUpdateEvent";
        public static bool GetLogTaskFinished = true;
        private PalizCodeMappings _palizCodeMappings;
        private static Dictionary<uint, DeviceBasicInfo> _onlineDevices;
        private static List<string> _onlineTerminals;
        internal readonly TiaraServerManager _serverManager;
        internal readonly object LoadFingerTemplateLock = new object();

        public PalizServer(TiaraServerManager serverManager, Dictionary<uint, DeviceBasicInfo> onlineDevices, UserService commonUserService
            , DeviceService commonDeviceService, UserCardService commonUserCardService, AccessGroupService commonAccessGroupService, FingerTemplateService fingerTemplateService
            , LogService logService, BlackListService blackListService, FaceTemplateService faceTemplateService, TaskService taskService
            , AccessGroupService accessGroupService, BiovationConfigurationManager biovationConfiguration, TaskItemTypes taskItemTypes
            , FingerTemplateTypes fingerTemplateTypes, PalizCodeMappings codeMappings, DeviceBrands deviceBrands
            , LogEvents logEvents, FaceTemplateTypes faceTemplateTypes, BiometricTemplateManager biometricTemplateManager
            , TaskStatuses taskStatuses, PalizCodeMappings palizCodeMappings, TaskTypes taskTypes, TaskPriorities taskPriorities)
        {
            _onlineDevices = onlineDevices;
            _commonDeviceService = commonDeviceService;
            _commonUserService = commonUserService;
            _commonUserCardService = commonUserCardService;
            _commonAccessGroupService = commonAccessGroupService;
            _fingerTemplateService = fingerTemplateService;
            _logService = logService;
            _palizCodeMappings = palizCodeMappings;
            _blackListService = blackListService;
            _faceTemplateService = faceTemplateService;
            _taskService = taskService;

            _taskStatuses = taskStatuses;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskItemTypes = taskItemTypes;

            _accessGroupService = accessGroupService;
            this.biovationConfiguration = biovationConfiguration;
            //_virdiLogService = virdiLogService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _faceTemplateTypes = faceTemplateTypes;
            _codeMappings = codeMappings;
            _onlineDevices = onlineDevices;
            _logEvents = logEvents;
            _deviceBrands = deviceBrands;
            _monitoringRestClient = (RestClient)new RestClient(biovationConfiguration.LogMonitoringApiUrl).UseSerializer(() => new RestRequestJsonSerializer());
            _onlineTerminals = new List<string>();
            Trace.TraceLevel = TraceLevel.Error;
            Trace.TraceListener += Listen;
            _serverManager = serverManager;

            // initialize events
            _serverManager.LiveTrafficLogEvent += OnLiveTrafficLogEvent;
            _serverManager.DeviceInfoEvent += GetDeviceInfoCallback;
            _serverManager.TiaraSettingsEvent += GetTiaraSettingsCallback;

            //foreach (var device in _onlineDevices)
            //{
            //    if (device.IsOnline)
            //    {
            //        await this.serverManager.GetLiveTrafficLogAsyncTask(device.TerminalName);
            //    }
            //}
        }

        private void GetTiaraSettingsCallback(object sender, TiaraSettingsEventArgs args)
        {
            if (args?.TiaraSettings is null)
            {
                return;
            }

            var tiaraSettings = args.TiaraSettings;

            var terminalName = tiaraSettings.ServerSetting.TerminalName;
            var terminalId = Convert.ToUInt32(tiaraSettings.ServerSetting.TerminalId);

            var existedDevice = _commonDeviceService.GetDevices(code: (uint)terminalId, brandId: DeviceBrands.PalizCode).Data?.Data?.FirstOrDefault() ?? new DeviceBasicInfo();

            Task.Run(async () =>
            {
                var connectionStatus = new ConnectionStatus
                {
                    DeviceId = existedDevice?.DeviceId ?? 0,
                    IsConnected = true
                };

                try
                {
                    var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
                    restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

                    await _monitoringRestClient.ExecuteAsync<ResultViewModel>(restRequest);
                    //integration
                    var connectionStatusList = new List<ConnectionStatus> { connectionStatus };
                    var biovationBrokerMessageData = new List<DataChangeMessage<ConnectionStatus>>
                        {
                            new DataChangeMessage<ConnectionStatus>
                            {
                                Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                                TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = connectionStatusList
                            }
                        };

                    //_deviceConnectionStateInternalSource.PushData(biovationBrokerMessageData);

                    await _logService.AddLog(new Log
                    {
                        DeviceId = existedDevice?.DeviceId ?? 0,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.Connect
                    });
                }
                catch (Exception)
                {
                    //ignore
                }
            }).ConfigureAwait(false);

            DeviceBasicInfo device;
            if (existedDevice?.DeviceId != default(int))
            {
                device = new DeviceBasicInfo
                {
                    Code = terminalId,
                    DeviceId = existedDevice.DeviceId,
                    Name = terminalName,
                    Brand = existedDevice.Brand,
                    Model = existedDevice.Model,
                    IpAddress = tiaraSettings.ServerSetting.ServerIP,
                    Port = tiaraSettings.ServerSetting.ServerPortNumber,
                    MacAddress = existedDevice.MacAddress,
                    RegisterDate = existedDevice.RegisterDate,
                    TimeSync = existedDevice.TimeSync,
                    Active = existedDevice.Active,
                    DeviceTypeId = existedDevice.DeviceTypeId
                };

                if (existedDevice.Code != terminalId || !string.Equals(existedDevice.IpAddress, tiaraSettings.ServerSetting.ServerIP, StringComparison.InvariantCultureIgnoreCase) || tiaraSettings.ServerSetting.ServerPortNumber != biovationConfiguration.PalizDevicesConnectionPort)
                    _commonDeviceService.ModifyDevice(device);
            }
            else
            {
                device = new DeviceBasicInfo
                {
                    Code = terminalId,
                    Brand = _deviceBrands.Paliz,
                    Model = new DeviceModel { Id = 8001 },
                    IpAddress = tiaraSettings.ServerSetting.ServerIP,
                    Port = tiaraSettings.ServerSetting.ServerPortNumber,
                    MacAddress = tiaraSettings.LanSetting.LanMac ?? "",
                    RegisterDate = DateTime.Now,
                    TimeSync = true,
                    Active = true
                };

                //device.Name = terminalId + "[" + device.IpAddress + "]";
                device.Name = terminalName;
                var result = _commonDeviceService.ModifyDevice(device);
                if (result.Validate == 1)
                    device.DeviceId = (int)result.Id;
            }

            lock (_onlineDevices)
            {
                if (!_onlineDevices.ContainsKey(terminalId))
                    _onlineDevices.Add(terminalId, device);
            }

            Logger.Log($"Connected device: {{ ID: {terminalId} , IP: {device.IpAddress} }}", logType: LogType.Information);
            Logger.Log($"Retrieving new log: {{ ID: {terminalId} , IP: {device.IpAddress} }}", logType: LogType.Information);

            //User createrUser;
            var creatorUser = _commonUserService.GetUsers(code: 987654321).Data?.Data?.FirstOrDefault();
            //if (creatorUserData != null)
            //{
            //    createrUser = creatorUserData.FirstOrDefault();
            //}
            //else
            //{
            //    createrUser = new User
            //}

            //var lastLogsOfDevice = _logService.GetLastLogsOfDevice((uint)DeviceInfo.DeviceId).Result;

            var task = new TaskInfo
            {
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = creatorUser,
                TaskType = _taskTypes.GetLogs,
                Priority = _taskPriorities.Medium,
                TaskItems = new List<TaskItem>(),
                DeviceBrand = _deviceBrands.Paliz,
                DueDate = DateTimeOffset.Now
            };

            task.TaskItems.Add(new TaskItem
            {
                Status = _taskStatuses.Queued,
                TaskItemType = _taskItemTypes.GetLogs,
                Priority = _taskPriorities.Medium,
                DeviceId = device.DeviceId,
                Data = JsonConvert.SerializeObject(device.DeviceId),
                IsParallelRestricted = true,
                IsScheduled = false,
                OrderIndex = 1
            });

            _taskService.InsertTask(task);
            //AccessLogData.GetAccessLogCountFromTerminal(0, terminalId, (int)VirdiDeviceLogType.New);
            _taskService.ProcessQueue(_deviceBrands.Paliz, device.DeviceId).ConfigureAwait(false);
        }

        private void GetDeviceInfoCallback(object sender, DeviceInfoEventArgs args)
        {
            // pass
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
            if (sender == null || args?.LiveTraffic == null)
            {
                return;
            }
            //var log = args.LiveTraffic;
            //var device = (DeviceSender)sender;
            //var objList = new List<Object>
            //{
            //    "tiarathegroudbreaker",
            //    7
            //};
            //var getAllLogsOfDevice = new PalizGetAllTrafficLogs(objList, this, _commonDeviceService);
            //getAllLogsOfDevice.Execute();
            //var userId = new UserIdModel(1);
        }

        private void ServerManagerOnDeviceLogEvent(object sender, DeviceLogEventArgs args)
        {
            var device = (DeviceSender)sender;
            if (args.DeviceLogModel.Logs == null || args?.DeviceLogModel?.Logs?.Length < 1)
            {
                return;
            }
        }

        private int T = 0;

        private async void Listen(string format, params object[] args)
        {
            if (T > 0)
            {
                return;
            }

            T++;

            if (args.Length < 1)
            {
                return;
            }

            if (args.Length != 3)
            {
                return;
            }

            var terminalName = args[1].ToString();

            lock (_onlineTerminals)
            {
                if (_onlineTerminals.Exists(x => x.ToLower() == terminalName.ToLower()))
                {
                    return;
                }

                _onlineTerminals.Add(terminalName);
            }

            //var existedDevice = _commonDeviceService.GetDevices(deviceName: terminalName).Data.Data.FirstOrDefault();
            await _serverManager.GetTiaraSettingsAsyncTask(terminalName);

            //await Task.Run(async () =>
            //{

            //    var connectionStatus = new ConnectionStatus
            //    {
            //        DeviceId = existedDevice?.DeviceId ?? 0,
            //        IsConnected = true
            //    };

            //    try
            //    {
            //        var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
            //        restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

            //        await _monitoringRestClient.ExecuteAsync<ResultViewModel>(restRequest);
            //        //integration
            //        var connectionStatusList = new List<ConnectionStatus> { connectionStatus };
            //        var biovationBrokerMessageData = new List<DataChangeMessage<ConnectionStatus>>
            //            {
            //                new DataChangeMessage<ConnectionStatus>
            //                {
            //                    Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
            //                    TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = connectionStatusList
            //                }
            //            };

            //        //_deviceConnectionStateInternalSource.PushData(biovationBrokerMessageData);

            //        await _logService.AddLog(new Log
            //        {
            //            DeviceId = existedDevice?.DeviceId ?? 0,
            //            LogDateTime = DateTime.Now,
            //            EventLog = _logEvents.Connect
            //        });
            //    }
            //    catch (Exception)
            //    {
            //        //ignore
            //    }
            //}).ConfigureAwait(false);

            //DeviceBasicInfo device;
            //if (existedDevice != null)
            //{
            //    device = new DeviceBasicInfo
            //    {
            //        Code = (uint)terminalId,
            //        DeviceId = existDevice.DeviceId,
            //        Name = existDevice.Name,
            //        Brand = existDevice.Brand,
            //        Model = existDevice.Model,
            //        IpAddress = terminalIp,
            //        Port = BiovationConfiguration.VirdiDevicesConnectionPort,
            //        MacAddress = existDevice.MacAddress,
            //        RegisterDate = existDevice.RegisterDate,
            //        TimeSync = existDevice.TimeSync,
            //        Active = existDevice.Active,
            //        DeviceTypeId = existDevice.DeviceTypeId
            //    };

            //    if (existDevice.Code != (uint)terminalId || !string.Equals(existDevice.IpAddress, terminalIp, StringComparison.InvariantCultureIgnoreCase) || existDevice.Port != BiovationConfiguration.VirdiDevicesConnectionPort)
            //        _commonDeviceService.ModifyDevice(device);
            //}
            //else
            //{
            //    device = new DeviceBasicInfo
            //    {
            //        Code = (uint)terminalId,
            //        Brand = _deviceBrands.Virdi,
            //        Model = new DeviceModel { Id = 1001 },
            //        IpAddress = terminalIp,
            //        Port = BiovationConfiguration.VirdiDevicesConnectionPort,
            //        MacAddress = "",
            //        RegisterDate = DateTime.Now,
            //        TimeSync = true,
            //        Active = true
            //    };

            //    device.Name = terminalId + "[" + device.IpAddress + "]";
            //    var result = _commonDeviceService.ModifyDevice(device);
            //    if (result.Validate == 1)
            //        device.DeviceId = (int)result.Id;
            //}

            //if (!_onlineDevices.ContainsKey((string)terminalName))
            //    _onlineDevices.Add((string)terminalName, new DeviceBasicInfo());

            await _serverManager.GetLiveTrafficLogAsyncTask(terminalName);

        }
    }
}
