using Biovation.CommonClasses.Models;
using DataAccessLayer;
using DataAccessLayer.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Repository
{
    public class UserGroupRepository : Repository<User>
    {
        private readonly GenericRepository _repository;

        public UserGroupRepository()
        {
            _repository = new GenericRepository();
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
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
        public ResultViewModel ModifyUserGroupMember(string node, int userGroupId)
        {

            var parameters = new List<SqlParameter> {
                new SqlParameter("@UserGroupMember",node),
                new SqlParameter("@UserGroupId",userGroupId)
            };

            return _repository.ToResultList<ResultViewModel>("ModifyUserGroupMember", parameters).Data.FirstOrDefault();
        }
        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public List<UserGroup> GetUserGroups(long userId)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@adminUserId",userId)
            };
            return _repository.ToResultList<UserGroup>("SelectUserGroups", parameters, fetchCompositions: true).Data;
        }

        public List<UserGroup> GetUserGroupsOfUser(long userId)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@UserId",userId)
            };
            return _repository.ToResultList<UserGroup>("SelectUserGroupsByUserId", parameters).Data;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public UserGroup GetUserGroup(int userGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", userGroupId)
            };

            return _repository.ToResultList<UserGroup>("SelectUserGroupByID", parameters, fetchCompositions: true).Data.FirstOrDefault();
        }

        public List<UserGroup> GetAccessControlUserGroup(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@AccessControlId", id)
            };

            return _repository.ToResultList<UserGroup>("SelectAccessControlUserGroup", parameters, fetchCompositions: true).Data;
        }


        public ResultViewModel DeleteUserGroup(int userGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", userGroupId)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteUserGroup", parameters).Data.FirstOrDefault();
        }

        public List<UserGroup> GetUserGroupsByAccessGroup(int accessGroupId)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@AccessGroupId", accessGroupId)
                };

            return _repository.ToResultList<UserGroup>("SelectUserGroupsByAccessGroupId", parameters, fetchCompositions: true).Data;
        }

        public ResultViewModel SyncUserGroupMember(string lstUser)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", lstUser)
            };

            return _repository.ToResultList<ResultViewModel>("SelectSearchAccessGroup", parameters).Data.FirstOrDefault();
        }

    }
}
