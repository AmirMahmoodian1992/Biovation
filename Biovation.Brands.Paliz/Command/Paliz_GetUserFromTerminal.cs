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
        private readonly LogEvents _logEvents;
        private readonly PalizCodeMappings _palizCodeMappings;
        private int TaskItemId { get; }
        private string TerminalName { get; }
        private int TerminalId { get; }
        private long StartDate { get; }
        private long EndDate { get; }
        private uint Code { get; }
        private int UserId { get; }
        private readonly PalizServer _palizServer;
        public PalizGetUserFromTerminal(IReadOnlyList<object> items, PalizServer palizServer, TaskService taskService, DeviceService deviceService, LogEvents logEvents, PalizCodeMappings palizCodeMappings)
        {
            TerminalId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);

            var taskItem = taskService.GetTaskItem(TaskItemId)?.Data ?? new TaskItem();
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data["userId"];
            try
            {
                var date = (DateTime)data["fromDate"];
                StartDate = date.Ticks;
            }
            catch (Exception)
            {
                StartDate = new DateTime(1970, 1, 1).Ticks;
            }
            try
            {
                var date = (DateTime)data["toDate"];
                EndDate = date.Ticks;
            }
            catch (Exception)
            {
                EndDate = DateTime.Now.AddYears(5).Ticks;
            }
            _palizCodeMappings = palizCodeMappings;
            _logEvents = logEvents;
            _palizServer = palizServer;
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
                //Callbacks.ModifyUserData = true;
                _palizServer._serverManager.UserInfoEvent += GetUserInfoEventCallBack;
                var userIdModel = new UserIdModel(UserId);
                _palizServer._serverManager.GetUserInfoAsyncTask(userIdModel, TerminalName);
                System.Threading.Thread.Sleep(1000);
                Logger.Log(GetDescription());
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = TerminalId, Message = $"  +User {UserId} successfully retrieved from device: {Code}.\n", Validate = 1 };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = TerminalId, Message = $"Exeption: {exception}", Validate = 0 };

            }
        }
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
            var fingerprintModelList = userInfoModel.Fingerprints;
            var t = new FingerTemplate();
            
            var isoEncoding = Encoding.GetEncoding(28591);
            var windowsEncoding = Encoding.GetEncoding(1256);

            //var user = new User
            //{
            //    Code = _terminalUserData.UserID,
            //    AdminLevel = args.UserInfoModel.Level,
            //    AuthMode = _terminalUserData.AuthType,
            //    Password = userInfoModel.Password,
            //    FullName = PalizTiara.Api.Helpers. userInfoModel.Name,
            //    IsActive = userInfoModel.Locked,
            //    ImageBytes = userInfoModel.Image,
            //    FingerTemplates = new List<FingerTemplate>(LinkedListNode,)
            //};
            
            //var userExists = _userService.GetUsers(code: _terminalUserData.UserID).FirstOrDefault();
            //if (userExists != null)
            //{
            //    user = new User
            //    {
            //        Id = userExists.Id,
            //        Code = userExists.Code,
            //        AdminLevel = _terminalUserData.IsAdmin,
            //        StartDate = _terminalUserData.StartAccessDate == "0000-00-00"
            //            ? userExists.StartDate
            //            : DateTime.Parse(_terminalUserData.StartAccessDate),
            //        EndDate = _terminalUserData.EndAccessDate == "0000-00-00"
            //            ? userExists.EndDate
            //            : DateTime.Parse(_terminalUserData.EndAccessDate),
            //        AuthMode = _terminalUserData.AuthType,
            //        Password = _terminalUserData.Password,
            //        UserName = string.IsNullOrEmpty(userName) ? userExists.UserName : userName,
            //        FirstName = firstName ?? userExists.FirstName,
            //        SurName = string.Equals(surName, userName) ? userExists.SurName ?? surName : surName,
            //        IsActive = userExists.IsActive,
            //        ImageBytes = picture
            //    };
            //}

            //var userInsertionResult = _userService.ModifyUser(user);

            _palizServer._serverManager.UserInfoEvent -= GetUserInfoEventCallBack;
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
