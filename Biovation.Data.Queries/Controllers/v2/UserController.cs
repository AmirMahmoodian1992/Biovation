using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        [Route("Users")]
        [Authorize]
        public Task<ResultViewModel<PagingResult<User>>> GetUsers(int from = default, int size = default, bool getTemplatesData = default, long userId = default,int code =default, string filterText = default, int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default, int pageSize = default)
        {
            var user = HttpContext.GetUser();
            return Task.Run(() => _userRepository.GetUsersByFilter(user.Id, from, size, getTemplatesData, userId, code, filterText, type, withPicture, isAdmin, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("AdminUserOfAccessGroup")]
        [Authorize]

        public Task<ResultViewModel<List<User>>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default)
        {
            return Task.Run(() => _userRepository.GetAdminUserOfAccessGroup(id, accessGroupId));
        }

        [HttpGet]
        [Route("UsersCount")]
        [Authorize]
        public Task<ResultViewModel<int>> GetUsersCount()
        {
            var user = (User)HttpContext.Items["User"];
            return Task.Run(() => _userRepository.GetUsersCount());
        }


        [HttpGet]
        [Route("AuthorizedDevicesOfUser")]
        [Authorize]

        public Task<ResultViewModel<List<DeviceBasicInfo>>> GetAuthorizedDevicesOfUser(int userId)
        {
            return Task.Run(() => _userRepository.GetAuthorizedDevicesOfUser(userId));
        }
    }
}
