﻿using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupService(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public async Task<ResultViewModel<PagingResult<UserGroup>>> UserGroups(int userGroupId = default, string token = default)
        {
            return await _userGroupRepository.UserGroups(userGroupId, token);
        }


        public async Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default, string token = default)
        {
            return await _userGroupRepository.GetAccessControlUserGroup(id, token);
        }

        public async Task<ResultViewModel> SyncUserGroupMember(string lstUser = default, string token = default)
        {
            return await _userGroupRepository.SyncUserGroupMember(lstUser, token);
        }

        public async Task<ResultViewModel> AddUserGroup(UserGroupMember userGroupMember = default, string token = default)
        {
            return await _userGroupRepository.AddUserGroup(userGroupMember, token);
        }

        public async Task<ResultViewModel> ModifyUserGroup(UserGroup userGroup = default, string token = default)
        {
            return await _userGroupRepository.ModifyUserGroup(userGroup, token);
        }

        public async Task<ResultViewModel> DeleteUserGroup(int groupId = default, string token = default)
        {
            return await _userGroupRepository.DeleteUserGroup(groupId, token);
        }
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId, string token = default)
        {
            return _userGroupRepository.ModifyUserGroupMember(member, userGroupId, token);
        }


    }
}