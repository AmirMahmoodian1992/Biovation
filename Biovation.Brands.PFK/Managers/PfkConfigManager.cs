using Biovation.Constants;
using Biovation.Domain;
using PFKParkingLibrary.Data;
using PFKParkingLibrary.Devices;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Logger = Biovation.CommonClasses.Logger;
using LogType = Biovation.CommonClasses.LogType;

namespace Biovation.Brands.PFK.Managers
{
    public static class PfkConfigManager
    {
        public static Config SetConfiguration(this Config config, Camera cameraInfo)
        {
            var cameraAddress = cameraInfo.ConnectionUrl.Split('@').LastOrDefault();
            var cameraConnectionUserName = cameraInfo.ConnectionUrl.Split('@').FirstOrDefault()?.Split('/').LastOrDefault()?.Split(':').FirstOrDefault();
            var cameraConnectionPassword = cameraInfo.ConnectionUrl.Split('@').FirstOrDefault()?.Split('/').LastOrDefault()?.Split(':').LastOrDefault();

            var executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var plateDetectionBasePath = Path.Combine(executingPath!, @"PlateReader");
            var plateDetectionSavePath = Path.Combine(executingPath!, $@"PlateReader\Enter{cameraInfo.Code}");
            var plateDetectionResultPath = Path.Combine(executingPath!, $@"PlateReader\Result{cameraInfo.Code}");
            var plateDetectionInstancePath = Path.Combine(executingPath!, $@"PlateReader\LPR{cameraInfo.Code}");

            Logger.Log($"Configuring Camera: {cameraInfo.Code}");
            if (!Directory.Exists(plateDetectionInstancePath))
            {
                Logger.Log("The main LPR directory does not exist, creating the directory");
                Directory.CreateDirectory(plateDetectionInstancePath);

                var files = Directory.GetFiles(Path.Combine(executingPath, @"PlateReader\LPR0"));
                foreach (var file in files)
                {
                    File.Copy(file, Path.Combine(plateDetectionInstancePath, Path.GetFileName(file)), false);
                }
            }

            var configGenerationResult = Generate(plateDetectionInstancePath);
            if (configGenerationResult == ResultEnum.OK || configGenerationResult == ResultEnum.FileExist)
            {
                Logger.Log($"File generation result: {(configGenerationResult == ResultEnum.OK ? "OK" : "FileExists")}");
                var outputConfigResult = SetOutPutConfig(plateDetectionInstancePath, plateDetectionBasePath, "H:", plateDetectionResultPath);
                var inputImageConfigResult = SetInputImageConfig(plateDetectionInstancePath, cameraInfo.ImageWidth, cameraInfo.ImageHeight, 3);
                //var cpuConfigResult = SetCpuConfig(plateDetectionInstancePath, 240);
                //var calibrationConfigResult = SetCalibrationConfig(plateDetectionInstancePath, 0, 15, 0, 3000, 2);

                Logger.Log(
                    //$"The Output Config Result = {outputConfigResult},{Environment.NewLine}    Input Image Config Result = {inputImageConfigResult},{Environment.NewLine}   Cpu Config Result = {cpuConfigResult},{Environment.NewLine}    Calibration Config Result = {calibrationConfigResult}",
                    $"The Output Config Result = {outputConfigResult},{Environment.NewLine}    Input Image Config Result = {inputImageConfigResult},{Environment.NewLine}",
                    string.Empty, LogType.Information);
            }


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

            //config.CamID = 0;
            config.CamID = (int)cameraInfo.Id;
            Logger.Log($"Camera {cameraInfo.Code} { (string.Equals(cameraInfo.Model.Id.ToString(), CameraModels.PanasonicCode, StringComparison.InvariantCultureIgnoreCase) ? $" the camera is panasonic and the IP is : {cameraInfo.ConnectionInfo.Ip}" : $" the camera is generic and the address is : {cameraAddress}")}");
            config.CameraConfig = new IpCamerConfig
            {
                //CameraIP = string.Equals(cameraInfo.Model.Id.ToString(), CameraModels.PanasonicCode, StringComparison.InvariantCultureIgnoreCase) ? cameraInfo.ConnectionInfo.Ip : cameraAddress,
                CameraIP = cameraAddress,
                CameraImageHeight = cameraInfo.ImageHeight,
                CameraImageWidth = cameraInfo.ImageWidth,
                CameraPass = cameraConnectionPassword,
                CameraRecordinFolder = $@"{executingPath}\PlateReader\Record{cameraInfo.Code}",
                //CameraType = string.Equals(cameraInfo.Model.Id.ToString(), CameraModels.PanasonicCode, StringComparison.InvariantCultureIgnoreCase) ? CameraType.Panasonic : CameraType.RTSP,
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
                Index = (int)cameraInfo.Id,
                IngouingDir = new PointF(-1, -1),
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
                CameraImageHeight = cameraInfo.ImageHeight,
                CameraImageWidth = cameraInfo.ImageWidth,
                CameraPass = default,
                //CameraRecordinFolder = $@"D:\FaceCameraLogs\{cameraInfo.Code}",
                CameraRecordinFolder = $@"{executingPath}\PlateReader\FaceCameraLogs\{cameraInfo.Code}",
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

            //config.SaveFilesCopyDir = $@"D:\PlateLogs\Enter{cameraInfo.DeviceId}";
            //config.SaveFilesCopyDir = $@"D:\PlateReader\Enter{cameraInfo.Code}";
            config.SaveFilesCopyDir = plateDetectionSavePath;
            config.SaveRowPlates = false;
            //config.SaveRowPlatesPath1 = $@"D:\PlateLogs\RowPlates{cameraInfo.Code}_1";
            config.SaveRowPlatesPath1 = $@"{executingPath}\PlateReader\Logs\RowPlates{cameraInfo.Code}_1";
            //config.SaveRowPlatesPath2 = $@"D:\PlateLogs\RowPlates{cameraInfo.Code}_2";
            config.SaveRowPlatesPath2 = $@"{executingPath}\PlateReader\Logs\RowPlates{cameraInfo.Code}_2";
            config.SenderConfig = new SenderConfig
            {
                DfsIndex = 0,
                DisableOCR = false,
                SenderIP = "127.0.0.1",
                SenderMaximumdelaySecond = 2,
                SenderPort = (int)(11124 + 6 * (cameraInfo.Code % 350) + 1),
                //SenderResulSavePath = $@"D:\PlateLogs\Result{cameraInfo.DeviceId}",
                //SenderResulSavePath = $@"D:\LPR{cameraInfo.Code}",
                SenderResulSavePath = plateDetectionResultPath,
                //Senderpath = $@"D:\PlateReader\LPR{cameraInfo.Code}\generalRawFixCamLPR.exe",
                Senderpath = $@"{plateDetectionInstancePath}\generalRawFixCamLPR.exe",
                //Senderworkingpath = $@"D:\PlateReader\LPR{cameraInfo.Code}",
                Senderworkingpath = plateDetectionInstancePath,
                TmpDir = "H:\\"
            };

            config.UseSoftWatchDog = false;
            //config.WorkingDir = @"D:\PlateReader";
            config.WorkingDir = plateDetectionBasePath;

            return config;
        }

        private static void MakeDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    return;
                Directory.CreateDirectory(path);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }
        }

