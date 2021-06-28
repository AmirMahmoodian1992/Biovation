﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v2
{
    public class PlateDetectionRepository
    {
        private readonly GenericRepository _repository;

        public PlateDetectionRepository(GenericRepository repository)
        {
            _repository = repository;
        }
        public ResultViewModel<PagingResult<PlateDetectionLog>> GetPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default,int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
        int pageSize = default)
        {
            var parameters = new List<SqlParameter>
                {
                     new SqlParameter("@" + nameof(firstLicensePlatePart),SqlDbType.NVarChar){Value = firstLicensePlatePart},
                     new SqlParameter("@" + nameof(secondLicensePlatePart),SqlDbType.NVarChar){Value = secondLicensePlatePart},
                     new SqlParameter("@" + nameof(thirdLicensePlatePart),SqlDbType.NVarChar){Value = thirdLicensePlatePart},
                     new SqlParameter("@" + nameof(fourthLicensePlatePart),SqlDbType.NVarChar){Value = fourthLicensePlatePart},
                     new SqlParameter("@LogId", SqlDbType.Int) {Value = logId},
                     new SqlParameter("@LicensePlate", SqlDbType.NVarChar) {Value = licensePlate},
                     new SqlParameter("@DetectorId", SqlDbType.Int) {Value = detectorId},
                     new SqlParameter("@FromDate", SqlDbType.DateTime) {Value = fromDate == default? (object) null: fromDate},
                     new SqlParameter("@ToDate", SqlDbType.DateTime) {Value = toDate == default? (object) null: toDate},
                     new SqlParameter("@MinPrecision", SqlDbType.TinyInt) {Value = minPrecision},
                     new SqlParameter("@MaxPrecision", SqlDbType.TinyInt) {Value = maxPrecision},
                     new SqlParameter("@WithPic", SqlDbType.Bit ){Value = withPic},
                     new SqlParameter("@SuccessTransfer", SqlDbType.Bit) {Value = successTransfer},
                     new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                     new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},
                };
                return _repository.ToResultList<PagingResult<PlateDetectionLog>>("SelectPlateDetectionLogs", parameters, fetchCompositions: true).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<ManualPlateDetectionLog>> GetManualPlateDetectionLog(int logId = default, long userId = default, long parentLogId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
        int pageSize = default)
        {
            var parameters = new List<SqlParameter>
                {
                     new SqlParameter("@LogId", SqlDbType.Int) {Value = logId},
                     new SqlParameter("@" + nameof(userId), SqlDbType.BigInt) {Value = userId},
                     new SqlParameter("@" + nameof(parentLogId), SqlDbType.BigInt) {Value = parentLogId},
                     new SqlParameter("@LicensePlate", SqlDbType.NVarChar) {Value = licensePlate},
                     new SqlParameter("@DetectorId", SqlDbType.Int) {Value = detectorId},
                     new SqlParameter("@FromDate", SqlDbType.DateTime) {Value = fromDate == default? (object) null: fromDate},
                     new SqlParameter("@ToDate", SqlDbType.DateTime) {Value = toDate == default? (object) null: toDate},
                     new SqlParameter("@MinPrecision", SqlDbType.TinyInt) {Value = minPrecision},
                     new SqlParameter("@MaxPrecision", SqlDbType.TinyInt) {Value = maxPrecision},
                     new SqlParameter("@WithPic", SqlDbType.Bit ){Value = withPic},
                     new SqlParameter("@SuccessTransfer", SqlDbType.Bit) {Value = successTransfer},
                     new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                     new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},
                };
            return _repository.ToResultList<PagingResult<ManualPlateDetectionLog>>("SelectManualPlateDetectionLog", parameters, fetchCompositions: true).FetchFromResultList();
        }
        public Task<ResultViewModel> AddPlateDetectionLog(PlateDetectionLog log,int nestingDepthLevel= 4)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@LicensePlateid", SqlDbType.Int) {Value =log.LicensePlate.EntityId},
                     new SqlParameter("@DetectorId", SqlDbType.Int) {Value = log.DetectorId},
                     new SqlParameter("@EventId", SqlDbType.Int) {Value = log.EventLog.Code},
                     new SqlParameter("@LogDateTime", SqlDbType.DateTime) {Value = log.LogDateTime},
                     new SqlParameter("@Ticks", SqlDbType.BigInt) {Value = log.DateTimeTicks},
                     new SqlParameter("@DetectionPrecision", SqlDbType.Int) {Value = log.DetectionPrecision},
                     new SqlParameter("@FullImage", SqlDbType.VarBinary) {Value = log.FullImage},
                     new SqlParameter("@PlateImage", SqlDbType.VarBinary) {Value = log.PlateImage},
                };

                return _repository.ToResultList<ResultViewModel>("InsertPlateDetectionLog", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).Data.FirstOrDefault();
            });
        }
        public Task<ResultViewModel> AddLicensePlate(LicensePlate licensePlate)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@LicensePlate", SqlDbType.NVarChar) {Value = licensePlate.LicensePlateNumber},
                    new SqlParameter("@IsActive", SqlDbType.Bit) {Value = licensePlate.IsActive},
                    new SqlParameter("@StartDate", SqlDbType.Date) {Value = licensePlate.StartDate},
                    new SqlParameter("@EndDate", SqlDbType.Date) {Value = licensePlate.EndDate},
                    new SqlParameter("@StartTime", SqlDbType.Time) {Value = licensePlate.StartTime},
                    new SqlParameter("@EndTime", SqlDbType.Time) {Value = licensePlate.EndTime},
                };

                return _repository.ToResultList<ResultViewModel>("InsertLicensePlate", parameters).Data.FirstOrDefault();
            });
        }
        public ResultViewModel<LicensePlate> GetLicensePlate(string licensePlate, int entityId,int nestingDepthLevel=4)
        {
            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@LicensePlate", SqlDbType.NVarChar) {Value = licensePlate},
                    new SqlParameter("@LicensePlateId", SqlDbType.Int) {Value = entityId}
                };

                return _repository.ToResultList<LicensePlate>("SelectLicensePlateByFilter", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }
    }
}