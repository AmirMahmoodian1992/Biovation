using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly UserGroupService _userGroupService;

        private readonly RestClient _restClient;

        public UserController(UserService userService, DeviceService deviceService, UserGroupService userGroupService)
        {
            _userService = userService;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }
        [HttpGet]
        public Task<ResultViewModel<PagingResult<User>>> GetUsersByFilter(long onlineId = default, int from = default,
            int size = default, bool getTemplatesData = default, long userId = default, string filterText = default,
            int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(async () => { return _userService.GetUsers(onlineId, from, size, getTemplatesData, userId, filterText, type,
                withPicture, isAdmin, pageNumber, pageSize); });
        }


        [HttpPost]
        public Task<IActionResult> AddUser([FromBody] User user)
        {
            throw null;
        }

        [HttpPut]
        public Task<IActionResult> ModifyUser([FromBody] User user)
        {
            throw null;
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<IActionResult> DeleteUser(int id = default)
        {
            throw null;
        }


        //batch delete
        [HttpPost]
        [Route("/DeleteUser")]
        public Task<IActionResult> DeleteUser([FromBody]List<int> ids = default)
        {
            throw null;
        }


        //if deviceId == 0 then send ids to all of device
        [HttpPut]
        [Route("UsersToDevice/{deviceId}")]
        public Task<IActionResult> SendUsersToDevice([FromBody]int[] ids, string deviceId = default)
        {
            throw null;
        }


        [HttpPatch]
        [Route("Password/{id}")]
        public Task<IActionResult> ModifyPassword(int id = default, string password = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("AdminUserOfAccessGroup")]
        public Task<ResultViewModel<List<User>>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default)
        {
            return Task.Run(async () => { return _userService.GetAdminUserOfAccessGroup(id,accessGroupId); });
        }

        ///// <param name="updateUsers">لیست افرادی که تغییر کرده و در گروه بایویی هم حضور دارند و باید به دستگاههای جدید ارسال شوند</param>
        ///// <param name="changeUsers">لیست افرادی که تغییر کرده اند و باید از روی دستگاهها پاک شوند</param>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeviceAndAccessGroup")]
        public Task<IActionResult> SyncDeviceAndAccessGroup([FromBody] ParamViewModel param)
        {
            throw null;
        }

        //in chikar mikone???
        private Task<IActionResult> Sync(long[] usersToSync = default, string updateUsers = default)
        {
            throw null;
        }

        //[HttpPost]
        //private bool Sync(long[] usersList)
        //{
        //    var deviceBrands = _deviceService.GetDeviceBrands();
        //    try
        //    {
        //        foreach (var brand in deviceBrands)
        //        {
        //            _communicationManager.CallRest(
        //                        $"/biovation/api/{brand.Name}/{brand.Name}User/DeleteUserFromAllTerminal", "Post", null, $"{JsonConvert.SerializeObject(usersList)}");
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Log(exception, "Delete User From Device");
        //        return false;
        //    }

        //    if (string.IsNullOrEmpty(updateUsers.Replace("<Root/>", "")))
        //    {
        //        return true;
        //    }
        //    //var xmlDoc = new XmlDocument();
        //    //xmlDoc.LoadXml(updateUsers);
        //    //var fromXml = JsonConvert.SerializeXmlNode(xmlDoc);
        //    //var fromJson = JsonConvert.DeserializeObject<List<User>>(fromXml);
        //    List<UserGroupMember> lstUserGroupMember;
        //    try
        //    {
        //        var serializer = new XmlSerializer(typeof(List<UserGroupMember>), new XmlRootAttribute("Root"));
        //        var stringReader = new StringReader(updateUsers);
        //        lstUserGroupMember = (List<UserGroupMember>)serializer.Deserialize(stringReader);
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Log(exception, "Error on serialize users");
        //        return false;
        //    }

        //    var resultUpdate = UpdateUserGroupMember(usersList, lstUserGroupMember);
        //    if (!resultUpdate)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        Task.Run(() =>
        //        {
        //            //updateUsers = updateUsers.Trim(',');
        //            //var lstupdateUsers = updateUsers.Split(',').Select(s => Convert.ToInt64(s)).ToArray();
        //            var service = new AccessGroupService();
        //            var count = lstUserGroupMember.Count();
        //            for (var i = 0; i < count; i++)
        //            {
        //                var accessGroups = service.GetAccessGroupsOfUser(lstUserGroupMember[i].UserId);
        //                foreach (var accessGroup in accessGroups)
        //                {
        //                    if (accessGroup.DeviceGroup == null)
        //                    {
        //                        Logger.Log($"Not a standard access group, [{accessGroup.Id}].\n", "The access group does not have any device group.");
        //                        continue;
        //                    }
        //                    foreach (var deviceGroup in accessGroup.DeviceGroup)
        //                    {
        //                        if (deviceGroup.Devices == null)
        //                        {
        //                            continue;
        //                        }
        //                        foreach (var device in deviceGroup.Devices)
        //                        {
        //                            var deviceBrand = deviceBrands.FirstOrDefault(devBrand => devBrand.Id == device.BrandId);
        //                            var parameters = new List<object> { $"code={device.Code}", $"userId=[{lstUserGroupMember[i].UserId}]", };
        //                            _communicationManager.CallRest(
        //                                $"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}User/SendUserToDevice", "Get", parameters, null);
        //                        }
        //                    }
        //                }
        //            }
        //        });

        //        return true;

        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Log(exception, "Add User To Devices");
        //        return false;
        //    }
        //}

        [HttpPost]
        [Route("FaceTemplate/{id}")]
        public Task<IActionResult> EnrollFaceTemplate(int id =default, int deviceId = default)
        {
            throw null;
        }

        [HttpPatch]
        [Route("UserGroupsOfUsers")]
        public Task<IActionResult> UpdateUserGroupsOfUser([FromBody]List<int> usersGroupIds = default, long id = default, bool sendUsersToDevice = default)
        {
            throw null;
        }
    }

    public class ParamViewModel
    {
        public string UpdateUsers { get; set; }
        public string ChangeUsers { get; set; }

        public string GetJson()
        {
            var model = new ParamViewModel
            {
                UpdateUsers = "<Root><UserGroupMember></UserGroupMember><UserGroupMember></UserGroupMember></Root>",
                ChangeUsers = "941364,951444,961515"
            };
            return JsonConvert.SerializeObject(model);
        }
    }
}
