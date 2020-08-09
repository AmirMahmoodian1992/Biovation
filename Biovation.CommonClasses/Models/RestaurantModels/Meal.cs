using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models.RestaurantModels
{
    public class Meal
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
