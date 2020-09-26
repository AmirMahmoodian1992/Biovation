using Biovation.Domain;
using DataAccessLayerCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayerCore.Extentions
{
    public static class FetchDataFromResultListExtension
    {
        public static ResultViewModel<T> FetchFromResultList<T>(this Result<List<T>> data)
        {
            return new ResultViewModel<T>
            {
                Success = data.Success,
                Code = Convert.ToInt64(data.Code),
                //Title = data.Title,
                Message = data.Message,
                //SeverityLevel = data.SeverityLevel,
                Data = data.Data.FirstOrDefault()
            };
        }
        public static ResultViewModel<List<T>> FetchResultList<T>(this Result<List<T>> data)
        {
            return new ResultViewModel<List<T>>
            {
                Success = data.Success,
                Code = Convert.ToInt64(data.Code),
                //Title = data.Title,
                Message = data.Message,
                //SeverityLevel = data.SeverityLevel,
                Data = data.Data
            };
        }
    }
}
