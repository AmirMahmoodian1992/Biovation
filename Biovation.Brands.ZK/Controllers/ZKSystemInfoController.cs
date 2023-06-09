﻿using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class ZkSystemInfoController : ControllerBase
    {
        [HttpGet]
        public ResultViewModel<ServiceInfo> GetInfo()
        {
            var brandInfo = new ServiceInfo();
            var result = new ResultViewModel<ServiceInfo>();
            brandInfo.Name = Assembly.GetExecutingAssembly().GetName().Name?.Split('.').LastOrDefault();
            brandInfo.Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            result.Data = brandInfo;
            return result;
        }

        [HttpGet]
        public void StopService()
        {
            Environment.Exit(0);
        }
    }
}