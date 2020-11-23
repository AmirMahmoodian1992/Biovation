using Biovation.Domain;
using Biovation.Tools.ConnectionMiddleware.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace Biovation.Tools.ConnectionMiddleware.Controllers
{
    [ApiController]
    [Route("biovation/api/[controller]")]
    public class UpdateMonitoringController : ControllerBase
    {
        private readonly RestClient _restClient;

        public UpdateMonitoringController(RestClient restClient)
        {
            _restClient = restClient;
        }

        [HttpPost]
        [Route("UpdateAttendance")]
        public async Task<ResultViewModel> UpdateAttendance(Log log)
        {
            var restRequest = new RestRequest("UpdateAttendance/UpdateAttendance", Method.POST);
            var compatibleModel = new AttendanceV9(log);
            restRequest.AddJsonBody(compatibleModel);

            var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return result.IsSuccessful && result.StatusCode == HttpStatusCode.OK ? result.Data ?? new ResultViewModel { Success = false, Message = result.Content } : new ResultViewModel { Success = false, Message = result.Content };
        }
    }
}
