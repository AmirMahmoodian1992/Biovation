using Biovation.Brands.Virdi.UniComAPI;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UNIONCOMM.SDK.UCBioBSP;
using Encoding = System.Text.Encoding;

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


        private readonly Callbacks _callbacks;
        private readonly LogEvents _logEvents;
        private readonly LogService _logService;
        private readonly UserCardService _userCardService;
        private readonly AdminDeviceService _adminDeviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly FaceTemplateService _faceTemplateService;

        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;

        public VirdiSendUserToDevice(IReadOnlyList<object> items, VirdiServer virdiServer, Callbacks callbacks, LogService logService, UserService userService, TaskService taskService, DeviceService deviceService, UserCardService userCardService, BlackListService blackListService, AdminDeviceService adminDeviceService, AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes)
        {
            _callbacks = callbacks;
            _logService = logService;
            _userCardService = userCardService;
            _adminDeviceService = adminDeviceService;
            _accessGroupService = accessGroupService;
            _faceTemplateService = faceTemplateService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: int.Parse(DeviceBrands.VirdiCode)).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;
            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data["UserId"];
            UserObj = userService.GetUsers(UserId,true).FirstOrDefault();

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

                //fpData.ClearFPData();
                _callbacks.ServerUserData.InitUserData();

                _callbacks.ServerUserData.IsBlacklist = UserObj.IsActive ? IsBlackList : 1;
                //                    _callbacks._serveruserData.IsBlacklist = 1;

                var isoEncoding = Encoding.GetEncoding(28591);
                var windowsEncoding = Encoding.GetEncoding(1256);
                //var windowsEncoding = Encoding.UTF32;
                var replacements = new Dictionary<string, string> { { "ک","~"}, {  "ژ" , "Z" } };

                var userName = replacements.Aggregate(UserObj.UserName, (current, replacement) => current.Replace(replacement.Key, replacement.Value));

                //userName = string.IsNullOrEmpty(UserObj.UserName) ? null : isoEncoding.GetString(windowsEncoding.GetBytes(UserObj.UserName));
                userName = string.IsNullOrEmpty(userName) ? null : isoEncoding.GetString(windowsEncoding.GetBytes(userName));

                _callbacks.ServerUserData.UserID = (int)UserObj.Id;
                _callbacks.ServerUserData.UniqueID = UserObj.Id.ToString();
                _callbacks.ServerUserData.UserName = userName;
                _callbacks.ServerUserData.SetPictureData(UserObj.ImageBytes.Length,"3",UserObj.ImageBytes);
              
                var adminDevices = _adminDeviceService.GetAdminDevicesByUserId(personId: UserId);
                _callbacks.ServerUserData.IsAdmin = adminDevices.Any(x => x.DeviceId == DeviceId) ? 1 : 0;

                _callbacks.ServerUserData.IsIdentify = UserObj.IsActive ? 1 : 0;
                // Set Access Flag
                _callbacks.ServerUserData.IsFace1toN = 0;

                var userCards = _userCardService.GetCardsByFilter(UserId);
                var activeCard = userCards?.Find(card => card.IsActive);

                if (activeCard != null)
                {
                    _callbacks.ServerUserData.SetCardData(1, activeCard.CardNum);
                    isCard = true;
                }


                if (!string.IsNullOrEmpty(UserObj.Password))
                {
                    try
                    {
                        var decryptedPassword = Decrypt(UserObj.Password);
                        _callbacks.ServerUserData.Password = decryptedPassword;
                        isPassword = true;
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

                var virdiFinger = UserObj.FingerTemplates.Where(x => x.FingerTemplateType.Code == FingerTemplateTypes.V400Code).ToList();

                if (virdiFinger.Count != 0)
                {
                    for (var i = 0; i < virdiFinger.Count; i += 2)
                    {
                        _callbacks.ServerUserData.AddFingerData(virdiFinger[i].FingerIndex.OrderIndex, (int)UCBioAPI.Type.TEMPLATE_TYPE.SIZE400, virdiFinger[i].Template, virdiFinger[i + 1].Template);
                    }

                    isFingerPrint = true;
                }

                // Face data

                var virdiFace = _faceTemplateService.FaceTemplates(userId: UserObj.Id).FirstOrDefault(w => w.FaceTemplateType.Code == FaceTemplateTypes.VFACECode);
                if (virdiFace != null)
                {
                    _callbacks.ServerUserData.FaceNumber = virdiFace.Index;
                    _callbacks.ServerUserData.FaceData = virdiFace.Template;
                    _callbacks.ServerUserData.IsFace1toN = UserObj.IsActive ? 1 : 0;
                    isFace = true;
                }


                if (isCard && isFingerPrint && isFace)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.CardOrFpOrFace;
                }
                else if (isCard && isPassword)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.CardOrPassword;
                }
                else if (isFace && isPassword)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.FaceOrPassword;
                }
                else if (isFingerPrint && isPassword)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.FpOrPassword;
                }
                else if (isFace && isFingerPrint)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.FpOrFace;
                }
                else if (isCard && isFingerPrint)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.CardOrFp;
                }
                else if (isCard && isFace)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.CardOrFace;
                }
                else if (isFace)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.Face;
                }
                else if (isFingerPrint)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.Fp;
                }
                else if (isCard)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.Card;
                }
                else if (isPassword)
                {
                    _callbacks.ServerUserData.AuthType = (int)AuthType.Password;
                }

                _callbacks.ServerUserData.SetAuthType(Convert.ToInt32(false), Convert.ToInt32(isFingerPrint),
                    Convert.ToInt32(false), Convert.ToInt32(isPassword),
                    Convert.ToInt32(isCard), Convert.ToInt32(false));

                _callbacks.ServerUserData.SetAuthTypeEx(Convert.ToInt32(isFace), 0, 0, 0, 0, 0, 0, 0);

                var userAccessGroups = _accessGroupService.GetAccessGroups(userId: UserObj.Id);

                var validAccessGroup =
                    userAccessGroups.FirstOrDefault(
                        ag => ag.DeviceGroup.Any(dg => dg.Devices.Any(dev => dev.DeviceId == DeviceId)));

                _callbacks.ServerUserData.AccessGroup = validAccessGroup?.Id.ToString("D4") ?? "****";

                if (UserObj.EndDate != default && UserObj.StartDate != default && UserObj.StartDate < UserObj.EndDate)
                    _callbacks.ServerUserData.SetAccessDate(1, UserObj.StartDate.Year, UserObj.StartDate.Month, UserObj.StartDate.Day, UserObj.EndDate.Year, UserObj.EndDate.Month, UserObj.EndDate.Day);

                _callbacks.ServerUserData.AddUserToTerminal(TaskItemId, (int)Code, 1);

                Logger.Log("-->Add user to terminal");

                if (_callbacks.ServerUserData.ErrorCode == 0)
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

                Logger.Log($"  +Cannot transfer user {UserId} to device: {Code}. Error code = {_callbacks.ServerUserData.ErrorCode}\n");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot transfer user {UserId} to device: {Code}. Error code = {_callbacks.ServerUserData.ErrorCode}\n", Validate = 1 };
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
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                if (encryptor != null)
                {
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
                }
            }
            return cipherText;
        }
    }
}
