using Biovation.Brands.PFK.Devices;
using Biovation.CommonClasses;
using Biovation.Service.Api.v2.RelayController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camera = Biovation.Brands.PFK.Devices.Camera;

namespace Biovation.Brands.PFK
{
    public class PfkServer
    {
        private readonly Dictionary<uint, Camera> _onlineCameras;
        private readonly PFKParkingLibrary.Data.Logger _pfkLogger = new PFKParkingLibrary.Data.Logger();

        private readonly List<Domain.Camera> _cameras;
        private readonly DeviceFactory _deviceFactory;

        public PfkServer(CameraService cameraService, Dictionary<uint, Camera> onlineCameras, DeviceFactory deviceFactory)
        {
            _onlineCameras = onlineCameras;
            _deviceFactory = deviceFactory;

            _cameras = cameraService.GetCamera().GetAwaiter().GetResult()?.Data?.Data?.Where(x => x.Active).ToList();
            _pfkLogger.LogArose += OnLogHappened;
        }

        public void StartServer()
        {
            if (_cameras is null)
            {
                Logger.Log("No active cameras found.");
                return;
            }

            Logger.Log("Service started.");
            //CheckExit();
            foreach (var device in _cameras)
            {
                ConnectToDevice(device).Wait();
                //CheckConnectionStatus(device);
            }
        }

        public void StopServer()
        {
            lock (_onlineCameras)
            {
                foreach (var onlineCamera in _onlineCameras)
                {
                    onlineCamera.Value.Disconnect();
                }
            }
        }

        public Dictionary<uint, Camera> GetOnlineDevices()
        {
            lock (_onlineCameras)
            {
                return _onlineCameras;
            }
        }

        public async Task ConnectToDevice(Domain.Camera deviceInfo)
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (_onlineCameras)
                    {
                        if (_onlineCameras.ContainsKey(deviceInfo.Code))
                        {
                            try
                            {
                                _onlineCameras[deviceInfo.Code].Disconnect();
                                _onlineCameras.Remove(deviceInfo.Code);
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }
                        }
                    }

                    if (!deviceInfo.Active) return;

                    var device = _deviceFactory.Factory(deviceInfo);
                    var connectResult = device.Connect();
                    if (!connectResult)
                        Logger.Log($"Cannot connect to device {deviceInfo.Code}.", logType: LogType.Warning);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
            });
        }

        public async Task DisconnectDevice(Domain.Camera cameraInfo)
        {
            await Task.Run(() =>
            {
                lock (_onlineCameras)
                {
                    if (!_onlineCameras.ContainsKey(cameraInfo.Code)) return;
                    try
                    {
                        _onlineCameras[cameraInfo.Code].Disconnect();
                        _onlineCameras.Remove(cameraInfo.Code);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                }
            });
        }


        public void OnLogHappened(object source, PFKParkingLibrary.Data.Log log)
        {
            if (log.Comment.ToLower().Contains("jammer camera buffer"))
            {
                Logger.Log($@"Log write by :   {source} 
    Log detail:
[ Log Camera Id ] = {log.ID}
[ Log DateTime ] = {log.DateTime}
[ Log Comment ] = {log.Comment}
[ Log Item ] = {log.Item}
", string.Empty, LogType.Verbose);
                return;
            }

            Logger.Log($@"Log write by :   {source} 
    Log detail:
[ Log Camera Id ] = {log.ID}
[ Log DateTime ] = {log.DateTime}
[ Log Comment ] = {log.Comment}
[ Log Item ] = {log.Item}
", string.Empty, log.Type == PFKParkingLibrary.Data.LogType.Errore ? LogType.Error : log.Type == PFKParkingLibrary.Data.LogType.Warning ? LogType.Warning : LogType.Information);
        }
    }
}
