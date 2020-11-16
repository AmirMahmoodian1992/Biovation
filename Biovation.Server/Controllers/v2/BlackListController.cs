using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiVersion("2.0")]
    [Route("biovation/api/v2/[controller]")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class BlackListController : ControllerBase
    {

        private readonly BlackListService _blackListService;
        private readonly RestClient _restClient;

        public BlackListController(BlackListService blackListService, RestClient restClient)
        {
            _blackListService = blackListService;
            _restClient = restClient;
        }

        [HttpPost]
        public Task<List<ResultViewModel>> CreateBlackList([FromBody]List<BlackList> blackLists)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {
                try
                {
                    var resultsBlackLists = blackLists.Select(blackList => _blackListService.CreateBlackList(blackList,token)).ToList();

                    Task.Run(async () =>
                    {
                        var successResult = new List<BlackList>();
                        foreach (var blackList in resultsBlackLists)
                        {
                            if (blackList.Validate == 1)
                            {
                                successResult.Add((_blackListService.GetBlacklist(id: (int)blackList.Id,token:token)).Data.Data.Find(l => l.Id == blackList.Id));

                            }
                        }

                        try
                        {

                            var groupByList = successResult.GroupBy(x => new { x.Device.Brand.Name }).ToList();

                            foreach (var list in groupByList)
                            {
                                var brandName = list.FirstOrDefault()?.Device.Brand.Name;
                                if (brandName == null) continue;
                                var restRequest =
                                    new RestRequest($"/{brandName}/{brandName}BlackList/SendBlackLisDevice",
                                        Method.POST);
                                restRequest.AddJsonBody(list);
                                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                                {
                                    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                                }
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
        [Route("{id}")]
        public Task<ResultViewModel<PagingResult<BlackList>>> GetBlackList(int id = default, int userid = default, int deviceId = default, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run( () => _blackListService.GetBlacklist(id,userid,deviceId,startDate,endDate,isDeleted,token:token));
        }

        [HttpPut]
        public Task<ResultViewModel> ChangeBlackList([FromBody] BlackList blackList)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {

                //var restRequest = new RestRequest($"Queries/v2/blackList", Method.PUT);
                //restRequest.AddJsonBody("blackList", blackList.ToString() ?? string.Empty);
                //var result = (_restClient.ExecuteAsync<ResultViewModel>(restRequest)).Result.Data;
                var result = _blackListService.ChangeBlackList(blackList, token: token);

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlacklist(id: (int)result.Id, token: token).Data.Data;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand?.Name != null)
                        {
                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);
                            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                            {
                                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                            }
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

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteBlackList(int id)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {

                //var restRequest = new RestRequest($"Queries/v2/blackList", Method.DELETE);
                //restRequest.AddQueryParameter("id", id.ToString());
                //var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                //var result = (requestResult.Result.Data);
                var result = _blackListService.DeleteBlackList(id, token: token);


                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlacklist(id: (int)result.Id, isDeleted: true, token:token).Data.Data;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand != null)
                        {
                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);
                            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                            {
                                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                            }
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
