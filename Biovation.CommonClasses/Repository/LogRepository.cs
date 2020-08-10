using Biovation.CommonClasses.Models;
using DataAccessLayerCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Repository
{
    public class LogRepository
    {
        private readonly GenericRepository _repository;

        public LogRepository()
        {
            _repository = new GenericRepository();
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

                    return logs.Rows.Count > 0 ? _repository.ToResultList<ResultViewModel>("SetLogIsTransferedBatch", parameters).Data.FirstOrDefault() : new ResultViewModel { Validate = 1, Message = "0" };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = "0" };
                }
            });
        }

        //public ResultViewModel AddLogBulk(List<Log> logs)
        //{
        //    ResultViewModel result = new ResultViewModel { Validate = 0 };
        //    var kiloLog = logs.Count / 950;

        //    for (var index = 0; index <= kiloLog; index++)
        //    {
        //        var values = string.Empty;

        //        for (var i = index * 950; i < index * 950 + 960; i++)
        //        {
        //            if (i == logs.Count)
        //            {
        //                break;
        //            }

        //            if (logs[i].UserId > 2147483646)
        //            {
        //                //TODO log that event!
        //                logs[i].UserId = 0;
        //            }

        //            values += "(" + logs[i].DeviceId + ", " + logs[i].EventId + ", " +
        //                      logs[i].UserId + ", '" +
        //                      logs[i].LogDateTime.ToString("yyyy/MM/dd HH:mm:ss") + "', " + logs[i].DateTimeTicks + ", " +
        //                      logs[i].SubEvent.Code + ", " + (logs[i].TnaEvent.ToString() == "65535" ? "1" : "0") +
        //                      ",0, 1" + ") , ";
        //        }

        //        values = values.Substring(0, values.Length - 3);

        //        try
        //        {
        //            var parameters = new List<SqlParameter>
        //            {
        //                new SqlParameter("@Values", SqlDbType.NVarChar) {Value = values}
        //            };

        //            result = _repository.ToResultList<ResultViewModel>("InsertLogBulk", parameters).Data.FirstOrDefault();
        //        }
        //        catch (Exception exception)
        //        {
        //            Logger.Log(exception);
        //            result = new ResultViewModel { Validate = 0 };
        //        }
        //    }

        //    return result;
        //}

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
        public Task<List<Log>> GetOfflineLogs()
        {
            return Task.Run(() => _repository.ToResultList<Log>("SelectOfflineLogs", fetchCompositions: true).Data);
        }

        public Task<List<Log>> GetLastLogsOfDevice(uint deviceId)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter> { new SqlParameter("@DeviceId", SqlDbType.BigInt) { Value = deviceId } };
                return _repository.ToResultList<Log>("SelectLastLogsByDeviceId", parameters, fetchCompositions: true).Data;
            });
        }

        public Task<List<Log>> GetOfflineLogsByDeviceId(uint deviceId)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter> { new SqlParameter("@DeviceId", SqlDbType.BigInt) { Value = deviceId } };
                return _repository.ToResultList<Log>("SelectOfflineLogsByDeviceId", parameters, fetchCompositions: true).Data;
            });
        }

        public Task<List<Log>> GetOfflineLogsByUserId(int userId)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter> { new SqlParameter("@UserId", SqlDbType.Int) { Value = userId } };
                return _repository.ToResultList<Log>("SelectOfflineLogsByUserId", parameters, fetchCompositions: true).Data;
            });
        }

        public Task<List<Log>> GetOfflineLogsOfPeriod(DateTime fromDate, DateTime toDate)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", SqlDbType.DateTime) {Value = fromDate},
                    new SqlParameter("@ToDate", SqlDbType.DateTime) {Value = toDate}
                };

                return _repository.ToResultList<Log>("SelectOfflineLogsOfPeriod", parameters, fetchCompositions: true).Data;
            });
        }
        public Task<List<Log>> SelectSearchedOfflineLogs(DeviceTraffic deviceTraffic)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@UserId", deviceTraffic.UserId),
                        new SqlParameter("@DeviceId", SqlDbType.BigInt) {Value = deviceTraffic.DeviceId},
                        new SqlParameter("@FromDate", deviceTraffic.FromDate),
                        new SqlParameter("@ToDate", deviceTraffic.ToDate),
                        new SqlParameter("@AdminUserId", deviceTraffic.OnlineUserId),
                        new SqlParameter("@State", deviceTraffic.State)
                    };
                return _repository.ToResultList<Log>("SelectSearchedOfflineLogs", parameters, fetchCompositions: true).Data;
            });
        }
        public Task<List<Log>> SelectSearchedOfflineLogsWithPaging(DeviceTraffic deviceTraffic)
        {
            return Task.Run(() =>
            {
                try
                {
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@UserId", deviceTraffic.UserId),
                        new SqlParameter("@DeviceId", SqlDbType.BigInt) {Value = deviceTraffic.DeviceId},
                        new SqlParameter("@FromDate", deviceTraffic.FromDate),
                        new SqlParameter("@ToDate", deviceTraffic.ToDate),
                        new SqlParameter("@PageNumber", deviceTraffic.PageNumber),
                        new SqlParameter("@PageSize", deviceTraffic.PageSize),
                        new SqlParameter("@Where", deviceTraffic.Where),
                        new SqlParameter("@Order", deviceTraffic.Order),
                        new SqlParameter("@AdminUserId", deviceTraffic.OnlineUserId),
                        new SqlParameter("@State", deviceTraffic.State)
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
        public Task<List<Log>> GetOfflineLogsOfPeriodByDeviceId(uint deviceId, DateTime fromDate, DateTime toDate)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@DeviceId", SqlDbType.BigInt) {Value = deviceId},
                    new SqlParameter("@FromDate", SqlDbType.DateTime) {Value = fromDate},
                    new SqlParameter("@ToDate", SqlDbType.DateTime) {Value = toDate}
                };

                return _repository.ToResultList<Log>("SelectOfflineLogsOfPeriodByDeviceId", parameters, fetchCompositions: true).Data;
            });
        }

        public Task<List<Log>> GetOfflineLogsOfPeriodByUserId(int userId, DateTime fromDate, DateTime toDate)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", SqlDbType.Int) {Value = userId},
                    new SqlParameter("@FromDate", SqlDbType.DateTime) {Value = fromDate},
                    new SqlParameter("@ToDate", SqlDbType.DateTime) {Value = toDate}
                };

                return _repository.ToResultList<Log>("SelectOfflineLogsOfPeriodByUserId", parameters, fetchCompositions: true).Data;
            });
        }

        public Task<Log> GetLog(long id)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", SqlDbType.BigInt) {Value = id}
                };

                return _repository.ToResultList<Log>("GetLog", parameters, fetchCompositions: true).Data.FirstOrDefault();
            });
        }
    }
}
