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

        public async Task<ResultViewModel<PagingResult<PlateDetectionLog>>> GetPlateDetectionLog(string firstLicensePLatePart = default, string secondLicensePLatePart = default, string thirdLicensePLatePart = default, string fourthLicensePLatePart = default, int logId = default,
            string licensePlate = default, int detectorId = default, DateTime fromDate = default,
            DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false,
            int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return await _plateDetectionRepository.GetPlateDetectionLog(firstLicensePLatePart, secondLicensePLatePart, thirdLicensePLatePart, fourthLicensePLatePart, logId, licensePlate, detectorId, fromDate, toDate,
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
    }
}
