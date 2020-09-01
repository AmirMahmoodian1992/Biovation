﻿using System;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Queries.Controllers.v2
{
    public class BlackListController : Controller
    {
        private readonly BlackListRepository _blackListRepository;


        public BlackListController(BlackListRepository blackListRepository)
        {
            _blackListRepository = blackListRepository;
        }
        [HttpGet]
        [Route("GetBlacklist")]
        public Task<ResultViewModel<PagingResult<BlackList>>> GetBlacklist(int id = default, int userId = default, int deviceId = 0, DateTime? startDate = null, DateTime? endDate = null,bool isDeleted = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _blackListRepository.GetBlacklist(id,userId,deviceId,startDate,endDate,isDeleted, pageNumber, pageSize));
        }
        
        [HttpGet]
        [Route("GetActiveBlacklist")]
        public Task<ResultViewModel<PagingResult<BlackList>>> GetActiveBlacklist(int id = default, int userId = default, int deviceId = 0, DateTime? today = null, bool isDeleted = default, int pageNumber = default, int pageSize = default)
        {
        return Task.Run(() => _blackListRepository.GetActiveBlacklist(id, userId, deviceId, today, isDeleted, pageNumber, pageSize));
    }

    }
}