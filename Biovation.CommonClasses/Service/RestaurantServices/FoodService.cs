using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository.RestaurantRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Models.RestaurantModels;

namespace Biovation.CommonClasses.Service.RestaurantServices
{
    public class FoodService
    {
        private readonly FoodRepository _foodRepository;

        public FoodService(FoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        public Task<List<Food>> GetFoods(int foodId = default)
        {
            return Task.Run(() => _foodRepository.GetFoods(foodId));
        }

        public Task<List<ResultViewModel>> ModifyFoods(List<Food> foods)
        {
            return Task.Run(() =>
            {
                var result = new List<ResultViewModel>();

                foreach (var food in foods)
                {
                    try
                    {
                        result.Add(_foodRepository.ModifyFood(food));
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
