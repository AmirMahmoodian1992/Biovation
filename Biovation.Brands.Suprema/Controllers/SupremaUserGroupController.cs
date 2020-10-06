using Biovation.Brands.Suprema.Service;
using Biovation.CommonClasses.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Biovation.CommonClasses;

namespace Biovation.Brands.Suprema.ApiControllers
{
    public class SupremaUserGroupController : ApiController
    {
        [HttpPost]
        public ResultViewModel ModifyUserGroupMember([FromBody] List<UserGroupMember> member)
        {
            try
            {
                FastSearchService.GetInstance().Initial();
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
