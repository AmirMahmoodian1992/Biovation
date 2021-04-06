using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using KasraLockRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class SystemInfoController : ControllerBase
    {

        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public SystemInfoController(BiovationConfigurationManager biovationConfigurationManager)
        {
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        [HttpGet]
        [Route("LockStatus")]
        public Task<ResultViewModel<SystemInfo>> LockStatus()
        {
            return Task.Run(() =>
            {
                try
                {
                    var requests = new KasraLockRequests.Requests();

                    var response = new CoconutServiceResultModel();
                    if (_biovationConfigurationManager.SoftwareLockAddress != default &&
                        _biovationConfigurationManager.SoftwareLockPort != default)
                    {
                        response = requests.RequestInfo(_biovationConfigurationManager.SoftwareLockAddress, _biovationConfigurationManager.SoftwareLockPort, "info", "1", "1");
                    }
                    if (response == null)
                    {
                        return new ResultViewModel<SystemInfo>
                        {
                            Validate = 0,
                            Data = new SystemInfo() { LockEndTime = null },
                        };

                    }

                    JObject lockInfo;
                    try
                    {
                        lockInfo = JsonConvert.DeserializeObject<JObject>(response.Message);
                    }
                    catch (Exception)
                    {
                        return new ResultViewModel<SystemInfo>
                        {
                            Validate = 0,
                            Data = new SystemInfo() { LockEndTime = null },
                        };
                    }

                    var lockActivationStatus = false;
                    string expirationDate = null;
                    if (!(lockInfo is null))
                    {
                        var subsystemsInfo =
                            JsonConvert.DeserializeObject<JArray>(lockInfo["SubSystems"]?.ToString() ?? string.Empty);
                        foreach (var subsystemInfo in subsystemsInfo)
                        {
                            if (!string.Equals(subsystemInfo["SubSystemId"]?.ToString(), "92"
                                , StringComparison.InvariantCultureIgnoreCase)) continue;
                            lockActivationStatus = true;
                            if (subsystemInfo["ExpirationDate"]?.ToString() != null)
                                expirationDate = subsystemInfo["ExpirationDate"].ToString();
                        }

                    }

                    if (!lockActivationStatus || expirationDate == null)
                    {
                        return new ResultViewModel<SystemInfo>
                        {
                            Validate = 0,
                            Data = new SystemInfo() { LockEndTime = null },
                        };
                    }

                    if (DateTime.Parse(expirationDate) >= DateTime.Now)
                    {
                        return new ResultViewModel<SystemInfo>
                        {
                            Validate = 1,
                            Data = new SystemInfo() {LockEndTime = expirationDate},
                        };
                    }
                    return new ResultViewModel<SystemInfo>
                    {
                        Validate = 0,
                        Data = new SystemInfo() { LockEndTime = expirationDate },
                    };
                }
                catch (Exception)
                {
                    return new ResultViewModel<SystemInfo>
                    {
                        Validate = 0,
                        Data = new SystemInfo() { LockEndTime = null },
                    };
                }
            });
        }

        //[HttpGet]
        //[Route("LoadedBrand")]
        //public Task<ResultViewModel<SystemInfo>> LoadedBrand()
        //{
        //    return Task.Run(async () =>
        //    {
        //        try
        //        {
        //            var systemInfo = new SystemInfo();
        //            var brandList = Lookups.DeviceBrands;
        //            systemInfo.Modules = new List<ModuleInfo>();

        //            foreach (var brand in brandList)
        //            {
        //                var restRequest = new RestRequest($"{brand.Name}/{brand.Name}SystemInfo/GetInfo");
        //                var requestResult = await _restClient.ExecuteAsync<ResultViewModel<ModuleInfo>>(restRequest);

        //                if (requestResult.StatusCode != HttpStatusCode.OK) continue;

        //                var res = requestResult.Data;
        //                res.Validate = string.IsNullOrEmpty(res.Message) ? 1 : res.Validate;

        //                systemInfo.Modules.Add(res.Data);
        //            }

        //            return new ResultViewModel<SystemInfo> { Validate = 1, Data = systemInfo };
        //        }
        //        catch (Exception e)
        //        {
        //            return new ResultViewModel<SystemInfo> { Validate = 0, Message = e.Message };
        //        }
        //    });
        //}


        //[HttpPost]
        //[Route("RestartModules")]
        //public Task<List<ResultViewModel<ModuleInfo>>> RestartModules(List<ModuleInfo> modules)
        //{
        //    return Task.Run(() =>
        //    {
        //        var resultList = new List<ResultViewModel<ModuleInfo>>();

        //        var moduleList = KernelManager.ModulesArray.ToList();

        //        foreach (var module in modules)
        //        {

        //            var moduleInfo = moduleList.FirstOrDefault(brand => brand.GetBrandName() == module.Name);
        //            if (moduleInfo == null)
        //            {
        //                resultList.Add(new ResultViewModel<ModuleInfo> { Validate = 1, Message = $"Module : {module.Name } Not Loaded", Data = module });
        //                continue;
        //            }
        //            try
        //            {
        //                moduleInfo.StopService();
        //                moduleInfo.StartService();
        //                Logger.Log($"Module : {moduleInfo.GetBrandName() } Restart");

        //                resultList.Add(new ResultViewModel<ModuleInfo> { Validate = 1, Message = $"Module : {moduleInfo.GetBrandName() } Restart", Data = modules.FirstOrDefault(brand => brand.Name == moduleInfo.GetBrandName()) });

        //            }
        //            catch (Exception e)
        //            {
        //                resultList.Add(new ResultViewModel<ModuleInfo> { Validate = 0, Message = e.Message, Data = modules.FirstOrDefault(brand => brand.Name == moduleInfo.GetBrandName()) });
        //                Console.WriteLine(e);
        //                throw;
        //            }
        //        }

        //        return resultList;
        //    });
        //}
    }
}
