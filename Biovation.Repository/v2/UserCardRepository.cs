﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v2
{
    public class UserCardRepository
    {
        private readonly GenericRepository _repository;

        public UserCardRepository(GenericRepository repository)
        {
            _repository = repository;
        }



        public ResultViewModel<PagingResult<UserCard>> GetCardsByFilter(long userId, bool isactive, int pageNumber = default, int PageSize = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@IsActive", isactive),
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize",PageSize)
            };
            return _repository.ToResultList<PagingResult<UserCard>>("SelectUserCardByFilter", parameters, fetchCompositions: true, compositionDepthLevel: 3).FetchFromResultList();

        }


        public ResultViewModel AddUserCard(UserCard userCard)
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

        /* public List<UserCard> GetActiveUserCard(long userId)
         {
             var parameters = new List<SqlParameter>
             {
                 new SqlParameter("@UserId", userId),
             };
             return _repository.ToResultList<UserCard>("SelectActiveUserCardByUserId", parameters).Data;
         }*/

        public ResultViewModel<User> FindUserByCardNumber(string cardNumber)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CardNumber", cardNumber),
            };
            return _repository.ToResultList<User>("SelectUserByCardNumber", parameters).FetchFromResultList();
        }

        public List<User> FindUsersByCardNumber(string cardNumber)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CardNumber", cardNumber),
            };
            return _repository.ToResultList<User>("SelectUserByCardNumber", parameters).Data;
        }

        /* public List<UserCard> GetAllUserCardsOfUser(int userId)
          {
              var parameters = new List<SqlParameter>
              {
                  new SqlParameter("@UserId", userId),
              };
              return _repository.ToResultList<UserCard>("SelectUserCardByUserId", parameters).Data;
          }*/

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
