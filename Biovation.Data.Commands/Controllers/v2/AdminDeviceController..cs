using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/queries/v2/[controller]")]
    public class AdminDeviceController : Controller
    {
        
        private readonly AdminDeviceRepository _adminDeviceRepository;


        public AdminDeviceController(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }


        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک دستگاه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="userId">کد دستگاه</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// [HttpGet]
        [HttpGet]
        [Route("GetAdminDeviceGroupsByUserId")]
        public Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDeviceGroupsByUserId(int userId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDeviceGroupsByUserId(userId, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("GetAdminDevicesByUserId")]
        public Task<ResultViewModel<PagingResult<AdminDevice>>> GetAdminDevicesByUserId(int userId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDevicesByUserId(userId, pageNumber, pageSize));
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک دستگاه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="adminDevice">کد دستگاه</param>
        /// <returns></returns>
        public Task<ResultViewModel> ModifyAdminDevice(string adminDevice)
        {

            return Task.Run(() => _adminDeviceRepository.ModifyAdminDevice(adminDevice));

        }

    }
}
