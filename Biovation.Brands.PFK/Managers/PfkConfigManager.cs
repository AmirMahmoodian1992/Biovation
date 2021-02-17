using System.Linq;
using Biovation.Domain;
using PFKParkingLibrary.Data;

namespace Biovation.Brands.PFK.Managers
{
    public static class PfkConfigManager
    {
        public static Config SetConfiguration(this Config config, DeviceBasicInfo cameraInfo)
        {
            var cameraAddress = cameraInfo.IpAddress.Split('@').LastOrDefault();
            var cameraConnectionUserName = cameraInfo.IpAddress.Split('@').FirstOrDefault()?.Split("//").LastOrDefault()?.Split(':').FirstOrDefault();
            var cameraConnectionPassword = cameraInfo.IpAddress.Split('@').FirstOrDefault()?.Split("//").LastOrDefault()?.Split(':').LastOrDefault();

            config.BoardConfig = new BoardConfig
            {
                AutoAlarm = true,
                AutoOpenDoor = true,
                BoardDelayInSecond = 1,
                BoardIP = "127.0.0.1",
                BouadRate = 1200,
                CancelMode = false,
                PinAlarm = 0,
                PinClose = 0,
                PinOpen = 0,
                PortNumber = "12064",
                ReadTimeOut = 2000,
                UseBoard = false,
                WriteTimeOut = 2000
            };

            config.CamID = cameraInfo.DeviceId;
            config.CameraConfig = new IpCamerConfig
            {
                CameraIP = cameraAddress,
                CameraImageHeight = 1080,
                CameraImageWidth = 1920,
                CameraPass = cameraConnectionPassword,
                CameraRecordinFolder = $@"D:\PlateLogs\{cameraInfo.DeviceId}"
            };

            return config;
        }
    }
}
