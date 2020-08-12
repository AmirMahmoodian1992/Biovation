using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Gateway.Controllers.v2
{
    [Route("biovation/api/[controller]")]
    public class BlackListController : Controller
    {

        private readonly BlackListService _blackListService;
        private readonly RestClient _restClient;

        public BlackListController(BlackListService blackListService)
        {
            _blackListService = blackListService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpPost]
        public Task<IActionResult> CreateBlackList([FromBody]List<BlackList> blackLists = default)
        {

            throw null;

        }



        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetBlackList(int id = default, int userid = default, int deviceId = default, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default)
        {
            throw null;
        }

        [HttpPut]
        public Task<IActionResult> ChangeBlackList([FromBody]BlackList blackList = default)
        {
            throw null;
        }
        [HttpDelete]
        [Route("{id}")]
        public Task<IActionResult> DeleteBlackList(int id = default)
        {
            throw null;
        }


    }
}
