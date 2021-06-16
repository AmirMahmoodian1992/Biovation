using Biovation.Brands.PFK.Managers;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using PFKParkingLibrary;
using PFKParkingLibrary.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Logger = Biovation.CommonClasses.Logger;

namespace Biovation.Brands.PFK.Devices
{
    public class Camera : IDevices
    {
        private readonly LogEvents _logEvents;
        private readonly DeviceBasicInfo _cameraInfo;
        private readonly Dictionary<uint, Camera> _connectedCameras;

        private readonly Config _pfkConfig = new Config();

        private PlateReader _plateReader;

        private readonly PlateDetectionService _plateDetectionService;
        private readonly RestClient _logExternalSubmissionRestClient;

        public Camera(DeviceBasicInfo cameraInfo, Dictionary<uint, Camera> connectedCameras, RestClient logExternalSubmissionRestClient, PlateDetectionService plateDetectionService, LogEvents logEvents)
        {
            _cameraInfo = cameraInfo;
            _connectedCameras = connectedCameras;
            _logExternalSubmissionRestClient = logExternalSubmissionRestClient;
            _plateDetectionService = plateDetectionService;
            _logEvents = logEvents;
        }


        public DeviceBasicInfo GetCameraInfo()
        {
            lock (_cameraInfo)
            {
                return _cameraInfo;
            }
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

                _plateReader = new PlateReader(_pfkConfig, Convert.ToInt32(_cameraInfo.Code) - 1);
                _plateReader.PlateDetected += OnPlateDetected;
                _plateReader.PlateUpdated += OnPlateUpdated;
                _plateReader.Start();
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
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
            try
            {
                _plateReader.Stop();
            }
            catch (Exception)
            {
                //ignore
            }

            return true;
        }

