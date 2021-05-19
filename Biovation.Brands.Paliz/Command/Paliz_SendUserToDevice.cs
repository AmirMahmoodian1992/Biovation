using Biovation.Brands.Paliz;
using Biovation.Brands.Paliz.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Encoding = System.Text.Encoding;

namespace Biovation.Brands.Virdi.Command
{
    public class PalizSendUserToDevice : ICommand
    {
        private Dictionary<uint, DeviceBasicInfo> _onlineDevices { get; }
        private readonly UserService _userService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FaceTemplateTypes _faceTemplateTypes;

        private readonly UserCardService _userCardService;
        private readonly TaskService _taskService;
        private readonly LogService _logService;

        //private UserActionEventHandler _getUserResult;
        private int TaskItemId { get; }
        private string terminalName { get; }
        private int terminalId { get; }
        private uint code { get; }
        private User _userObj { get; set; }
        private uint _userId;
        private int taskItemId { get; }
        private readonly PalizServer _palizServer;

        private DeviceBasicInfo onlineDevice { get; set; }

        private LogEvents _logEvents;
        private PalizCodeMappings _palizCodeMappings;
        private bool userSendingFinished;

        /// <summary>
        /// All connected devices
        /// </summary>
        //private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        //private int DeviceId { get; }
        //private uint Code { get; }
        //private int UserId { get; }
        //private User UserObj { get; }
        //private int TaskItemId { get; }
        //private int IsBlackList { get; }

        //private readonly VirdiServer _virdiServer;
        //private readonly LogEvents _logEvents;
        //private readonly LogService _logService;
        //private readonly UserCardService _userCardService;
        //private readonly AdminDeviceService _adminDeviceService;
        //private readonly AccessGroupService _accessGroupService;
        //private readonly FaceTemplateService _faceTemplateService;

        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;

        private readonly List<char> _persianLetters = new List<char>
        {
            'آ', 'ا', 'ب', 'پ', 'ت', 'ث', 'ج', 'چ', 'ح', 'خ', 'د', 'ذ', 'ر', 'ز', 'ژ', 'س', 'ش', 'ص', 'ض', 'ط', 'ظ',
            'ع', 'غ', 'ف', 'ق', 'ک', 'گ', 'ل', 'م', 'ن', 'و', 'ه', 'ی', 'ي', 'ء', 'إ', 'أ', 'ؤ', 'ئ', 'ة', 'ك'
        };

        public PalizSendUserToDevice(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService
                , DeviceService deviceService, UserService userService, BiometricTemplateManager biometricTemplateManager
                , FingerTemplateTypes fingerTemplateTypes, FingerTemplateService fingerTemplateService, LogService logService
                , FaceTemplateService faceTemplateService, FaceTemplateTypes faceTemplateTypes, UserCardService userCardService)
        {
            terminalId = Convert.ToInt32(items[0]);
            taskItemId = Convert.ToInt32(items[1]);
            //var taskItem = taskService.GetTaskItem(TaskItemId)?.GetAwaiter().GetResult().Data ?? new TaskItem();
            //var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            //if (data != null)
            //{
            //    UserId = (int)data["userId"];
            //}

            _palizServer = palizServer;
            _userService = userService;
            _logService = _logService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _fingerTemplateService = fingerTemplateService;
            _faceTemplateService = faceTemplateService;
            _faceTemplateTypes = faceTemplateTypes;
            _userCardService = userCardService;

            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode).GetAwaiter().GetResult();
            if (devices is null)
            {
                _onlineDevices = new Dictionary<uint, DeviceBasicInfo>();
                return;
            }

