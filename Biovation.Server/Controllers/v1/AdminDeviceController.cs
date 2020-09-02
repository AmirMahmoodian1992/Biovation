using System;
using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AdminDeviceController : Controller
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly RestClient _restClient;

        public AdminDeviceController(RestClient restClient)
        {
            _restClient = restClient;
        }

        //public AdminDeviceController()
        //{
        //    //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        //}

        [HttpGet, Route("GetAdminDevicesByPersonId")]
        public List<AdminDeviceGroup> GetAdminDevicesByPersonId(int personId)
        {
            var restRequest = new RestRequest($"Queries/v2/AdminDevice/AdminDevicesByPersonId", Method.GET);
            restRequest.AddQueryParameter("personId", personId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AdminDeviceGroup>>>(restRequest);
            return requestResult.Result.Data.Data.Data;
        }

        [HttpPost, Route("ModifyAdminDevice")]
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


                var restRequest = new RestRequest($"Queries/v2/AdminDevice/ModifyAdminDevice", Method.GET);
                restRequest.AddJsonBody("Admin", node);
                var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                return requestResult.Result.Data;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Validate = 0 };
            }
        }
    }
}