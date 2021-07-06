using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Repository.Sql.v2
{
    public class PlateDetectionRepository
    {
        private readonly GenericRepository _repository;

        public PlateDetectionRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public ResultViewModel<PagingResult<PlateDetectionLog>> GetPlateDetectionLog(string firstLicensePlatePart = default, string secondLicensePlatePart = default, string thirdLicensePlatePart = default, string fourthLicensePlatePart = default, int logId = default, string licensePlate = default, int detectorId = default, DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0, bool withPic = true, bool successTransfer = false, int pageNumber = default,
        int pageSize = default, int nestingDepthLevel = 4)
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
            return _repository.ToResultList<PagingResult<PlateDetectionLog>>("SelectPlateDetectionLogs", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<ManualPlateDetectionLog>> GetManualPlateDetectionLog(int logId = default,
            long userId = default, long parentLogId = default, string licensePlate = default, int detectorId = default,
            DateTime fromDate = default, DateTime toDate = default, int minPrecision = 0, int maxPrecision = 0,
            bool withPic = true, bool successTransfer = false, int pageNumber = default, int pageSize = default,
            string whereClause = default, string orderByClause = default)
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
                     new SqlParameter("@Where", string.IsNullOrWhiteSpace(whereClause) ? "" : whereClause),
                     new SqlParameter("@Order", string.IsNullOrWhiteSpace(orderByClause) ? "" : orderByClause),
                };
            return _repository.ToResultList<PagingResult<ManualPlateDetectionLog>>("SelectManualPlateDetectionLog", parameters, fetchCompositions: true).FetchFromResultList();
        }
        public ResultViewModel AddPlateDetectionLog(PlateDetectionLog log, int nestingDepthLevel = 4)
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
                     new SqlParameter("@InOrOut", SqlDbType.TinyInt) {Value = log.InOrOut},
                };

            return _repository.ToResultList<ResultViewModel>("InsertPlateDetectionLog", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).Data.FirstOrDefault();
        }

        public Task<ResultViewModel> AddManualPlateDetectionLog(ManualPlateDetectionLog log, int nestingDepthLevel = 4)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@LicensePlateId", SqlDbType.Int) {Value =log.LicensePlate.LicensePlateNumber},
                    new SqlParameter("@UserId",SqlDbType.BigInt){Value = log.User.Id},
                    new SqlParameter("@ParentLogId",SqlDbType.BigInt){Value = log.ParentLog?.Id ?? 0},
                    new SqlParameter("@DetectorId", SqlDbType.Int) {Value = log.DetectorId},
                    new SqlParameter("@EventId", SqlDbType.Int) {Value = log.EventLog.Code},
                    new SqlParameter("@LogDateTime", SqlDbType.DateTime) {Value = log.LogDateTime},
                    new SqlParameter("@Ticks", SqlDbType.BigInt) {Value = log.DateTimeTicks},
                    new SqlParameter("@DetectionPrecision", SqlDbType.Int) {Value = log.DetectionPrecision},
                    new SqlParameter("@FullImage", SqlDbType.VarBinary) {Value = log.FullImage},
                    new SqlParameter("@PlateImage", SqlDbType.VarBinary) {Value = log.PlateImage},
                    new SqlParameter("@InOrOut", SqlDbType.TinyInt) {Value = log.InOrOut},
                };

                return _repository.ToResultList<ResultViewModel>("InsertManualPlateDetectionLog", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).Data.FirstOrDefault();
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

                return _repository.ToResultList<ResultViewModel>("InsertLicensePlate", parameters).Data
                    .FirstOrDefault();
            });
        }
        public ResultViewModel<LicensePlate> GetLicensePlate(string licensePlate, int entityId, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@LicensePlate", SqlDbType.NVarChar) {Value = licensePlate},
                    new SqlParameter("@LicensePlateId", SqlDbType.Int) {Value = entityId}
                };

            return _repository.ToResultList<LicensePlate>("SelectLicensePlateByFilter", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel<List<LicensePlate>> ReadLicensePlate(string licensePlateNumber, int entityId, bool isActive, DateTime startDate, DateTime endDate, TimeSpan intervalBeginningStartTime, TimeSpan intervalEndStartTime, TimeSpan intervalBeginningFinishTime, TimeSpan intervalEndFinishTime)
        {
            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@LicensePlate", SqlDbType.NVarChar) {Value = licensePlateNumber},
                    new SqlParameter("@IsActive", SqlDbType.Bit) {Value = isActive},
                    new SqlParameter("@StartDate", SqlDbType.Date) {Value = startDate},
                    new SqlParameter("@EndDate", SqlDbType.Date) {Value = endDate},
                    new SqlParameter("@IntervalBeginningStartTime", SqlDbType.Time) {Value = intervalBeginningStartTime != default ? intervalBeginningStartTime:TimeSpan.Zero},
                    new SqlParameter("@IntervalEndStartTime", SqlDbType.Time) {Value = intervalEndStartTime != default ? intervalEndStartTime: new TimeSpan(23,59,59)},
                    new SqlParameter("@IntervalBeginningFinishTime", SqlDbType.Time) {Value = intervalBeginningFinishTime != default ? intervalBeginningFinishTime :TimeSpan.Zero},
                    new SqlParameter("@IntervalEndFinishTime", SqlDbType.Time) {Value = intervalEndFinishTime!= default ? intervalEndStartTime :new TimeSpan(23,59,59)},
                };

            return _repository.ToResultList<ResultViewModel<List<LicensePlate>>>("SelectLicensePlate", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteLicensePlate(LicensePlate licensePlate, DateTime modifiedAt, string modifiedBy,
            string action)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@LicensePlate", SqlDbType.NVarChar) {Value = licensePlate.LicensePlateNumber},
                new SqlParameter("@IsActive", SqlDbType.Bit) {Value = licensePlate.IsActive},
                new SqlParameter("@StartDate", SqlDbType.Date) {Value = licensePlate.StartDate},
                new SqlParameter("@EndDate", SqlDbType.Date) {Value = licensePlate.EndDate},
                new SqlParameter("@StartTime", SqlDbType.Time) {Value = licensePlate.StartTime},
                new SqlParameter("@EndTime", SqlDbType.Time) {Value = licensePlate.EndTime},
                new SqlParameter("@modifiedAt", SqlDbType.DateTime) {Value = modifiedAt},
                new SqlParameter("@modifiedBy", SqlDbType.NVarChar) {Value = modifiedBy},
                new SqlParameter("@action", SqlDbType.NVarChar) {Value = action},
            };
            return _repository.ToResultList<ResultViewModel>("DeleteLicensePlate",
            parameters).Data.FirstOrDefault();
        }

        public Task<ResultViewModel<PlateDetectionLog>> SelectPreviousPlateDetectionLog(int id = default, string licensePlateNumber = default, DateTime? logDateTime = null)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@" + nameof(id), SqlDbType.Int) {Value = id},
                    new SqlParameter("@" + nameof(licensePlateNumber), SqlDbType.NVarChar) {Value = licensePlateNumber},
                    new SqlParameter("@" + nameof(logDateTime), SqlDbType.DateTime) {Value = logDateTime}
                };

                return _repository.ToResultList<PlateDetectionLog>("SelectPreviousPlateDetectionLog", parameters, fetchCompositions: true, compositionDepthLevel: 4).FetchFromResultList();
            });
        }
    }
}