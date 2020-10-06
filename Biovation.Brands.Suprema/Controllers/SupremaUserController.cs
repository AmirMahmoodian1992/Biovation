using Biovation.Brands.Suprema.Commands;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biovation.Brands.Suprema.Service;
using Biovation.CommonClasses;

namespace Biovation.Brands.Suprema.ApiControllers
{
    public class SupremaUserController : ApiController
    {
        private readonly UserService _commonUserService = new UserService();
        private readonly AccessGroupService _accessGroupService = new AccessGroupService();

        [HttpGet]
        public Task<List<User>> Users()
        {
            var user = _commonUserService.GetUsers();
            return user;
        }

        [HttpGet]
        public User Users(int id)
        {
            var user = _commonUserService.GetUser(id);
            return user;
        }

        [HttpGet]
        public List<AccessGroup> GetUserAccessGroups(int userId)
        {
            var accessGroups = _accessGroupService.GetAccessGroupsOfUser(userId);
            return accessGroups.ToList();
        }

        [HttpPost]
        public ResultViewModel ModifyUser([FromBody]User user)
        {
            try
            {
                FastSearchService.GetInstance().Initial();
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }

        [HttpGet]
        public Task<ResultViewModel> SendUserToDevice(uint code, string userId)
        {
            return Task.Run(() =>
            {

                //var resultList = new List<ResultViewModel>();
                try
                {
                    var userIds = JsonConvert.DeserializeObject<long[]>(userId);
                    //var listResult = new List<ResultViewModel>();
                    foreach (var receivedUserId in userIds)
                    {
                        //var addUserToTerminalCommand = CommandFactory.Factory(CommandType.SendUserToDevice,
                        //    new List<object> {code, receivedUserId});
                        //var result = addUserToTerminalCommand.Execute();
                        CommandFactory.Factory(CommandType.SendUserToDevice, new List<object> { code, receivedUserId })
                            .Execute();
                        //listResult.Add(new ResultViewModel {Message = userId, Validate = Convert.ToInt32(result)});

                    }

                    return new ResultViewModel { Validate = 1 };
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    return new ResultViewModel { Validate = 0, Message = e.Message };
                }
            });



            //try
            //{
            //    var userIds = JsonConvert.DeserializeObject<long[]>(userId);
            //    var listResult = new List<ResultViewModel>();
            //    foreach (var receivedUserId in userIds)
            //    {
            //        var addUserToTerminalCommand = CommandFactory.Factory(CommandType.SendUserToDevice,
            //            new List<object> { code, receivedUserId });
            //        var result = addUserToTerminalCommand.Execute();
            //        listResult.Add(new ResultViewModel { Message = userId, Validate = Convert.ToInt32(result) });
            //    }
            //    return listResult;
            //}
            //catch (Exception e)
            //{
            //    Logger.Log($"--> SendUserToDevice Code: {code}  {e}");
            //    return new List<ResultViewModel> { new ResultViewModel { Validate = 0 } };
            //}

        }

        [HttpPost]
        public ResultViewModel SendUserToAllDevices([FromBody]User user)
        {
            var accessGroups = _accessGroupService.GetAccessGroupsOfUser(user.Id);
            if (!accessGroups.Any())
            {
                return new ResultViewModel { Id = user.Id, Validate = 0 };
            }
            foreach (var accessGroup in accessGroups)
            {
                foreach (var deviceGroup in accessGroup.DeviceGroup)
                {
                    foreach (var deviceGroupMember in deviceGroup.Devices)
                    {
                        var addUserToTerminalCommand = CommandFactory.Factory(CommandType.SyncAllUsers,
                            new List<object> { deviceGroupMember.Code, user.Code });

                        addUserToTerminalCommand.Execute();
                    }
                }
            }

            return new ResultViewModel { Id = user.Id, Validate = 1 };
        }
    }
}
