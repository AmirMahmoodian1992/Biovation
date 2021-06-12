using Biovation.Brands.Shahab.Devices;
using Biovation.Brands.Shahab.Model;
using Biovation.CommonClasses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using static Biovation.Brands.Shahab.ShahabApi;

namespace Biovation.Brands.Shahab
{
    public class ShahabServer
    {
        private readonly Dictionary<uint, Device> _onlineDevices;

        private readonly List<DeviceBasicInfo> _shahabDevices;
        private readonly PlateDetectionService _plateDetectionService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly DeviceFactory _deviceFactory;
        private readonly MatchingTypes _matchingTypes;
        private readonly RestClient _logExternalSubmissionRestClient;
        private readonly Dictionary<int, Timer> _timers = new Dictionary<int, Timer>();


        private readonly string[] _dirStrings = { "", "IN", "OUT" };
        private readonly byte _drawMethod = 3; //{ DRAW_GDI, DRAW_OPENGL, DRAW_SDL, DRAW_NONE }; //best method is DRAW_SDL but it may differ based on PC config
        protected ANPR_EVENT_CALLBACK HandleAnprEventsDelegate;


        public ShahabServer(DeviceService deviceService, PlateDetectionService plateDetectionService, RestClient logExternalSubmissionRestClient, Dictionary<uint, Device> onlineDevices, LogEvents logEvents, MatchingTypes matchingTypes, LogSubEvents logSubEvents, DeviceFactory deviceFactory)
        {
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _onlineDevices = onlineDevices;
            _matchingTypes = matchingTypes;
            _deviceFactory = deviceFactory;
            _plateDetectionService = plateDetectionService;
            _logExternalSubmissionRestClient = logExternalSubmissionRestClient;

            _shahabDevices = deviceService.GetDevices(brandId: DeviceBrands.ShahabCode).Where(x => x.Active).ToList();

            try
            {
                HandleAnprEventsDelegate = HandleAnprEvents;
                anpr_set_event_callback(HandleAnprEventsDelegate);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }
        }

        //public static Dictionary<uint, Device> GetOnlineDevices()
        //{
        //    lock (_onlineDevices)
        //    {
        //        return _onlineDevices;
        //    }
        //}

        public void StartServer()
        {
            Logger.Log("Service started.");
            //CheckExit();
            foreach (var device in _shahabDevices)
            {
                ConnectToDevice(device);
                //CheckConnectionStatus(device);
            }
        }

        public async void ConnectToDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (_onlineDevices)
                    {
                        if (_onlineDevices.ContainsKey(deviceInfo.Code))
                        {
                            try
                            {
                                _onlineDevices[deviceInfo.Code].Disconnect();
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }
                        }
                    }

                    if (!deviceInfo.Active) return;

