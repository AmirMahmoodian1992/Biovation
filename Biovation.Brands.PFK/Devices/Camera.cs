using System;
using System.Collections.Generic;
using Biovation.Brands.PFK.Managers;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using PFKParkingLibrary;
using PFKParkingLibrary.Data;

namespace Biovation.Brands.PFK.Devices
{
    public class Camera : IDevices
    {
        private readonly DeviceBasicInfo _cameraInfo;
        private readonly Dictionary<uint, Camera> _connectedCameras;

        private readonly Config _pfkConfig = new Config();

        private PlateReader _plateReader;

        public Camera(DeviceBasicInfo cameraInfo, Dictionary<uint, Camera> connectedCameras)
        {
            _cameraInfo = cameraInfo;
            _connectedCameras = connectedCameras;
        }


        public bool TransferUser(User user)
        {
            throw new NotImplementedException();
        }

        public ResultViewModel ReadOfflineLog(object cancellationToken, bool fileSave)
        {
            throw new NotImplementedException();
        }

        public int AddDeviceToDataBase()
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(uint sUserId)
        {
            throw new NotImplementedException();
        }

        public bool Connect()
        {
            try
            {
                _pfkConfig.SetConfiguration(_cameraInfo);

                _plateReader = new PlateReader(_pfkConfig, _cameraInfo.DeviceId);
                _plateReader.PlateDetected += OnPlateDetected;
                _plateReader.PlateUpdated += OnPlateUpdated;
                _plateReader.Start();
            }
            catch (Exception exception)
            {
                CommonClasses.Logger.Log(exception);
                lock (_connectedCameras)
                {
                    if (_connectedCameras.ContainsKey(_cameraInfo.Code))
                        _connectedCameras.Remove(_cameraInfo.Code);
                }

                return false;
            }

            lock (_connectedCameras)
            {
                if (!_connectedCameras.ContainsKey(_cameraInfo.Code))
                    _connectedCameras.Add(_cameraInfo.Code, this);
            }

            return true;
        }

        public bool Disconnect()
        {
            _plateReader.PlateDetected -= OnPlateDetected;
            _plateReader.PlateUpdated -= OnPlateUpdated;
            _plateReader.Stop();
            return true;
        }

        public void OnPlateDetected(object source, Plate detectedPlate)
        {
            CommonClasses.Logger.Log($@"Plate Detected by :  {source}
    Plate detail:
[ Detected Plate ] = {detectedPlate.DetectedPlate}
[ Detected Plate Id] = {detectedPlate.ID}
[ Detected Plate Accuracy ] = {detectedPlate.Accuracy}
[ Detected Plate Data Folder Path] = {detectedPlate.DataFolderPath}
[ Detected Plate Direction] = {(detectedPlate.Direction == DirectionType.Ingoing ? "Ingoing" : "Outgoing")}
[ Detected Plate Plate Type ] = {detectedPlate.PlateType}
[ Detected Plate Plate Location X ] = {detectedPlate.PlateLocation.X}
[ Detected Plate Plate Location Y ] = {detectedPlate.PlateLocation.Y}
");
        }

        public void OnPlateUpdated(object source, Plate updatedPlate)
        {
            CommonClasses.Logger.Log($@"Plate Updated by :  {source}
    Plate detail:
[ Detected Plate ] = {updatedPlate.DetectedPlate}
[ Detected Plate Id] = {updatedPlate.ID}
[ Detected Plate Accuracy ] = {updatedPlate.Accuracy}
[ Detected Plate Data Folder Path] = {updatedPlate.DataFolderPath}
[ Detected Plate Direction] = {(updatedPlate.Direction == DirectionType.Ingoing ? "Ingoing" : "Outgoing")}
[ Detected Plate Plate Type ] = {updatedPlate.PlateType}
[ Detected Plate Plate Location X ] = {updatedPlate.PlateLocation.X}
[ Detected Plate Plate Location Y ] = {updatedPlate.PlateLocation.Y}
");
        }
    }
}