            code = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == terminalId)?.Code ?? 7;
            terminalName = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == terminalId)?.Name ?? string.Empty;
            _onlineDevices = palizServer.GetOnlineDevices();
        }

        //public VirdiSendUserToDevice(IReadOnlyList<object> items, VirdiServer virdiServer, LogService logService, UserService userService, TaskService taskService, DeviceService deviceService, UserCardService userCardService, BlackListService blackListService, AdminDeviceService adminDeviceService, AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes)
        //{
        //    _virdiServer = virdiServer;
        //    _logService = logService;
        //    _userCardService = userCardService;
        //    _adminDeviceService = adminDeviceService;
        //    _accessGroupService = accessGroupService;
        //    _faceTemplateService = faceTemplateService;
        //    _logEvents = logEvents;
        //    _logSubEvents = logSubEvents;
        //    _matchingTypes = matchingTypes;

        //    DeviceId = Convert.ToInt32(items[0]);
        //    TaskItemId = Convert.ToInt32(items[1]);
        //    Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;
        //    var taskItem = taskService.GetTaskItem(TaskItemId);
        //    var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
        //    UserId = (int)data["UserId"];
        //    UserObj = userService.GetUsers(UserId, withPicture: true, getTemplatesData: true).FirstOrDefault();

        //    var blackList = blackListService.GetBlacklist(id: default, userId: UserId, deviceId: DeviceId, startDate: DateTime.Now, endDate: DateTime.Now).Result.FirstOrDefault();
        //    IsBlackList = blackList != null ? 1 : 0;

        //    OnlineDevices = virdiServer.GetOnlineDevices();
        //}

        public object Execute()
        {
            if (_onlineDevices.All(device => device.Key != code))
            {
                Logger.Log($"SendUserToDevice,The device: {code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = terminalId, Message = $"The device: {code} is not connected.", Validate = 1 };
            }

            onlineDevice = _onlineDevices.FirstOrDefault(device => device.Key == code).Value;

            try
            {
                var taskItem = _taskService.GetTaskItem(TaskItemId).GetAwaiter().GetResult().Data;
                var taskData = JsonConvert.DeserializeObject<JObject>(taskItem.Data);
                if (taskData != null && taskData.ContainsKey("UserId"))
                {
                    var parseResult = uint.TryParse(taskData["UserId"].ToString() ?? "0", out var userId);

                    if (!parseResult || userId == 0)
                        return new ResultViewModel
                        {
                            Id = taskItem.Id,
                            Code = Convert.ToInt64(TaskStatuses.FailedCode),
                            Message =
                                $"Error in processing task item {taskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}",
                            Validate = 0
                        };

                    _userId = userId;
                    _userObj = _userService.GetUsers(userId: _userId, getTemplatesData: true)?.GetAwaiter().GetResult()?.Data.Data.FirstOrDefault();

                    if (_userObj == null)
                    {
                        Logger.Log($"User {_userId} does not exist.");
                        return new ResultViewModel { Validate = 0, Id = taskItem.Id, Message = $"User {_userId} does not exist.", Code = Convert.ToInt64(TaskStatuses.FailedCode) };
                    }
                }
                else
                {
                    try
                    {
                        _userObj = JsonConvert.DeserializeObject<User>(taskItem.Data);
                    }
                    catch (Exception)
                    {
                        Logger.Log($"Bad user data in task item {taskItem.Id}.");
                        return new ResultViewModel { Validate = 0, Id = taskItem.Id, Message = $"Bad user data in task item {taskItem.Id}.", Code = Convert.ToInt64(TaskStatuses.FailedCode) };
                    }
                }

                if (_userObj == null)
                {
                    Logger.Log($"User {_userId} does not exist.");
                    return new ResultViewModel { Validate = 0, Id = terminalId, Message = $"User {_userId} does not exist.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
                }

                //var isFingerPrint = false;
                //var isCard = false;
                //var isPassword = false;
                //var isFace = false;

                var isoEncoding = Encoding.GetEncoding(28591);
                var windowsEncoding = Encoding.GetEncoding(1256);
                var userName = _userObj.UserName ?? string.Empty;
                userName = string.IsNullOrEmpty(userName) ? null : isoEncoding.GetString(windowsEncoding.GetBytes(userName));

                var request = new UserInfoModel
                {
                    Id = _userObj.Id,
                    Image = _userObj.ImageBytes,
                    Level = _userObj.AdminLevel,
                    Name = userName,
                    VerificationType = _userObj.AuthMode,
                    Locked = false
                };

                if (_userObj.IdentityCard != null && _userObj.IdentityCard.IsActive)
                {
                    request.Cards = new long[] { Convert.ToInt64(_userObj.IdentityCard.Number) };
                }

                Logger.Log($"Sending user to device: {code} started successfully.");

                _palizServer._serverManager.AddUserEvent += AddUserEventCallBack;
                _palizServer._serverManager.AddUserAsyncTask(terminalName, request);

                System.Threading.Thread.Sleep(500);
                while (!userSendingFinished)
                {
                    System.Threading.Thread.Sleep(500);
                }

                _palizServer._serverManager.AddUserEvent -= AddUserEventCallBack;

                //Logger.Log(GetDescription());
                Logger.Log($"UserId {_userId} successfully added to DeviceId {code}");

                PalizServer.SendUserFinished = true;
                return PalizServer.SendUserFinished
                    ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = terminalId, Message = 0.ToString(), Validate = 1 }
                    : new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = terminalId, Message = 0.ToString(), Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = terminalId, Message = "Error in command execute", Validate = 0 };
            }
        }

        private void AddUserEventCallBack(object sender, UserActionEventArgs args)
        {

            userSendingFinished = true;
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
            return $"Adding user: {_userId} to device: {terminalId}.";
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
