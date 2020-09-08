using System;
using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
{
    public class BlackListService
    {
        private readonly BlackListRepository _blackListRepository;

        public BlackListService(BlackListRepository blackListRepository)
        {
            _blackListRepository = blackListRepository;
        }

        public ResultViewModel<PagingResult<BlackList>> GetBlacklist(int id = default, int userId = default,
            int deviceId = 0, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default,
            int pageNumber = default, int pageSize = default)
        {
            return _blackListRepository.GetBlacklist(id, userId, deviceId, startDate, endDate, isDeleted, pageNumber,
                pageSize);
        }
        public ResultViewModel CreateBlackList(BlackList blackList = default)
        {
            return _blackListRepository.CreateBlackList(blackList);
        }

        public ResultViewModel DeleteBlackList(int id = default)
        {
            return _blackListRepository.DeleteBlackList(id);
        }
        public ResultViewModel DeleteBlackLists(List<uint> ids = default)
        {
            return _blackListRepository.DeleteBlackLists(ids);
        }

        public ResultViewModel ChangeBlackList(BlackList blackList = default)
        {
            return _blackListRepository.ChangeBlackList(blackList);
        }




    }
}
