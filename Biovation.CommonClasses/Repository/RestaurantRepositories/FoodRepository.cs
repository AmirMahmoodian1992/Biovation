using Biovation.CommonClasses.Models;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.CommonClasses.Models.RestaurantModels;

namespace Biovation.CommonClasses.Repository.RestaurantRepositories
{
    public class FoodRepository
    {
        private readonly GenericRepository _repository = new GenericRepository();

        public List<Food> GetFoods(int foodId = default(int))
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@foodId", foodId)
            };

            return _repository.ToResultList<Food>("SelectFoodsById", parameters).Data;
        }

        public ResultViewModel ModifyFood(Food food)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", food.Id),
                new SqlParameter("@Name", food.Name),
                new SqlParameter("@Description", food.Description),
            };

            return _repository.ToResultList<ResultViewModel>("ModifyFood", parameters).Data.FirstOrDefault();
        }
    }
}
