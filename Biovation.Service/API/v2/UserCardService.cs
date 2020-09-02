using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
{
    public class UserCardService
    {
        private readonly UserCardRepository _userCardRepository;

        public UserCardService(UserCardRepository userCardRepository)
        {
            _userCardRepository = userCardRepository;
        }

        public ResultViewModel<PagingResult<UserCard>> GetCardsByFilter(long userId, bool isActive,
            int pageNumber = default, int pageSize = default)
        {
            return _userCardRepository.GetCardsByFilter(userId, isActive, pageNumber, pageSize);
        }

        public int ReadCardNumber(string brandName = default, int deviceId = default)
        {
            return _userCardRepository.ReadCardNumber(brandName, deviceId);
        }


    }
}
