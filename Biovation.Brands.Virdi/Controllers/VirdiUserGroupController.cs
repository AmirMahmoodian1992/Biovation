﻿using Biovation.CommonClasses;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Biovation.Brands.Virdi.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiUserGroupController : ControllerBase
    {
        private readonly Callbacks _callbacks;

        public VirdiUserGroupController(Callbacks callbacks)
        {
            _callbacks = callbacks;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyUserGroupMember([FromBody] List<UserGroupMember> member)
        {
            try
            {
                _callbacks.LoadFingerTemplates().ConfigureAwait(false);
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
