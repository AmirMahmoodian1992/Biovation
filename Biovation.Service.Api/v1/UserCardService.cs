using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;

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
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _userCardRepository.GetCardsByFilter(userId, isActive, pageNumber, pageSize, token)?.Data?.Data ?? new List<UserCard>();
        }

        public User FindUserByCardNumber(string cardNumber = default, string token = default)
        {
            return _userCardRepository.FindUserByCardNumber(cardNumber, token)?.Data ?? new User();
        }

        public ResultViewModel ModifyUserCard(UserCard card = default, string token = default)
        {
            return _userCardRepository.ModifyUserCard(card, token);
        }

        public ResultViewModel DeleteUserCard(int id = default, string token = default)
        {
            return _userCardRepository.DeleteUserCard(id, token);
        }

        public int ReadCardNumber(string brandName = default, int deviceId = default, string token = default)
        {
            return _userCardRepository.ReadCardNumber(brandName, deviceId, token).Data;
        }
    }
}
