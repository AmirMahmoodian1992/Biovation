using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;
using Biovation.CommonClasses.Interface;
using Biovation.Brands.Paliz.Manager;
using PalizTiara.Api.CallBacks;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetAllUsersFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private readonly UserService _userService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly UserCardService _userCardService;
        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private uint Code { get; }
        private readonly PalizServer _palizServer;
        public PalizGetAllUsersFromTerminal(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService,
            UserService userService, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, FingerTemplateService fingerTemplateService,
            FaceTemplateService faceTemplateService, FaceTemplateTypes faceTemplateTypes, UserCardService userCardService)
        {
            TerminalId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);

            _palizServer = palizServer;
            _userService = userService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _fingerTemplateService = fingerTemplateService;
            _faceTemplateService = faceTemplateService;
            _faceTemplateTypes = faceTemplateTypes;
            _userCardService = userCardService;

            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
            TerminalName = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Name ?? string.Empty;
            Code = devices?.Data?.Data.FirstOrDefault(d => d.DeviceId == TerminalId)?.Code ?? 7;
            OnlineDevices = palizServer.GetOnlineDevices();
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"RetriveUser,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = TerminalId, Message = $"The device: {Code} is not connected.", Validate = 1 };
            }

            try
            {
                _palizServer._serverManager.MassUserInfoEvent += GetMassUserInfoEventCallBack;
                var massUserIdModel = new MassUserIdModel(1, long.MaxValue);
                _palizServer._serverManager.GetMassUserInfoTask(TerminalName, massUserIdModel);
                System.Threading.Thread.Sleep(1000);
                Logger.Log(GetDescription());
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = TerminalId, Message = $"  +Users list successfully retrieved from device: {Code}.\n", Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = TerminalId, Message = $"Exeption: {exception}", Validate = 0 };

            }
        }
        private void ModifyFingerTemplates(UserInfoModel userInfoModel, User user)
        {
            if (userInfoModel?.Fingerprints == null)
            {
                return;
            }
            var fingerprints = userInfoModel.Fingerprints;
            var fingerTemplateList = new List<FingerTemplate>();
            if (user != null)
            {
                // TODO - Ask why this is here
                var fingerTemplates = _fingerTemplateService.FingerTemplates(userId: (int)(user.Id))?.Data?.Data
                    .Where(ft => ft.FingerTemplateType.Code == FingerTemplateTypes.V400Code).ToList();

                fingerTemplateList.AddRange(fingerprints.Select(fingerprint => new FingerTemplate
                {
                    // TODO - Ask this if casting is ok.
                    UserId = user.Id,
                    Template = fingerprint.Template,
                    FingerIndex = _biometricTemplateManager.GetFingerIndex(fingerprint.Index),
                    EnrollQuality = fingerprint.Quality,
                    FingerTemplateType = _fingerTemplateTypes.V400,
                    Index = _fingerTemplateService.FingerTemplates(userId: (int)(user.Id))?.Data?.Data.Count(ft => ft.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(fingerprint.Index).Code) ?? 0 + 1
                }));
            }

            fingerTemplateList.AddRange(fingerprints.Select(fingerprint => new FingerTemplate
            {
                // TODO - Ask this if casting is ok.
                UserId = fingerprint.UserId,
                Template = fingerprint.Template,
                FingerIndex = _biometricTemplateManager.GetFingerIndex(fingerprint.Index),
                EnrollQuality = fingerprint.Quality,
                FingerTemplateType = _fingerTemplateTypes.V400,
                Index = fingerprint.Index
            }));

            if (!fingerTemplateList.Any()) return;

            foreach (var fingerTemplate in fingerTemplateList)
            {
                _fingerTemplateService.ModifyFingerTemplate(fingerTemplate);
            }
        }
        private void ModifyFaceTemplates(UserInfoModel userInfoModel, User user)
        {
            if (userInfoModel?.Faces == null)
            {
                return;
            }
            var faceTemplateList = new List<FaceTemplate>();
            var userFaces = _faceTemplateService.FaceTemplates(userId: userInfoModel.Id);
            if (user != null)
            {
                user.FaceTemplates = (userFaces.Any() ? userFaces : new List<FaceTemplate>());
            }
            foreach (var faceModel in userInfoModel.Faces)
            {
                var faceData = faceModel.Template;
                var faceTemplate = new FaceTemplate
                {
                    Index = userInfoModel.Faces.Length,
                    FaceTemplateType = _faceTemplateTypes.VFACE,
                    UserId = faceModel.UserId,
                    Template = faceData,
                    CheckSum = faceData.Sum(x => x),
                    Size = faceData.Length
                };
                if (user != null)
                {
                    if (!user.FaceTemplates.Exists(fp => fp.FaceTemplateType.Code == FaceTemplateTypes.VFACECode))
                    {
                        faceTemplateList.Add(faceTemplate);
                    }
                }
                else
                {
                    faceTemplateList.Add(faceTemplate);
                }

                if (faceTemplateList.Any())
                {
                    foreach (var faceTemplates in faceTemplateList)
                    {
                        _faceTemplateService.ModifyFaceTemplate(faceTemplates);
                    }
                }

            }
        }

        private void ModifyUserCards(UserInfoModel userInfoModel, long userId)
        {
            try
            {
                if (userInfoModel?.Cards == null)
                {
                    return;
                }
                foreach (var card in userInfoModel.Cards)
                {
                    var userCard = new UserCard
                    {
                        CardNum = card.ToString(),
                        IsActive = true,
                        UserId = userId
                    };

                    _userCardService.ModifyUserCard(userCard);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private void GetMassUserInfoEventCallBack(object sender, MassUserInfoEventArgs args)
        {
            //if (TerminalId != TaskItemId)
            //{
            //    return;
            //}

            if (args.Result == false)
            {
                return;
            }

            var userInfoModels = args.Users.UserInfoModels;
            foreach(var userInfoModel in userInfoModels)
            {
                var getUsersRes = _userService.GetUsers(code: userInfoModel.Id)?.Data?.Data.FirstOrDefault();
                var user = new User
                {
                    Code = userInfoModel.Id,
                    AdminLevel = (int)userInfoModel.Level,
                    StartDate = DateTime.Parse("1970/01/01"),
                    EndDate = DateTime.Parse("2050/01/01"),
                    AuthMode = userInfoModel.Locked ? 0 : 1,
                    Password = userInfoModel.Password,
                    FullName = userInfoModel.Name,
                    IsActive = userInfoModel.Locked,
                    ImageBytes = userInfoModel.Image
                };

                var existingUser = _userService.GetUsers(code: userInfoModel.Id)?.Data?.Data?.FirstOrDefault();
                if (existingUser != null)
                {
                    user.Id = existingUser.Id;
                }

                var userInsertionResult = _userService.ModifyUser(user);
                Logger.Log("<--User is Modified");

                user.Id = userInsertionResult.Id;
                try
                {
                    Logger.Log($"   +TotalCardCount:{userInfoModel?.Cards?.Length ?? 0}");
                    ModifyUserCards(userInfoModel, user.Id);


                    Logger.Log($"   +TotalFingerCount:{userInfoModel?.Fingerprints?.Length ?? 0}");
                    ModifyFingerTemplates(userInfoModel, user);

                    Logger.Log($"   +TotalFaceCount:{userInfoModel?.Faces?.Length ?? 0}");
                    ModifyFaceTemplates(userInfoModel, user);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            
            _palizServer._serverManager.MassUserInfoEvent -= GetMassUserInfoEventCallBack;
        }
        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get all logs of a device command";
        }

        public string GetDescription()
        {
            return "Getting all logs of a device and insert into database.";
        }
    }
}
