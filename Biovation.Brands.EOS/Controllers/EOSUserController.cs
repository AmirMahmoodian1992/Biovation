using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.EOS.Commands;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biovation.Brands.EOS.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class EosUserController : Controller
    {
        private readonly CommandFactory _commandFactory;
      
        private readonly UserService _userService;
        private readonly AccessGroupService _accessGroupService;

        public EosUserController(UserService userService, AccessGroupService accessGroupService, CommandFactory commandFactory)
        {
            _userService = userService;
            _accessGroupService = accessGroupService;
            _commandFactory = commandFactory;
        }

        [HttpGet]
        public Task<List<User>> Users()
        {
            return Task.Run(() =>
            {

                var user = _userService.GetUsers();
                return user;
            }
        );
        }


        [HttpGet]
        [Authorize]
        public User Users(int id)
        {
            var user = _userService.GetUsers(userId: id)?.FirstOrDefault();
            return user;
        }

        [HttpGet]
        [Authorize]
        public List<AccessGroup> GetUserAccessGroups(int userId)
        {
            var accessGroups = _accessGroupService.GetAccessGroups(userId: userId);
            return accessGroups.ToList();
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyUser([FromBody]User user)
        {
            try
            {
                //_fastSearchService.Initial();
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }

        [HttpGet]
       // [Authorize]
      //  public Task<ResultViewModel> SendUserToDevice(uint code, string userId)
        public Task<ResultViewModel> SendUserToDevice(uint code, int receivedUserId)
        {
            return Task.Run(() =>
            {

                try
                {
                   // var userIds = JsonConvert.DeserializeObject<uint[]>(userId);
                   // foreach (var receivedUserId in userIds)
                   // {
                      _commandFactory.Factory(CommandType.SendUserToDevice, new List<object> { code, receivedUserId })
                            .Execute();
                        //listResult.Add(new ResultViewModel {Message = userId, Validate = Convert.ToInt32(result)});

                  //  }

                    return new ResultViewModel { Validate = 1 };
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    return new ResultViewModel { Validate = 0, Message = e.Message };
                }
            });

        }

        [HttpPost]
        [Authorize]
        public ResultViewModel SendUserToAllDevices([FromBody]User user)
        {
            var accessGroups = _accessGroupService.GetAccessGroups(userId: user.Id);
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
                        var addUserToTerminalCommand = _commandFactory.Factory(CommandType.SyncAllUsers,
                            new List<object> { deviceGroupMember.Code, user.Code });

                        addUserToTerminalCommand.Execute();
                    }
                }
            }

            return new ResultViewModel { Id = user.Id, Validate = 1 };
        }
    }
}
