﻿using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class UserGroupController : Controller
    {

        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupController(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        [HttpGet]
        [Route("UsersGroup")]
        public Task<ResultViewModel<PagingResult<UserGroup>>> UsersGroup(int id, long userId, int accessGroupId, long adminUserId = default, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _userGroupRepository.GetUserGroups(id, adminUserId, accessGroupId, userId,pageNumber,pageSize));
        }

        [HttpGet]
        [Route("AccessControlUserGroup/{id}")]
        public Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default)
        {
            return Task.Run(() => _userGroupRepository.GetAccessControlUserGroup(id));
        }

        [HttpGet]
        [Route("SyncUserGroupMember")]
        public Task<ResultViewModel> SyncUserGroupMember(string lstUser,int id, int adminUserId, int deviceGroupId)
        {
            return Task.Run(() => _userGroupRepository.SyncUserGroupMember(lstUser,id,adminUserId,deviceGroupId));
        }
    }
}