using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BlackListController : Controller
    {
        private readonly RestClient _restClient;
        private readonly BlackListService _blackListService;

        public BlackListController(RestClient restClient, BlackListService blackListService)
        {
            _restClient = restClient;
            _blackListService = blackListService;
        }

        [HttpPost]
        [Route("CreateBlackList")]
        public Task<List<ResultViewModel>> CreateBlackList(List<BlackList> blackLists)
        {

            return Task.Run(() =>
            {
                try
                {
                    var resultsBlackLists = blackLists.Select(blackList => _blackListService.CreateBlackList(blackList)).ToList();

                    Task.Run(async () =>
                    {
                        var successResult = new List<BlackList>();
                        foreach (var blackList in resultsBlackLists)
                        {
                            if (blackList.Validate == 1)
                            {
                                successResult.Add((_blackListService.GetBlacklist(id: (int)blackList.Id)).Data.Data.Find(l => l.Id == blackList.Id));
                                
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
            return Task.Run(() => _blackListService.GetBlacklist(id: id, userId: userid, deviceId: deviceId, startDate: startDate,
                endDate: endDate, isDeleted: isDeleted).Data.Data);
        }

        [HttpPost]
        [Route("ChangeBlackList")]
        public Task<ResultViewModel> ChangeBlackList([FromBody] BlackList blackList)
        {
            return Task.Run(() =>
            {

                //var restRequest = new RestRequest($"Queries/v2/blackList", Method.PUT);
                //restRequest.AddJsonBody("blackList", blackList.ToString() ?? string.Empty);
                //var result = (_restClient.ExecuteAsync<ResultViewModel>(restRequest)).Result.Data;
                var result = _blackListService.ChangeBlackList(blackList);

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlacklist(id:(int)result.Id).Data.Data;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand?.Name != null)
                        {
                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
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

                //var restRequest = new RestRequest($"Queries/v2/blackList", Method.DELETE);
                //restRequest.AddQueryParameter("id", id.ToString());
                //var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                //var result = (requestResult.Result.Data);
                var result = _blackListService.DeleteBlackList(id);
                

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlacklist(id:(int)result.Id,isDeleted:true).Data.Data;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand != null)
                        {
                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
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
