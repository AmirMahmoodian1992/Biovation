//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using Biovation.CommonClasses;
//using Biovation.CommonClasses.Manager;
//using Biovation.CommonClasses.Models;
//using Microsoft.AspNetCore.Mvc;
//using RestSharp;

//namespace Biovation.Gateway.Controllers.v1
//{
//    [Route("biovation/api/v{version:apiVersion}/[controller]")]
//    [ApiVersion("1.0")]
//    [ApiController]
//    public class SystemInfoController : Controller
//    {
//        private readonly RestClient _restClient;

//        public SystemInfoController()
//        {
//            _restClient = (RestClient)new RestClient($"http://localhost:{ConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
//        }

//        [HttpGet]
//        [Route("LoadedBrand")]
//        public Task<ResultViewModel<SystemInfo>> LoadedBrand()
//        {
//            return Task.Run(async () =>
//            {
//                try
//                {
//                    var systemInfo = new SystemInfo();
//                    var brandList = Lookups.DeviceBrands;
//                    systemInfo.Modules=new List<ModuleInfo>();

//                    foreach (var brand in brandList)
//                    {
//                        var restRequest = new RestRequest($"{brand.Name}/{brand.Name}SystemInfo/GetInfo");
//                        var requestResult = await _restClient.ExecuteAsync<ResultViewModel<ModuleInfo>>(restRequest);

//                        if (requestResult.StatusCode != HttpStatusCode.OK) continue;

//                        var res = requestResult.Data;
//                        res.Validate = string.IsNullOrEmpty(res.Message) ? 1 : res.Validate;

//                        systemInfo.Modules.Add(res.Data);
//                    }

//                    return new ResultViewModel<SystemInfo> { Validate = 1, Data = systemInfo };
//                }
//                catch (Exception e)
//                {
//                    return new ResultViewModel<SystemInfo> { Validate = 0, Message = e.Message };
//                }
//            });
//        }


//        [HttpPost]
//        [Route("RestartModules")]
//        public Task<List<ResultViewModel<ModuleInfo>>> RestartModules(List<ModuleInfo> modules)
//        {
//            return Task.Run(() =>
//            {
//                var resultList = new List<ResultViewModel<ModuleInfo>>();

//                var moduleList = KernelManager.ModulesArray.ToList();

//                foreach (var module in modules)
//                {

//                    var moduleInfo = moduleList.FirstOrDefault(brand => brand.GetBrandName() == module.Name);
//                    if (moduleInfo == null)
//                    {
//                        resultList.Add(new ResultViewModel<ModuleInfo> { Validate = 1, Message = $"Module : {module.Name } Not Loaded", Data = module });
//                        continue;
//                    }
//                    try
//                    {
//                        moduleInfo.StopService();
//                        moduleInfo.StartService();
//                        Logger.Log($"Module : {moduleInfo.GetBrandName() } Restart");

//                        resultList.Add(new ResultViewModel<ModuleInfo> { Validate = 1, Message = $"Module : {moduleInfo.GetBrandName() } Restart", Data = modules.FirstOrDefault(brand => brand.Name == moduleInfo.GetBrandName()) });

//                    }
//                    catch (Exception e)
//                    {
//                        resultList.Add(new ResultViewModel<ModuleInfo> { Validate = 0, Message = e.Message, Data = modules.FirstOrDefault(brand => brand.Name == moduleInfo.GetBrandName()) });
//                        Console.WriteLine(e);
//                        throw;
//                    }
//                }

//                return resultList;
//            });
//        }
//    }
//}
