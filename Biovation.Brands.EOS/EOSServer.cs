using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Brands.EOS.Devices;
using System.Linq;
using Newtonsoft.Json;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using RestSharp;

namespace Biovation.Brands.EOS
{
    public class EosServer
    {
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly List<DeviceBasicInfo> _eosDevices;

        private readonly LogEvents _logEvents;
        private readonly LogService _logService;
        private readonly RestClient _restClient;
        private readonly DeviceFactory _deviceFactory;

        public int Count = 0;
   
        public EosServer(LogService logService, DeviceService deviceService, Dictionary<uint, Device> onlineDevices, RestClient restClient, LogEvents logEvents, DeviceFactory deviceFactory)
        {
            _logService = logService;
            _onlineDevices = onlineDevices;
            _restClient = restClient;
            _logEvents = logEvents;
            _deviceFactory = deviceFactory;
            _eosDevices = deviceService.GetDevices(brandId: DeviceBrands.EosCode)?.Data?.Data?.Where(x => x.Active).ToList() ?? new List<DeviceBasicInfo>();
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
                var connectResult = device.Connect();
                if (!connectResult) return;

                lock (_onlineDevices)
                {
                    if (!_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        _onlineDevices.Add(deviceInfo.Code, device);
                    }
                }

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
            });
        }

        public async void DisconnectFromDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                if (_onlineDevices.ContainsKey(deviceInfo.Code))
                {
                    _onlineDevices[deviceInfo.Code].Disconnect();
                    _onlineDevices.Remove(deviceInfo.Code);
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
                //});
            });
        }

        public void StopServer()
        {
            foreach (var onlineDevice in _onlineDevices)
            {
                onlineDevice.Value.Disconnect();
            }
        }
        public void StartServer()
        {
            Logger.Log("EOS Server Started!");
             foreach (var eosDevice in _eosDevices)
            {
                ConnectToDevice(eosDevice);
            }
        }

        //public static Dictionary<uint, Device> GetOnlineDevices()
        //{
        //    return OnlineDevices;
        //}
    }
}
