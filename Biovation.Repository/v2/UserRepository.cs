using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayerCore.Extentions;


namespace Biovation.Repository.SQL.v2
{
    public class UserRepository
    {
        private readonly GenericRepository _repository;

        public UserRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultViewModel ModifyUser(User user)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@FName", user.FirstName),
                new SqlParameter("@LName", user.SurName),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@Active", user.IsActive),
                new SqlParameter("@Type", user.Type),
                new SqlParameter("@AdminLevel", user.AdminLevel),
                new SqlParameter("@AuthMode", user.AuthMode),
                new SqlParameter("@EDate", user.EndDate),
                new SqlParameter("@SDate", user.StartDate),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@TelNumber", user.TelNumber),
                new SqlParameter("@Image", user.Image),
                new SqlParameter("@EntityId", user.EntityId),
                new SqlParameter("@IsAdmin", user.IsAdmin),
            };

            return _repository.ToResultList<ResultViewModel>("ModifyUser", parameters).Data.FirstOrDefault();
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>

        public Task<ResultViewModel<PagingResult<User>>> GetUsersByFilter(long onlineUserId = 0, int from = 0, int size = 0, bool getTemplatesData = true, long userId = default, string filterText = null, int type = default, bool withPicture = true, bool isAdmin = false, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@adminUserID", onlineUserId),
                    new SqlParameter("@from", from),
                    new SqlParameter("@size", size),
                    new SqlParameter("@getTemplatesData", getTemplatesData),
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@WithPicture", withPicture),
                    new SqlParameter("@FilterText", filterText),
                    new SqlParameter("@Type", type),
                    new SqlParameter("@isAdmin",isAdmin),
                    new SqlParameter("@PageNumber", pageNumber),
                    new SqlParameter("@PageSize",pageSize)

                };
                return _repository.ToResultList<PagingResult<User>>("SelectUsersByFilter", parameters, fetchCompositions: true,
                    compositionDepthLevel: 5).FetchFromResultList();
            });
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public ResultViewModel<int> GetUsersCount()
        {
            return _repository.ToResultList<int>("SelectUsersCount").FetchFromResultList();
        }

        /// <summary>
        /// گرفتن لیست ادمین ها
        /// </summary>
        /// <returns></returns>

        public ResultViewModel<List<User>> GetAdminUserOfAccessGroup(long userId = 0, int accessGroupId = 0)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@adminUserID", userId),
                new SqlParameter("@accessGroupId", accessGroupId)

            };
            return _repository.ToResultList<User>("SelectAdminUser", parameters).FetchResultList();
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultViewModel DeleteUser(int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", userId)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteUserByID", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteUsers(List<int> userIds)
        {

            var parameters = new List<SqlParameter> { new SqlParameter("@json", SqlDbType.VarChar) { Value = userIds } };

            return _repository.ToResultList<ResultViewModel>("DeleteUsers", parameters).Data.FirstOrDefault();

        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ResultViewModel ModifyPassword(int userId, string password)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", userId),
                new SqlParameter("@Password", password)
            };
            var result = _repository.ToResultList<ResultViewModel>("ModifyPassword", parameters).Data.FirstOrDefault();
            return result;
        }

        public ResultViewModel DeleteUserGroupsOfUser(int userId, int userTypeId = 1)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@userTypeId", userTypeId)
            };
            var result = _repository.ToResultList<ResultViewModel>("DeleteUserGroupsOfUser", parameters).Data.FirstOrDefault();
            return result;
        }

        public ResultViewModel DeleteUserGroupOfUser(int userId, int userGroupId, int userTypeId = 1)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@userGroupId", userGroupId),
                new SqlParameter("@userTypeId", userTypeId)
            };
            var result = _repository.ToResultList<ResultViewModel>("DeleteUserGroupOfUser", parameters).Data.FirstOrDefault();
            return result;
        }

        public Task<ResultViewModel<List<DeviceBasicInfo>>> GetAuthorizedDevicesOfUser(long userId,int nestingDepthLevel = 4)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", userId)
                };

                return _repository.ToResultList<DeviceBasicInfo>("SelectAuthorizedDevicesOfUser", parameters, fetchCompositions: true, compositionDepthLevel: nestingDepthLevel).FetchResultList();
            });
        }
    }
}