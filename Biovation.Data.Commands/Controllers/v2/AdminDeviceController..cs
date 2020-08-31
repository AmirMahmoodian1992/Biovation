using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;

namespace Biovation.Data.Queries.Controllers.v2
{
    public class AdminDeviceController : Controller
    {
        [Route("biovation/api/queries/v2/[controller]")]
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
        /// <returns></returns>
        /// [HttpGet]
        [HttpGet]
        [Route("GetAdminDeviceGroupsByUserId")]
        public Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDeviceGroupsByUserId(int userId, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDeviceGroupsByUserId(userId, pageNumber, PageSize));
        }

        [HttpGet]
        [Route("GetAdminDevicesByUserId")]
        public Task<List<AdminDevice>> GetAdminDevicesByUserId(int userId, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDevicesByUserId(userId, pageNumber, PageSize));
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
