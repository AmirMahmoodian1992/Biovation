using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository;

namespace Biovation.Service.Sql.v1
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<ResultViewModel> ModifyUser(User user)
        {
            return Task.Run(() =>_userRepository.ModifyUser(user));
        }

        public Task<List<User>> GetUsersByFilter(long onlineUserId = 0, int from = 0, int size = 0, bool getTemplatesData = true, long userId = default, string filterText = null, int type = default, bool withPicture = true, bool isAdmin = false)
        {
            return _userRepository.GetUsersByFilter(onlineUserId, from, size, getTemplatesData, userId, filterText, type, withPicture, isAdmin);
        }

        public Task<List<User>> GetUsers(long onlineUserId = 0, int from = 0, int size = 0, bool getTemplatesData = true)
        {
            return Task.Run(async () =>
            {
                if (from != 0 || size != 0)
                    return await _userRepository.GetUsers(onlineUserId, @from, size, getTemplatesData);

                var userCount = _userRepository.GetUsersCount();

                const int groupSize = 100;
                var loopUpperBound = userCount / groupSize;
                loopUpperBound = loopUpperBound == 0 ? 1 : loopUpperBound;
                loopUpperBound = userCount % groupSize <= 0 ? loopUpperBound : loopUpperBound + 1;

                //var users = new List<User>();
                //var result = Parallel.For(0, loopUpperBound, async index =>
                //{
                //    var tempUsers =
                //        await _userRepository.GetUsers(onlineUserId, index * groupSize, groupSize, getTemplatesData);

                //    //lock (users)
                //    users.AddRange(tempUsers);
                //});

                var tasks = Enumerable.Range(0, loopUpperBound)
                    .Select(index => _userRepository.GetUsers(onlineUserId, index * groupSize, groupSize, getTemplatesData));
                var users = (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();

                //var tasks = new List<Task>();
                //for (var i = 0; i < loopUpperBound; i++)
                //{
                //    var index = i;
                //    tasks.Add(Task.Run(async () =>
                //    {
                //        var tempUsers = await _userRepository.GetUsers(onlineUserId, index * groupSize, groupSize, getTemplatesData);

                //        //lock (users)
                //        users.AddRange(tempUsers);
                //    }));
                //}

                //Task.WaitAll(tasks.ToArray());

                return users;
            });
        }

        public int GetUserCount()
        {
            return _userRepository.GetUsersCount();
        }


        public List<User> GetAdminUser(long userId = 0)
        {
            return _userRepository.GetAdminUser(userId);
        }
        public List<User> GetAdminUserOfAccessGroup(long userId = 0, int accessGroupId = 0)
        {
            return _userRepository.GetAdminUserOfAccessGroup(userId, accessGroupId);
        }
        public User GetUser(long userId, bool withPicture = true)
        {
            return _userRepository.GetUser(userId, withPicture);
        }
        public List<User> GetUser(string filterText, int type, long userId)
        {
            return _userRepository.GetUser(filterText, type, userId);
        }
        public List<User> GetUser(string filterText, long userId)
        {
            return _userRepository.GetUser(filterText, userId);
        }

        public List<ResultViewModel> DeleteUser(int[] userIds)
        {
            var result = new List<ResultViewModel>();
            foreach (var id in userIds)
            {
                result.Add(_userRepository.DeleteUser(id));
            }
            return result;
        }

        public ResultViewModel ModifyPassword(int userId, string password)
        {
            return _userRepository.ModifyPassword(userId, password);
        }

        public ResultViewModel DeleteUserGroupsOfUser(int userId, int userTypeId = 1)
        {
            return _userRepository.DeleteUserGroupsOfUser(userId, userTypeId);
        }

        public ResultViewModel DeleteUserGroupOfUser(int userId, int userGroupId, int userTypeId = 1)
        {
            return _userRepository.DeleteUserGroupOfUser(userId, userGroupId, userTypeId);
        }

        public Task<List<DeviceBasicInfo>> GetAuthorizedDevicesOfUser(long userId)
        {
            return _userRepository.GetAuthorizedDevicesOfUser(userId);
        }
    }
}
