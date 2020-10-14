using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        [Route("Users")]
       // [Authorize]
        public Task<ResultViewModel<PagingResult<User>>> GetUsers(long onlineId = default, int from = default, int size = default, bool getTemplatesData = default, long userId = default, string filterText = default, int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default, int pageSize = default)
        {
          
            return Task.Run(() => _userRepository.GetUsersByFilter(onlineId, from, size, getTemplatesData, userId, filterText, type, withPicture, isAdmin, pageNumber, pageSize));
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
