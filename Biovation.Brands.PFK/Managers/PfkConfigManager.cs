using System.Drawing;
using System.Linq;
using Biovation.Domain;
using PFKParkingLibrary.Data;
using PFKParkingLibrary.Devices;

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
                CameraRecordinFolder = $@"D:\PlateLogs\Record{cameraInfo.DeviceId}",
                CameraType = CameraType.RTSP,
                CameraUserName = cameraConnectionUserName,
                CameraofflinePath = default,
                ConnectToCamera = true,
                ResizeImageForUI = true
            };

            config.CostumContent = new CostumContent
            {
                AboutUs = "شرکت پویا فن آوران کوثر",
                AutoWriteBill = false,
                CompanyName = "شرکت پویا فن آوران کوثر",
                IsBehbin = true,
                LoginContent = "سامانه ثبت پلاک خودرو بهبین",
                MainFormTitle = "سامانه ثبت پلاک خودرو بهبین",
                ProductNmae = "سامانه پلاک خوان خودرو ",
                ReportProductName = "نرم افزار مدیریت سامانه ثبت پلاک خودرو ",
                ShowBoardControls = false,
                ShowDriverFace = false,
                UseBilling = false,
                UseCarClass = false,
                UseOwnerofCars = false
            };

            config.DisplayConfig = null;
            config.DoorConfig = new DoorConfig
            {
                BothDirections = true,
                Direction = DirectionType.Ingoing,
                Index = cameraInfo.DeviceId,
                IngouingDir = new PointF(-1,-1),
                Name = cameraInfo.Name,
                OpenAutomaticaly = true,
                OutgouingDir = new PointF(-1, 1),
                RegisterOnlyCarsInMainDirection = false,
                UseDirectionBorder = true
            };

            config.FaceCamerFaceConfig = new FaceCamerFaceConfig
            {
                ConnectToCamera = false,
                CameraUserName = default,
                CameraofflinePath = default,
                ResizeImageForUI = false,
                CameraType = CameraType.RTSP,
                CameraIP = default,
                CameraImageHeight = 1080,
                CameraImageWidth = 1920,
                CameraPass = default,
                CameraRecordinFolder = $@"D:\FaceCameraLogs\{cameraInfo.DeviceId}",
                FaceImageDelayInms = 0,
                UseAsShowCamera = false,
                UseFaceCamera = false
            };

            config.JustConnectToCamera = false;
            config.PlateRegisteringConfig = new PlateRegisteringConfig
            {
                MinCharachterSimilarity = 6,
                MinPlateAccToValidate = 870,
                MinPlateCountToValidate = 2,
                RepeatCarIntervalSec = 60
            };

            config.PlatesPathForRemove = default;
            config.RFIDConfig = default;
            config.RemoteConfig = new RemoteConfig
            {
                RemoteServerIP = "127.0.0.1",
                RemoteServerPort = 4249,
                ServerIsOnLocalPc = true,
                ShowRemoteWindow = true
            };

            config.SaveFilesCopyDir = $@"D:\PlateLogs\Enter{cameraInfo.DeviceId}";
            config.SaveRowPlates = false;
            config.SaveRowPlatesPath1 = $@"D:\PlateLogs\RowPlates{cameraInfo.DeviceId}_1";
            config.SaveRowPlatesPath2 = $@"D:\PlateLogs\RowPlates{cameraInfo.DeviceId}_2";
            config.SenderConfig = new SenderConfig
            {
                DfsIndex = 0,
                DisableOCR = false,
                SenderIP = "127.0.0.1",
                SenderMaximumdelaySecond = 2,
                SenderPort = 11124,
                SenderResulSavePath = $@"D:\PlateLogs\Result{cameraInfo.DeviceId}",
                Senderpath = "D:\\PlateReader\\LPR1\\generalRawFixCamLPR.exe",
                Senderworkingpath = "D:\\PlateReader\\LPR1\\generalRawFixCamLPR.exe",
                TmpDir = "H:\\"
            };

            config.UseSoftWatchDog = false;
            config.WorkingDir = "D:\\PlateReader";

            return config;
        }
    }
}
