using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //todo:add user
        [HttpPost]
        [Authorize]

        public Task<ResultViewModel> AddUser([FromBody] User user)
        {
            return Task.Run(() => _userRepository.AddUser(user));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyUser([FromBody] User user)
        {
            return Task.Run(() => _userRepository.ModifyUser(user));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteUser(int id = default)
        {
            return Task.Run(() => _userRepository.DeleteUser(id));
        }


        //batch delete
        [HttpPost]
        [Route("/DeleteUsers")]
        [Authorize]
        public Task<ResultViewModel> DeleteUsers([FromBody]List<int> ids = default)
        {
            return Task.Run(() => _userRepository.DeleteUsers(ids));
        }

        [HttpPatch]
        [Route("Password/{id}")]
        [Authorize]
        public Task<ResultViewModel> ModifyPassword(int id = default, string password = default)
        {
            return Task.Run(() => _userRepository.ModifyPassword(id, password));
        }

        [HttpDelete]
        [Route("/UserGroupsOfUser")]
        [Authorize]
        public Task<ResultViewModel> DeleteUserGroupsOfUser(int userId, int userTypeId = 1)
        {
            return Task.Run(() => _userRepository.DeleteUserGroupsOfUser(userId, userTypeId));
        }

        [HttpDelete]
        [Route("/UserGroupOfUser")]
        [Authorize]
        public Task<ResultViewModel> DeleteUserGroupOfUser(int userId, int userGroupId, int userTypeId = 1)
        {
            return Task.Run(() => _userRepository.DeleteUserGroupOfUser(userId, userGroupId, userTypeId));
        }
    }
}
