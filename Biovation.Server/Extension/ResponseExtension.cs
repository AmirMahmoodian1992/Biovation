using System.Net;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Extension
{
    public static class ResponseExtension
    {
        public static JsonResult JsonFromResultViewModel<T>(this HttpResponse response, ResultViewModel<T> result, int? httpStatusCode = null)
        {
            var jsonResult = new JsonResult(new ResultViewModel<T>
            {
                Validate = result.Validate,
                Success = result.Success,
                Message = result.Message,
                Data = result.Data,
                Id = result.Id
            })
            {
                StatusCode = httpStatusCode ?? ((int)result.Code != 0 ? (int)result.Code: 200)
            };
            return jsonResult;
        }

        public static JsonResult JsonFromResultViewModel(this HttpResponse response, ResultViewModel result, int? httpStatusCode = null)
        {
            var jsonResult = new JsonResult(new ResultViewModel()
            {
                Validate = result.Validate,
                Message = result.Message,
                Id = result.Id,
                Code = result.Code
            })
            {
                StatusCode = httpStatusCode ?? ((int)result.Code != 0 ? (int)result.Code : 200)
            };

            return jsonResult;
        }



        public static JsonResult JsonFromBriefResult(this HttpResponse response, int validate = default, bool success = default, long code = 200, string message = default,
            string title = default, long id = default, int? httpStatusCode = null)
        {
            var jsonResult = new JsonResult(new ResultViewModel()
            {
                Validate = validate,
                Success = success,
                Message = message,
                Id = id

            })
            {
                StatusCode = (int?) (httpStatusCode ?? (code != 0 ? code : 200))
            };

            return jsonResult;
        }


        //T = data.getType()
        public static JsonResult JsonFromBriefResult<T>(this HttpResponse response, bool success = default , int validate = default, long code = 200, string message = default,
            string title = default, long id = default, object data = default, int? httpStatusCode = null)
        {
            var jsonResult = new JsonResult(new ResultViewModel<T>()
            {
                Validate = validate,
                Success = success,
                Message = message,
                Id = id,
                Data = (T)data

            })
            {
                StatusCode = (int?)(httpStatusCode ?? (code != 0 ? code : 200))
            };

            return jsonResult;
        }


    }

}