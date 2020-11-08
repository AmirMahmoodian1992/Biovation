using System;
using System.Globalization;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class PlateDetectionRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public PlateDetectionRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<LicensePlate> GetLicensePlate(string licensePlate, int entityId)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection/LicensePlate", Method.GET);
            restRequest.AddQueryParameter("licensePlate", licensePlate);
            restRequest.AddQueryParameter("entityId", entityId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<LicensePlate>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel<PagingResult<PlateDetectionLog>> GetPlateDetectionLog(int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection/PlateDetectionLog", Method.GET);
            restRequest.AddQueryParameter("logId", logId.ToString());
            restRequest.AddQueryParameter("licensePlate", licensePlate ?? string.Empty);
            restRequest.AddQueryParameter("detectorId", detectorId.ToString());
            restRequest.AddQueryParameter("fromDate", fromDate.ToString(CultureInfo.InvariantCulture));
            restRequest.AddQueryParameter("toDate", toDate.ToString(CultureInfo.InvariantCulture));
            restRequest.AddQueryParameter("minPrecision", minPrecision.ToString());
            restRequest.AddQueryParameter("maxPrecision", maxPrecision.ToString());
            restRequest.AddQueryParameter("withPic", withPic.ToString());
            restRequest.AddQueryParameter("successTransfer", successTransfer.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<PlateDetectionLog>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddLicensePlate(LicensePlate licensePlate, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/PlateDetection/LicensePlate", Method.POST);
            restRequest.AddJsonBody(licensePlate);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddPlateDetectionLog(PlateDetectionLog log, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/PlateDetection/PlateDetectionLog", Method.POST);
            restRequest.AddJsonBody(log);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}