                    var device = _deviceFactory.Factory(deviceInfo);
                    var connectResult = device.Connect(_drawMethod);
                    if (!connectResult)
                        Logger.Log($"Cannot connect to device {deviceInfo.Code}.", logType: LogType.Warning);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
            });
        }

        public async void DisconnectDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                lock (_onlineDevices)
                {
                    if (!_onlineDevices.ContainsKey(deviceInfo.Code)) return;
                    try
                    {
                        _onlineDevices[deviceInfo.Code].Disconnect();
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                }
            });
        }

        protected void HandleAnprEvents(int eventType, byte instanceId, byte plateIndex)
        {
            try
            {
                Device detectorInstance;
                lock (_onlineDevices)
                {
                    detectorInstance = _onlineDevices.ElementAt(instanceId).Value;
                }

                if (detectorInstance is null)
                    return;

                switch (eventType)
                {
                    case WM_CONNECTED:
                        {
                            try
                            {
                                vlpr_get_frame_info(instanceId, ref detectorInstance.FrameWidth, ref detectorInstance.FrameHeight, ref detectorInstance.FrameChannels, ref detectorInstance.FrameStep);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log("Failed to get frame info in WM_CONNECTED" + ex, logType: LogType.Warning);
                            }

                            if (detectorInstance.FrameWidth < 1)
                            {
                                detectorInstance.Grabbing = 0;
                                Logger.Log("Could not connect to camera, Please check the configuration and try again.", logType: LogType.Warning);

                                detectorInstance.ReConnect(_drawMethod);
                                _timers[instanceId]?.Stop();
                                _timers[instanceId]?.Start();
                                _timers[instanceId].Enabled = false;
                                _timers[instanceId].Enabled = true;
                                return;
                            }

                            // _grabbing == 1 play clicked else play vlc clicked

                            lock (_onlineDevices)
                            {
                                if (!_onlineDevices.ContainsKey(detectorInstance.GetDeviceInfo().Code))
                                    _onlineDevices.Add(detectorInstance.GetDeviceInfo().Code, detectorInstance);
                            }

                            //set Timer for each instanceId
                            if (!_timers.ContainsKey(instanceId))
                                SetTimer(instanceId, _drawMethod);

                            detectorInstance.Frame = null; // new Bitmap(FrameW, FrameH, PixelFormat.Format24bppRgb);
                            if (!detectorInstance.AnprSettings.Repeat)
                                detectorInstance.FrameCounter = 0;
                            break;
                        }

                    case WM_CAM_NOT_FOUND:
                        //New Frame Captured
                        detectorInstance.Grabbing = 0;
                        Logger.Log("Could not connect to camera stream. Please check the Url, Username, Password or other configuration and try again.", logType: LogType.Warning);

                        if (_timers.ContainsKey(instanceId))
                        {
                            detectorInstance.ReConnect(_drawMethod);
                            _timers[instanceId]?.Stop();
                            _timers[instanceId]?.Start();
                            _timers[instanceId].Enabled = false;
                            _timers[instanceId].Enabled = true;
                        }
                        break;

                    case WM_NEW_FRAME:
                        //plate detected
                        UpdateFrame(instanceId);
                        break;
                    case WM_PLATE_DETECTED:
                        //possible plate detected
                        ProcessDetectedPlateAsync(plateIndex, instanceId);
                        break;

                    case WM_INITIAL_PLATE:
                        {
                            //if (plt_count > 1)
                            // vlpr_pause_or_resume(0, 1);
                            var plate = new PlateResult();
                            for (byte i = 0; i < plateIndex; i++) //plt_idx = number of plates
                            {
                                try
                                {
                                    anpr_get_plate(instanceId, i, ref plate);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log("Failed to get plate in WM_INITIAL_PLATE" + ex, logType: LogType.Warning);
                                }
                            }


                            break;
                        }

                    //Scene changed
                    case WM_SCENE_CHANGED:
                        break;

                    case WM_END_OF_VIDEO:
                        detectorInstance.Grabbing = 0;
                        Logger.Log("End of the Video", logType: LogType.Warning);

                        detectorInstance.ReConnect(_drawMethod);
                        _timers[instanceId]?.Stop();
                        _timers[instanceId]?.Start();
                        _timers[instanceId].Enabled = false;
                        _timers[instanceId].Enabled = true;
                        break;

                    case WM_PLATE_NOT_DETECTED:
                        {
                            //_missedCount++; To maintain missed plate frames
                            var pFrame = IntPtr.Zero;
                            try
                            {
                                pFrame = vlpr_get_frame(instanceId);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log("Failed to get frame in WM_PLATE_NOT_DETECTED" + ex, logType: LogType.Warning);
                            }

                            if (pFrame == IntPtr.Zero) return;

                            //if (frame_vehicle == null)
                            //if (_frameWidth > 0 && _frameHeight > 0 && _frameStep > 0)
                            //    _frameVehicle = new Bitmap(_frameWidth, _frameHeight, _frameStep, PixelFormat.Format24bppRgb, pFrame);
                            //picVehicle.Image = frame_vehicle;
                            //picVehicle.BackColor = Color.DarkRed;
                            //picVehicle.Invalidate();

                            Logger.Log($@"Oooopppss we have missed the {detectorInstance.FrameCounter}th frame", logType: LogType.Verbose);
                            //try //we just try to store failed frames and Nulls are not saved.
                            //{
                            //    var outputFileName = $@"frames\failedFrames\{_frameCounter}.jpg";
                            //    //frame.Save($"E:\\Downloads\\Video\\shahab_frames\\{frame_counter}", ImageFormat.Jpeg);
                            //    var x = new Bitmap(_frameW, _frameH, _frameStep, PixelFormat.Format24bppRgb, pFrame);
                            //    x.Save(outputFileName, ImageFormat.Jpeg);
                            //    //string outputFileName = $@"E:\Downloads\shahab_frames\{frame_counter}";
                            //    //using (MemoryStream memory = new MemoryStream())
                            //    //{
                            //    //    using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                            //    //    {
                            //    //        frame.Save(memory, ImageFormat.Jpeg);
                            //    //        byte[] bytes = memory.ToArray();
                            //    //        fs.Write(bytes, 0, bytes.Length);
                            //    //    }
                            //    //}
                            //}
                            //catch (Exception e)
                            //{
                            //    Console.WriteLine(e);
                            //}
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }
        }

        private void ProcessDetectedPlateAsync(byte plateIndex, byte instanceId)
        {
            Task.Run(async () =>
            {
                Device detectorInstance;
                lock (_onlineDevices)
                {
                    detectorInstance = _onlineDevices.ElementAt(instanceId).Value;
                }

                if (detectorInstance is null)
                    return;

                var plate = new PlateResult();
                try
                {
                    anpr_get_plate(instanceId, plateIndex, ref plate);
                }
                catch (Exception ex)
                {
                    Logger.Log("Failed to get plate" + ex, logType: LogType.Warning);
                }


                if (plate.PlateData.Length < 3 || plateIndex > 4)
                {
                    Logger.Log("Invalid Plate: " + plate.PlateData, logType: LogType.Warning);
                    return;
                }

                if (!detectorInstance.AnprSettings.ReportNonStandardPlates && detectorInstance.Grabbing != 0)
                {
                    if (plate.LettersCount > 1) //پلاک استاندارد حداکثر یک حرف دارد و بقیه رقم هستند
                        return;

                    if (detectorInstance.AnprSettings.NumValidChars[1] > 0)
                    {
                        if (plate.CharactersCount != detectorInstance.AnprSettings.NumValidChars[0] &&
                            plate.CharactersCount != detectorInstance.AnprSettings.NumValidChars[1])
                            return;
                    }
                    else if (plate.CharactersCount != detectorInstance.AnprSettings.NumValidChars[0])
                    {
                        return;
                    }
                }

                //_mean += plate.DetectionPrecision; To compute precision average

                var resultEn = new string(' ', 20);
                try
                {
                    anpr_get_en_result(plate.PlateData, resultEn);
                }
                catch (Exception ex)
                {
                    Logger.Log("Failed to get english result" + ex, logType: LogType.Warning);
                }

                //UpdateFarsiResult(last_result);/////////////////////////////in bayad dorost she

                //for (var i = 19; i > 0; i--)
                //    if (resultEn[i] == 0)
                //    {
                //        resultEn = resultEn.Substring(0, i);
                //        break;
                //    }

                resultEn = resultEn.Trim('0');
                lock (detectorInstance.GetDeviceInfo())
                    Logger.Log(
                        $@"Device[{detectorInstance.GetDeviceInfo().Code}] Plate detected: {resultEn} ({plate.DetectionPrecision:0.00}) {_dirStrings[plate.Direction]}",
                        logType: LogType.Information);

                var plateImage = DrawPlate(plate.PlateCoordinates, plate.PlateImage);

                //frame
                var frameImageMemoryStream = new MemoryStream();
                detectorInstance.Frame.Save(frameImageMemoryStream, ImageFormat.Jpeg);
                var frameImage = frameImageMemoryStream.ToArray();



                var licensePlate = _plateDetectionService.GetLicensePlate(plate.PlateData)?.Data;

                if (licensePlate == null)
                {
                    licensePlate = new LicensePlate
                    {
                        StartDate = default,
                        EndDate = default,
                        IsActive = false,
                        LicensePlateNumber = plate.PlateData,
                        StartTime = default,
                        EndTime = default
                    };
                    await Task.Run(() =>
                    {
                        try
                        {
                            var resultAddLicensePlate = _plateDetectionService.AddLicensePlate(licensePlate);
                            licensePlate.EntityId = (int)resultAddLicensePlate.Code;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                        }
                    });
                }


                var permission = licensePlate.StartDate.Date > DateTime.Now.Date
                                   && licensePlate.EndDate.Date < DateTime.Now.Date
                                   && licensePlate.StartTime > DateTime.Now.TimeOfDay
                                   && licensePlate.EndTime < DateTime.Now.TimeOfDay
                                   && licensePlate.IsActive;


                var detectedLog = new PlateDetectionLog
                {
                    DetectorId = detectorInstance.GetDeviceInfo().DeviceId,
                    EventLog = permission ? _logEvents.Authorized : _logEvents.UnAuthorized,
                    LicensePlate = licensePlate,
                    DetectionPrecision = plate.DetectionPrecision,
                    LogDateTime = DateTime.Now,
                    FullImage = frameImage,
                    PlateImage = plateImage,
                    SuccessTransfer = false
                };

                await Task.Run(async () =>
                {
                    try
                    {
                        var restRequest = new RestRequest("UpdateMonitoring/UpdateMonitoring", Method.POST);

                        restRequest.AddJsonBody(new
                        {
                            //resultAddLog.Result.Id,
                            DeviceId = detectedLog.DetectorId,
                            DeviceCode = detectorInstance.GetDeviceInfo().Code,
                            UserId = detectedLog.LicensePlate.LicensePlateNumber,
                            detectedLog.LogDateTime,
                            detectedLog.EventLog,
                            MatchingType = _matchingTypes.Car,
                            PicByte = detectedLog.PlateImage,
                            SubEvent = _logSubEvents.Normal,
                            DeviceName = detectorInstance.GetDeviceInfo().Name,
                        });

                        await _logExternalSubmissionRestClient.ExecuteAsync(restRequest);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                });

                _plateDetectionService.AddPlateDetectionLog(detectedLog);



                //DrawPlate(plate.roi, plate.rc, plate.img_plate);
                //picPlate[plt_idx].Image = img_plate[plate.roi];
                //            picPlate[plt_idx].Image = img_plate[plate.roi];
                //            PicPlateAddedByMe.Add(img_plate[plate.roi]);
                //            try
                //            {
                //                string outputFileName = $@"frames\succeededFrames\{frame_counter}.jpg";
                //                //frame.Save($"E:\\Downloads\\Video\\shahab_frames\\{frame_counter}", ImageFormat.Jpeg);
                //                img_plate[plate.roi].Save(outputFileName, ImageFormat.Jpeg);
                //                //var x = new Bitmap(FrameW, FrameH, FrameStep, PixelFormat.Format24bppRgb, IntPtr.Zero);
                //                //x = img_plate[plate.roi];
                //                //x.Save(outputFileName, ImageFormat.Jpeg);
                //                //string outputFileName = $@"E:\Downloads\shahab_frames\{frame_counter}";
                //                //using (MemoryStream memory = new MemoryStream())
                //                //{
                //                //    using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                //                //    {
                //                //        frame.Save(memory, ImageFormat.Jpeg);
                //                //        byte[] bytes = memory.ToArray();
                //                //        fs.Write(bytes, 0, bytes.Length);
                //                //    }
                //                //}
                //            }
                //            catch (Exception e)
                //            {
                //                Console.WriteLine(e);
                //            }
                //picPlate[plt_idx].Invalidate();

                //Draw Car Image corresponding to the plate
                //if (plate.img_car == IntPtr.Zero || _frameW < 10 || _frameH < 10)
                //MessageBox.Show("Null Image");
                //frame_vehicle = new Bitmap(FrameW, FrameH, FrameStep, PixelFormat.Format24bppRgb, plate.img_car);
                //picVehicle.Image = frame_vehicle;
                //picVehicle.BackColor = SystemColors.ButtonFace;
                //picVehicle.Invalidate();
            });
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        private static byte[] DrawPlate(RectangleCoordinates rc, IntPtr pBuf = default)
        {
            try
            {

                var imageBitmap = new Bitmap(rc.Right - rc.Left, rc.Bottom - rc.Top, PixelFormat.Format24bppRgb);

                BitmapData dataSrc = null;
                if (pBuf == IntPtr.Zero)//use current frame
                    dataSrc = imageBitmap.LockBits(new Rectangle(rc.Left, rc.Top, rc.Right - rc.Left + 1, rc.Bottom - rc.Top + 1), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                var dataDst = imageBitmap.LockBits(new Rectangle(0, 0, imageBitmap.Width, imageBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                var src = pBuf;
                var step = 3 * imageBitmap.Width;
                if (src == IntPtr.Zero)
                {
                    if (dataSrc != null)
                    {
                        src = dataSrc.Scan0;
                        step = dataSrc.Stride;
                    }
                }
                var dst = dataDst.Scan0;
                for (var y = 0; y < imageBitmap.Height; y++)
                {
                    CopyMemory(dst, src, (uint)imageBitmap.Width * 3);
                    src += step;
                    dst += dataDst.Stride;
                }

                if (dataSrc != null)
                    imageBitmap.UnlockBits(dataSrc);
                imageBitmap.UnlockBits(dataDst);


                var stream = new MemoryStream();
                imageBitmap.Save(stream, ImageFormat.Jpeg);
                var bitmapData = stream.ToArray();
                return bitmapData;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new byte[1];
            }
        }


        private void UpdateFrame(byte instanceId)
        {

            _timers[instanceId]?.Stop();
            _timers[instanceId]?.Start();
            _timers[instanceId].Enabled = false;
            _timers[instanceId].Enabled = true;

            Device detectorInstance;
            lock (_onlineDevices)
            {
                detectorInstance = _onlineDevices.ElementAt(instanceId).Value;
            }

            if (detectorInstance is null)
                return;

            if (_drawMethod != 3)
            {
                //در حالتهای 0 تا 2، فریمها توسط کتابخانه ترسیم می شود
                //لذا اینجا فقط شمارنده فریم را به روز رسانی می کنیم
                detectorInstance.FrameCounter++;
                //lblFrame.Text = frame_counter.ToString();
                return;
            }

            if (detectorInstance.FrameWidth <= 0 || detectorInstance.Grabbing <= 0) return;
            //vlpr_pause_or_resume(1); //Prevent changing frame in C++ while showing it here in C#
            IntPtr pFrame = IntPtr.Zero;
            try
            {
                pFrame = vlpr_get_frame(instanceId);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to get frame" + ex, logType: LogType.Warning);
            }

            if (pFrame == IntPtr.Zero)
            {
                //_countEmptyFrame++;
                //vlpr_pause_or_resume(0);
                //if (_countEmptyFrame > 10)
                //StopEveryThing();
                return;
            }

            detectorInstance.FrameCounter++;


            if (detectorInstance.Frame == null)
                detectorInstance.Frame = new Bitmap(detectorInstance.FrameWidth, detectorInstance.FrameHeight, detectorInstance.FrameStep, PixelFormat.Format24bppRgb, pFrame);
            //                    picture.Image = frame;

            //try
            //{
            //    var outputFileName = $@"frames\allFrames\{frame_counter}.jpg";
            //    var x = new Bitmap(FrameW, FrameH, FrameStep, PixelFormat.Format24bppRgb, pFrame);
            //    x.Save(outputFileName, ImageFormat.Jpeg);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            //vlpr_pause_or_resume(0);//Resume changing frame
            //                picture.Invalidate();
        }

        private void SetTimer(byte instanceId, byte drawMethod)
        {
            lock (_onlineDevices)
            {
                var device = _onlineDevices.ElementAt(instanceId).Value;
                _timers.Add(instanceId, new Timer(10000));


                _timers[instanceId].Elapsed += (sender, e) =>
                {
                    if (!_onlineDevices.ContainsKey(instanceId))
                    {
                        _timers[instanceId].Enabled = false;
                        return;
                    }
                    try
                    {
                        device.ReConnect(drawMethod);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }

                };
            }
            _timers[instanceId].AutoReset = true;
            _timers[instanceId].Enabled = true;
        }
    }
}
