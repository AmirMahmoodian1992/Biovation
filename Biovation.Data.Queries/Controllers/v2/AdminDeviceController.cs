using System;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("2.0")]
    public class AdminDeviceController : Controller
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly AdminDeviceRepository _adminDeviceRepository;

        public AdminDeviceController(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }

        //public AdminDeviceController()
        //{
        //    //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        //}

        [HttpGet]
        [Route("GetAdminDevicesByPersonId")]
        public Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDevicesByPersonId(int personId, int pageNumber = default,int PageSize = default)
        {
              return Task.Run(() => _adminDeviceRepository.GetAdminDeviceGroupsByUserId(personId,pageNumber,PageSize));         
        }
        [HttpGet]
        [Route("GetAdminDevicesByUserId")]
        public Task<ResultViewModel<PagingResult<AdminDevice>>> GetAdminDevicesByUserId(int userId, int pageNumber = 0, int PageSize = 0)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDevicesByUserId(userId, pageNumber, PageSize));
        }

    [HttpPost]
    [Route("ModifyAdminDevice")]
    public ResultViewModel ModifyAdminDevice([FromBody] JObject adminDevice)
    {
        try
        {
            var ss = adminDevice.ToString();
            ss = ss.Replace("]}\"", "]}");
            ss = ss.Replace("\"{", "{");
            ss = ss.Replace("\r\n", "");
            ss = ss.Replace(@"\", "");

            string node = JsonConvert.DeserializeXNode(ss, "Root")?.ToString();
            var result = _adminDeviceRepository.ModifyAdminDevice(node);
            return result;
        }
        catch (Exception e)
        {
            return new ResultViewModel { Message = e.Message, Validate = 0 };
        }
    }
}
}