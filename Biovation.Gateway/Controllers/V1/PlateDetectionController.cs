using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using RestSharp;

namespace Biovation.WebService.APIControllers
{
    public class PlateDetectionController : ApiController
    {
        private readonly PlateDetectionService _plateDetectionService = new PlateDetectionService();
        private readonly RestClient _restClient;

        public PlateDetectionController()
        {
            _restClient = (RestClient)new RestClient($"http://localhost:{ConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpPost]
        public Task<ResultViewModel> AddLicensePlate(LicensePlate licensePlate)
        {
            return Task.Run(async () =>
            {
                var result = _plateDetectionService.AddLicensePlate(licensePlate).Result;
                if (result.Validate != 1) return result;
                return result;
            });
        }

    }
}
