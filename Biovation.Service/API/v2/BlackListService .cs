using System;
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




    }
}
