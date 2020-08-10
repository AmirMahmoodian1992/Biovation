using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Gateway.Controllers.v1
{
    [Route("[controller]")]
    [ApiController]
    public class BlackListController : ControllerBase
    {

        private readonly BlackListService _blackListService = new BlackListService();
        private readonly RestClient _restClient;

        public BlackListController()
        {
            _restClient = (RestClient)new RestClient($"http://localhost:{ConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpPost]
        [Route("CreateBlackList")]
        public Task<List<ResultViewModel>> CreateBlackList(List<BlackList> blackLists)
        {

            return Task.Run(() =>
            {
                try
                {
                    var resultsBlackLists = _blackListService.CreateBlackList(blackLists);

                    Task.Run(async () =>
                    {
                        var successResult = new List<BlackList>();
                        foreach (var blackList in resultsBlackLists)
                        {
                            if (blackList.Validate == 1)
                            {
                                successResult.Add(_blackListService.GetBlackList((int)blackList.Id).Result
                                    .FirstOrDefault());
                            }
                        }

                        try
                        {

                            var groupByList = successResult.GroupBy(x => new { x.Device.Brand.Name }).ToList();

                            foreach (var list in groupByList)
                            {
                                var brandName = list.FirstOrDefault().Device.Brand.Name;
                                if (brandName == null) continue;
                                var restRequest =
                                    new RestRequest($"/{brandName}/{brandName}BlackList/SendBlackLisDevice",
                                        Method.POST);
                                restRequest.AddJsonBody(list);

                                var restResult = await _restClient.ExecuteTaskAsync<List<ResultViewModel>>(restRequest);

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
            return Task.Run(() => _blackListService.GetBlackList(id, userid, deviceId, startDate, endDate, isDeleted));
        }

        [HttpPost]
        [Route("ChangeBlackList")]
        public Task<ResultViewModel> ChangeBlackList([FromBody] BlackList blackList)
        {
            return Task.Run(() =>
            {
                var result = _blackListService.ChangeBlackList(blackList);

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlackList((int)result.Id).Result;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand?.Name != null)
                        {
                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);

                            var restResult = await _restClient.ExecuteTaskAsync<List<ResultViewModel>>(restRequest);

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
                var result = _blackListService.DeleteBlackList(id);

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlackList((int)result.Id, isDeleted: true).Result;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand != null)
                        {
                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);

                            var restResult = await _restClient.ExecuteTaskAsync<List<ResultViewModel>>(restRequest);

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