        public void OnPlateDetected(object source, Plate detectedPlate)
        {
            Logger.Log($@"Plate Detected by :  {source}
    Plate detail:
[ Detected Plate ] = {detectedPlate.DetectedPlate}
[ Detected Plate Id] = {detectedPlate.ID}
[ Detected Plate Accuracy ] = {detectedPlate.Accuracy}
[ Detected Plate Key ] = {detectedPlate.PlateDetectionKey}
[ Detected Plate Data Folder Path] = {detectedPlate.DataFolderPath}
[ Detected Plate Direction] = {(detectedPlate.Direction == DirectionType.Ingoing ? "Ingoing" : "Outgoing")}
[ Detected Plate Plate Type ] = {detectedPlate.PlateType}
[ Detected Plate Plate Location X ] = {detectedPlate.PlateLocation.X}
[ Detected Plate Plate Location Y ] = {detectedPlate.PlateLocation.Y}
");

            ProcessDetectedPlateAsync(detectedPlate);
        }

        public void OnPlateUpdated(object source, Plate updatedPlate)
        {
            Logger.Log($@"Plate Updated by :  {source}
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

            ProcessDetectedPlateAsync(updatedPlate);
        }




        private void ProcessDetectedPlateAsync(Plate plate)
        {
            Task.Run(async () =>
            {
                Logger.Log("ProcessDetectedPlateAsync", logType: CommonClasses.LogType.Information);

                if (plate.DetectedPlate.Length < 8)
                {
                    Logger.Log("Invalid Plate: " + plate.DetectedPlate, logType: CommonClasses.LogType.Warning);
                    return;
                }

                var detectedPlate = BiovationPlateFormat(plate.DetectedPlate);

                //_mean += plate.DetectionPrecision; To compute precision average

                //var resultEn = new string(' ', 20);
                //try
                //{
                //    anpr_get_en_result(plate.PlateData, resultEn);
                //}
                //catch (Exception ex)
                //{
                //    Logger.Log("Failed to get english result" + ex, logType: LogType.Warning);
                //}

                //UpdateFarsiResult(last_result);/////////////////////////////in bayad dorost she

                //for (var i = 19; i > 0; i--)
                //    if (resultEn[i] == 0)
                //    {
                //        resultEn = resultEn.Substring(0, i);
                //        break;
                //    }

                //resultEn = resultEn.Trim('0');
                lock (_cameraInfo)
                    Logger.Log(
                        $@"Device[{_cameraInfo.Code}] Plate detected: {detectedPlate} ({plate.Accuracy:0.00}) {(plate.Direction == DirectionType.Ingoing ? "Ingoing" : "Outgoing")}",
                        logType: CommonClasses.LogType.Information);

                var plateImagePath = Path.Combine(plate.DataFolderPath, "plate.bmp");
                var frameImagePath = Path.Combine(plate.DataFolderPath, "car.bmp");


                byte[] plateImage = null;
                byte[] frameImage = null;
                try
                {
                    //var plateImage = DrawPlate(plate.PlateCoordinates, plate.PlateImage);
                    plateImage = ImageToByteArray(Image.FromFile(plateImagePath));

                    ////frame
                    //var frameImageMemoryStream = new MemoryStream();
                    //detectorInstance.Frame.Save(frameImageMemoryStream, ImageFormat.Jpeg);
                    //var frameImage = frameImageMemoryStream.ToArray();
                    frameImage = ImageToByteArray(Image.FromFile(frameImagePath));
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message);
                }



                var licensePlate = (await _plateDetectionService.GetLicensePlate(detectedPlate, 0))?.Data;

                if (licensePlate == null)
                {
                    licensePlate = new LicensePlate
                    {
                        StartDate = default,
                        EndDate = default,
                        IsActive = false,
                        LicensePlateNumber = detectedPlate,
                        StartTime = default,
                        EndTime = default
                    };
                    await Task.Run(async () =>
                    {
                        try
                        {
                            var resultAddLicensePlate = await _plateDetectionService.AddLicensePlate(licensePlate);
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
                    DetectorId = _cameraInfo.DeviceId,
                    EventLog = permission ? _logEvents.Authorized : _logEvents.UnAuthorized,
                    LicensePlate = licensePlate,
                    DetectionPrecision = ((float)plate.Accuracy) / 10,
                    LogDateTime = DateTime.Now,
                    FullImage = frameImage,
                    PlateImage = plateImage,
                    SuccessTransfer = false
                };

                await Task.Run(async () =>
                {
                    try
                    {
                        Logger.Log($@"UpdateMonitoring for {detectedPlate}", logType: CommonClasses.LogType.Warning);
                        var restRequest = new RestRequest("UpdatePlateMonitoring/OnUpdatePlate", Method.POST);

                        var tmpPlateNumber = detectedLog.LicensePlate.LicensePlateNumber;
                        try
                        {
                            Logger.Log("Checking Detected Plate", logType: CommonClasses.LogType.Warning);

                            const string pattern = @"[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]";
                            var regexDetect = Regex.Match(tmpPlateNumber, pattern);
                            if (regexDetect.Success)
                                tmpPlateNumber = (tmpPlateNumber.Substring(3, 3) + "-" + tmpPlateNumber.Substring(6, 2) + tmpPlateNumber.Substring(2, 1) + tmpPlateNumber.Substring(0, 2));
                            else
                                tmpPlateNumber = "مشکل در شناسایی پلاک";
                        }
                        catch
                        {
                            tmpPlateNumber = "مشکل در شناسایی پلاک";
                        }

                        //detectedLog.LicensePlate.LicensePlateNumber = tmpPlateNumber;

                        //restRequest.AddJsonBody(new
                        //{
                        //    //resultAddLog.Result.Id,
                        //    DeviceId = detectedLog.DetectorId,
                        //    DeviceCode = _cameraInfo.Code,
                        //    UserId = tmpPlateNumber,
                        //    detectedLog.LogDateTime,
                        //    detectedLog.EventLog,
                        //    MatchingType = MatchingTypes.Car,
                        //    PicByte = detectedLog.PlateImage,
                        //    SubEvent = LogSubEvents.Normal,
                        //    DeviceName = _cameraInfo.Name,
                        //});
                        restRequest.AddJsonBody(detectedLog);
                        await _logExternalSubmissionRestClient.ExecuteAsync(restRequest);

                        var altRestRequest = new RestRequest("UpdateMonitoring/UpdateMonitoring", Method.POST);
                        altRestRequest.AddJsonBody(detectedLog);
                        await _logExternalSubmissionRestClient.ExecuteAsync(altRestRequest);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                });

                await _plateDetectionService.AddPlateDetectionLog(detectedLog);



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

        public byte[] ImageToByteArray(Image imageIn)
        {
            using var ms = new MemoryStream();
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }

        private static string BiovationPlateFormat(string plateNumber)
        {
            //GetPersianNumber
            plateNumber = string.Concat(plateNumber.Where(c => !char.IsWhiteSpace(c)));
            for (var i = 48; i < 58; i++)
            {
                plateNumber = plateNumber.Replace(Convert.ToChar(i), Convert.ToChar(1728 + i));
            }

            //ReverseText
            plateNumber = plateNumber.Substring(6, 2) + plateNumber.Substring(5, 1) + plateNumber.Substring(2, 3) + plateNumber.Substring(0, 2);
            return plateNumber;
        }

        //private void SetTimer()   /////TEEEESSSSTT
        //{
        //    var dirName = $@"D:\\PlateReader\\Enter_test";
        //    var di = Directory.CreateDirectory(dirName);


        //    _plateTimer = new Timer(10000);
        //    _plateTimer.Elapsed += (sender, e) =>
        //    {
        //        if (Directory.Exists(dirName))
        //        {
        //            var dirs = Directory.GetDirectories(dirName);
        //            var address = dirs[_random.Next(dirs.Length)];
        //            string firstLine = File.ReadLines("doc.txt").First();
        //            var tmp = new Plate(1122, null, "99 892ج68", DirectionType.Ingoing, 745, PlateType.Afghan,
        //                address, 0);
        //            var timer = sender as Timer;
        //            timer?.Stop();
        //            ProcessDetectedPlateAsync(tmp);
        //            timer?.Start();
        //        }
        //    };
        //    _plateTimer.Enabled = true;
        //    _plateTimer.AutoReset = true;
        //}
    }

}
