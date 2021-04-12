using Biovation.CommonClasses;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Biovation.Brands.Paliz.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class PalizUserGroupController : ControllerBase
    {
        //private readonly VirdiServer _virdiServer;

        public PalizUserGroupController(/*VirdiServer virdiServer*/)
        {
           // _virdiServer = virdiServer;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyUserGroupMember([FromBody] List<UserGroupMember> member)
        {
            try
            {
                //_virdiServer.LoadFingerTemplates().ConfigureAwait(false);
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
