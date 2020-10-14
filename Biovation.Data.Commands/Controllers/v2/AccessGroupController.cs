using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Biovation.Data.Commands.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class AccessGroupController : Controller
    {
        private readonly AccessGroupRepository _accessGroupRepository;


        public AccessGroupController(AccessGroupRepository accessGroupRepository)
        {
            _accessGroupRepository = accessGroupRepository;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="accessGroup"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddAccessGroup([FromBody] AccessGroup accessGroup = default)
        {
            return Task.Run(() => _accessGroupRepository.AddAccessGroup(accessGroup));
        }



        [HttpPut]
        [Route("AccessGroup")]
        [Authorize]
        public Task<ResultViewModel> ModifyAccessGroup([FromBody]AccessGroup accessGroup)
        {
            return Task.Run(() => _accessGroupRepository.ModifyAccessGroup(accessGroup));
        }


        [HttpPut]
        [Route("AccessGroupUserGroup")]
        [Authorize]
        public Task<ResultViewModel> ModifyAccessGroupUserGroup(string xmlUserGroup, int accessGroupId)
        {
            return Task.Run(() => _accessGroupRepository.ModifyAccessGroupUserGroup(xmlUserGroup, accessGroupId));
        }


        [HttpPut]
        [Route("AccessGroupAdminUsers")]
        [Authorize]
        public Task<ResultViewModel> ModifyAccessGroupAdminUsers(string xmlAdminUsers, int accessGroupId)
        {
            return Task.Run(() => _accessGroupRepository.ModifyAccessGroupAdminUsers(xmlAdminUsers, accessGroupId));
        }


        [HttpPut]
        [Route("AccessGroupDeviceGroup")]
        [Authorize]
        public Task<ResultViewModel> ModifyAccessGroupDeviceGroup(string xmlDeviceGroup, int accessGroupId)
        {
            return Task.Run(() => _accessGroupRepository.ModifyAccessGroupDeviceGroup(xmlDeviceGroup, accessGroupId));
        }


        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteAccessGroup(int id)
        {
            return Task.Run(() => _accessGroupRepository.DeleteAccessGroup(id));
        }
    }

}

