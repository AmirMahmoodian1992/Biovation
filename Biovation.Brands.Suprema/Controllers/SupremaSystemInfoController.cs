﻿using System.Linq;
using System.Reflection;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.Suprema.Controllers
{
    [Microsoft.AspNetCore.Components.Route("Biovation/Api/[controller]/[action]")]
    public class SupremaSystemInfoController : Controller
    {
        [HttpGet]
        public ResultViewModel<ServiceInfo> GetInfo()
        {
            var brandInfo = new ServiceInfo();
            var result = new ResultViewModel<ServiceInfo>();
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            if (name != null)
                brandInfo.Name = name.Split('.').LastOrDefault();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null)
                brandInfo.Version = version.ToString();
            result.Data = brandInfo;
            return result;
        }

    }
}