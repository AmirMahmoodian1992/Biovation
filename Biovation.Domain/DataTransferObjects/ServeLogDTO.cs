using System;

namespace Biovation.Domain.DataTransferObjects
{
    public class ServeLogDTO
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public int FoodId { get; set; }
        public int MealId { get; set; }
        public int StatusId { get; set; }
        public int DeviceId { get; set; }
        public int Count { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsSynced { get; set; }
    }
}
