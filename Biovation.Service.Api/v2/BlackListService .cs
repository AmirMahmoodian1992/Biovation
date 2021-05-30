using System;
using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
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
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _blackListRepository.GetBlacklist(id, userId, deviceId, startDate, endDate, isDeleted, pageNumber,
                pageSize, token);
        }
        public ResultViewModel CreateBlackList(BlackList blackList = default, string token = default)
        {
            return _blackListRepository.CreateBlackList(blackList, token);
        }

        public ResultViewModel DeleteBlackList(int id = default, string token = default)
        {
            return _blackListRepository.DeleteBlackList(id, token);
        }
        public ResultViewModel DeleteBlackLists(List<uint> ids = default, string token = default)
        {
            return _blackListRepository.DeleteBlackLists(ids, token);
        }

        public ResultViewModel ChangeBlackList(BlackList blackList = default, string token = default)
        {
            return _blackListRepository.ChangeBlackList(blackList, token);
        }
        
        // TODO - Verify the method.
        public void SendBlackListDevice(string brandName, object list, string token = default)
        { 
            _blackListRepository.SendBlackListDevice(brandName, list, token);
        }

        // TODO - Verify the method.
        public void SendBlackListDevice(string brandName, List<BlackList> successBlackList, string token = default)
        {
            _blackListRepository.SendBlackListDevice(brandName, successBlackList, token);
        }
    }
}
