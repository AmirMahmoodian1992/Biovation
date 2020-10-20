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
    public class SupremaAccessGroupController : Controller
    {
        private readonly CommandFactory _commandFactory;
        private readonly FastSearchService _fastSearchService;
        private readonly AccessGroupService _accessGroupServices;

        public SupremaAccessGroupController(AccessGroupService accessGroupServices, CommandFactory commandFactory, FastSearchService fastSearchService)
        {
            _accessGroupServices = accessGroupServices;
            _commandFactory = commandFactory;
            _fastSearchService = fastSearchService;
        }

        [HttpGet]
        public List<AccessGroup> AccessGroups()
        {
            var accessGroups = _accessGroupServices.GetAccessGroups();
            return accessGroups;
        }

        [HttpGet]
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            var sendAccessGroupCommand = _commandFactory.Factory(CommandType.SendAccessGroupToDevice,
                new List<object> { code, accessGroupId });

            var result = (bool)sendAccessGroupCommand.Execute();

            return new ResultViewModel { Validate = result ? 1 : 0, Message = code.ToString() };
        }

        [HttpPost]
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
