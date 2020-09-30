using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Domain.RestaurantModels;
using Biovation.Repository.Sql.v1.RestaurantRepositories;

namespace Biovation.Service.Sql.v1.RestaurantServices
{
    public class MealService
    {
        private readonly MealRepository _mealRepository;

        public MealService(MealRepository mealRepository)
        {
            _mealRepository = mealRepository;
        }

        public Task<List<Meal>> GetMeals(int mealId = default(int))
        {
            return Task.Run(() => _mealRepository.GetMeals(mealId));
        }

        public Task<List<MealTiming>> GetMealTimings(int mealTimingId = default(int))
        {
            return Task.Run(() => _mealRepository.GetMealTimings(mealTimingId));
        }

        public Task<List<MealTiming>> GetMealTimingsByMealId(int mealId)
        {
            return Task.Run(() => _mealRepository.GetMealTimingsByMealId(mealId));
        }

        public Task<List<ResultViewModel>> ModifyMeals(List<Meal> meals)
        {
            return Task.Run(() =>
            {
                var result = new List<ResultViewModel>();

                foreach (var meal in meals)
                {
                    try
                    {
                        result.Add(_mealRepository.ModifyMeal(meal));

                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                }

                return result;
            });
        }
    }
}
