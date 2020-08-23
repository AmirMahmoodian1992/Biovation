using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Repository.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/dataFlow/queries/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class DeviceController : Controller
    {
        private readonly DeviceRepository _deviceRepository;


        public DeviceController(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }


        [HttpGet]
        [Route("{id}")]
        public  Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> Devices(long id = 0, long adminUserId = 0, int groupId = 0, uint code = 0,
            int brandId = 0, string name = null, int modelId = 0, int typeId = 0, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _deviceRepository.GetDevices( adminUserId, groupId, code, brandId, name, modelId, typeId,pageNumber,PageSize));
        }

    }
}