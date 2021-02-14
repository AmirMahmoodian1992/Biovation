using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;

namespace Biovation.Brands.PFK.Devices
{
    public class Camera : IDevices
    {
        public Camera(DeviceBasicInfo cameraInfo, Dictionary<uint, Camera> connectedCameras)
        {
            
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
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}
