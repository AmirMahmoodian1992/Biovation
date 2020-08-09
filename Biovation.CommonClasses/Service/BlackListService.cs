﻿using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Service
{
    public class BlackListService
    {
        private readonly BlackListRepository _blackListRepository = new BlackListRepository();

        public List<ResultViewModel> CreateBlackList(List<BlackList> blackLists)
        {
            var results = new List<ResultViewModel>();
            foreach (var blackList in blackLists)
            {
                results.Add(_blackListRepository.CreateBlackList(blackList));
            }

            return results;
        }
        public Task<List<BlackList>> GetBlackList(int id = default, int userid = default, int deviceId = default, DateTime? startDate = null, DateTime? endDate = null,bool isDeleted=default)
        {
            return Task.Run(() => _blackListRepository.GetBlacklist(id, userid, deviceId, startDate, endDate,isDeleted));
        }
        public Task<List<BlackList>> GetActiveBlackList(int id = default, int userid = default, int deviceId = default,DateTime? today = null, bool isDeleted = default)
        {
            return Task.Run(() => _blackListRepository.GetActiveBlacklist(id, userid, deviceId,today , isDeleted));
        }
        public ResultViewModel ChangeBlackList(BlackList blackList)
        {
            return _blackListRepository.ChangeBlackList(blackList);
        }
        public ResultViewModel DeleteBlackList(int id)
        {
            return _blackListRepository.DeleteBlackList(id);
        }
    }
}