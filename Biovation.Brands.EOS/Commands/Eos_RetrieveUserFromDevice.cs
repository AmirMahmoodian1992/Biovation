using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Biovation.Brands.Eos.Commands
{
    public class EosRetrieveUserFromDevice : ICommand
    {
        //private uint DeviceId { get; }
        //private uint UserId { get; }
        //private uint Code { get; }
        //private readonly int _taskItemId;
        private TaskItem TaskItem { get; }

        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        //private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly UserCardService _userCardService;
        private readonly Dictionary<uint, Device> _onlineDevices;

        //private readonly ILogger<EosRetrieveUserFromDevice> _logger;
        //private readonly UserCardService _userCardService;
        private readonly FingerTemplateService _fingerTemplateService;
        //private readonly FaceTemplateService _faceTemplateService;
        //private readonly AccessGroupService _commonAccessGroupService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        //private readonly TaskStatuses _taskStatuses;

        public EosRetrieveUserFromDevice(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, DeviceService deviceService, UserService userService, UserCardService userCardService, FingerTemplateService fingerTemplateService, FingerTemplateTypes fingerTemplateTypes)
        {
            _fingerTemplateService = fingerTemplateService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _userCardService = userCardService;
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _userService = userService;
            TaskItem = taskItem;
            //_logger = logger;
            //int.TryParse(items[0].ToString(), out _taskItemId);

            //UserId = userId;
            //DeviceId = deviceId;
            //Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId((int)DeviceId, DeviceBrands.EosCode)?.Code ?? 0;
        }

        public object Execute()
        {
            //var _taskItem = _taskItemId == 0 ? null : _taskService.GetTaskItem(_taskItemId)?.Data;

            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            var parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["userCode"].ToString() ?? "0", out var userCode);

            if (!parseResult || userCode == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}", Validate = 0 };

            var device = _deviceService.GetDevice(deviceId)?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!_onlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };


            var userOfDevice = _onlineDevices[device.Code].GetUser(userCode);


            if (userOfDevice is null)
                return new ResultViewModel
                { Id = deviceId, Message = "0", Validate = 0, Code = Convert.ToInt64(TaskStatuses.DoneCode) };

            try
            {
                var userName = string.IsNullOrEmpty(userOfDevice.UserName)
                    ? null
                    : Convert.ToString(userOfDevice.UserName);
                var indexOfSpace = userName?.IndexOf(' ') ?? 0;
                var firstName = indexOfSpace > 0 ? userName?.Substring(0, indexOfSpace) : null;
                var surName = indexOfSpace > 0
                    ? userName?.Substring(indexOfSpace, userName.Length - indexOfSpace)
                    : userName;

                var user = new User
                {
                    Id = 0,
                    Code = userOfDevice.Code,
                    AdminLevel = userOfDevice.AdminLevel,
                    StartDate = userOfDevice.StartDate.ToString(CultureInfo.InvariantCulture) == "0000-00-00" || userOfDevice.StartDate == default
                        ? DateTime.Parse("1970/01/01")
                        : DateTime.Parse(userOfDevice.StartDate.ToString(CultureInfo.InvariantCulture)),
                    EndDate = userOfDevice.EndDate.ToString(CultureInfo.InvariantCulture) == "0000-00-00" || userOfDevice.EndDate == default
                        ? DateTime.Parse("2050/01/01")
                        : DateTime.Parse(userOfDevice.EndDate.ToString(CultureInfo.InvariantCulture)),
                    AuthMode = userOfDevice.AuthMode,
                    Password = userOfDevice.Password,
                    UserName = userName,
                    FirstName = firstName,
                    SurName = surName,
                    IsActive = true
                };

                var existUser = _userService.GetUsers(code: userOfDevice.Code)?.Data?.Data?.FirstOrDefault();

                if (existUser != null)
                {
                    user = new User
                    {
                        Id = existUser.Id,
                        Code = userOfDevice.Code,
                        AdminLevel = userOfDevice.AdminLevel,
                        StartDate = userOfDevice.StartDate.ToString(CultureInfo.InvariantCulture) == "0000-00-00"
                            ? existUser.StartDate
                            : existUser.StartDate == DateTime.Parse("1970/01/01") ? DateTime.Parse(userOfDevice.StartDate.ToString(CultureInfo.InvariantCulture)) : existUser.StartDate,
                        EndDate = userOfDevice.EndDate.ToString(CultureInfo.InvariantCulture) == "0000-00-00"
                            ? existUser.EndDate
                            : existUser.EndDate == DateTime.Parse("2050/01/01") ? DateTime.Parse(userOfDevice.EndDate.ToString(CultureInfo.InvariantCulture)) : existUser.EndDate,
                        AuthMode = userOfDevice.AuthMode,
                        Password = string.IsNullOrWhiteSpace(existUser.Password) ? userOfDevice.Password : existUser.Password,
                        UserName = string.IsNullOrWhiteSpace(userName) ? existUser.UserName : userName,
                        FirstName = string.IsNullOrWhiteSpace(firstName) ? existUser.FirstName : firstName,
                        SurName = string.IsNullOrWhiteSpace(surName) ? existUser.SurName : surName,
                        IsActive = existUser.IsActive
                    };
                }

                _userService.ModifyUser(user);
                user.Id = _userService.GetUsers(code: user.Code)?.Data?.Data.FirstOrDefault()?.Id ?? -1;
                if (user.Id == -1)
                    return new ResultViewModel { Id = deviceId, Message = "Error on adding user to database", Validate = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode) };

                //Card
                try
                {
                    Logger.Log($"   +TotalCardCount:{userOfDevice.IdentityCard}");
                    if (!(userOfDevice.IdentityCard is null))
                    {

                        if (int.Parse(userOfDevice.IdentityCard.Number) > 0)
                            for (var i = 0; i < int.Parse(userOfDevice.IdentityCard.Number); i++)
                            {
                                var card = new UserCard
                                {
                                    CardNum = userOfDevice.IdentityCard.Id.ToString(),
                                    IsActive = true,
                                    //UserId = userOfDevice.Id
                                    UserId = user.Id
                                };
                                _userCardService.ModifyUserCard(card);
                            }
                    }
                }

                catch (Exception exception)
                {
                    Logger.Log(exception);
                }

                //Finger
                try
                {
                    var nFpDataCount = userOfDevice.FingerTemplates.Count;
                    Logger.Log($"   +TotalFingerCount:{nFpDataCount}");

                    if (user.FingerTemplates is null)
                        user.FingerTemplates = new List<FingerTemplate>();
                    user.FingerTemplates = userOfDevice.FingerTemplates;

                    for (var i = 0; i < nFpDataCount; i++)
                    {
                        if (existUser != null && existUser.FingerTemplates.Any(template => template.FingerTemplateType == _fingerTemplateTypes.SU384 && template.CheckSum == user.FingerTemplates[i].CheckSum))
                            continue;
                        
                        user.FingerTemplates[i].UserId = user.Id;
                            _fingerTemplateService.ModifyFingerTemplate(user.FingerTemplates[i]);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }


            return new ResultViewModel { Id = deviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
        }

        public string GetDescription()
        {
            //return "Get user" + UserId + " of device : " + DeviceId + " command";
            return "Get user of a device command";
        }

        public string GetTitle()
        {
            return "Get user of a device command";
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}