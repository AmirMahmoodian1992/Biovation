using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Service
{
    public class PlateDetectionService
    {
        private readonly PlateDetectionRepository _plateDetectionRepository;

        public PlateDetectionService(PlateDetectionRepository plateDetectionRepository)
        {
            _plateDetectionRepository = plateDetectionRepository;
        }

        public Task<List<PlateDetectionLog>> GetPlateDetectionLog(int logId = default, string licensePlate = default,
            int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0,
            int maxPrecision = 0, bool withPic = true, bool successTransfer = false)
        {
            return Task.Run(() => _plateDetectionRepository.GetPlateDetectionLog(logId, licensePlate, detectorId,
                fromDate, toDate, minPrecision, maxPrecision, withPic, successTransfer));
        }

        public Task<ResultViewModel> AddPlateDetectionLog(PlateDetectionLog log)
        {
            return Task.Run(() => { return Task.Run(() => _plateDetectionRepository.AddPlateDetectionLog(log)); });

        }

        public Task<ResultViewModel> AddLicensePlate(LicensePlate licensePlate)
        {
            return Task.Run(() => { return Task.Run(() => _plateDetectionRepository.AddLicensePlate(licensePlate)); });
        }
        public Task<LicensePlate> GetLicensePlate(string licensePlate = default, int entityId = default)
        {
            return Task.Run(() => { return Task.Run(() => _plateDetectionRepository.GetLicensePlate(licensePlate, entityId)); });
        }

    }
}