using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var result = _plateDetectionRepository.GetLicensePlate(licensePlate, entityId).Result;
            if (result is null)
            {
                return null;
            }

            try
            {
                var str = result.Data.LicensePlateNumber;
                result.Data.LicensePlateNumber =
                    str.Substring(3, 3) + "-" + str.Substring(6, 2) + str.Substring(2, 1) + str.Substring(0, 2);
            }
            catch
            {
                // ignored
            }

            return result;
        }

        public ResultViewModel<PagingResult<ManualPlateDetectionLog>> GetPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default,
            string licensePlate = default, int detectorId = default, DateTime fromDate = default,
            DateTime toDate = default,
            int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false,
            int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return _plateDetectionRepository.GetAllPlateDetectionLog(firstLicensePlatePart, secondLicensePlatePart, thirdLicensePlatePart, fourthLicensePlatePart, logId, licensePlate, detectorId, fromDate, toDate,
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

        public async Task<List<LicensePlate>> ReadLicensePlate(string licensePlateNumber, int entityId, bool isActive, DateTime startDate, DateTime endDate, TimeSpan intervalBeginningStartTime, TimeSpan intervalEndStartTime, TimeSpan intervalBeginningFinishTime, TimeSpan intervalEndFinishTime)
        {
            return await Task.Run(() =>
            {
                var results = _plateDetectionRepository.ReadLicensePlate(licensePlateNumber, entityId, isActive,
                    startDate, endDate, intervalBeginningStartTime, intervalEndStartTime, intervalBeginningFinishTime,
                    intervalEndFinishTime).Data;
                if (results is null)
                    return null;
                foreach (var result in results)
                {
                    try
                    {
                        var str = result.LicensePlateNumber;
                        result.LicensePlateNumber =
                            str.Substring(3, 3) + "-" + str.Substring(6, 2) + str.Substring(2, 1) + str.Substring(0, 2);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                return results;
            });
        }

        public Task<ResultViewModel> DeleteLicensePlate(LicensePlate licensePlate, string modifiedBy = default, string action = default, DateTime? modifiedAt = null)
        {
            return Task.Run(() => _plateDetectionRepository.DeleteLicensePlate(licensePlate, modifiedAt ?? DateTime.Now, modifiedBy, action));
        }
    }
}
