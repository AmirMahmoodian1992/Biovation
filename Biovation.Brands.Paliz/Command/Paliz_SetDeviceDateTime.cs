using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Helpers;
using PalizTiara.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizSetDeviceDateTime : ICommand
    {
        private Dictionary<uint, DeviceBasicInfo> _onlineDevices { get; }
        private readonly TaskService _taskService;
        private readonly string _terminalName;
        private readonly int _terminalId;
        private readonly uint _code;
        private readonly int _taskItemId;
        private readonly PalizServer _palizServer;
        private DateTime _inputDateTime;
        private bool _datetimeSyncingFinished;
        private AutoResetEvent waitHandle = new AutoResetEvent(false);

        public PalizSetDeviceDateTime(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService
                , DeviceService deviceService)
        {
            _terminalId = Convert.ToInt32(items[0]);
            _taskItemId = Convert.ToInt32(items[1]);
            //var taskItem = taskService.GetTaskItem(TaskItemId)?.GetAwaiter().GetResult().Data ?? new TaskItem();
            //var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            //if (data != null)
            //{
            //    UserId = (int)data["userId"];
            //}

            _palizServer = palizServer;
            _taskService = taskService;

            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode).GetAwaiter().GetResult();
            if (devices is null)
            {
                _onlineDevices = new Dictionary<uint, DeviceBasicInfo>();
                return;
            }

            _code = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == _terminalId)?.Code ?? 0;
            _terminalName = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == _terminalId)?.Name ?? string.Empty;
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
            if (_onlineDevices.All(device => device.Key != _code))
            {
                Logger.Log($"SetDeviceDateTime,The device: {_code} is not connected.");
                return new ResultViewModel
                {
                    Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode)
                    ,
                    Id = _terminalId,
                    Message = $"The device: {_code} is not connected.",
                    Validate = 1
                };
            }

            try
            {
                var taskItem = _taskService.GetTaskItem(_taskItemId).GetAwaiter().GetResult().Data;
                var taskData = JsonConvert.DeserializeObject<JObject>(taskItem.Data);
                if (taskData != null && taskData.ContainsKey("DateTime"))
                {
                    var parseResult = DateTime.TryParse(taskData["DateTime"].ToString() ?? DateTime.Now.ToString(), out var dateTime);
                    if (parseResult)
                    {
                        _inputDateTime = (DateTime)taskData["DateTime"];
                    }
                    else
                    {
                        _inputDateTime = DateTime.Now;
                    }

                    //if (!parseResult || dateTime == 0)
                    //    return new ResultViewModel
                    //    {
                    //        Id = taskItem.Id,
                    //        Code = Convert.ToInt64(TaskStatuses.FailedCode),
                    //        Message =
                    //            $"Error in processing task item {taskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}",
                    //        Validate = 0
                    //    };

                    //_userId = userId;
                    //_userObj = _userService.GetUsers(userId: _userId, getTemplatesData: true)?.GetAwaiter().GetResult()?.Data?.Data?.FirstOrDefault();

                    //if (_userObj == null)
                    //{
                    //    Logger.Log($"User {_userId} does not exist.");
                    //    return new ResultViewModel { Validate = 0, Id = taskItem.Id, Message = $"User {_userId} does not exist.", Code = Convert.ToInt64(TaskStatuses.FailedCode) };
                    //}
                }
                else
                {
                    _inputDateTime = DateTime.Now;
                }

                //int hour = int.Parse(this.DateHourTextBox.Text);
                //int min = int.Parse(this.DateMinuteTextBox.Text);
                //int sec = int.Parse(this.DateSecondTextBox.Text);
                //date = date?.AddHours(hour);
                //date = date?.AddMinutes(min);
                //date = date?.AddSeconds(sec);

                var epochTime = StaticHelpers.GetEpochTime(_inputDateTime);
                var request = new DateModel(epochTime);

                Logger.Log($"Setting new date and time: {_inputDateTime} started successfully.");

                _palizServer._serverManager.SetDateTimeEvent += SetDatetimeEventCallBack;
                _palizServer._serverManager.SetDateTimeAsyncTask(request, _terminalName);

                waitHandle.WaitOne();

                //while (!_datetimeSyncingFinished)
                //{
                //    Thread.Sleep(50);
                //}

                _palizServer._serverManager.SetDateTimeEvent -= SetDatetimeEventCallBack;

                //Logger.Log(GetDescription());
                Logger.Log($"Date and time successfully synced, DeviceId {_code}");

                //PalizServer.SendUserFinished = true;
                return _datetimeSyncingFinished
                    ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = _terminalId, Message = 0.ToString(), Validate = 1 }
                    : new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = _terminalId, Message = 0.ToString(), Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = _terminalId, Message = "Error in command execute", Validate = 0 };
            }
        }

        private void SetDatetimeEventCallBack(object sender, SetActionEventArgs args)
        {
            _datetimeSyncingFinished = true;

            waitHandle.Set();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Set device date and time";
        }

        public string GetDescription()
        {
            return $"Setting new date and time: {_inputDateTime} on device: {_terminalId}.";
        }

        //public static string Decrypt(string cipherText)
        //{
        //    var EncryptionKey = "Kasra";
        //    cipherText = cipherText.Replace(" ", "+");
        //    var cipherBytes = Convert.FromBase64String(cipherText);
        //    using (var encryptor = Aes.Create())
        //    {
        //        var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //        if (encryptor != null)
        //        {
        //            encryptor.Key = pdb.GetBytes(32);
        //            encryptor.IV = pdb.GetBytes(16);
        //            using var ms = new MemoryStream();
        //            using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(),
        //                CryptoStreamMode.Write))
        //            {
        //                cs.Write(cipherBytes, 0, cipherBytes.Length);
        //                cs.Close();
        //            }

        //            cipherText = Encoding.Unicode.GetString(ms.ToArray());
        //        }
        //    }
        //    return cipherText;
        //}
    }
}

