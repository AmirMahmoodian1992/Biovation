using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.Api.v1
{
    public class UserCardService
    {
        private readonly UserCardRepository _userCardRepository;

        public UserCardService(UserCardRepository userCardRepository)
        {
            _userCardRepository = userCardRepository;
        }

        public List<UserCard> GetCardsByFilter(long userId = default, bool isActive = default,
            int pageNumber = default, int pageSize = default)
        {
            return _userCardRepository.GetCardsByFilter(userId, isActive, pageNumber, pageSize).Data.Data;
        }

        public ResultViewModel<User> FindUserByCardNumber(string cardNumber = default)
        {
            return _userCardRepository.FindUserByCardNumber(cardNumber);
        }

        public ResultViewModel ModifyUserCard(UserCard card = default)
        {
            return _userCardRepository.ModifyUserCard(card);
        }

        public ResultViewModel DeleteUserCard(int id = default)
        {
            return _userCardRepository.DeleteUserCard(id);
        }

        public int ReadCardNumber(string brandName = default, int deviceId = default)
        {
            return _userCardRepository.ReadCardNumber(brandName, deviceId);
        }


    }
}
