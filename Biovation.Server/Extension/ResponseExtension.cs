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
            JsonResult jsonResult;
            //if (Enum.IsDefined(typeof(HttpStatusCode), (int)result.Code))
            //{
            //    jsonResult = new JsonResult(new ResultViewModel<T>
            //    {
            //        Validate = result.Validate,
            //        Message = result.Message,
            //        Data = result.Data,
            //        Id = result.Id
            //    })
            //    {
            //        StatusCode = (int)result.Code
            //    };

            //}
            //else
            //{
            //    jsonResult = new JsonResult(new ResultViewModel<T>()
            //    {
            //        Validate = result.Validate,
            //        Message = result.Message,
            //        Data = result.Data,
            //        Id = result.Id,
            //        Code = result.Code
            //    })
            //    {
            //        StatusCode = httpStatusCode
            //    };
            //}


            jsonResult = new JsonResult(new ResultViewModel<T>
            {
                Validate = result.Validate,
                Message = result.Message,
                Data = result.Data,
                Id = result.Id
            })
            {
                StatusCode = httpStatusCode ?? (int)result.Code
            };
            return jsonResult;

        }

        public static JsonResult JsonFromResultViewModel(this HttpResponse response, ResultViewModel result, int? httpStatusCode = null)
        {

            JsonResult jsonResult;
            //if (Enum.IsDefined(typeof(HttpStatusCode), (int)result.Code))
            //{
            //    jsonResult = new JsonResult(new ResultViewModel
            //    {
            //        Validate = result.Validate,
            //        Message = result.Message,
            //        Id = result.Id
            //    })
            //    {
            //        StatusCode = (int)result.Code
            //    };

            //}
            //else
            //{
            //    jsonResult = new JsonResult(new ResultViewModel()
            //    {
            //        Validate = result.Validate,
            //        Message = result.Message,
            //        Id = result.Id,
            //        Code = result.Code
            //    })
            //    {
            //        StatusCode = httpStatusCode
            //    };
            //}

            jsonResult = new JsonResult(new ResultViewModel()
            {
                Validate = result.Validate,
                Message = result.Message,
                Id = result.Id,
                Code = result.Code
            })
            {
                StatusCode = httpStatusCode ?? (int)result.Code
            };

            return jsonResult;

        }



        public static JsonResult JsonFromBriefResult(this HttpResponse response, bool validate = default, long code = 200, string message = default,
            string title = default, long id = default, int? httpStatusCode = null)
        {
            JsonResult jsonResult;


            jsonResult = new JsonResult(new ResultViewModel()
            {
                Validate = validate,
                Message = message,
                Id = id

            })
            {
                StatusCode = httpStatusCode ?? (int)code
            };

            return jsonResult;
        }


        //T = data.getType()
        public static JsonResult JsonFromBriefResult<T>(this HttpResponse response, bool validate = default, long code = 200, string message = default,
            string title = default, long id = default, object data = default, int? httpStatusCode = null)
        {
            JsonResult jsonResult;


            jsonResult = new JsonResult(new ResultViewModel<T>()
            {
                Validate = validate,
                Message = message,
                Id = id,
                Data = (T)data

            })
            {
                StatusCode = httpStatusCode ?? (int)code
            };

            return jsonResult;
        }

        public static ResultViewModel SubstitutionCode(this HttpResponse response, ResultViewModel result, int httpStatusCode = (int)HttpStatusCode.OK)
        {
            return new ResultViewModel()
            {
                Validate = result.Validate,
                Message = result.Message,
                Id = result.Id,
                Code = httpStatusCode
            };
        }

        public static ResultViewModel<T> SubstitutionCode<T>(this HttpResponse response, ResultViewModel<T> result, int httpStatusCode = (int)HttpStatusCode.OK)
        {
            return new ResultViewModel<T>()
            {
                Validate = result.Validate,
                Message = result.Message,
                Id = result.Id,
                Data = result.Data,
                Code = httpStatusCode
            };
        }


    }

}