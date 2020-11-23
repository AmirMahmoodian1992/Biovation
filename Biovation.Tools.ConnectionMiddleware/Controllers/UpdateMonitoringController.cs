using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Tools.ConnectionMiddleware.Controllers
{
    [ApiController]
    [Route("biovation/api/[controller]")]
    public class UpdateMonitoringController : ControllerBase
    {
        private readonly RestClient _restClient;

        [HttpPost]
        [Route("UpdateAttendance")]
        public async Task<ResultViewModel> UpdateAttendance(Log log)
        {
            var restRequest = new RestRequest("UpdateAttendance/UpdateAttendance", Method.POST);
            restRequest.AddJsonBody(log);

            var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return new ResultViewModel();
        }
    }
}
