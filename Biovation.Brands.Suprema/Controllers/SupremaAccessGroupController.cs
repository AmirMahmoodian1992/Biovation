using System;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System.Collections.Generic;
using System.Web.Http;
using Biovation.Brands.Suprema.Commands;
using Biovation.Brands.Suprema.Service;
using Biovation.CommonClasses;

namespace Biovation.Brands.Suprema.ApiControllers
{
    public class SupremaAccessGroupController : ApiController
    {
        private readonly AccessGroupService _accessGroupServices = new AccessGroupService();

        [HttpGet]
        public List<AccessGroup> AccessGroups()
        {
            var accessGroups = _accessGroupServices.GetAllAccessGroups();
            return accessGroups;
        }

        [HttpGet]
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            var sendAccessGroupCommand = CommandFactory.Factory(CommandType.SendAccessGroupToDevice,
                new List<object> { code, accessGroupId });

            var result = (bool)sendAccessGroupCommand.Execute();

            return new ResultViewModel { Validate = result ? 1 : 0, Message = code.ToString() };
        }

        [HttpPost]
        public ResultViewModel ModifyAccessGroup(string accessGroup)
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
