using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;

namespace Biovation.Service.Api.v1
{
    public class PlateDetectionService
    {
        private readonly PlateDetectionRepository _plateDetectionRepository;

        public PlateDetectionService(PlateDetectionRepository plateDetectionRepository)
        {
            _plateDetectionRepository = plateDetectionRepository;
        }

        public ResultViewModel<LicensePlate> GetLicensePlate(string licensePlate = default, int entityId = default)
        {
            return _plateDetectionRepository.GetLicensePlate(licensePlate, entityId).Result;
        }

        public ResultViewModel<PagingResult<PlateDetectionLog>> GetPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default,
            string licensePlate = default, int detectorId = default, DateTime fromDate = default,
            DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false,
            int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return _plateDetectionRepository.GetPlateDetectionLog(firstLicensePlatePart,secondLicensePlatePart,thirdLicensePlatePart,fourthLicensePlatePart,logId, licensePlate, detectorId, fromDate, toDate,
                minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, token).Result;
        }

        public ResultViewModel AddLicensePlate(LicensePlate licensePlate, string token = default)
        {
            return _plateDetectionRepository.AddLicensePlate(licensePlate, token).Result;
        }

        public ResultViewModel AddPlateDetectionLog(PlateDetectionLog log, string token = default)
        {
            return _plateDetectionRepository.AddPlateDetectionLog(log, token).Result;
        }
    }
}
