using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiRetrieveUsersListFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
        private int DeviceId { get; }
        private uint Code { get; }
        private int TaskItemId { get; }
        //private TaskItem TaskItem { get; }

        private readonly Callbacks _callbacks;


        public VirdiRetrieveUsersListFromTerminal(IReadOnlyList<object> items, VirdiServer virdiServer, Callbacks callbacks, DeviceService deviceService)
        {
            _callbacks = callbacks;
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: int.Parse(DeviceBrands.VirdiCode)).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;
         
            OnlineDevices = virdiServer.GetOnlineDevices();
        }
        public object Execute()
        {

            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"RetriveUser,The device: {Code} is not connected.");

                //TaskItem.Status.Code = TaskStatuses.DeviceDisconnectedCode;
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 0, Data = new List<User>() };

            }

            try
            {
                
                // Callbacks.modifyUserData = false;
                Callbacks.GetUserTaskFinished = true;
                Callbacks.RetrieveUsers = new List<User>();

                _callbacks.TerminalUserData.GetUserInfoListFromTerminal(TaskItemId, (int)Code);
                System.Threading.Thread.Sleep(500);
                Logger.Log(GetDescription());

                if (_callbacks.TerminalUserData.ErrorCode == 0)
                {
                    Logger.Log($"  +Retrieving users from device: {Code} started successful.\n");

                    while (!Callbacks.GetUserTaskFinished)
                        System.Threading.Thread.Sleep(500);

                    var result = Callbacks.RetrieveUsers;

                    Callbacks.RetrieveUsers = new List<User>();
                    Callbacks.GetUserTaskFinished = true;

                    //TaskItem.Status.Code = TaskStatuses.DoneCode;
                    return new ResultViewModel<List<User>> { Data = result, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };

                }

                Logger.Log($"  +Cannot retrieve users from device: {Code}. Error code = {_callbacks.TerminalUserData.ErrorCode}\n");
                //TaskItem.Status.Code = TaskStatuses.FailedCode;
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.FailedCode), Data = new List<User>(), Id = DeviceId, Message = "0", Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel<List<User>> { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = "0", Validate = 0 };

            }
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
