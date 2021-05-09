using Biovation.Brands.Suprema.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.Suprema.Devices;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Brands.Suprema.Commands
{
    public class SupremaRetrieveUserFromDevice : ICommand
    {


        private readonly Dictionary<uint, Device> _onlineDevices;



        private uint DeviceId { get; set; }

        private uint UserCode { get; set; }

        private readonly DeviceService _deviceService;
        private readonly UserService _userService;
        private readonly UserCardService _userCardService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly AccessGroupService _commonAccessGroupService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private TaskItem TaskItem { get; }

        public SupremaRetrieveUserFromDevice(TaskItem taskItem, DeviceService deviceService, Dictionary<uint, Device> onlineDevices, UserService userService, UserCardService userCardService, FingerTemplateService fingerTemplateService, FaceTemplateService faceTemplateService, AccessGroupService commonAccessGroupService, FingerTemplateTypes fingerTemplateTypes, BiometricTemplateManager biometricTemplateManager, FaceTemplateTypes faceTemplateTypes)
        {
            _onlineDevices = onlineDevices;
            _userService = userService;
            _userCardService = userCardService;
            _fingerTemplateService = fingerTemplateService;
            _faceTemplateService = faceTemplateService;
            _commonAccessGroupService = commonAccessGroupService;
            _fingerTemplateTypes = fingerTemplateTypes;

            _biometricTemplateManager = biometricTemplateManager;
            _faceTemplateTypes = faceTemplateTypes;
            TaskItem = taskItem;
            _deviceService = deviceService;
            //Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId((int)DeviceId, DeviceBrands.SupremaCode)?.Code ?? 0;
        }




        public object Execute()
        {

            if (TaskItem is null)
                return new ResultViewModel { Id = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item.{Environment.NewLine}", Validate = 0 };

            DeviceId = (uint) TaskItem.DeviceId;
            var parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["userId"]?.ToString() ?? "0", out var outUserCode);
            UserCode = outUserCode;
            if (!parseResult || UserCode == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}", Validate = 0 };

            var device = _deviceService.GetDevice(DeviceId).Result?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!_onlineDevices.ContainsKey(device.Code))
            {
                Logger.Log($"The device: {device.DeviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }
            var userOfDevice = _onlineDevices[DeviceId].GetUser(UserCode);
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




                    var existUser = _userService.GetUsers( Convert.ToInt32(userOfDevice.Id))?.Data?.Data.FirstOrDefault();

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
                    user.Id = _userService.GetUsers(userId:userOfDevice.Id).Data.Data.FirstOrDefault().Id;

                    //Card
                    try
                    {
                        Logger.Log($"   +TotalCardCount:{userOfDevice.IdentityCard}");
                        if (!(userOfDevice.IdentityCard is null))
                        {
                            //card ro nmitoonim chandtayii dar nazar begirim???????????
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
                    catch (Exception e)
                    {
                        Logger.Log(e);
                    }

                    //Finger
                    try
                    {
                        var nFpDataCount = userOfDevice.FingerTemplates.Count;
                        Logger.Log($"   +TotalFingerCount:{nFpDataCount}");

                        if (user.FingerTemplates is null)
                            user.FingerTemplates = new List<FingerTemplate>();


                        for (var i = 0; i < nFpDataCount; i += 2)
                        {
                            //var fingerIndex = userOfDevice.FingerTemplates[i].FingerIndex;
                            if (existUser != null)
                            {

                                // if (existUser.FingerTemplates.Exists(fp =>
                                //fp.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(fingerIndex).Code && fp.FingerTemplateType == _fingerTemplateTypes.V400))


                                var firstTemplateSampleChecksum = userOfDevice.FingerTemplates[i].CheckSum;
                                int secondTemplateSampleChecksum = 0;
                                try
                                {
                                    //secondTemplateSampleChecksum = user.FingerTemplates.Where(x => x.FingerIndex == fingerIndex && x.Index == 2).Select(x => x.CheckSum);
                                    secondTemplateSampleChecksum = userOfDevice.FingerTemplates[i + 1].CheckSum;
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception);
                                }

                                //var fingerTemplates = _fingerTemplateService.GetFingerTemplateByUserId(existUser.Id).Where(ft => ft.FingerTemplateType.Code == tempFingerTemplateTypes.Code).ToList();
                                //var fingerTemplates = _fingerTemplateService.GetFingerTemplateByUserId(existUser.Id).ToList();
                                var fingerTemplates = existUser.FingerTemplates
                                    .Where(ft => ft.FingerTemplateType.Code == _fingerTemplateTypes.SU384.Code).ToList();


                                if (fingerTemplates.Exists(ft => ft.CheckSum == firstTemplateSampleChecksum) &&
                                    fingerTemplates.Exists(ft => ft.CheckSum == (secondTemplateSampleChecksum)))
                                    continue;

                                //for (var j = 0; j < fingerTemplates.Count; j += 2)
                                //{
                                //    if (firstTemplateSampleChecksum != fingerTemplates[j].CheckSum || secondTemplateSampleChecksum != fingerTemplates[j+1].CheckSum) continue;
                                //    sameTemplateExists = true;
                                //    break;
                                //}

                                //if (sameTemplateExists) continue;

                                user.FingerTemplates.Add(new FingerTemplate
                                {
                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                                    Index = _fingerTemplateService.FingerTemplates((int) existUser.Id)?.Data.Data.Count(ft =>
                                        ft.FingerIndex.Code ==
                                        userOfDevice.FingerTemplates[i].FingerIndex.Code) ?? 0 + 1,
                                    TemplateIndex = 0,
                                    Size = userOfDevice.FingerTemplates[i].Size,
                                    Template = userOfDevice.FingerTemplates[i].Template,
                                    CheckSum = firstTemplateSampleChecksum,
                                    UserId = user.Id,
                                    FingerTemplateType = _fingerTemplateTypes.SU384
                                });


                                user.FingerTemplates.Add(new FingerTemplate
                                {
                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                                    Index = _fingerTemplateService.FingerTemplates((int) existUser.Id)?.Data.Data.Count(ft =>
                                        ft.FingerIndex.Code ==
                                        userOfDevice.FingerTemplates[i].FingerIndex.Code) ?? 0 + 1,
                                    TemplateIndex = 1,
                                    Size = userOfDevice.FingerTemplates[i + 1].Size,
                                    Template = userOfDevice.FingerTemplates[i + 1].Template,
                                    CheckSum = secondTemplateSampleChecksum,
                                    UserId = user.Id,
                                    FingerTemplateType = _fingerTemplateTypes.SU384
                                });
                            }
                            else
                            {

                                //var firstTemplateSampleChecksum = userOfDevice.FingerTemplates.Where(x => x.FingerIndex == fingerIndex && x.Index == 1).Select(x => x.CheckSum);
                                var firstTemplateSampleChecksum = userOfDevice.FingerTemplates[i].CheckSum;
                                var secondTemplateSampleChecksum = 0;
                                try
                                {
                                    //secondTemplateSampleChecksum = user.FingerTemplates.Where(x => x.FingerIndex == fingerIndex && x.Index == 2).Select(x => x.CheckSum);
                                    secondTemplateSampleChecksum = userOfDevice.FingerTemplates[i + 1].CheckSum;
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception);
                                }



                                user.FingerTemplates.Add(new FingerTemplate
                                {
                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                                    Index = userOfDevice.FingerTemplates[i].Index,
                                    TemplateIndex = 0,
                                    Size = userOfDevice.FingerTemplates[i].Size,
                                    Template = userOfDevice.FingerTemplates[i].Template,
                                    CheckSum = firstTemplateSampleChecksum,
                                    UserId = user.Id,
                                    FingerTemplateType = _fingerTemplateTypes.SU384
                                });

                                user.FingerTemplates.Add(new FingerTemplate
                                {
                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(0),
                                    Index = userOfDevice.FingerTemplates[i + 1].Index,
                                    TemplateIndex = 1,
                                    Size = userOfDevice.FingerTemplates[i + 1].Size,
                                    Template = userOfDevice.FingerTemplates[i + 1].Template,
                                    CheckSum = secondTemplateSampleChecksum,
                                    UserId = user.Id,
                                    FingerTemplateType = _fingerTemplateTypes.SU384
                                });
                            }
                        }

                        if (user.FingerTemplates.Any())
                            foreach (var fingerTemplate in user.FingerTemplates)
                            {
                                _fingerTemplateService.ModifyFingerTemplate(fingerTemplate);
                            }
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e);
                    }

                    //Face
                    try
                    {
                        if (!(userOfDevice.FaceTemplates is null))
                        {

                            var faceCount = userOfDevice.FaceTemplates.Count;
                            Logger.Log($"   +TotalFaceCount:{faceCount}");

                            if (faceCount > 0)
                            {
                                if (user.FaceTemplates is null)
                                    user.FaceTemplates = new List<FaceTemplate>();

                                var userFaces = _faceTemplateService.FaceTemplates(userId:user.Id);
                                //existUser.FaceTemplates = new List<FaceTemplate>();

                                if (existUser != null)
                                    existUser.FaceTemplates = (userFaces.Any() ? userFaces : new List<FaceTemplate>());

                                var faceTemplate = new FaceTemplate
                                {
                                    Index = faceCount,
                                    FaceTemplateType = _faceTemplateTypes.VFACE,
                                    UserId = user.Id,
                                    Template = userOfDevice.FaceTemplates[0].Template,
                                    CheckSum = userOfDevice.FaceTemplates[0].CheckSum,
                                    Size = userOfDevice.FaceTemplates[0].Size
                                };
                                if (existUser != null)
                                {
                                    if (!existUser.FaceTemplates.Exists(fp =>
                                        fp.FaceTemplateType.Code == _faceTemplateTypes.VFACE.Code))
                                        user.FaceTemplates.Add(faceTemplate);
                                }
                                else
                                    user.FaceTemplates.Add(faceTemplate);

                                if (user.FaceTemplates.Any())
                                    foreach (var faceTemplates in user.FaceTemplates)
                                    {
                                        _faceTemplateService.ModifyFaceTemplate(faceTemplates);
                                    }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e);
                    }



                    if (user.FingerTemplates != null && user.FingerTemplates.Count > 0)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                //todo
                                //  var accessGroupsOfUser = _commonAccessGroupService.GetAccessGroupsOfUser(user.Id, 4);
                                var accessGroupsOfUser = _commonAccessGroupService.GetAccessGroups(user.Id,userGroupId: 4)?.Data.Data;

                                if (accessGroupsOfUser is null || accessGroupsOfUser.Count == 0)
                                {
                                    var onlineDevices =
                                        _deviceService.GetDevices(brandId:Convert.ToInt32(DeviceBrands.SupremaCode).ToString()).Result?.Data.Data;

                                    if (onlineDevices == null) return;
                                    foreach (var deviceBasicInfo in onlineDevices)
                                    {
                                        //AddUserToDeviceFastSearch(device.Code, (int)user.Id);
                                        var restRequest =
                                            new RestRequest($"Suprema/SupremaDevice/SendUserToDevice");
                                        restRequest.AddQueryParameter("deviceId",
                                            deviceBasicInfo.DeviceId.ToString());
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
                                            foreach (var deviceBasicInfo in deviceGroup.Devices)
                                            {
                                                //AddUserToDeviceFastSearch(device.Code, (int)user.Id);
                                                var restRequest =
                                                    new RestRequest($"Suprema/SupremaDevice/SendUserToDevice");
                                                restRequest.AddQueryParameter("deviceId", deviceBasicInfo.DeviceId.ToString());
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
            return "Get user" + UserCode + " of device : " + DeviceId + " command";
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