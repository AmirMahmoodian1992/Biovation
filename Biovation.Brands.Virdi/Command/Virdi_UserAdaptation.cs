using Biovation.Brands.Virdi.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiUserAdaptation : ICommand
    {
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private readonly UCSAPI _ucsApi;
        private readonly ITerminalUserData _terminalUserData;

        private readonly TaskTypes _taskTypes;
        private readonly RestClient _restClient;
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly TaskStatuses _taskStatuses;
        private readonly DeviceService _deviceService;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;

        private Dictionary<uint, uint> EquivalentCodes { get; set; }
        private List<uint> UserCodesToChange { get; set; }
        private DeviceBasicInfo DeviceInfo { get; set; }
        private User CreatorUser { get; set; }
        private string Token { get; set; }
        private TaskItem TaskItem { get; }

        public VirdiUserAdaptation(IReadOnlyList<object> items, Dictionary<uint, DeviceBasicInfo> devices, DeviceService deviceService, TaskTypes taskTypes, TaskService taskService,
            TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, UserService userService, RestClient restClient, FaceTemplateTypes faceTemplateTypes, FingerTemplateTypes fingerTemplateTypes, BiometricTemplateManager biometricTemplateManager, UCSAPI ucsApi, ITerminalUserData terminalUserData)
        {
            var taskItem = (TaskItem)items[0];
            TaskItem = taskItem;

            OnlineDevices = devices;
            _taskTypes = taskTypes;
            _restClient = restClient;
            _faceTemplateTypes = faceTemplateTypes;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _ucsApi = ucsApi;
            _terminalUserData = terminalUserData;
            _taskService = taskService;
            _userService = userService;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _deviceService = deviceService;
            _taskPriorities = taskPriorities;

            //_terminalUserData = ucsApi.TerminalUserData as ITerminalUserData;
        }

        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            DeviceInfo = _deviceService.GetDevice(deviceId);
            if (DeviceInfo is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!OnlineDevices.ContainsKey(DeviceInfo.Code))
            {
                Logger.Log($"The device: {DeviceInfo.Code} is not connected.");
                return new ResultViewModel
                {
                    Id = TaskItem.Id,
                    Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode),
                    Message =
                        $"  Enroll User face from device: {DeviceInfo.Code} failed. The device is disconnected.{Environment.NewLine}",
                    Validate = 0
                };
            }

            var data = (JObject)JsonConvert.DeserializeObject(TaskItem.Data);
            try
            {
                if (data != null)
                {
                    Token = Convert.ToString(data["token"]);
                    EquivalentCodes =
                        JsonConvert.DeserializeObject<Dictionary<uint, uint>>(Convert.ToString(data["serializedEquivalentCodes"]) ?? string.Empty);
                    CreatorUser = _userService.GetUsers(Convert.ToUInt32(data["creatorUserId"])).FirstOrDefault();
                }
                else
                    return new ResultViewModel { Success = false, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }
            catch (Exception e)
            {
                Logger.Log($"The Data of device {DeviceInfo.Code} is not valid.");
                Logger.Log(e, logType: LogType.Error);
                return new ResultViewModel { Success = false, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }

            var restRequest = new RestRequest($"{DeviceInfo.Brand.Name}/{DeviceInfo.Brand.Name}Device/RetrieveUsersListFromDevice", Method.GET);
            restRequest.AddQueryParameter("code", DeviceInfo.Code.ToString());
            restRequest.ReadWriteTimeout = 3600000;
            restRequest.Timeout = 3600000;
            restRequest.AddHeader("Authorization", Token ?? string.Empty);

            var userList = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest).Result.Data?.Data;
            if (userList is null)
                return new ResultViewModel { Success = false, Message = "The device is offline" };

            _ucsApi.EventGetUserData += GetUserDataCallback;
            UserCodesToChange = EquivalentCodes.Keys.Where(userCode => userList.Any(user => user.Code == userCode)).ToList();
            lock (UserCodesToChange)
            {
                foreach (var userCode in UserCodesToChange)
                {
                    try
                    {
                        _terminalUserData.GetUserDataFromTerminal(TaskItem.Id, (int)DeviceInfo.Code, (int)userCode);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                        return new ResultViewModel { Validate = 0, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
                    }
                }
            }

            //wait


            return new ResultViewModel { Validate = 0, Id = deviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
        }

        private void GetUserDataCallback(int clientId, int terminalId)
        {
            if (clientId != TaskItem.Id)
                return;

            try
            {
                lock (_ucsApi)
                {
                    lock (_terminalUserData)
                    {
                        var isoEncoding = Encoding.GetEncoding(28591);
                        var windowsEncoding = Encoding.GetEncoding(1256);

                        var deviceUserName = _terminalUserData.UserName;
                        var replacements = new Dictionary<string, string> { { "˜", "\u0098" }, { "Ž", "\u008e" } };
                        var userName = replacements.Aggregate(deviceUserName, (current, replacement) => current.Replace(replacement.Key, replacement.Value));

                        userName = string.IsNullOrEmpty(userName) ? null : windowsEncoding.GetString(isoEncoding.GetBytes(userName)).Trim();

                        var indexOfSpace = userName?.IndexOf(' ') ?? 0;
                        var firstName = indexOfSpace > 0 ? userName?.Substring(0, indexOfSpace).Trim() : null;
                        var surName = indexOfSpace > 0 ? userName?.Substring(indexOfSpace, userName.Length - indexOfSpace).Trim() : userName;

                        byte[] picture = null;

                        try
                        {
                            if (_terminalUserData.PictureDataLength > 0)
                                picture = _terminalUserData.PictureData as byte[];
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                        }

                        var user = new User
                        {
                            Code = _terminalUserData.UserID,
                            AdminLevel = _terminalUserData.IsAdmin,
                            StartDate = _terminalUserData.StartAccessDate == "0000-00-00"
                                   ? DateTime.Parse("1970/01/01")
                                   : DateTime.Parse(_terminalUserData.StartAccessDate),
                            EndDate = _terminalUserData.EndAccessDate == "0000-00-00"
                                   ? DateTime.Parse("2050/01/01")
                                   : DateTime.Parse(_terminalUserData.EndAccessDate),
                            AuthMode = _terminalUserData.AuthType,
                            Password = _terminalUserData.Password,
                            UserName = userName,
                            FirstName = firstName,
                            SurName = surName,
                            IsActive = true,
                            ImageBytes = picture
                        };

                        Logger.Log("<--User is Modified");

                        //Card
                        try
                        {
                            Logger.Log($"   +TotalCardCount:{_terminalUserData.CardNumber}");
                            if (_terminalUserData.CardNumber > 0)
                                for (var i = 0; i < _terminalUserData.CardNumber; i++)
                                {
                                    user.IdentityCard = new IdentityCard { Number = _terminalUserData.RFID[i], IsActive = true };
                                }
                        }
                        catch (Exception e)
                        {
                            Logger.Log(e);
                        }

                        //Finger
                        try
                        {
                            var nFpDataCount = _terminalUserData.TotalFingerCount;
                            Logger.Log($"   +TotalFingerCount:{nFpDataCount}");

                            if (user.FingerTemplates is null)
                                user.FingerTemplates = new List<FingerTemplate>();

                            for (var i = 0; i < nFpDataCount; i++)
                            {
                                var fingerIndex = _terminalUserData.FingerID[i];

                                var firstSampleCheckSum = 0;
                                var secondSampleCheckSum = 0;

                                var firstTemplateSample = _terminalUserData.FPSampleData[fingerIndex, 0] as byte[];
                                byte[] secondTemplateSample = null;
                                try
                                {
                                    secondTemplateSample = _terminalUserData.FPSampleData[fingerIndex, 1] as byte[];
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception);
                                }

                                if (firstTemplateSample != null) firstSampleCheckSum = firstTemplateSample.Sum(x => x);
                                if (secondTemplateSample != null) secondSampleCheckSum = secondTemplateSample.Sum(x => x);

                                user.FingerTemplates.Add(new FingerTemplate
                                {
                                    FingerIndex = _biometricTemplateManager.GetFingerIndex(_terminalUserData.FingerID[i]),
                                    Index = fingerIndex,
                                    TemplateIndex = 0,
                                    Size = _terminalUserData.FPSampleDataLength[fingerIndex, 0],
                                    Template = firstTemplateSample,
                                    CheckSum = firstSampleCheckSum,
                                    UserId = user.Id,
                                    FingerTemplateType = _fingerTemplateTypes.V400
                                });

                                if (secondTemplateSample != null)
                                {
                                    user.FingerTemplates.Add(new FingerTemplate
                                    {
                                        FingerIndex = _biometricTemplateManager.GetFingerIndex(_terminalUserData.FingerID[i]),
                                        Index = fingerIndex,
                                        TemplateIndex = 1,
                                        Size = _terminalUserData.FPSampleDataLength[fingerIndex, 1],
                                        Template = secondTemplateSample,
                                        CheckSum = secondSampleCheckSum,
                                        UserId = user.Id,
                                        FingerTemplateType = _fingerTemplateTypes.V400
                                    });
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Log(e);
                        }

                        //Face
                        try
                        {
                            var faceCount = _terminalUserData.FaceNumber;
                            Logger.Log($"   +TotalFaceCount:{faceCount}");

                            if (faceCount > 0)
                            {
                                if (user.FaceTemplates is null)
                                    user.FaceTemplates = new List<FaceTemplate>();

                                var faceData = (byte[])_terminalUserData.FaceData;
                                var faceTemplate = new FaceTemplate
                                {
                                    Index = faceCount,
                                    FaceTemplateType = _faceTemplateTypes.VFACE,
                                    UserId = user.Id,
                                    Template = faceData,
                                    CheckSum = faceData.Sum(x => x),
                                    Size = faceData.Length
                                };

                                user.FaceTemplates.Add(faceTemplate);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Log(e);
                        }
                        //}


                        Logger.Log($@"<--EventGetUserData
    +ClientID:{clientId}
    +TerminalID:{terminalId}
    +ErrorCode:{_ucsApi.ErrorCode}
    +UserID:{_terminalUserData.UserID}
    +Admin:{_terminalUserData.IsAdmin}
    +Admin:{_terminalUserData.FaceNumber}
    +AccessGroup:{_terminalUserData.AccessGroup}
    +AccessDateType:{_terminalUserData.AccessDateType}
    +AccessDate:{_terminalUserData.StartAccessDate}~{_terminalUserData.EndAccessDate}
    +AuthType:{_terminalUserData.AuthType}
    +Password:{_terminalUserData.Password}
    +Progress:{_terminalUserData.CurrentIndex}/{_terminalUserData.TotalNumber}", logType: LogType.Information);


                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = CreatorUser,
                            TaskType = _taskTypes.DeleteUsers,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = DeviceInfo.Brand,
                            TaskItems = new List<TaskItem>(),
                            DueDate = DateTime.Today
                        };
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                            Priority = _taskPriorities.Medium,
                            DeviceId = DeviceInfo.DeviceId,
                            Data = JsonConvert.SerializeObject(new { userCode = user.Code }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                            CurrentIndex = 0,
                            TotalCount = 1
                        });

                        _taskService.InsertTask(task);


                        task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = CreatorUser,
                            TaskType = _taskTypes.SendUsers,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = DeviceInfo.Brand,
                            TaskItems = new List<TaskItem>(),
                            DueDate = DateTime.Today
                        };

                        //var correctedUser = userList.First(x => x.Code == userCode);

                        user.Code = EquivalentCodes[(uint)user.Code];

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.SendUser,
                            Priority = _taskPriorities.Medium,
                            DeviceId = DeviceInfo.DeviceId,
                            Data = JsonConvert.SerializeObject(user),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                            CurrentIndex = 0,
                            TotalCount = 1
                        });

                        _taskService.InsertTask(task);
                        lock (UserCodesToChange)
                        {
                            UserCodesToChange.Remove((uint)user.Code);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"--> Error On GetUserDataCallback Error: {e.Message} ");
            }

            if (UserCodesToChange.Count == 0)
            {
                _ucsApi.EventGetUserData -= GetUserDataCallback;

                try
                {
                    var restRequest = new RestRequest($"{DeviceInfo.Brand.Name}/{DeviceInfo.Brand.Name}Task/RunProcessQueue", Method.POST);
                    _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Upgrade device firmware";
        }

        public string GetDescription()
        {
            return "Upgrading firmware of device .....";
        }
    }
}
