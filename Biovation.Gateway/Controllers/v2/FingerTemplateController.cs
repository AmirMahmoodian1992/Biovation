using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Gateway.Controllers.v2
{
    [Route("biovation/api/[controller]")]
    public class FingerController : Controller
    {
        private readonly FingerTemplateService _fingerTemplateService;

        public FingerController(FingerTemplateService fingerTemplateService)
        {
            _fingerTemplateService = fingerTemplateService;
        }


        [HttpGet]
        public List<FingerTemplate> GetFingerTemplateByUserId(long userId = default, int templateIndex = default)
        {
            throw null;
        }

        [HttpPut]
        public ResultViewModel ModifyUser(FingerTemplate fingerTemplate =default)
        {
            throw null;
        }

        [HttpDelete]
        public ResultViewModel DeleteFingerTemplateByUserId(int userId = default, int templateIndex = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("TemplateCount")]
        public List<UserTemplateCount> GetTemplateCount()
        {
            throw null;
        }

        [HttpGet]
        [Route("FingerTemplateTypes")]
        public Task<ResultViewModel<List<Lookup>>> GetFingerTemplateTypes(string brandId = default)
        {
            throw null;
        }
    }
}
