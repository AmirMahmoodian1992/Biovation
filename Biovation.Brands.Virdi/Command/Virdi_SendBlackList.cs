using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Encoding = System.Text.Encoding;

namespace Biovation.Brands.Virdi.Command
{
    public class VirdiSendBlackList : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private int UserId { get; set; }
        private int BlackListId { get; set; }

        private User UserObj { get; }
        private int TaskItemId { get; }
        private int IsBlackList { get; }


        public VirdiSendBlackList(IReadOnlyList<object> items, VirdiServer virdiServer, TaskService taskService, UserService userService, DeviceService deviceService, BlackListService blackListService)
        {
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = deviceService.GetDevices(brandId: DeviceBrands.VirdiCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0;
            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            BlackListId = (int)data["BlackListId"];
            UserObj = userService.GetUsers(code: UserId, withPicture: false).FirstOrDefault();
            UserId = Convert.ToInt32(UserObj.Id);

            var blackList = blackListService.GetBlacklist(BlackListId, userId: UserId, deviceId: DeviceId, DateTime.Now).Result.FirstOrDefault();
            IsBlackList = blackList != null ? 1 : 0;

            OnlineDevices = virdiServer.GetOnlineDevices();
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"SendBlackList,The device: {Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"The device: {Code} is not connected.", Validate = 1 };
            }

            if (UserObj == null)
            {
                Logger.Log($"SendBlackList,User {UserId} does not exist.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"User {UserId} does not exist.", Validate = 1 };
            }
            if (IsBlackList == 0)
            {
                Logger.Log($"SendBlackList,BlackList {BlackListId} does not exist any more.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"BlackList { BlackListId } does not exist any more.", Validate = 1 };
            }

            try
            {


                //Todo: fix command
                //var deviceId = DeviceId;
                //var creatorUser = _userService.GetUser(123456789, false);
                //var task = new TaskInfo
                //{
                //    CreatedAt = DateTimeOffset.Now,
                //    CreatedBy = creatorUser,
                //    TaskType = TaskTypes.SendUsers,
                //    Priority = TaskPriorities.Medium,
                //    DeviceBrand = DeviceBrands.Virdi,
                //    TaskItems = new List<TaskItem>()
                //};
                //task.TaskItems.Add(new TaskItem
                //{
                //    Status = TaskStatuses.Queued,
                //    TaskItemType = TaskItemTypes.SendUser,
                //    Priority = TaskPriorities.Medium,
                //    DueDate = DateTime.Today,
                //    DeviceId = deviceId,

                //    Data = JsonConvert.SerializeObject(new {UserId }),
                //    IsParallelRestricted = true,
                //    IsScheduled = false,
                //    OrderIndex = 1
                //});
                //_taskService.InsertTask(task).Wait();
                //_taskManager.ProcessQueue();


                Logger.Log($"  +Cannot blacklist user {UserId} to device: {Code}. Error code = erData.ErrorCode\n");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"  +Cannot transfer user {UserId} to device: {Code}. Error code = \n", Validate = 1 };
            }
            catch (Exception ex)
            {
                Logger.Log("Error! ErrorMessage:{0}", ex.Message);
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.FailedCode), Id = DeviceId, Message = $"Error! ErrorMessage:{ex.Message}", Validate = 1 };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Add user to terminal";
        }

        public string GetDescription()
        {
            return $"Adding user: {UserId} to device: {Code}.";
        }

        public static string Decrypt(string cipherText)
        {
            var EncryptionKey = "Kasra";
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                if (encryptor != null)
                {
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(),
                            CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }

                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            return cipherText;
        }
    }
}
