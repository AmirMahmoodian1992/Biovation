using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RestaurantModels;
using Biovation.Repository.Sql.v1.RestaurantRepositories;

namespace Biovation.Service.Sql.v1.RestaurantServices
{
    public class RestaurantService
    {
        private readonly RestaurantRepository _restaurantRepository;

        public RestaurantService(RestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public Task<List<Restaurant>> GetRestaurants(int restaurantId = default(int))
        {
            return Task.Run(() => _restaurantRepository.GetRestaurants(restaurantId));
        }

        public Task<List<ResultViewModel>> ModifyRestaurants(List<Restaurant> restaurants)
        {
            return Task.Run(() =>
            {
                var result = new List<ResultViewModel>();

                foreach (var restaurant in restaurants)
                {
                    result.Add(_restaurantRepository.ModifyRestaurant(restaurant));
                }

                return result;
            });
        }
    }
}
