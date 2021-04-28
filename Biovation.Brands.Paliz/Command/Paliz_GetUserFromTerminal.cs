using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using PalizTiara.Api.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Biovation.CommonClasses.Interface;
using Biovation.Brands.Paliz.Manager;
using PalizTiara.Api.CallBacks;
using System.Text;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetUserFromTerminal : ICommand
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
        private readonly AccessGroupService _accessGroupService;
        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private uint Code { get; }
        private int UserId { get; }
        private readonly PalizServer _palizServer;
        private bool _userRetrievied;
        public PalizGetUserFromTerminal(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService, UserService userService, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, FingerTemplateService fingerTemplateService, FaceTemplateService faceTemplateService, FaceTemplateTypes faceTemplateTypes)
        {
            TerminalId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);

            var taskItem = taskService.GetTaskItem(TaskItemId)?.Data ?? new TaskItem();
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data["userId"];
            _palizServer = palizServer;
            _userService = userService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _fingerTemplateService = fingerTemplateService;
            _faceTemplateService = faceTemplateService;
            _faceTemplateTypes = faceTemplateTypes;

            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
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
                _palizServer._serverManager.UserInfoEvent += GetUserInfoEventCallBack;
                var userIdModel = new UserIdModel(UserId);
                _palizServer._serverManager.GetUserInfoAsyncTask(userIdModel, TerminalName);
                while (!_userRetrievied)
                {
                    System.Threading.Thread.Sleep(500);
                }
                Logger.Log(GetDescription());
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = TerminalId, Message = $"  +User {UserId} successfully retrieved from device: {Code}.\n", Validate = 1 };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = TerminalId, Message = $"Exeption: {exception}", Validate = 0 };

            }
        }
        private void ModifyFingerTemplates(FingerprintModel[] fingerprintList, User getUsersRes)
        {
            var fingerTemplateList = new List<FingerTemplate>();
            if (getUsersRes != null)
            {
                var fingerTemplates = _fingerTemplateService.FingerTemplates(userId: (int)(getUsersRes.Id))?.Data?.Data
                    .Where(ft => ft.FingerTemplateType.Code == FingerTemplateTypes.V400Code).ToList();
                foreach (var fingerprint in fingerprintList)
                {
                    fingerTemplateList.Add(new FingerTemplate
                    {
                        // TODO - Ask this if casting is ok.
                        Id = (int)fingerprint.Id,
                        UserId = fingerprint.UserId,
                        Template = fingerprint.Template,
                        FingerIndex = _biometricTemplateManager.GetFingerIndex(fingerprint.Index),
                        EnrollQuality = fingerprint.Quality,
                        FingerTemplateType = _fingerTemplateTypes.V400,
                        Index = _fingerTemplateService.FingerTemplates(userId: (int)(getUsersRes.Id))?.Data?.Data.Count(ft => ft.FingerIndex.Code == _biometricTemplateManager.GetFingerIndex(fingerprint.Index).Code) ?? 0 + 1
                    });
                }
            }
            foreach(var fingerprint in fingerprintList)
            {
                fingerTemplateList.Add(new FingerTemplate
                {
                    // TODO - Ask this if casting is ok.
                    Id = (int) fingerprint.Id,
                    UserId = fingerprint.UserId,
                    Template = fingerprint.Template,
                    FingerIndex = _biometricTemplateManager.GetFingerIndex(fingerprint.Index),
                    EnrollQuality = fingerprint.Quality,
                    FingerTemplateType = _fingerTemplateTypes.V400,
                    Index = fingerprint.Index
                });
            }
            if (fingerTemplateList.Any())
            {
                foreach (var fingerTemplate in fingerTemplateList)
                {
                    _fingerTemplateService.ModifyFingerTemplate(fingerTemplate);
                }
            }
        }
        private void ModifyFaceTemplates(UserInfoModel userInfoModel, User getUsersRes)
        {
            var faceTemplateList = new List<FaceTemplate>();
            var userFaces = _faceTemplateService.FaceTemplates(userId: userInfoModel.Id);
            if (getUsersRes != null)
            {
                getUsersRes.FaceTemplates = (userFaces.Any() ? userFaces : new List<FaceTemplate>());
            }
            foreach(var faceModel in userInfoModel.Faces)
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
                if (getUsersRes != null)
                {
                    if (!getUsersRes.FaceTemplates.Exists(fp => fp.FaceTemplateType.Code == FaceTemplateTypes.VFACECode))
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

        // TODO - If ever needed, use this method to get cards info.
        //private void ModifyUserCards(UserInfoModel userInfoModel)
        //{
        //    try
        //    {
        //        foreach (var card in userInfoModel.Cards)
        //        {
        //            var userCard = new UserCard
        //            {
        //                CardNum = card.ToString(),
        //                IsActive = true,
        //                UserId = userInfoModel.Id
        //            };

        //            _userCardService.ModifyUserCard(card);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log(e);
        //    }
        //}

        private void GetUserInfoEventCallBack(object sender, UserInfoEventArgs args)
        {
            if (TerminalId != TaskItemId)
            {
                return;
            }

            if (args.Result == false)
            {
                return;
            }

            var userInfoModel = args.UserInfoModel;
            
            var isoEncoding = Encoding.GetEncoding(28591);
            var windowsEncoding = Encoding.GetEncoding(1256);

            var getUsersRes = _userService.GetUsers(code: userInfoModel.Id)?.Data?.Data.FirstOrDefault();
            var user = new User
            {
                AuthMode = userInfoModel.Locked ? 0 : 1,
                Password = userInfoModel.Password,
                FullName = userInfoModel.Name,
                IsActive = userInfoModel.Locked,
                ImageBytes = userInfoModel.Image,
            };
            try
            {
                Logger.Log($"   +TotalFingerCount:{userInfoModel.Fingerprints.Length}");
                ModifyFingerTemplates(userInfoModel.Fingerprints, getUsersRes);

                Logger.Log($"   +TotalFaceCount:{userInfoModel.Faces.Length}");
                ModifyFaceTemplates(userInfoModel, getUsersRes);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            _palizServer._serverManager.UserInfoEvent -= GetUserInfoEventCallBack;
            _userRetrievied = true;
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
