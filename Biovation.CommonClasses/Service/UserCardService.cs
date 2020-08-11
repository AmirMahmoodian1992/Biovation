using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Biovation.CommonClasses.Service
{
    public class UserCardService
    {
        private readonly UserCardRepository _userCardRepository;

        public UserCardService(UserCardRepository userCardRepository)
        {
            _userCardRepository = userCardRepository;
        }

        public ResultViewModel ModifyUserCard(UserCard userCard)
        {
            return _userCardRepository.ModifyUserCard(userCard);
        }

        public List<UserCard> GetActiveUserCard(long userId)
        {
            return _userCardRepository.GetActiveUserCard(userId);
        }

        public User FindUserByCardNumber(string cardNumber)
        {
            return _userCardRepository.FindUserByCardNumber(cardNumber);
        }

        public List<User> FindUsersByCardNumber(string cardNumber)
        {
            return _userCardRepository.FindUsersByCardNumber(cardNumber);
        }

        public List<UserCard> GetAllUserCardsOfUser(int userId)
        {
            return _userCardRepository.GetAllUserCardsOfUser(userId);
        }

        public ResultViewModel DeleteUserCard(int userId)
        {
            return _userCardRepository.DeleteUserCard(userId);
        }
    }
}
