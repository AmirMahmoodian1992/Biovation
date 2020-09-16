using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;

namespace Biovation.Repository.SQL.v2
{
    public class UserGroupRepository
    {
        private readonly GenericRepository _repository;

        public UserGroupRepository(GenericRepository repository)
        {
            _repository = repository;
        }
        public Task<ResultViewModel> ModifyUserGroup(UserGroup userGroup)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", userGroup.Id),
                    new SqlParameter("@Name", userGroup.Name ?? string.Empty),
                    new SqlParameter("@AccessGroup", userGroup.AccessGroup ?? string.Empty),
                    new SqlParameter("@Description", userGroup.Description ?? string.Empty),
                    new SqlParameter("@Users", JsonConvert.SerializeObject(userGroup.Users?.Select(user => new User{Id = user.UserId, UserName = user.UserName}) ?? new List<User>()))
                };

                return _repository.ToResultList<ResultViewModel>("ModifyUserGroup", parameters).Data.FirstOrDefault();
            });
        }
        public ResultViewModel AddUserGroupMember(UserGroupMember userMember)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userMember.UserId),
                new SqlParameter("@GroupId", userMember.GroupId),
                new SqlParameter("@UserType", 1),
            };

            return _repository.ToResultList<ResultViewModel>("InsertUserGroupMember", parameters).Data.FirstOrDefault();
        }
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId)
        {
            
            var parameters = new List<SqlParameter> {
                new SqlParameter("@UserGroupMember",JsonConvert.SerializeObject(member)),
                new SqlParameter("@UserGroupId",userGroupId)
            };

            return _repository.ToResultList<ResultViewModel>("ModifyUserGroupMember", parameters).Data.FirstOrDefault();
        }
        public ResultViewModel<PagingResult<UserGroup>> GetUserGroups(int id, long adminUserId, int accessGroupId, long userId, int pageNumber = default,
            int pageSize = default)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@Id",id),
                new SqlParameter("@adminUserId",adminUserId),              
                new SqlParameter("@accessGroupId",accessGroupId ),
                new SqlParameter("@UserId",userId),
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},
            };
            return _repository.ToResultList<PagingResult<UserGroup>>("SelectUserGroups", parameters, fetchCompositions: true,
                    compositionDepthLevel: 5).FetchFromResultList();
        }
        public ResultViewModel<List<UserGroup>> GetAccessControlUserGroup(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@AccessControlId", id)
            };

            return _repository.ToResultList<UserGroup>("SelectAccessControlUserGroup", parameters, fetchCompositions: true,
                    compositionDepthLevel: 5).FetchResultList();
        }
        public ResultViewModel DeleteUserGroup(int userGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", userGroupId)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteUserGroup", parameters).Data.FirstOrDefault();
        }
        public ResultViewModel SyncUserGroupMember(string lstUser,int id,int adminUserId, int deviceGroupId)
        {        
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", lstUser),
                new SqlParameter("@Id", id),
                new SqlParameter("@adminUserId", adminUserId),
                new SqlParameter("@deviceGroupId", deviceGroupId)
            };

            return _repository.ToResultList<ResultViewModel>("SelectSearchAccessGroup", parameters, fetchCompositions: true,
                    compositionDepthLevel: 5).Data.FirstOrDefault();
        }

    }
}
