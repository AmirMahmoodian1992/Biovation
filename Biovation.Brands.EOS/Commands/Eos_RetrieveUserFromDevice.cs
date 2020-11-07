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
        private readonly Dictionary<uint, Device> _onlineDevices;

        //private readonly ILogger<EosRetrieveUserFromDevice> _logger;
        //private readonly UserCardService _userCardService;
        //private readonly FingerTemplateService _fingerTemplateService;
        //private readonly FaceTemplateService _faceTemplateService;
        //private readonly AccessGroupService _commonAccessGroupService;
        //private readonly FingerTemplateTypes _fingerTemplateTypes;
        //private readonly FaceTemplateTypes _faceTemplateTypes;
        //private readonly TaskStatuses _taskStatuses;

        public EosRetrieveUserFromDevice(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, DeviceService deviceService, UserService userService)
        {
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
            var readLogOfPeriod = _onlineDevices[device.Code]
                .ReadLogOfPeriod(new DateTime(2000), new DateTime(2020, 12, 20));

            if (userOfDevice is null)
                return new ResultViewModel<User>
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
                    StartDate = userOfDevice.StartDate.ToString(CultureInfo.InvariantCulture) == "0000-00-00"
                        ? DateTime.Parse("1970/01/01")
                        : DateTime.Parse(userOfDevice.StartDate.ToString(CultureInfo.InvariantCulture)),
                    EndDate = userOfDevice.EndDate.ToString(CultureInfo.InvariantCulture) == "0000-00-00"
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
                //var savedUser= _userService.GetUsers(code:userOfDevice.Code).FirstOrDefault();
                //if(savedUser != null)
                //{
                //    user.Id = savedUser.Id;


                //    if (user.FingerTemplates != null && user.FingerTemplates.Count > 0)
                //    {
                //        Task.Run(() =>
                //        {
                //            try
                //            {
                //                //todo
                //                //  var accessGroupsOfUser = _commonAccessGroupService.GetAccessGroupsOfUser(user.Id, 4);
                //                var accessGroupsOfUser = _commonAccessGroupService.GetAccessGroups(user.Id, userGroupId: 4);

                //                if (accessGroupsOfUser is null || accessGroupsOfUser.Count == 0)
                //                {
                //                    var onlineDevices =
                //                        _deviceService.GetDevices(brandId: Convert.ToInt32(DeviceBrands.EosCode).ToString());

                //                    foreach (var device in onlineDevices)
                //                    {
                //                        //AddUserToDeviceFastSearch(device.Code, (int)user.Id);
                //                        var restRequest = new RestRequest($"Eos/EosDevice/SendUserToDevice");
                //                        restRequest.AddQueryParameter("deviceId", device.DeviceId.ToString());
                //                        //restRequest.AddQueryParameter("userId", user.Code.ToString());
                //                        restRequest.AddQueryParameter("userId", user.Id.ToString());
                //                    }
                //                }

                //                else
                //                {
                //                    foreach (var accessGroup in accessGroupsOfUser)
                //                    {
                //                        foreach (var deviceGroup in accessGroup.DeviceGroup)
                //                        {
                //                            foreach (var device in deviceGroup.Devices)
                //                            {
                //                                //AddUserToDeviceFastSearch(device.Code, (int)user.Id);
                //                                var restRequest =
                //                                    new RestRequest($"Eos/EosDevice/SendUserToDevice");
                //                                restRequest.AddQueryParameter("deviceId", device.DeviceId.ToString());
                //                                restRequest.AddQueryParameter("userId", user.Id.ToString());
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //            catch (Exception exception)
                //            {
                //                //Logger.Log(exception);
                //                _logger.LogWarning(exception, "Error on sending user {UserId} to other devices", UserId);
                //            }
                //        });
                //    }
                //}
            }
            catch (Exception exception)
            {
                //_logger.LogWarning(exception, "Error on retrieving user {UserId} from device {DeviceId}", userId, deviceId);
                Logger.Log(exception, $"Error on retrieving user {userCode} from device {deviceId}");
            }

            return new ResultViewModel<User> { Data = userOfDevice, Id = deviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };

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