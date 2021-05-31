using Biovation.Brands.Suprema.Commands;
using Biovation.Brands.Suprema.Services;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Biovation.Brands.Suprema.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class AccessGroupController : ControllerBase
    {
        private readonly FastSearchService _fastSearchService;
        private readonly AccessGroupService _accessGroupServices;

        public AccessGroupController(AccessGroupService accessGroupServices, FastSearchService fastSearchService)
        {
            _accessGroupServices = accessGroupServices;
            _fastSearchService = fastSearchService;
        }

        [HttpGet]
        [Authorize]
        public List<AccessGroup> AccessGroups()
        {
            var accessGroups = _accessGroupServices.GetAccessGroups();
            return accessGroups;
        }

        [HttpGet]
        [Authorize]
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            return new ResultViewModel { Validate = 1, Message = code.ToString() };
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyAccessGroup(string accessGroup)
        {
            try
            {
                _fastSearchService.Initial();
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }
    }
}