        public static ResultEnum Generate(string path)
        {
            if (File.Exists(path + "\\basic_param.XML"))
                return ResultEnum.FileExist;
            MakeDirectory(path);
            XmlDocument xmlDocument = new XmlDocument();
            string xml = "<?xml version=\"1.0\"?><opencv_storage><GRAB_IMAGE_BUFFER_SIZE>50</GRAB_IMAGE_BUFFER_SIZE><advanceShareUserName>behkar</advanceShareUserName><advanceSharePass>\"12321\"</advanceSharePass><BASE_FOLDER_FOR_SAVE>D:</BASE_FOLDER_FOR_SAVE><BASE_RAM_DISK_FOLDER>H:</BASE_RAM_DISK_FOLDER><saveResultFolderPath>D:\\LPR0</saveResultFolderPath><PLATE_READING_PLATE_TYPE_CODE>1</PLATE_READING_PLATE_TYPE_CODE><GRAB_IMAGE_CONFIG><GRAB_IMAGE_COLS>1920</GRAB_IMAGE_COLS><GRAB_IMAGE_ROWS>1080</GRAB_IMAGE_ROWS><GRAB_IMAGE_CHANNEL>3</GRAB_IMAGE_CHANNEL></GRAB_IMAGE_CONFIG><MARGIN_3D_ROTATE><IS_3D_ROTATION_RUN>0</IS_3D_ROTATION_RUN><TOP_WORK>0</TOP_WORK><DOWN_WORK>1080</DOWN_WORK><LEFT_WORK>0</LEFT_WORK><RIGHT_WORK>1920</RIGHT_WORK><Camera_alfa_3DRotate>0.</Camera_alfa_3DRotate><Camera_beta_3DRotate>0.</Camera_beta_3DRotate><Camera_gama_3DRotate>0.</Camera_gama_3DRotate></MARGIN_3D_ROTATE><PlateLocateConfig><CDIM_MAX_H>40.</CDIM_MAX_H><CDIM_MIN_H>20.</CDIM_MIN_H><PREDICTED_H_CALC_SHIB>8.7999999999999988e-03</PREDICTED_H_CALC_SHIB><PREDICTED_H_CALC_ARZ>3.1960000000000001e+01</PREDICTED_H_CALC_ARZ><MAX_PLATE_ROTATION_DEGREE>15.</MAX_PLATE_ROTATION_DEGREE><LOCATE_PLATE_ALFA_DEGREE>0.</LOCATE_PLATE_ALFA_DEGREE><LOCATE_PLATE_BETA_DEGREE>15.</LOCATE_PLATE_BETA_DEGREE><LOCATE_PLATE_GAMA_DEGREE>0.</LOCATE_PLATE_GAMA_DEGREE></PlateLocateConfig><CPU_Config><ALL_CPU_ID>255.</ALL_CPU_ID><IO_CPU_ID>255.</IO_CPU_ID><LPR_CPU_ID>255.</LPR_CPU_ID><IMAGE_GRAB_CPU_ID>255.</IMAGE_GRAB_CPU_ID></CPU_Config><SOCKET_PORT_IP><SEND_PLATES_INFO_SOCKET_PORT>1231</SEND_PLATES_INFO_SOCKET_PORT><GET_REQUEST_SOCKET_PORT>1232</GET_REQUEST_SOCKET_PORT><PLATE_MANAGER_SOCKET_IP>\"127.0.0.1\"</PLATE_MANAGER_SOCKET_IP></SOCKET_PORT_IP><SEND_LIVE_IMAGE><SEND_LIVE_IMAGE_SOCKET_PORT>11521</SEND_LIVE_IMAGE_SOCKET_PORT><SEND_LIVE_IMAGE_RESIZE_RATIO>2.</SEND_LIVE_IMAGE_RESIZE_RATIO></SEND_LIVE_IMAGE><AlfaBetaGama><Alfa>0.</Alfa><BETA>15.</BETA><GAMA>0.</GAMA></AlfaBetaGama><CAMERA_BASE_PARAM><Camera_F>3000.</Camera_F><CAMERA_HEIGHT>2.</CAMERA_HEIGHT><CAMERA_PERIOD_SECOND>2.0000000000000001e-01</CAMERA_PERIOD_SECOND></CAMERA_BASE_PARAM></opencv_storage>";
            xmlDocument.LoadXml(xml);
            if (!Directory.Exists(path))
                return ResultEnum.DirNotValid;
            try
            {
                xmlDocument.Save(path + "\\basic_param.XML");
                return ResultEnum.OK;
            }
            catch (Exception ex)
            {
                return ResultEnum.Exception;
            }
        }

