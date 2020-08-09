using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models.RestaurantModels
{
    public class ServeLogStatus
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public static class ReservationStatuses
    {
        public const int ReservationAccepted = 1;
        public const int ReservationRejected = 2;
        public const int NotReservedAccepted = 3;
        public const int NotReservedRejected = 4;
    }
}
