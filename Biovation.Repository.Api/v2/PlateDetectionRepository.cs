using System;
using System.Globalization;
using System.Threading.Tasks;
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

        public async Task<ResultViewModel<LicensePlate>> GetLicensePlate(string licensePlate, int entityId, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection/LicensePlate", Method.GET);
            restRequest.AddQueryParameter("licensePlate", licensePlate);
            restRequest.AddQueryParameter("entityId", entityId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<LicensePlate>>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel<PagingResult<PlateDetectionLog>>> GetPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection", Method.GET);
            restRequest.AddQueryParameter(nameof(firstLicensePlatePart), firstLicensePlatePart ?? string.Empty);
            restRequest.AddQueryParameter(nameof(secondLicensePlatePart), secondLicensePlatePart ?? string.Empty);
            restRequest.AddQueryParameter(nameof(thirdLicensePlatePart), thirdLicensePlatePart ?? string.Empty);
            restRequest.AddQueryParameter(nameof(fourthLicensePlatePart), fourthLicensePlatePart ?? string.Empty);
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
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<PlateDetectionLog>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetManualPlateDetectionLog( int logId = default, long userId = default, long parentLogId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
           int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
           int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection", Method.GET);
            restRequest.AddQueryParameter("logId", logId.ToString());
            restRequest.AddQueryParameter(nameof(userId), userId.ToString());
            restRequest.AddQueryParameter(nameof(parentLogId), parentLogId.ToString());
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
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<ManualPlateDetectionLog>>>(restRequest);
            return requestResult.Data;
        }


        public async Task<ResultViewModel> AddLicensePlate(LicensePlate licensePlate, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/PlateDetection/LicensePlate", Method.POST);
            restRequest.AddJsonBody(licensePlate);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> AddPlateDetectionLog(PlateDetectionLog log, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/PlateDetection/PlateDetectionLog", Method.POST);
            restRequest.AddJsonBody(log);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}