﻿using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/queries/v2/[controller]")]

    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        [Route("GetUsers")]
        public Task<ResultViewModel<PagingResult<User>>> GetUsers(long onlineId = default, int from = default, int size = default, bool getTemplatesData = default, long userId = default, string filterText = default, int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _userRepository.GetUsersByFilter(onlineId, from, size, getTemplatesData, userId, filterText, type, withPicture, isAdmin, pageNumber, PageSize));
        }

        [HttpGet]
        [Route("AdminUserOfAccessGroup")]
        public Task<ResultViewModel<List<User>>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default)
        {
            return Task.Run(() => _userRepository.GetAdminUserOfAccessGroup(id, accessGroupId));
        }

        [HttpGet]
        [Route("GetUsersCount")]
        public Task<ResultViewModel<int>> GetUsers()
        {
            return Task.Run(() => _userRepository.GetUsersCount());
        }


        [HttpGet]
        [Route("GetAuthorizedDevicesOfUser")]
        public Task<ResultViewModel<List<DeviceBasicInfo>>> GetAuthorizedDevicesOfUser(int userId)
        {
            return Task.Run(() => _userRepository.GetAuthorizedDevicesOfUser(userId));
        }

    }
}