        public static ResultEnum SetOutPutConfig(string path, string watchDogFolder, string logFolder, string resultFolder)
        {
            if (!File.Exists(path + "\\basic_param.XML"))
                return ResultEnum.FileNotExist;


            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path + "\\basic_param.XML");
            Logger.Log($"Checking watch dog folder existence on: {watchDogFolder}");
            MakeDirectory(watchDogFolder);
            if (!Directory.Exists(watchDogFolder))
            {
                Logger.Log("Watch dog directory does not exist");
                return ResultEnum.DirNotValid;
            }
            watchDogFolder = "\"" + watchDogFolder.Trim('\"') + "\"";
            xmlDocument.SelectSingleNode("opencv_storage/BASE_FOLDER_FOR_SAVE").ChildNodes[0].InnerText = watchDogFolder;

            Logger.Log($"Checking log folder existence on: {logFolder}");
            MakeDirectory(logFolder);
            if (!Directory.Exists(logFolder))
            {
                Logger.Log("Log directory does not exist");
                return ResultEnum.DirNotValid;
            }
            logFolder = "\"" + logFolder.Trim('\"') + "\"";
            xmlDocument.SelectSingleNode("opencv_storage/BASE_RAM_DISK_FOLDER").ChildNodes[0].InnerText = logFolder;

