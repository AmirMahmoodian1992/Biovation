using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;

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

        public ResultViewModel<PagingResult<PlateDetectionLog>> GetPlateDetectionLog(int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default, string whereClause = "", string orderByClause = "", string token = default)
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
            restRequest.AddQueryParameter("whereClause", whereClause);
            restRequest.AddQueryParameter("orderByClause", orderByClause);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<PlateDetectionLog>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddLicensePlate(LicensePlate licensePlate, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/PlateDetection", Method.POST);
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



        public ResultViewModel<LicensePlate> GetLicensePlate(string licensePlate, int entityId, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/PlateDetection", Method.GET);

            restRequest.AddQueryParameter("@LicensePlate", licensePlate);
            restRequest.AddQueryParameter("@LicensePlateId", entityId.ToString());
            restRequest.AddQueryParameter("@IntervalBeginningStartTime", TimeSpan.Zero.ToString());
            restRequest.AddQueryParameter("@IntervalEndStartTime", "23:59:59");
            restRequest.AddQueryParameter("@IntervalBeginningFinishTime", TimeSpan.Zero.ToString());
            restRequest.AddQueryParameter("@IntervalEndFinishTime", "23:59:59");

            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<LicensePlate>>(restRequest);
            return requestResult.Result.Data;
        }


        public ResultViewModel<List<LicensePlate>> ReadLicensePlate(string licensePlateNumber, int entityId, bool isActive, DateTime startDate, DateTime endDate, TimeSpan intervalBeginningStartTime, TimeSpan intervalEndStartTime, TimeSpan intervalBeginningFinishTime, TimeSpan intervalEndFinishTime, string token = default)
        {

            var restRequest = new RestRequest("Queries/v2/PlateDetection/LicensePlate/ReadLicensePlate", Method.GET);
            restRequest.AddQueryParameter("@LicensePlate", licensePlateNumber);
            restRequest.AddQueryParameter("@IsActive", isActive.ToString());
            restRequest.AddQueryParameter("@StartDate", startDate.ToString(CultureInfo.InvariantCulture));
            restRequest.AddQueryParameter("@EndDate", endDate.ToString(CultureInfo.InvariantCulture));
            restRequest.AddQueryParameter("@IntervalBeginningStartTime",
                intervalBeginningStartTime != default
                    ? intervalBeginningStartTime.ToString()
                    : TimeSpan.Zero.ToString());
            restRequest.AddQueryParameter("@IntervalEndStartTime",
                intervalEndStartTime != default ? intervalEndStartTime.ToString() : new TimeSpan(23, 59, 59).ToString());
            restRequest.AddQueryParameter("@IntervalBeginningFinishTime",
                intervalBeginningFinishTime != default ? intervalBeginningFinishTime.ToString(
                    ) : TimeSpan.Zero.ToString());
            restRequest.AddQueryParameter("@IntervalEndFinishTime",
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
            restRequest.AddQueryParameter("@modifiedAt", modifiedAt.ToString(CultureInfo.InvariantCulture));
            restRequest.AddQueryParameter("@modifiedBy", modifiedBy);
            restRequest.AddQueryParameter("@action", action);
            restRequest.AddJsonBody(licensePlate);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);

            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

    }
}