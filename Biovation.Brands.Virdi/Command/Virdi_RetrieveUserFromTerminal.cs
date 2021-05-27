using Biovation.Brands.Virdi.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biovation.Service.Api.v2;
using UCBioBSPCOMLib;
using UCSAPICOMLib;
using AccessGroupService = Biovation.Service.Api.v1.AccessGroupService;
using DeviceService = Biovation.Service.Api.v1.DeviceService;
using FaceTemplateService = Biovation.Service.Api.v1.FaceTemplateService;
using FingerTemplateService = Biovation.Service.Api.v1.FingerTemplateService;
using TaskService = Biovation.Service.Api.v1.TaskService;
using UserCardService = Biovation.Service.Api.v1.UserCardService;
using UserService = Biovation.Service.Api.v1.UserService;


namespace Biovation.Brands.Virdi.Command
{
    public class VirdiRetrieveUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private readonly UCSAPI _ucsApi;
        private readonly IFPData _fpData;
        private readonly IMatching _matching;
        private readonly ITerminalUserData _terminalUserData;

        private int TaskItemId { get; }
        private int DeviceId { get; }
        private int UserId { get; }
        private uint Code { get; }

        private readonly VirdiServer _virdiServer;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly UserCardService _userCardService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly IrisTemplateTypes _irisTemplateTypes;
        private readonly AccessGroupService _accessGroupService;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly IrisTemplateService _irisTemplateService;
        private readonly BiometricTemplateManager _biometricTemplateManager;

