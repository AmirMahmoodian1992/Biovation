using Biovation.Brands.Virdi.Manager;
using Biovation.Brands.Virdi.UniComAPI;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UNIONCOMM.SDK.UCBioBSP;
using AccessGroupService = Biovation.Service.Api.v1.AccessGroupService;
using AdminDeviceService = Biovation.Service.Api.v1.AdminDeviceService;
using BlackListService = Biovation.Service.Api.v1.BlackListService;
using DeviceService = Biovation.Service.Api.v1.DeviceService;
using Encoding = System.Text.Encoding;
using FaceTemplateService = Biovation.Service.Api.v1.FaceTemplateService;
using LogService = Biovation.Service.Api.v1.LogService;
using TaskService = Biovation.Service.Api.v1.TaskService;
using UserCardService = Biovation.Service.Api.v1.UserCardService;
using UserService = Biovation.Service.Api.v1.UserService;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiSendUserToDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private int UserId { get; }
        private User UserObj { get; }
        private int TaskItemId { get; }
        private int IsBlackList { get; }


        private readonly VirdiServer _virdiServer;
        private readonly LogEvents _logEvents;
        private readonly LogService _logService;
        private readonly UserCardService _userCardService;
        private readonly AdminDeviceService _adminDeviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly IrisTemplateService _irisTemplateService;

        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;
        private readonly VirdiCodeMappings _virdiCodeMappings;

        private readonly List<char> _persianLetters = new List<char>
        {
            'آ', 'ا', 'ب', 'پ', 'ت', 'ث', 'ج', 'چ', 'ح', 'خ', 'د', 'ذ', 'ر', 'ز', 'ژ', 'س', 'ش', 'ص', 'ض', 'ط', 'ظ',
            'ع', 'غ', 'ف', 'ق', 'ک', 'گ', 'ل', 'م', 'ن', 'و', 'ه', 'ی', 'ي', 'ء', 'إ', 'أ', 'ؤ', 'ئ', 'ة', 'ك'
        };

        public VirdiSendUserToDevice(IReadOnlyList<object> items, VirdiServer virdiServer, LogService logService, UserService userService, TaskService taskService, DeviceService deviceService, UserCardService userCardService, BlackListService blackListService, AdminDeviceService adminDeviceService, AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes, VirdiCodeMappings virdiCodeMappings, IrisTemplateService irisTemplateService)
        {
            _virdiServer = virdiServer;
            _logService = logService;
            _userCardService = userCardService;
            _adminDeviceService = adminDeviceService;
            _accessGroupService = accessGroupService;
            _faceTemplateService = faceTemplateService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
            _virdiCodeMappings = virdiCodeMappings;
            _irisTemplateService = irisTemplateService;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;
            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data?["UserId"];
            UserObj = userService.GetUsers(UserId, withPicture: true, getTemplatesData: true).FirstOrDefault();

            var blackList = blackListService.GetBlacklist(id: default, userId: UserId, deviceId: DeviceId, startDate: DateTime.Now, endDate: DateTime.Now).Result.FirstOrDefault();
            IsBlackList = blackList != null ? 1 : 0;

            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"SendUser,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 1 };
            }

            if (UserObj == null)
            {
                Logger.Log($"SendUser,User {UserId} does not exist.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"User {UserId} does not exist.", Validate = 1 };
            }

            try
            {
                var isFingerPrint = false;
                var isCard = false;
                var isPassword = false;
                var isFace = false;
                var isIris = false;

                //fpData.ClearFPData();
                _virdiServer.ServerUserData.InitUserData();

                _virdiServer.ServerUserData.IsBlacklist = UserObj.IsActive ? IsBlackList : 1;
                //                    _virdiServer._serveruserData.IsBlacklist = 1;

                var isoEncoding = Encoding.GetEncoding(28591);
                var windowsEncoding = Encoding.GetEncoding(1256);
                //var windowsEncoding = Encoding.UTF32;
                var userName = UserObj.UserName ?? string.Empty;
                userName = string.IsNullOrEmpty(userName) ? null : isoEncoding.GetString(windowsEncoding.GetBytes(userName));

                if (UserObj.UserName?.Count(c => _persianLetters.Contains(c)) > 3)
                {
                    var replacements = new Dictionary<string, string> { { "\u0098", "˜" }, { "\u008e", "Ž" } };
                    userName = replacements.Aggregate(userName, (current, replacement) => current.Replace(replacement.Key, replacement.Value));
                }

                _virdiServer.ServerUserData.UserID = (int)UserObj.Code;
                _virdiServer.ServerUserData.UniqueID = Math.Abs(UserObj.UniqueId).ToString();
                _virdiServer.ServerUserData.UserName = userName;

                if (UserObj.ImageBytes != null && UserObj.ImageBytes.Length > 0)
                    _virdiServer.ServerUserData.SetPictureData(UserObj.ImageBytes.Length, "JPG", UserObj.ImageBytes);

                var adminDevices = _adminDeviceService.GetAdminDevicesByUserId(personId: UserId);
                _virdiServer.ServerUserData.IsAdmin = adminDevices.Any(x => x.DeviceId == DeviceId) ? 1 : 0;

                _virdiServer.ServerUserData.IsIdentify = UserObj.IsActive ? 1 : 0;
                // Set Access Flag
                _virdiServer.ServerUserData.IsFace1toN = 0;

                var userCards = _userCardService.GetCardsByFilter(UserId).Result;
                var activeCard = userCards?.Find(card => card.IsActive);

                if (activeCard != null)
                {
                    _virdiServer.ServerUserData.SetCardData(1, activeCard.CardNum);
                    isCard = true;
                }


                if (!string.IsNullOrEmpty(UserObj.Password))
                {
                    try
                    {
                        //var decryptedPassword = Encoding.ASCII.GetBytes(UserObj.Password);
                        //_virdiServer.ServerUserData.Password = decryptedPassword;
                        _virdiServer.ServerUserData.Password = UserObj.Password;
                        isPassword = true;
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e);
                    }
                }

                var virdiFinger = UserObj.FingerTemplates.Where(x => x.FingerTemplateType.Code == FingerTemplateTypes.V400Code).ToList();

                if (virdiFinger.Count != 0)
                {
                    for (var i = 0; i < virdiFinger.Count; i += 2)
                    {
                        _virdiServer.ServerUserData.AddFingerData(virdiFinger[i].FingerIndex.OrderIndex, (int)UCBioAPI.Type.TEMPLATE_TYPE.SIZE400, virdiFinger[i].Template, virdiFinger[i + 1].Template);
                    }

                    isFingerPrint = true;
                }

                // Face data

                var virdiFace = _faceTemplateService.FaceTemplates(userId: UserObj.Id).FirstOrDefault(w => w.FaceTemplateType.Code == FaceTemplateTypes.VFACECode);
                if (virdiFace != null)
                {
                    _virdiServer.ServerUserData.FaceNumber = virdiFace.Index;
                    _virdiServer.ServerUserData.FaceData = virdiFace.Template;
                    _virdiServer.ServerUserData.IsFace1toN = UserObj.IsActive ? 1 : 0;
                    isFace = true;
                }

                var vWthFace = _faceTemplateService.FaceTemplates(userId: UserObj.Id).FirstOrDefault(w => w.FaceTemplateType.Code == FaceTemplateTypes.VWTFACECode);
                if (vWthFace != null && vWthFace.Size > 0)
                {
                    var dataType = _virdiCodeMappings.GetFaceTemplateManufactureCode(FaceTemplateTypes.VWTFACECode);
                    _virdiServer.ServerUserData.SetWalkThroughData(int.Parse(dataType), vWthFace.Size, vWthFace.Template);
                    _virdiServer.ServerUserData.IsFace1toN = UserObj.IsActive ? 1 : 0;
                    isFace = true;
                }

                //Iris
                var irisData = _irisTemplateService.IrisTemplates(userId: UserObj.Id)
                    .FirstOrDefault(i => i.IrisTemplateType.Code == IrisTemplateTypes.VIrisCode);
                if (irisData != null && irisData.Size > 0)
                {
                    _virdiServer.ServerUserData.SetIrisData(irisData.Size, irisData.Template);
                    _virdiServer.ServerUserData.IsIris1toN = UserObj.IsActive ? 1 : 0;
                    isIris = true;
                }


                if (isCard && isFingerPrint && (isFace || isIris))
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.CardOrFpOrFace;
                }
                else if (isCard && isPassword)
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.CardOrPassword;
                }
                else if ((isFace || isIris) && isPassword)
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.FaceOrPassword;
                }
                else if (isFingerPrint && isPassword)
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.FpOrPassword;
                }
                else if ((isFace || isIris) && isFingerPrint)
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.FpOrFace;
                }
                else if (isCard && isFingerPrint)
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.CardOrFp;
                }
                else if (isCard && (isFace || isIris))
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.CardOrFace;
                }
                else if ((isFace || isIris))
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.Face;
                }
                else if (isFingerPrint)
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.Fp;
                }
                else if (isCard)
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.Card;
                }
                else if (isPassword)
                {
                    _virdiServer.ServerUserData.AuthType = (int)AuthType.Password;
                }

                _virdiServer.ServerUserData.SetAuthType(Convert.ToInt32(false), Convert.ToInt32(isFingerPrint),
                    Convert.ToInt32(false), Convert.ToInt32(isPassword),
                    Convert.ToInt32(isCard), Convert.ToInt32(isIris));

                _virdiServer.ServerUserData.SetAuthTypeEx(Convert.ToInt32(isFace), 0, 0, Convert.ToInt32(isIris), 0, 0, 0, 0);

                var userAccessGroups = _accessGroupService.GetAccessGroups(userId: UserObj.Id);

                var validAccessGroup =
                    userAccessGroups.FirstOrDefault(
                        ag => ag.DeviceGroup.Any(dg => dg.Devices.Any(dev => dev.DeviceId == DeviceId)));

                _virdiServer.ServerUserData.AccessGroup = validAccessGroup?.Id.ToString("D4") ?? "****";

                if (UserObj.EndDate != default && UserObj.StartDate != default && UserObj.StartDate < UserObj.EndDate)
                    _virdiServer.ServerUserData.SetAccessDate(1, UserObj.StartDate.Year, UserObj.StartDate.Month, UserObj.StartDate.Day, UserObj.EndDate.Year, UserObj.EndDate.Month, UserObj.EndDate.Day);

                _virdiServer.ServerUserData.AddUserToTerminal(TaskItemId, (int)Code, 1);

                Logger.Log("-->Add user to terminal");

                if (_virdiServer.ServerUserData.ErrorCode == 0)
                {
                    Logger.Log($"  +User {UserId} successfully transferred to device: {Code}.");

                    var log = new Log
                    {
                        DeviceId = DeviceId,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.AddUserToDevice,
                        UserId = UserId,
                        MatchingType = _matchingTypes.Unknown,
                        SubEvent = _logSubEvents.Normal,
                        TnaEvent = 0
                    };

                    _logService.AddLog(log);
                    return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = DeviceId, Message = $"  +User {UserId} successfully transferred to device: {Code}.", Validate = 1 };
                }

                Logger.Log($"  +Cannot transfer user {UserId} to device: {Code}. Error code = {_virdiServer.ServerUserData.ErrorCode}\n");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot transfer user {UserId} to device: {Code}. Error code = {_virdiServer.ServerUserData.ErrorCode}\n", Validate = 1 };
            }
            catch (Exception ex)
            {
                Logger.Log("Error! ErrorMessage:{0}", ex.Message);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"Error! ErrorMessage:{ex.Message}", Validate = 1 };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Add user to terminal";
        }

        public string GetDescription()
        {
            return $"Adding user: {UserId} to device: {Code}.";
        }

        public static string Decrypt(string cipherText)
        {
            var EncryptionKey = "Kasra";
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using var encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
          
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(),
                    CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }

                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            

            return cipherText;
        }
    }
}
