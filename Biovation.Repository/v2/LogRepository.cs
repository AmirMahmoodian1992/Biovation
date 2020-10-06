using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v2
{
    public class LogRepository
    {
        private readonly GenericRepository _repository;

        public LogRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(() =>
            {
                try
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@DeviceId", SqlDbType.Int) {Value = log.DeviceId},
                        new SqlParameter("@EventId", log.EventLog?.Code ?? 0.ToString()),
                        new SqlParameter("@UserId", log.UserId),
                        new SqlParameter("@DateTime", SqlDbType.DateTime) {Value = log.LogDateTime},
                        new SqlParameter("@Ticks", SqlDbType.BigInt) {Value = log.DateTimeTicks},
                        new SqlParameter("@SubEvent", log.SubEvent?.Code ?? 0.ToString()),
                        new SqlParameter("@TNAEvent", SqlDbType.Int) {Value = Convert.ToInt32(log.TnaEvent)},
                        new SqlParameter("@InOutMode", SqlDbType.Int) {Value = log.InOutMode},
                        new SqlParameter("@MatchingType", log.MatchingType?.Code ?? 0.ToString()),
                        new SqlParameter("@SuccessTransfer", log.SuccessTransfer),
                        new SqlParameter("@Image", log.Image)
                    };

                    return _repository.ToResultList<ResultViewModel>("InsertLog", parameters).Data.FirstOrDefault();
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0 };
                }
            });
        }

        public Task<ResultViewModel> AddLog(DataTable logs)
        {
            return Task.Run(() =>
            {
                try
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@LogTable", logs)
                    };

                    if (logs.Rows.Count > 0)
                    {
                        return _repository.ToResultList<ResultViewModel>("InsertLogBatch", parameters).Data
                            .FirstOrDefault();
                    }

                    return new ResultViewModel { Validate = 1, Message = "0" };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = "0" };
                }
            });
        }

        public Task<ResultViewModel> UpdateLog(DataTable logs)
        {
            return Task.Run(() =>
            {
                try
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@LogTable", logs)
                    };

                    return logs.Rows.Count > 0
                        ? _repository.ToResultList<ResultViewModel>("SetLogIsTransferedBatch", parameters).Data
                            .FirstOrDefault()
                        : new ResultViewModel { Validate = 1, Message = "0" };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = "0" };
                }
            });
        }



        public Task<List<Log>> CheckLogInsertion(List<Log> logs)
        {
            return Task.Run(() =>
            {
                List<Log> result;

                if (logs.Count <= 0)
                {
                    return new List<Log>();
                }

                try
                {
                    var logTimes = "(";

                    foreach (var log in logs)
                    {
                        logTimes += log.DateTimeTicks + ", ";
                    }

                    logTimes = logTimes.Substring(0, logTimes.Length - 2);
                    logTimes += ")";

                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@LogTimes", SqlDbType.NVarChar) {Value = logTimes},
                        new SqlParameter("@DeviceId", SqlDbType.BigInt) {Value = logs[0].DeviceId}
                    };

                    result = _repository.ToResultList<Log>("CheckLogInsertation", parameters).Data;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    result = new List<Log>();
                }

                return result;
            });
        }

        public Task<ResultViewModel> UpdateLog(Log log)
        {
            return Task.Run(() =>
            {
                try
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@DeviceId", SqlDbType.BigInt) {Value = log.DeviceId},
                        new SqlParameter("@EventId", log.EventLog.Code),
                        new SqlParameter("@UserId", log.UserId),
                        new SqlParameter("@Ticks", SqlDbType.BigInt) {Value = log.DateTimeTicks},
                        new SqlParameter("@MatchingType", log.MatchingType?.Code ?? 0.ToString())
                    };

                    return _repository.ToResultList<ResultViewModel>("SetLogIsTransfered", parameters).Data
                        .FirstOrDefault();
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0 };
                }
            });
        }

        public Task<ResultViewModel> AddLogImage(Log log)
        {
            return Task.Run(() =>
            {
                try
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@DateTime", log.LogDateTime),
                        new SqlParameter("@UserId", log.UserId),
                        new SqlParameter("@DeviceId", log.DeviceId),
                        new SqlParameter("@EventId", log.EventLog.Code),
                        new SqlParameter("@ImageFilePath", log.Image)
                    };

                    return _repository.ToResultList<ResultViewModel>("AddLogImage", parameters).Data
                        .FirstOrDefault();
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0 };
                }
            });
        }

        public Task<List<Log>> Logs(int id = default, int deviceId = default, int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default, int pageSize = default, string where = default, string order = default, long onlineUserId = default, bool? successTransfer = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@DeviceId", SqlDbType.BigInt) {Value = deviceId},
                        new SqlParameter("@FromDate",fromDate),
                        new SqlParameter("@ToDate",toDate),
                        new SqlParameter("@PageNumber", pageNumber),
                        new SqlParameter("@PageSize", pageSize),
                        new SqlParameter("@Where", where),
                        new SqlParameter("@Order",order),
                        new SqlParameter("@AdminUserId",onlineUserId),
                        new SqlParameter("@State", successTransfer)
                    };

                    return _repository.ToResultList<Log>("SelectSearchedOfflineLogsWithPaging", parameters,
                        fetchCompositions: true).Data;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new List<Log>();
                }
            });
        }

        //TODO
        /* public ResultViewModel<PagingResult<Log>> Logs(int id = default, int deviceId = default, int userId = default,
             DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default, int pageSize = default)
         {
             var parameters = new List<SqlParameter>
             {
                 new SqlParameter("@Id", SqlDbType.BigInt) {Value = id},
                 new SqlParameter("@DeviceId", SqlDbType.BigInt) { Value = deviceId },
                 new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
                 new SqlParameter("@FromDate", SqlDbType.DateTime) {Value = fromDate},
                 new SqlParameter("@ToDate", SqlDbType.DateTime) {Value = toDate},
                 new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber },
                 new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize }

             };



             return _repository.ToResultList<PagingResult<Log>>("GetLog", parameters, fetchCompositions: true).FetchFromResultList();

         }*/


    }
}
