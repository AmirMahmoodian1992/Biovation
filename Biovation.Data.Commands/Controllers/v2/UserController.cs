using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

            
        //todo:add user
        /*[HttpPost] 
        [Route("AddUser")]
        public Task<IActionResult> AddUser([FromBody] User user)
        {
            /* return Task.Run(() => _userRepository.());*/
         /*   throw null;
        }*/

        [HttpPut]
        public Task<ResultViewModel> ModifyUser([FromBody] User user)
        {
            return Task.Run(() => _userRepository.ModifyUser(user));
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteUser(int id = default)
        {
            return Task.Run(() => _userRepository.DeleteUser(id));
        }

        //batch delete
        [HttpPost]
        [Route("/DeleteUsers")]
        public Task<ResultViewModel> DeleteUsers([FromBody]List<int> ids = default)
        {
            return Task.Run(() => _userRepository.DeleteUsers(ids));
        }

        [HttpPatch]
        [Route("Password/{id}")]
        public Task<ResultViewModel> ModifyPassword(int id = default, string password = default)
        {
            return Task.Run(() => _userRepository.ModifyPassword(id,password));
        }

        [HttpDelete]
        [Route("/UserGroupsOfUser")]
        public Task<ResultViewModel> DeleteUserGroupsOfUser(int userId, int userTypeId = 1)
        {
            return Task.Run(() => _userRepository.DeleteUserGroupsOfUser(userId, userTypeId));
        }

        [HttpDelete]
        [Route("/UserGroupOfUser")]
        public Task<ResultViewModel> DeleteUserGroupOfUser(int userId, int userGroupId, int userTypeId = 1)
        {
            return Task.Run(() => _userRepository.DeleteUserGroupOfUser(userId,userGroupId,userTypeId));
        }

    }
}
