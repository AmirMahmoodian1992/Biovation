using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.EOS.Devices;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Eos.Commands
{
    public class EosRetrieveUserFromDevice : ICommand
    {


        private readonly Dictionary<uint, Device> _onlineDevices;



        private uint DeviceId { get; }

        private uint UserId { get; }
        //private uint Code { get; }

        private readonly DeviceService _deviceService;
        private readonly UserService _userService;
        private readonly UserCardService _userCardService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly AccessGroupService _commonAccessGroupService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly TaskStatuses _taskStatuses;

        public EosRetrieveUserFromDevice(uint deviceId, uint userId, DeviceService deviceService, Dictionary<uint, Device> onlineDevices, UserService userService, UserCardService userCardService, FingerTemplateService fingerTemplateService, FaceTemplateService faceTemplateService, AccessGroupService commonAccessGroupService, FingerTemplateTypes fingerTemplateTypes,FaceTemplateTypes faceTemplateTypes,TaskStatuses taskStatuses)
        {
            DeviceId = deviceId;
            _onlineDevices = onlineDevices;
            _userService = userService;
            _userCardService = userCardService;
            _fingerTemplateService = fingerTemplateService;
            _faceTemplateService = faceTemplateService;
            _commonAccessGroupService = commonAccessGroupService;
            _fingerTemplateTypes = fingerTemplateTypes;

            _faceTemplateTypes = faceTemplateTypes;
            UserId = userId;
            _deviceService = deviceService;
            _taskStatuses = taskStatuses;
            //Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId((int)DeviceId, DeviceBrands.EosCode)?.Code ?? 0;

        }

        public object Execute()
        {
            var device = _deviceService.GetDevice(DeviceId);

            if (!_onlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };

      
            var userOfDevice = _onlineDevices[device.Code].GetUser(UserId);
            if (!(userOfDevice is null))
            {
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
                        //todo
                        Code = userOfDevice.Id,
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




                    var existUser = _userService.GetUsers(code: userOfDevice.Code).FirstOrDefault();

                    if (existUser != null)
                    {
                        user = new User
                        {
                            Id = 0,
                            //Code = userOfDevice.Id,
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
                    }

                    _userService.ModifyUser(user);
                    var savedUser= _userService.GetUsers(code:userOfDevice.Code).FirstOrDefault();
                    if(savedUser != null)
                    {
                        user.Id = savedUser.Id;


                        if (user.FingerTemplates != null && user.FingerTemplates.Count > 0)
                        {
                            Task.Run(() =>
                            {
                                try
                                {
                                    //todo
                                    //  var accessGroupsOfUser = _commonAccessGroupService.GetAccessGroupsOfUser(user.Id, 4);
                                    var accessGroupsOfUser = _commonAccessGroupService.GetAccessGroups(user.Id, userGroupId: 4);

                                    if (accessGroupsOfUser is null || accessGroupsOfUser.Count == 0)
                                    {
                                        var onlineDevices =
                                            _deviceService.GetDevices(brandId: Convert.ToInt32(DeviceBrands.EosCode).ToString());

                                        foreach (var device in onlineDevices)
                                        {
                                            //AddUserToDeviceFastSearch(device.Code, (int)user.Id);
                                            var restRequest = new RestRequest($"Eos/EosDevice/SendUserToDevice");
                                            restRequest.AddQueryParameter("deviceId", device.DeviceId.ToString());
                                            //restRequest.AddQueryParameter("userId", user.Code.ToString());
                                            restRequest.AddQueryParameter("userId", user.Id.ToString());
                                        }
                                    }

                                    else
                                    {
                                        foreach (var accessGroup in accessGroupsOfUser)
                                        {
                                            foreach (var deviceGroup in accessGroup.DeviceGroup)
                                            {
                                                foreach (var device in deviceGroup.Devices)
                                                {
                                                    //AddUserToDeviceFastSearch(device.Code, (int)user.Id);
                                                    var restRequest =
                                                        new RestRequest($"Eos/EosDevice/SendUserToDevice");
                                                    restRequest.AddQueryParameter("deviceId", device.DeviceId.ToString());
                                                    restRequest.AddQueryParameter("userId", user.Id.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception);
                                    Logger.Log(exception);
                                }
                            });
                        }

                    }




                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return new ResultViewModel<User> { Data = userOfDevice, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
            }
            return new ResultViewModel<User> { Id = DeviceId, Message = "0", Validate = 0, Code = Convert.ToInt64(TaskStatuses.DoneCode) };

        }

        public string GetDescription()
        {
            return "Get user" + UserId + " of device : " + DeviceId + " command";
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