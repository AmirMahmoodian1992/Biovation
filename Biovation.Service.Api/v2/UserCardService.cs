using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class UserCardService
    {
        private readonly UserCardRepository _userCardRepository;

        public UserCardService(UserCardRepository userCardRepository)
        {
            _userCardRepository = userCardRepository;
        }

        public ResultViewModel<PagingResult<UserCard>> GetCardsByFilter(long userId = default, bool isActive = default,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _userCardRepository.GetCardsByFilter(userId, isActive, pageNumber, pageSize, token);
        }

        public ResultViewModel<User> FindUserByCardNumber(string cardNumber = default, string token = default)
        {
            return _userCardRepository.FindUserByCardNumber(cardNumber, token);
        }

        public ResultViewModel ModifyUserCard(UserCard card = default, string token = default)
        {
            return _userCardRepository.ModifyUserCard(card, token);
        }

        public ResultViewModel DeleteUserCard(int id = default, string token = default)
        {
            return _userCardRepository.DeleteUserCard(id, token);
        }

        public ResultViewModel<int> ReadCardNumber(int deviceId = default, string token = default)
        {
            return _userCardRepository.ReadCardNumber(deviceId, token);
        }
    }
}
