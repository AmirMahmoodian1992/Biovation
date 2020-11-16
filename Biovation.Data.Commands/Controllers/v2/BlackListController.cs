﻿using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;


namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    public class BlackListController : ControllerBase
    {
        private readonly BlackListRepository _blackListRepository;

        public BlackListController(BlackListRepository blackListRepository)
        {
            _blackListRepository = blackListRepository;
        }
        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> CreateBlackList([FromBody]BlackList blackList)
        {
            return Task.Run(() => _blackListRepository.CreateBlackList(blackList));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public Task<ResultViewModel> DeleteBlackList(int id)
        {
            return Task.Run(() => _blackListRepository.DeleteBlackList(id));
        }

        [HttpDelete]
        [Route("    ")]
        [Authorize]
        public Task<ResultViewModel> DeleteBlackLists([FromBody] List<uint> ids = default)
        {
            return Task.Run(() => _blackListRepository.DeleteBlackLists(ids));
        }


        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ChangeBlackList([FromBody]BlackList blackList)
        {
            return Task.Run(() => _blackListRepository.ChangeBlackList(blackList));
        }

    }
}