        public VirdiRetrieveUserFromTerminal(IReadOnlyList<object> items, VirdiServer virdiServer, UCSAPI ucsApi, TaskService taskService, UserService userService, DeviceService deviceService, UserCardService userCardService, FaceTemplateTypes faceTemplateTypes, AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, FingerTemplateTypes fingerTemplateTypes, FingerTemplateService fingerTemplateService, BiometricTemplateManager biometricTemplateManager, IrisTemplateService irisTemplateService, IrisTemplateTypes irisTemplateTypes)
        {
            _virdiServer = virdiServer;
            _ucsApi = ucsApi;
            _userService = userService;
            _deviceService = deviceService;
            _userCardService = userCardService;
            _faceTemplateTypes = faceTemplateTypes;
            _accessGroupService = accessGroupService;
            _faceTemplateService = faceTemplateService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _fingerTemplateService = fingerTemplateService;
            _biometricTemplateManager = biometricTemplateManager;
            _irisTemplateService = irisTemplateService;
            _irisTemplateTypes = irisTemplateTypes;

            var ucBioBsp = new UCBioBSPClass();
            _fpData = ucBioBsp.FPData as IFPData;
            _matching = ucBioBsp.Matching as IMatching;
            _terminalUserData = ucsApi.TerminalUserData as ITerminalUserData;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;

            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data["userId"];
            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        { if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"RetriveUser,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 1 };
            }

            try
            {
                //Callbacks.ModifyUserData = true;
                _ucsApi.EventGetUserData += GetUserDataCallback;
                _virdiServer.TerminalUserData.GetUserDataFromTerminal(TaskItemId, (int)Code, UserId);

                Logger.Log(GetDescription());

                if (_virdiServer.TerminalUserData.ErrorCode == 0)
                {
                    Logger.Log($"  +User {UserId} successfully retrieved from device: {Code}.\n");
                    return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = DeviceId, Message = $"  +User {UserId} successfully retrieved from device: {Code}.\n", Validate = 1 };
                }

                Logger.Log($"  +Cannot retrieve user {Code} from device: {Code}. Error code = {_virdiServer.TerminalUserData.ErrorCode}\n");

                //Callbacks.ModifyUserData = false;

                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot retrieve user {Code} from device: {Code}. Error code = {_virdiServer.TerminalUserData.ErrorCode}\n", Validate = 0 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"Exeption: {exception}", Validate = 0 };

            }
        }


        private void GetUserDataCallback(int clientId, int terminalId)
        {
            if (clientId != TaskItemId)
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
                        //user.Id = _userService.GetUsers(code: TerminalUserData.UserID, withPicture: false)?.FirstOrDefault()?.Id == null
                        //   ? 0
                        //   : _userService.GetUsers(code: TerminalUserData.UserID, withPicture: false).FirstOrDefault().Id;

                        //if (RetrieveUsers.All(retrievedUser => retrievedUser.Id != user.Id))
                        //{
                        //    RetrieveUsers.Add(user);
                        //}

                        //if (ModifyUserData)
                        //{
                        var existUser = _userService.GetUsers(code: _terminalUserData.UserID).FirstOrDefault();
                        if (existUser != null)
                        {
                            user = new User
                            {
                                Id = existUser.Id,
                                Code = existUser.Code,
                                AdminLevel = _terminalUserData.IsAdmin,
                                StartDate = _terminalUserData.StartAccessDate == "0000-00-00"
                                    ? existUser.StartDate
                                    : DateTime.Parse(_terminalUserData.StartAccessDate),
                                EndDate = _terminalUserData.EndAccessDate == "0000-00-00"
                                    ? existUser.EndDate
                                    : DateTime.Parse(_terminalUserData.EndAccessDate),
                                AuthMode = _terminalUserData.AuthType,
                                Password = _terminalUserData.Password,
                                UserName = string.IsNullOrEmpty(userName) ? existUser.UserName : userName,
                                FirstName = firstName ?? existUser.FirstName,
                                SurName = string.Equals(surName, userName) ? existUser.SurName ?? surName : surName,
                                IsActive = existUser.IsActive,
                                ImageBytes = picture
                            };
                        }

                        var userInsertionResult = _userService.ModifyUser(user);

                        Logger.Log("<--User is Modified");
                        user.Id = userInsertionResult.Id;

                        //Card
                        try
                        {
                            Logger.Log($"   +TotalCardCount:{_terminalUserData.CardNumber}");
                            if (_terminalUserData.CardNumber > 0)
                                for (var i = 0; i < _terminalUserData.CardNumber; i++)
                                {
                                    var card = new UserCard
                                    {
                                        CardNum = _terminalUserData.RFID[i],
                                        IsActive = true,
                                        UserId = user.Id
                                    };
                                    _userCardService.ModifyUserCard(card);
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
                                var sameTemplateExists = false;
                                var fingerIndex = _terminalUserData.FingerID[i];
                                if (existUser != null)
                                {
                                    // if (existUser.FingerTemplates.Exists(fp =>
                                    //fp.FingerIndex.Code == BiometricTemplateManager.GetFingerIndex(fingerIndex).Code && fp.FingerTemplateType == FingerTemplateTypes.V400))
                                    lock (_fpData)
                                    {
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

                                        if (_fpData == null) continue;
                                        _fpData.ClearFPData();
                                        _fpData.Import(1, _terminalUserData.CurrentIndex, 2, 400, 400, firstTemplateSample, secondTemplateSample);

                                        var deviceTemplate = _fpData.TextFIR;
                                        var fingerTemplates = _fingerTemplateService.FingerTemplates(userId: (int)(existUser.Id)).Where(ft => ft.FingerTemplateType.Code == FingerTemplateTypes.V400Code).ToList();

                                        if (fingerTemplates.Exists(ft => ft.CheckSum == firstSampleCheckSum) || fingerTemplates.Exists(ft => ft.CheckSum == secondSampleCheckSum))
                                            continue;

                                        for (var j = 0; j < fingerTemplates.Count - 1; j += 2)
                                        {
                                            if (_fpData == null) continue;
                                            _fpData.ClearFPData();
                                            _fpData.Import(1, fingerTemplates[j].FingerIndex.OrderIndex, 2, 400, 400,
                                                fingerTemplates[j].Template, fingerTemplates[j + 1].Template);
                                            var firTemplate = _fpData.TextFIR;
                                            lock (_matching)
                                            {
                                                _matching.VerifyMatch(deviceTemplate, firTemplate);
                                                if (_matching.MatchingResult == 0) continue;
                                            }

                                            sameTemplateExists = true;
                                            break;
                                        }

                                        if (sameTemplateExists) continue;

                                        user.FingerTemplates.Add(new FingerTemplate
                                        {
                                            FingerIndex = _biometricTemplateManager.GetFingerIndex(_terminalUserData.FingerID[i]),
                                            Index = _fingerTemplateService.FingerTemplates(userId: (int)(existUser.Id))?.Count(ft => ft.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(_terminalUserData.FingerID[i]).Code) ?? 0 + 1,
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
                                                Index = _fingerTemplateService.FingerTemplates(userId: (int)(existUser.Id))?.Count(ft => ft.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(_terminalUserData.FingerID[i]).Code) ?? 0 + 1,
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
                                else
                                {
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
                            var faceCount = _terminalUserData.FaceNumber;
                            Logger.Log($"   +TotalFaceCount:{faceCount}");

                            if (faceCount > 0)
                            {
                                if (user.FaceTemplates is null)
                                    user.FaceTemplates = new List<FaceTemplate>();

                                var userFaces = _faceTemplateService.FaceTemplates(userId: _terminalUserData.UserID);
                                //existUser.FaceTemplates = new List<FaceTemplate>();

                                if (existUser != null)
                                    existUser.FaceTemplates = (userFaces.Any() ? userFaces : new List<FaceTemplate>());

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


                                if (existUser != null)
                                {
                                    if (!existUser.FaceTemplates.Exists(fp => fp.FaceTemplateType.Code == FaceTemplateTypes.VFACECode))
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
                        catch (Exception e)
                        {
                            Logger.Log(e);
                        }
                        //}

                        //WalkThroughFace
                        try
                        {
                            var walkThroughLenght = _terminalUserData.WalkThroughLength;
                            if (walkThroughLenght > 0)
                            {
                                var faceWalkData = _terminalUserData.WalkThroughData;

                                if (user.FaceTemplates is null)
                                    user.FaceTemplates = new List<FaceTemplate>();

                                var userFaces = _faceTemplateService.FaceTemplates(userId: _terminalUserData.UserID);
                                //existUser.FaceTemplates = new List<FaceTemplate>();

                                if (existUser != null)
                                    existUser.FaceTemplates = (userFaces.Any() ? userFaces : new List<FaceTemplate>());

                                
                                var faceData = (byte[])faceWalkData;
                                var faceTemplate = new FaceTemplate
                                {
                                    Index = 1,
                                    FaceTemplateType = _faceTemplateTypes.VWTFACE,
                                    UserId = user.Id,
                                    Template = faceData,
                                    CheckSum = faceData.Sum(x => x),
                                    Size = walkThroughLenght
                                };

                                if (existUser != null)
                                {
                                    if (!existUser.FaceTemplates.Exists(fp => fp.FaceTemplateType.Code == FaceTemplateTypes.VWTFACECode))
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
                        catch (Exception e)
                        {
                            Logger.Log(e);
                        }


                        //Iris
                        try
                        {
                            var irisDataLength = _terminalUserData.IrisDataLength;
                            if (irisDataLength > 0)
                            {
                                var irisData = _terminalUserData.IrisData;
                                if (user.IrisTemplates is null)
                                    user.IrisTemplates = new List<IrisTemplate>();
                                var userIrises = _irisTemplateService.IrisTemplates(userId: _terminalUserData.UserID);
                                if (existUser != null)
                                    existUser.IrisTemplates = (userIrises.Any() ? userIrises : new List<IrisTemplate>());
                                var irisTemplateData = (byte[]) irisData;
                                var irisTemplate = new IrisTemplate
                                {
                                    Index = 1,
                                    IrisTemplateType = _irisTemplateTypes.VIris,
                                    UserId = user.Id,
                                    Template = irisTemplateData,
                                    CheckSum = irisTemplateData.Sum(x => x),
                                    Size = irisDataLength
                                };

                                if (existUser != null)
                                {
                                    if (!existUser.IrisTemplates.Exists(fp => fp.IrisTemplateType.Code == IrisTemplateTypes.VIrisCode))
                                        user.IrisTemplates.Add(irisTemplate);
                                }
                                else
                                    user.IrisTemplates.Add(irisTemplate);

                                if (user.IrisTemplates.Any())
                                    foreach (var irisTemplates in user.IrisTemplates)
                                    {
                                        _irisTemplateService.ModifyIrisTemplate(irisTemplates);
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
                                    lock (_virdiServer.LoadFingerTemplateLock)
                                    {
                                        var accessGroupsOfUser = _accessGroupService.GetAccessGroups(userId: user.Id);
                                        if (accessGroupsOfUser is null || accessGroupsOfUser.Count == 0)
                                        {
                                            var devices =
                                                _deviceService.GetDevices(brandId: DeviceBrands.VirdiCode);

                                            foreach (var device in devices)
                                            {
                                                _virdiServer.AddUserToDeviceFastSearch(device.Code, (int)user.Code).ConfigureAwait(false);
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
                                                        _virdiServer.AddUserToDeviceFastSearch(device.Code, (int)user.Code).ConfigureAwait(false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Logger.Log(exception);
                                }
                            });
                        }

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
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"--> Error On GetUserDataCallback Error: {e.Message} ");
            }

            _ucsApi.EventGetUserData -= GetUserDataCallback;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Retrieve user from terminal";
        }

        public string GetDescription()
        {
            return $"Retrieving user: {UserId} from device: {Code}.";
        }
    }
}
