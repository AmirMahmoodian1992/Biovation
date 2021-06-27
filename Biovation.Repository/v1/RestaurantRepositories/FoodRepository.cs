using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using Biovation.Domain.RestaurantModels;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v1.RestaurantRepositories
{
    public class FoodRepository
    {
        private readonly GenericRepository _repository;

        public FoodRepository(GenericRepository repository)
        {
            _repository = repository;
        }

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
