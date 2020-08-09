﻿using Biovation.CommonClasses.Models;
using DataAccessLayer.Repositories;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.CommonClasses.Models.RestaurantModels;

namespace Biovation.CommonClasses.Repository.RestaurantRepositories
{
    public class RestaurantRepository
    {
        private readonly GenericRepository _repository = new GenericRepository();

        public List<Restaurant> GetRestaurants(int restaurantId = default(int))
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@restaurantId", restaurantId)
            };

            return _repository.ToResultList<Restaurant>("SelectRestaurantsById", parameters, fetchCompositions: true).Data;
        }

        public ResultViewModel ModifyRestaurant(Restaurant restaurant)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", restaurant.Id),
                new SqlParameter("@Name", restaurant.Name),
                new SqlParameter("@Description", restaurant.Description),
            };

            return _repository.ToResultList<ResultViewModel>("ModifyFood", parameters).Data.FirstOrDefault();
        }
    }
}
