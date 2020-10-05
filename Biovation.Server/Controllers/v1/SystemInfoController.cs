﻿using Biovation.CommonClasses;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v1
{
    public class SystemInfoController : Controller
    {
        private readonly SystemInfo _systemInfo;
        private readonly RestClient _restClient;

        public SystemInfoController(RestClient restClient, SystemInfo systemInfo)
        {
            _systemInfo = systemInfo;
            _restClient = restClient;
        }

        [HttpGet]
        public Task<ResultViewModel<SystemInfo>> LoadedBrand()
        {
            return Task.Run(() => new ResultViewModel<SystemInfo> { Validate = 1, Data = _systemInfo });
        }


        [HttpPost]
        public Task<List<ResultViewModel<ServiceInfo>>> RestartServices(List<ServiceInfo> services)
        {
            return Task.Run(async () =>
            {
                var resultList = new List<ResultViewModel<ServiceInfo>>();

                var loadedServices = _systemInfo.Services;

                foreach (var service in services)
                {
                    var moduleInfo = loadedServices.FirstOrDefault(brand => string.Equals(brand.Name, service.Name));
                    if (moduleInfo == null)
                    {
                        resultList.Add(new ResultViewModel<ServiceInfo> { Validate = 1, Message = $"Module : {service.Name } Not Loaded", Data = service });
                        continue;
                    }
                    try
                    {
                        var restRequest = new RestRequest($"{moduleInfo.Name}/{moduleInfo.Name}Service/Restart", Method.POST);
                        await _restClient.ExecuteAsync(restRequest);

                        Logger.Log($"Module : {moduleInfo.Name} Restart");

                        resultList.Add(new ResultViewModel<ServiceInfo> { Validate = 1, Message = $"Module : {moduleInfo.Name} Restart", Data = services.FirstOrDefault(brand => brand.Name == moduleInfo.Name) });

                    }
                    catch (Exception e)
                    {
                        resultList.Add(new ResultViewModel<ServiceInfo> { Validate = 0, Message = e.Message, Data = services.FirstOrDefault(brand => brand.Name == moduleInfo.Name) });
                        Console.WriteLine(e);
                        throw;
                    }
                }

                return resultList;
            });
        }
    }
}
