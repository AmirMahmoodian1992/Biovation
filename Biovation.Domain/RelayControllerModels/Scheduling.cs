using System;

namespace Biovation.Domain.RelayControllerModels
{
    public class Scheduling
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Enum Mode { get; set; }
    }
}