using Biovation.Brands.Shahab.Model;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using Biovation.Domain;
using static Biovation.Brands.Shahab.ShahabApi;

namespace Biovation.Brands.Shahab.Devices
{
    public class Device : IDevices
    {
        protected readonly DeviceBasicInfo DeviceInfo;
        private byte _instanceId;

        private VideoProcessingOptions _videoProcessingOptions;
        public readonly CameraOptions AnprSettings;

        private readonly Dictionary<uint, Device> _connectedCameras;

        public Bitmap Frame; //bitmap of playing frames on picture control (in video mode)
        public int FrameCounter;


        //Frame width, height, number of channels and step size of the frame (usually = width x number of channels)
        public int FrameWidth;
        public int FrameHeight;
        public int FrameChannels;
        public int FrameStep;

        public int Grabbing; //indicates whether we are grabbing or not: 0 --> not grabbing, 1 regular grabbing, 2 VLC grabbing 

        // Open App.Config of executable
        // Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        internal Device(DeviceBasicInfo info, Dictionary<uint, Device> onlineDevices)
        {
            DeviceInfo = info;
            _connectedCameras = onlineDevices;
            AnprSettings = new CameraOptions();
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

        public DeviceBasicInfo GetDeviceInfo()
        {
            lock (DeviceInfo)
            {
                return DeviceInfo;
            }
        }

        public bool Connect(byte drawMethod)
        {
            lock (_connectedCameras)
            {
                _instanceId = Convert.ToByte(_connectedCameras.Count);
                if (!_connectedCameras.ContainsKey(DeviceInfo.Code))
                    _connectedCameras.Add(DeviceInfo.Code, this);
            }

            bool initialResult;
            try
            {
                initialResult = Initialize();
                ConnectToCamera(drawMethod);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                lock (_connectedCameras)
                {
                    if (_connectedCameras.ContainsKey(DeviceInfo.Code))
                        _connectedCameras.Remove(DeviceInfo.Code);
                }
                return false;
            }

            return initialResult;
        }

        public bool Disconnect()
        {
            try
            {
                vlpr_stop_process(_instanceId);
                System.Threading.Thread.Sleep(1000);
                vlpr_stop_grabbingVLC(_instanceId);

                lock (_connectedCameras)
                {
                    if (_connectedCameras.ContainsKey(DeviceInfo.Code))
                        _connectedCameras.Remove(DeviceInfo.Code);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return false;
            }

            return true;
        }

        private bool Initialize()
        {
            const string securityCode = "www.shahaab-co.ir 02332300204";
            var successResult = 0;
            try
            {
                successResult = anpr_create(_instanceId, securityCode, log_level: 2);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to initialize" + ex, logType: LogType.Warning);
            }

            return successResult >= 0;

            //it is not required to call anpr_set_params with default params, but if you want to change them, you must call it
        }

        private void SetParams()
        {

            _videoProcessingOptions = new VideoProcessingOptions
            {
                PlateBufSize = AnprSettings.PlateBufSize,
                NumValidChars = AnprSettings.NumValidChars,
                DetectMotor = AnprSettings.DetectMotor ? (byte)1 : (byte)0,
                DetectMultiPlate = AnprSettings.DetectMultiPlate ? (byte)1 : (byte)0,
                DiffThresh = AnprSettings.DiffThresh,
                NFrmSkipOnSuccess = AnprSettings.NFrmSkipOnSuccess,
                ResizeThresh = AnprSettings.ResizeThresh,
                SavePlateOption = AnprSettings.SavePlateOption,
                SkipSamePlateTime = 20, //اگر پلاکی بعد از 60 ثانیه دوباره جلوی دوربین قرار گیرد، گزارش خواهد شد
                MinCharW = AnprSettings.MinCharW,
                MaxCharW = AnprSettings.MaxCharW,
                MinCharH = AnprSettings.MinCharH,
                MaxCharH = AnprSettings.MaxCharW,
                SkewCoefficient = AnprSettings.SkewCoef,
                VlcNetCacheTime = AnprSettings.VlcNetCacheTime,
                PlateType = AnprSettings.PlateType,
                Reserved2 = AnprSettings.Reserved2 ? (byte)1 : (byte)0,
                IgnoreInvertedPlates = AnprSettings.IgnoreInvertedPlates ? (byte)1 : (byte)0,
                medianKernel = AnprSettings.MedianKernel,
                PlayAudioFromCamera = AnprSettings.PlayAudioFromCamera ? (byte)1 : (byte)0,
                MinThreshHist = AnprSettings.MinThreshHist,
                MaxThreshHist = AnprSettings.MaxThreshHist,
                blur_kernel = AnprSettings.BlurKernel,
                ImgBinTh = AnprSettings.ImgBinTh,
                PltBinTh = AnprSettings.PltBinTh
            };

            //VideoProcessingOptions.SetDefaultOptions(_instanceId, _videoProcessingOptions);

            anpr_set_params(_instanceId, ref _videoProcessingOptions);

            anpr_set_debug_mode(_instanceId, AnprSettings.DebugLevel);
            anpr_clear_ROIs(_instanceId); //SetROI();
        }


        private void ConnectToCamera(byte drawMethod)
        {
            FrameHeight = 0;
            FrameWidth = 0;

            SetParams();
            var interval = (byte)(1000 / AnprSettings.FrameRate);
            //rtsp://admin:admin@192.168.55.160:554/h264
            var str = DeviceInfo?.IpAddress;
            if (DeviceInfo != null)
            {
                var takeShots = AnprSettings.TakeShotsFromCamera ? (byte)1 : (byte)0;
                //byte car_detection = chbCarDetection.Checked ? (byte)1 : (byte)0;
                Grabbing = 1;
                try
                {
                    if (vlpr_start_grabbingVLC(_instanceId, str, interval, /*picture.Handle*/IntPtr.Zero, takeShots, drawMethod) < 0)
                        Grabbing = 0;

                }
                catch (Exception ex)
                {
                    Logger.Log("Failed to start grabbing" + ex, logType: LogType.Warning);
                    return;
                }
            }

            try
            {
                vlpr_start_process(_instanceId);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to start grabbing plates process" + ex, logType: LogType.Warning);
                return;
            }
           
            lock (_connectedCameras)
            {
                if (DeviceInfo != null && !_connectedCameras.ContainsKey(DeviceInfo.Code))
                    _connectedCameras.Add(DeviceInfo.Code, this);
            }
            //Wait for WM_CONNECTED message
        }

        public bool ReConnect(byte drawMethod)
        {
            Logger.Log("Reconnecting ...", logType: LogType.Warning);
            bool initialResult;
            try
            {
                Disconnect();
                initialResult = Initialize();
                ConnectToCamera(drawMethod);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to ReConnect" + ex, logType: LogType.Warning);
                return false;
            }

            return initialResult;

        }



        #region RedundantSampleCodes

        //TODO: Add 
        //private Rectangle _roi1, _roi2;

        //private Bitmap _frameVehicle; //bitmap of last frame containing car (may be detected or not)

        //[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        //public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        //private void SetROI()
        //{
        //    /*if (Grabbing > 0)
        //    {
        //        vlpr_pause_or_resume(0, 1);
        //        System.Threading.Thread.Sleep(500);
        //    }*/
        //    anpr_clear_ROIs(0);
        //    //if (chkROI.Checked)
        //    //{
        //    //    sel_rect.SetPictureBox(this.picture);
        //    //    //if (picture.Image != null)                
        //    //    anpr_add_ROI(0, ClientRect2ImageRect(Rect2RECT(sel_rect.rect)));
        //    //}
        //    //else
        //    //    sel_rect.SetPictureBox(null);

        //    //if (chkROI2.Checked)
        //    //{
        //    //    sel_rect2.SetPictureBox(this.picture);
        //    //    //if (picture.Image != null)
        //    //    anpr_add_ROI(0, ClientRect2ImageRect(Rect2RECT(sel_rect2.rect)));
        //    //}
        //    //else
        //    //    sel_rect2.SetPictureBox(null);


        //    /*if (Grabbing > 0)
        //    {
        //        vlpr_pause_or_resume(0, 0);
        //    }*/
        //}
        #endregion
    }
}