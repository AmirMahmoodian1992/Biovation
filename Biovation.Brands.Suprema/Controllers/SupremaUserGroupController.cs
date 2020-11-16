using Biovation.Brands.Suprema.Services;
using Biovation.CommonClasses;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Biovation.Brands.Suprema.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class SupremaUserGroupController : ControllerBase
    {
        private readonly FastSearchService _fastSearchService;

        public SupremaUserGroupController(FastSearchService fastSearchService)
        {
            _fastSearchService = fastSearchService;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyUserGroupMember([FromBody] List<UserGroupMember> member)
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
