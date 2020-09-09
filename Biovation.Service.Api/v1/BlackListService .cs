using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.Api.v1
{
    public class BlackListService
    {
        private readonly BlackListRepository _blackListRepository;

        public BlackListService(BlackListRepository blackListRepository)
        {
            _blackListRepository = blackListRepository;
        }

        public Task<List<BlackList>> GetBlacklist(int id = default, int userId = default,
            int deviceId = 0, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default,
            int pageNumber = default, int pageSize = default)
        {
            
            return Task.Run(() => _blackListRepository.GetBlacklist(id, userId, deviceId, startDate, endDate, isDeleted, pageNumber,
                pageSize).Data.Data);
        }
        public List<ResultViewModel> CreateBlackList(List<BlackList> blackLists = default)
        {
            return (blackLists ?? new List<BlackList>()).Select(blackList => _blackListRepository.CreateBlackList(blackList)).ToList();
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
