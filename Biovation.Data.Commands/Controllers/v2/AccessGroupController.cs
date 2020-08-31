using Biovation.Domain;
using Biovation.Repository.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Biovation.Data.Commands.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Microsoft.AspNetCore.Mvc.Route("biovation/api/queries/v2/[controller]")]
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

        [HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("ModifyAccessGroup")]
        public Task<ResultViewModel> ModifyAccessGroup(AccessGroup accessGroup)
        {
            return Task.Run(() => _accessGroupRepository.ModifyAccessGroup(accessGroup));
        }

        /// <summary>
        /// <En></En>
        /// <Fa>ذخیره جدول کاربران گروه دسترسی</Fa>
        /// </summary>
        /// <param name="xmlUserGroup">رشته آبجکت</param>
        /// <param name="accessGroupId">شناهس گرئه دسترسی</param>
        /// <returns></returns>
        /// 

        [HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("ModifyAccessGroupUserGroup")]
        public Task<ResultViewModel> ModifyAccessGroupUserGroup(string xmlUserGroup, int accessGroupId)
        {
            return Task.Run(() => _accessGroupRepository.ModifyAccessGroupUserGroup(xmlUserGroup, accessGroupId));
        }

        /// <summary>
        /// <En></En>
        /// <Fa>ذخیره ادمین های گروه دسترسی</Fa>
        /// </summary>
        /// <param name="xmlAdminUsers">رشته آبجکت</param>
        /// <param name="accessGroupId">شناهس گرئه دسترسی</param>
        /// <returns></returns>
        [HttpPut]
        [Route("ModifyAccessGroupAdminUsers")]
        public Task<ResultViewModel> ModifyAccessGroupAdminUsers(string xmlAdminUsers, int accessGroupId)
        {
            return Task.Run(() => _accessGroupRepository.ModifyAccessGroupAdminUsers(xmlAdminUsers, accessGroupId));
        }

        /// <summary>
        /// <En></En>
        /// <Fa>ذخیره جدول دستگاه گروه دسترسی</Fa>
        /// </summary>
        /// <param name="xmlDeviceGroup">رشته آبجکت</param>
        /// <param name="accessGroupId">شناسه گروه دسترسی</param>
        /// <returns></returns>
        [HttpPut]
        [Route("ModifyAccessGroupDeviceGroup")]
        public Task<ResultViewModel> ModifyAccessGroupDeviceGroup(string xmlDeviceGroup, int accessGroupId)
        {
            return Task.Run(() => _accessGroupRepository.ModifyAccessGroupDeviceGroup(xmlDeviceGroup, accessGroupId));
        }


        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteAccessGroup(int id)
        {
            return Task.Run(() => _accessGroupRepository.DeleteAccessGroup(id));
        }
    }

}

