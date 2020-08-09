using Biovation.CommonClasses.Models;
using DataAccessLayerCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DataAccessLayerCore.Repositories;

namespace Biovation.CommonClasses.Repository
{
    public class UserCardRepository
    {
        private readonly GenericRepository _repository;

        public UserCardRepository()
        {
            _repository = new GenericRepository();
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>ذخیره اطلاعات کارت کاربر.</Fa>
        /// </summary>
        /// <param name="userCard"></param>
        /// <returns></returns>
        public ResultViewModel ModifyUserCard(UserCard userCard)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", userCard.Id),
                new SqlParameter("@UserId", userCard.UserId),
                new SqlParameter("@CardNum", userCard.CardNum),
                new SqlParameter("@DataCheck", userCard.DataCheck),
            };
            return _repository.ToResultList<ResultViewModel>("ModifyUserCard", parameters).Data.FirstOrDefault();
        }
        
        public List<UserCard> GetActiveUserCard(long userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
            };
            return _repository.ToResultList<UserCard>("SelectActiveUserCardByUserId", parameters).Data;
        }

        public User FindUserByCardNumber(string cardNumber)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CardNumber", cardNumber),
            };
            return _repository.ToResultList<User>("SelectUserByCardNumber", parameters).Data.FirstOrDefault();
        }

        public List<User> FindUsersByCardNumber(string cardNumber)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CardNumber", cardNumber),
            };
            return _repository.ToResultList<User>("SelectUserByCardNumber", parameters).Data;
        }

        public List<UserCard> GetAllUserCardsOfUser(int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
            };
            return _repository.ToResultList<UserCard>("SelectUserCardByUserId", parameters).Data;
        }

        public ResultViewModel DeleteUserCard(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@id", id),
            };
            return _repository.ToResultList<ResultViewModel>("DeleteUserCard", parameters).Data.FirstOrDefault();
        }
    }
}
