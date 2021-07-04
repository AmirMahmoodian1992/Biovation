using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class PlateDetectionService
    {
        private readonly PlateDetectionRepository _plateDetectionRepository;

        public PlateDetectionService(PlateDetectionRepository plateDetectionRepository)
        {
            _plateDetectionRepository = plateDetectionRepository;
        }

        public async Task<ResultViewModel<LicensePlate>> GetLicensePlate(string licensePlate, int entityId, string token = default)
        {
            return await _plateDetectionRepository.GetLicensePlate(licensePlate, entityId, token);
        }

        public async Task<ResultViewModel<PagingResult<PlateDetectionLog>>> GetPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default,
            string licensePlate = default, int detectorId = default, DateTime fromDate = default,
            DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false,
            int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return await _plateDetectionRepository.GetPlateDetectionLog(firstLicensePlatePart, secondLicensePlatePart, thirdLicensePlatePart, fourthLicensePlatePart, logId, licensePlate, detectorId, fromDate, toDate,
                minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel<PagingResult<ManualPlateDetectionLog>>> GetManualPlateDetectionLog(int logId = default, long userId = default, long parentLogId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return await _plateDetectionRepository.GetManualPlateDetectionLog(logId, userId, parentLogId, licensePlate, detectorId, fromDate, toDate,
                minPrecision, maxPrecision, withPic, successTransfer, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel> AddLicensePlate(LicensePlate licensePlate, string token = default)
        {
            return await _plateDetectionRepository.AddLicensePlate(licensePlate, token);
        }

        public async Task<ResultViewModel> AddPlateDetectionLog(PlateDetectionLog log, string token = default)
        {
            return await _plateDetectionRepository.AddPlateDetectionLog(log, token);
        }
        public async Task<ResultViewModel> AddManualPlateDetectionLog(ManualPlateDetectionLog log, string token = default)
        {
            return await _plateDetectionRepository.AddManualPlateDetectionLog(log, token);
        }

        public async Task<ResultViewModel<PlateDetectionLog>> SelectPreviousPlateDetectionLog(int id = default, string licensePlateNumber = default, DateTime? logDateTime = null, string token = default)
        {
            return await _plateDetectionRepository.SelectPreviousPlateDetectionLog(id, licensePlateNumber, logDateTime);
        }
    }
}
