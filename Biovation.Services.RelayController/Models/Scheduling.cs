using System;

namespace Biovation.Services.RelayController.Models
{
    public class Scheduling
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTiem { get; set; }
        public Enum Mode { get; set; }
    }
}