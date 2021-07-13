using Biovation.Brands.Suprema.Services;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly FastSearchService _fastSearchService;

        private readonly UserService _userService;
        private readonly AccessGroupService _accessGroupService;

        public UserController(UserService userService, AccessGroupService accessGroupService, FastSearchService fastSearchService)
        {
            _userService = userService;
            _accessGroupService = accessGroupService;
            _fastSearchService = fastSearchService;
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
        public ResultViewModel ModifyUser([FromBody] User user)
        {
            try
            {
                _fastSearchService.Initial();
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> SendUserToDevice(uint code, string userId)
        {
            return Task.Run(() => { 
                try
                {
                    return new ResultViewModel { Validate = 1, Message = "Sending user queued" };
                }
                catch (Exception e)
                {
                    Logger.Log($" --> SendUserToDevice Code: {code}  {e}");
                    return new ResultViewModel { Validate = 0, Message = e.Message };
                }
            });
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> SendUserToAllDevices([FromBody] User user)
        {
            return Task.Run(() =>
            {
                return new ResultViewModel {Id = user.Id, Validate = 1};
            });
        }
    }
}
