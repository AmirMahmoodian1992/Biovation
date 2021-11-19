using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

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
            //restRequest.AddQueryParameter("IntervalBeginningStartTime", TimeSpan.Zero.ToString());
            //restRequest.AddQueryParameter("IntervalEndStartTime", "23:59:59");
            //restRequest.AddQueryParameter("IntervalBeginningFinishTime", TimeSpan.Zero.ToString());
            //restRequest.AddQueryParameter("IntervalEndFinishTime", "23:59:59");
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<LicensePlate>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetAllPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default, string whereClause = "", string orderByClause = "", string token = default)
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
            restRequest.AddQueryParameter("whereClause", whereClause);
            restRequest.AddQueryParameter("orderByClause", orderByClause);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<ManualPlateDetectionLog>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetCameraPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default, string whereClause = "", string orderByClause = "", string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection/CameraPlateDetectionLog", Method.GET);
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
            restRequest.AddQueryParameter("whereClause", whereClause);
            restRequest.AddQueryParameter("orderByClause", orderByClause);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<ManualPlateDetectionLog>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetManualPlateDetectionLog(int logId = default, long userId = default, long parentLogId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
           int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
           int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection/ManualPlateDetectionLog", Method.GET);
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
            var restRequest = new RestRequest("Commands/v2/PlateDetection", Method.POST);
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

        public ResultViewModel<List<LicensePlate>> ReadLicensePlate(string licensePlateNumber, int entityId, bool isActive, DateTime startDate, DateTime endDate, TimeSpan intervalBeginningStartTime, TimeSpan intervalEndStartTime, TimeSpan intervalBeginningFinishTime, TimeSpan intervalEndFinishTime, string token = default)
        {

            var restRequest = new RestRequest("Queries/v2/PlateDetection/LicensePlate/ReadLicensePlate", Method.GET);
            restRequest.AddQueryParameter("LicensePlate", licensePlateNumber);
            restRequest.AddQueryParameter("IsActive", isActive.ToString());
            restRequest.AddQueryParameter("StartDate", startDate.ToString(CultureInfo.InvariantCulture));
            restRequest.AddQueryParameter("EndDate", endDate.ToString(CultureInfo.InvariantCulture));
            restRequest.AddQueryParameter("IntervalBeginningStartTime",
                intervalBeginningStartTime != default
                    ? intervalBeginningStartTime.ToString()
                    : TimeSpan.Zero.ToString());
            restRequest.AddQueryParameter("IntervalEndStartTime",
                intervalEndStartTime != default ? intervalEndStartTime.ToString() : new TimeSpan(23, 59, 59).ToString());
            restRequest.AddQueryParameter("IntervalBeginningFinishTime",
                intervalBeginningFinishTime != default ? intervalBeginningFinishTime.ToString(
                    ) : TimeSpan.Zero.ToString());
            restRequest.AddQueryParameter("IntervalEndFinishTime",
                intervalEndFinishTime != default
                    ? intervalEndStartTime.ToString()
                    : new TimeSpan(23, 59, 59).ToString());

            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);

            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<LicensePlate>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteLicensePlate(LicensePlate licensePlate, DateTime modifiedAt, string modifiedBy, string action, string token = default)
        {

            var restRequest = new RestRequest("Commands/v2/PlateDetection/", Method.DELETE);
            restRequest.AddQueryParameter("modifiedAt", modifiedAt.ToString(CultureInfo.InvariantCulture));
            restRequest.AddQueryParameter("modifiedBy", modifiedBy);
            restRequest.AddQueryParameter("action", action);
            restRequest.AddJsonBody(licensePlate);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);

            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public async Task<ResultViewModel> AddManualPlateDetectionLog(PlateDetectionLog log, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/PlateDetection/ManualPlateDetectionLog", Method.POST);
            restRequest.AddJsonBody(log);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> AddManualPlateDetectionLogOfExistLog(ManualPlateDetectionLog log, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/PlateDetection/{parentLogId}/ManualPlateDetectionLog", Method.PUT);
            restRequest.AddUrlSegment("parentLogId", log.ParentLog?.Id ?? 0);
            restRequest.AddJsonBody(log);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PlateDetectionLog>> SelectPreviousPlateDetectionLog(int id = default, string licensePlateNumber = default, DateTime? logDateTime = null, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection/PreviousPlateDetectionLog", Method.GET);
            restRequest.AddQueryParameter(nameof(id), id.ToString());
            if (licensePlateNumber != null)
                restRequest.AddQueryParameter(nameof(licensePlateNumber), licensePlateNumber);
            if (logDateTime.HasValue)
                restRequest.AddQueryParameter(nameof(logDateTime), logDateTime.Value.ToString(CultureInfo.CurrentCulture));
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PlateDetectionLog>>(restRequest);
            return requestResult.Data;
        }
    }
}