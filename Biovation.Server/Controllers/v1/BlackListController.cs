using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BlackListController : Controller
    {
        private readonly RestClient _restClient;

        public BlackListController(RestClient restClient)
        {
            _restClient = restClient;
        }

        [HttpPost]
        [Route("CreateBlackList")]
        public Task<List<ResultViewModel>> CreateBlackList(List<BlackList> blackLists)
        {

            return Task.Run(() =>
            {
                try
                {
                    var restRequest = new RestRequest($"Queries/v2/blackLists", Method.POST);
                    restRequest.AddJsonBody("blackLists", blackLists.ToString() ?? string.Empty);
                    var resultsBlackLists = (_restClient.ExecuteAsync<List<ResultViewModel>>(restRequest)).Result.Data;
                   // var resultsBlackLists = _blackListService.CreateBlackList(blackLists);

                    Task.Run(async () =>
                    {
                        var successResult = new List<BlackList>();
                        foreach (var blackList in resultsBlackLists)
                        {
                            if (blackList.Validate == 1)
                            {
                                restRequest = new RestRequest($"Queries/v2/blackList", Method.GET);
                                restRequest.AddQueryParameter("id", blackList.Id.ToString());
                                var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<BlackList>>>(restRequest);
                                successResult.Add(requestResult.Result.Data.Data.Data.FirstOrDefault());
                            }
                        }

                        try
                        {

                            var groupByList = successResult.GroupBy(x => new { x.Device.Brand.Name }).ToList();

                            foreach (var list in groupByList)
                            {
                                var brandName = list.FirstOrDefault()?.Device.Brand.Name;
                                if (brandName == null) continue;
                                 restRequest =
                                    new RestRequest($"/{brandName}/{brandName}BlackList/SendBlackLisDevice",
                                        Method.POST);
                                restRequest.AddJsonBody(list);

                                 await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

                                //result.Add(restResult.Data);
                            }

                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    });


                    return resultsBlackLists;

                }
                catch (Exception)
                {
                    return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = "error" } };
                }
            });

        }



        [HttpGet]
        [Route("GetBlackList")]
        public Task<List<BlackList>> GetBlackList(int id = default, int userid = default, int deviceId = default, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default)
        {
            return Task.Run(() =>
            {
                var restRequest = new RestRequest($"Queries/v2/blackList", Method.GET);
                restRequest.AddQueryParameter("id", id.ToString());
                restRequest.AddQueryParameter("userid", userid.ToString());
                restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                restRequest.AddQueryParameter("startDate", startDate.ToString());
                restRequest.AddQueryParameter("startDate", startDate.ToString());
                restRequest.AddQueryParameter("isDeleted", isDeleted.ToString());
                return (_restClient.ExecuteAsync<ResultViewModel<PagingResult<BlackList>>>(restRequest)).Result.Data.Data.Data;
            });
        }

        [HttpPost]
        [Route("ChangeBlackList")]
        public Task<ResultViewModel> ChangeBlackList([FromBody] BlackList blackList)
        {
            return Task.Run(() =>
            {

                var restRequest = new RestRequest($"Queries/v2/blackList", Method.PUT);
                restRequest.AddJsonBody("blackList", blackList.ToString() ?? string.Empty);
                var result = (_restClient.ExecuteAsync<ResultViewModel>(restRequest)).Result.Data;

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            restRequest = new RestRequest($"Queries/v2/blackList", Method.GET);
                            restRequest.AddQueryParameter("id", result.Id.ToString());
                            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<BlackList>>>(restRequest);
                            successBlackList = (requestResult.Result.Data.Data.Data);
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand?.Name != null)
                        {
                             restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);

                             await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                });

                return result;

            });
        }
        [HttpPost]
        [Route("DeleteBlackList")]
        public Task<ResultViewModel> DeleteBlackList(int id)
        {
            return Task.Run(() =>
            {

                var restRequest = new RestRequest($"Queries/v2/blackList", Method.DELETE);
                restRequest.AddQueryParameter("id", id.ToString());
                var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                var result = (requestResult.Result.Data);
                

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            restRequest = new RestRequest($"Queries/v2/blackList", Method.GET);
                            restRequest.AddQueryParameter("id", result.Id.ToString());
                            restRequest.AddQueryParameter("isDeleted", true.ToString());
                            var reqResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<BlackList>>>(restRequest);
                            successBlackList = (reqResult.Result.Data.Data.Data);
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand != null)
                        {
                             restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);

                            await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                });

                return result;

            });
        }


    }
}
