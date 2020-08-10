using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System.Collections.Generic;

namespace Biovation.CommonClasses.Service
{
    public class UserCardService
    {
        public ResultViewModel ModifyUserCard(UserCard userCard)
        {
            var userRepository = new UserCardRepository();
            return userRepository.ModifyUserCard(userCard);
        }

        public List<UserCard> GetActiveUserCard(long userId)
        {
            var userRepository = new UserCardRepository();
            return userRepository.GetActiveUserCard(userId);
        }

        public User FindUserByCardNumber(string cardNumber)
        {
            var userRepository = new UserCardRepository();
            return userRepository.FindUserByCardNumber(cardNumber);
        }

        public List<User> FindUsersByCardNumber(string cardNumber)
        {
            var userRepository = new UserCardRepository();
            return userRepository.FindUsersByCardNumber(cardNumber);
        }

        public List<UserCard> GetAllUserCardsOfUser(int userId)
        {
            var userRepository = new UserCardRepository();
            return userRepository.GetAllUserCardsOfUser(userId);
        }

        public ResultViewModel DeleteUserCard(int userId)
        {
            var userRepository = new UserCardRepository();
            return userRepository.DeleteUserCard(userId);
        }
    }
}
