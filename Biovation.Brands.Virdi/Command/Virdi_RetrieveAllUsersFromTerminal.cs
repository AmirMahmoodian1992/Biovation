using Biovation.Brands.Virdi.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCBioBSPCOMLib;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiRetrieveUsersListFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private readonly List<User> _users;
        private readonly UCSAPI _ucsApi;
        private readonly ITerminalUserData _terminalUserData;
        private readonly ManualResetEventSlim _doneEvent;
        private int DeviceId { get; }
        private uint Code { get; }
        private int TaskItemId { get; }

        public VirdiRetrieveUsersListFromTerminal(IReadOnlyList<object> items, VirdiServer virdiServer, Callbacks callbacks, UCSAPI ucsApi, TaskService taskService, UserService userService, DeviceService deviceService, UserCardService userCardService, FaceTemplateTypes faceTemplateTypes, AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, FingerTemplateTypes fingerTemplateTypes, FingerTemplateService fingerTemplateService, BiometricTemplateManager biometricTemplateManager)
        {
            _ucsApi = ucsApi;
            _terminalUserData = ucsApi.TerminalUserData as ITerminalUserData;

            _users = new List<User>();
            _doneEvent = new ManualResetEventSlim(false);

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;

            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"RetrieveUser,The device: {Code} is not connected.");
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 0, Data = new List<User>() };
            }

            try
            {
                //Callbacks.GetUserTaskFinished = true;
                //Callbacks.RetrieveUsers = new List<User>();
                lock (_ucsApi)
                    _ucsApi.EventGetUserInfoList += GetUserListCallback;
                lock (_terminalUserData)
                    _terminalUserData.GetUserInfoListFromTerminal(TaskItemId, (int)Code);
                
                Logger.Log(GetDescription());
                if (_terminalUserData.ErrorCode == 0)
                {
                    Logger.Log($"  +Retrieving users from device: {Code} started successful.\n");
                    _doneEvent.Wait(TimeSpan.FromMinutes(2));

                    return new ResultViewModel<List<User>> { Data = _users, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
                }

                Logger.Log($"  +Cannot retrieve users from device: {Code}. Error code = {_terminalUserData.ErrorCode}\n");
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.FailedCode), Data = new List<User>(), Id = DeviceId, Message = "0", Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = "0", Validate = 0 };
            }
        }

        private void GetUserListCallback(int clientId, int terminalId)
        {
            if (_terminalUserData.UserID == 0 || clientId != TaskItemId)
                return;

            lock (_ucsApi)
            {
                lock (_terminalUserData)
                {
                    var isoEncoding = Encoding.GetEncoding(28591);
                    var windowsEncoding = Encoding.GetEncoding(1256);

                    var deviceUserName = _terminalUserData.UserName;
                    var replacements = new Dictionary<string, string> { { "˜", "\u0098" }, { "Ž", "\u008e" } };
                    var userName = replacements.Aggregate(deviceUserName,
                        (current, replacement) => current.Replace(replacement.Key, replacement.Value));
                    userName = string.IsNullOrEmpty(userName)
                        ? null
                        : windowsEncoding.GetString(isoEncoding.GetBytes(userName)).Trim();

                    var indexOfSpace = userName?.IndexOf(' ') ?? 0;
                    var firstName = indexOfSpace > 0 ? userName?.Substring(0, indexOfSpace).Trim() : null;
                    var surName = indexOfSpace > 0
                        ? userName?.Substring(indexOfSpace, userName.Length - indexOfSpace).Trim()
                        : userName;

                    Logger.Log($@"<--EventGetUserInfoList
    +TerminalID:{terminalId}
    +ErrorCode:{_ucsApi.ErrorCode}
    +UserID:{_terminalUserData.UserID}
    +Admin:{_terminalUserData.IsAdmin}
    +AuthType:{_terminalUserData.AuthType}
    +Blacklist:{_terminalUserData.IsBlacklist}
    +Progress:{_terminalUserData.CurrentIndex}/{_terminalUserData.TotalNumber}", logType: LogType.Verbose);

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

                    // _terminalUserData.GetUserDataFromTerminal(0, terminalId, _terminalUserData.UserID);
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
                    //user.Id = _commonUserService.GetUsers(code: _terminalUserData.UserID, withPicture: false)?.FirstOrDefault()?.Id == null
                    //    ? 0
                    //    : _commonUserService.GetUsers(code: _terminalUserData.UserID, withPicture: false).FirstOrDefault().Id;
                    _users.Add(user);
                    //int totalCount = _terminalUserData.TotalNumber;
                    //int currentIndex = _terminalUserData.CurrentIndex;
                    if (_terminalUserData.CurrentIndex != _terminalUserData.TotalNumber) return;
                    _doneEvent.Set();
                    _ucsApi.EventGetUserInfoList += GetUserListCallback;
                }
            }
            /*Task.Run(async () =>
            {

                var taskItem = _taskService.GetTaskItem(clientId);
                if (taskItem != null)
                {
                    taskItem.TotalCount = totalCount;
                    taskItem.CurrentIndex = currentIndex;

                }

                _taskService.UpdateTaskStatus(taskItem);
            });*/
            //for (var i = 0; i < 20; i++)
            //{
            //    Thread.Sleep(100);
            //    if (RetrieveUsers.Count == _terminalUserData.TotalNumber) break;
            //}

            ////while (RetrieveUsers.Count != _terminalUserData.TotalNumber) { }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Retrieve all users from terminal";
        }

        public string GetDescription()
        {
            return $"Retrieving all users from device: {Code}.";
        }
    }
}
