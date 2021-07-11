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
        //private readonly PalizServer _palizServer;

        public PalizUserGroupController(/*PalizServer palizServer*/)
        {
           // _palizServer = palizServer;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyUserGroupMember([FromBody] List<UserGroupMember> member)
        {
            try
            {
                //_palizServer.LoadFingerTemplates().ConfigureAwait(false);
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
