﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Gateway.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class FingerController : Controller
    {
        private readonly FingerTemplateService _fingerTemplateService;

        public FingerController(FingerTemplateService fingerTemplateService)
        {
            _fingerTemplateService = fingerTemplateService;
        }


        [HttpGet]
        [Route("{userId}")]
        public Task<IActionResult> GetFingerTemplateByUserId(long userId = default, int templateIndex = default)
        {
            throw null;
        }

        [HttpPost]
        [Route("{userId}")]
        public Task<IActionResult> AddUserFingerTemplate([FromBody]FingerTemplate fingerTemplate = default)
        {
            throw null;
        }

        [HttpPatch]
        public Task<IActionResult> ModifyUserFingerTemplate([FromBody]FingerTemplate fingerTemplate =default)
        {
            throw null;
        }

        [HttpDelete]
        [Route("{userId}")]
        public Task<IActionResult> DeleteFingerTemplateByUserId(int userId = default, int templateIndex = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("TemplateCount")]
        public Task<IActionResult> GetTemplateCount()
        {
            throw null;
        }

        [HttpGet]
        [Route("FingerTemplateTypes")]
        public Task<IActionResult> GetFingerTemplateTypes(string brandId = default)
        {
            throw null;
        }
    }
}
