using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using Biovation.Domain.RestaurantModels;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v1.RestaurantRepositories
{
    public class MealRepository
    {
        private readonly GenericRepository _repository;

        public MealRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public List<Meal> GetMeals(int mealId = default(int))
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@mealId", mealId)
            };

            return _repository.ToResultList<Meal>("SelectMealsById", parameters).Data;
        }

        public List<MealTiming> GetMealTimings(int mealTimingId = default(int))
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@mealTimingId", mealTimingId)
            };

            return _repository.ToResultList<MealTiming>("SelectMealTimingsById", parameters, fetchCompositions: true, compositionDepthLevel: 3).Data;
        }

        public List<MealTiming> GetMealTimingsByMealId(int mealId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@mealId", mealId)
            };

            return _repository.ToResultList<MealTiming>("SelectMealTimingsByMealId", parameters, fetchCompositions: true, compositionDepthLevel: 3).Data;
        }

        public ResultViewModel ModifyMeal(Meal meal)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", meal.Id),
                new SqlParameter("@Name", meal.Name),
                new SqlParameter("@Description", meal.Description),
            };

            return _repository.ToResultList<ResultViewModel>("ModifyMeal", parameters).Data.FirstOrDefault();
        }
    }
}