            Logger.Log($"Checking result folder existence on: {resultFolder}");
            MakeDirectory(resultFolder);
            if (!Directory.Exists(resultFolder))
            {
                Logger.Log("Result directory does not exist");
                return ResultEnum.DirNotValid;
            }
            resultFolder = "\"" + resultFolder.Trim('\"') + "\"";
            xmlDocument.SelectSingleNode("opencv_storage/saveResultFolderPath").ChildNodes[0].InnerText = resultFolder;
            try
            {
                //Logger.Log(xmlDocument.InnerText);
                xmlDocument.Save(path + "\\basic_param.XML");
                return ResultEnum.OK;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return ResultEnum.Exception;
            }
        }

        public static ResultEnum SetCpuConfig(string path, int? allCpuId, int? ioCpuId = default, int? lprCpuId = default, int? imageGrabbingCpuId = default)
        {
            if (!File.Exists(path + "\\basic_param.XML"))
                return ResultEnum.FileNotExist;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path + "\\basic_param.XML");
            XmlNodeList childNodes = xmlDocument.SelectSingleNode("opencv_storage/CPU_Config").ChildNodes;
            int? allCpuId1 = allCpuId;
            int num = 0;
            if (!(allCpuId1.GetValueOrDefault() > num & allCpuId1.HasValue))
                return ResultEnum.ValueNotValid;
            XmlNode xmlNode1 = childNodes[0];
            int? allCpuId2 = allCpuId;
            string str1 = allCpuId2.ToString();
            xmlNode1.InnerText = str1;
            XmlNode xmlNode2 = childNodes[1];
            allCpuId2 = ioCpuId ?? allCpuId;
            string str2 = allCpuId2.ToString();
            xmlNode2.InnerText = str2;
            XmlNode xmlNode3 = childNodes[2];
            allCpuId2 = lprCpuId ?? allCpuId;
            string str3 = allCpuId2.ToString();
            xmlNode3.InnerText = str3;
            XmlNode xmlNode4 = childNodes[3];
            allCpuId2 = imageGrabbingCpuId ?? allCpuId;
            string str4 = allCpuId2.ToString();
            xmlNode4.InnerText = str4;
            try
            {
                xmlDocument.Save(path + "\\basic_param.XML");
                return ResultEnum.OK;
            }
            catch (Exception ex)
            {
                return ResultEnum.Exception;
            }
        }

        public static ResultEnum SetInputImageConfig(string path, int grabImageCols, int grabImageRows, int grabImageChannel)
        {
            if (!File.Exists(path + "\\basic_param.XML"))
                return ResultEnum.FileExist;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path + "\\basic_param.XML");
            XmlNodeList childNodes = xmlDocument.SelectSingleNode("opencv_storage/GRAB_IMAGE_CONFIG").ChildNodes;
            int num;
            if (grabImageCols > 0)
            {
                XmlNode xmlNode = childNodes[0];
                num = grabImageCols;
                string str = num.ToString();
                xmlNode.InnerText = str;
            }
            if (grabImageRows > 0)
            {
                XmlNode xmlNode = childNodes[1];
                num = grabImageRows;
                string str = num.ToString();
                xmlNode.InnerText = str;
            }
            if (grabImageChannel == 1)
            {
                XmlNode xmlNode = childNodes[2];
                num = grabImageChannel;
                string str = num.ToString();
                xmlNode.InnerText = str;
            }
            else
                childNodes[2].InnerText = "3";
            try
            {
                xmlDocument.Save(path + "\\basic_param.XML");
                return ResultEnum.OK;
            }
            catch (Exception ex)
            {
                return ResultEnum.Exception;
            }
        }

        public static ResultEnum SetCalibrationConfig(string path, int? Alfa, int? Beta, int? Gama, int? CameraF, float? CameraHeight)
        {
            if (!File.Exists(path + "\\basic_param.XML"))
                return ResultEnum.FileNotExist;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path + "\\basic_param.XML");
            XmlNodeList childNodes1 = xmlDocument.SelectSingleNode("opencv_storage/AlfaBetaGama").ChildNodes;
            XmlNodeList childNodes2 = xmlDocument.SelectSingleNode("opencv_storage/PlateLocateConfig").ChildNodes;
            int? alfa = Alfa;
            int num1 = 25;
            int? nullable;
            int num2;
            if (alfa.GetValueOrDefault() < num1 & alfa.HasValue)
            {
                nullable = Alfa;
                int num3 = -25;
                num2 = nullable.GetValueOrDefault() > num3 & nullable.HasValue ? 1 : 0;
            }
            else
                num2 = 0;
            if (num2 == 0)
                return ResultEnum.ValueNotValid;
            XmlNode xmlNode1 = childNodes1[0];
            nullable = Alfa;
            string str1 = nullable.ToString();
            xmlNode1.InnerText = str1;
            XmlNode xmlNode2 = childNodes2[5];
            nullable = Alfa;
            string str2 = nullable.ToString();
            xmlNode2.InnerText = str2;
            nullable = Beta;
            int num4 = 25;
            int num5;
            if (nullable.GetValueOrDefault() < num4 & nullable.HasValue)
            {
                nullable = Beta;
                int num3 = -25;
                num5 = nullable.GetValueOrDefault() > num3 & nullable.HasValue ? 1 : 0;
            }
            else
                num5 = 0;
            if (num5 == 0)
                return ResultEnum.ValueNotValid;
            XmlNode xmlNode3 = childNodes1[1];
            nullable = Beta;
            string str3 = nullable.ToString();
            xmlNode3.InnerText = str3;
            XmlNode xmlNode4 = childNodes2[6];
            nullable = Beta;
            string str4 = nullable.ToString();
            xmlNode4.InnerText = str4;
            nullable = Gama;
            if (nullable.HasValue)
            {
                XmlNode xmlNode5 = childNodes1[2];
                nullable = Gama;
                string str5 = nullable.ToString();
                xmlNode5.InnerText = str5;
                XmlNode xmlNode6 = childNodes2[7];
                nullable = Gama;
                string str6 = nullable.ToString();
                xmlNode6.InnerText = str6;
            }
            XmlNodeList childNodes3 = xmlDocument.SelectSingleNode("opencv_storage/CAMERA_BASE_PARAM").ChildNodes;
            nullable = CameraF;
            int num6;
            if (nullable.HasValue)
            {
                nullable = CameraF;
                int num3 = 0;
                num6 = nullable.GetValueOrDefault() > num3 & nullable.HasValue ? 1 : 0;
            }
            else
                num6 = 0;
            if (num6 != 0)
            {
                XmlNode xmlNode5 = childNodes3[0];
                nullable = CameraF;
                string str5 = nullable.ToString();
                xmlNode5.InnerText = str5;
            }
            float? cameraHeight;
            int num7;
            if (CameraHeight.HasValue)
            {
                cameraHeight = CameraHeight;
                float num3 = 0.0f;
                num7 = cameraHeight.GetValueOrDefault() > (double)num3 & cameraHeight.HasValue ? 1 : 0;
            }
            else
                num7 = 0;
            if (num7 != 0)
            {
                XmlNode xmlNode5 = childNodes3[1];
                cameraHeight = CameraHeight;
                string str5 = cameraHeight.ToString();
                xmlNode5.InnerText = str5;
            }
            try
            {
                xmlDocument.Save(path + "\\basic_param.XML");
                return ResultEnum.OK;
            }
            catch (Exception ex)
            {
                return ResultEnum.Exception;
            }
        }

        public enum ResultEnum
        {
            OK,
            FileExist,
            FileNotExist,
            DirNotValid,
            ValueNotValid,
            Exception,
        }
    }
}
