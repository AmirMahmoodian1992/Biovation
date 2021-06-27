using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class TaskItem
    {
        [Id]
        public int Id { get; set; }
        [OneToOne]
        public Lookup Status { get; set; }
        [OneToOne]
        public Lookup Priority { get; set; }
        [OneToOne]
        public Lookup TaskItemType { get; set; }
        public int DeviceId { get; set; }
        public int OrderIndex { get; set; }
        public bool IsParallelRestricted { get; set; }  
        public string Data { get; set; }
        public string Result { get; set; }
        public bool IsScheduled { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalCount { get; set; }
        public DateTimeOffset FinishedAt { get; set; }
        public DateTimeOffset ExecutionAt { get; set; }

        // [OneToOne]
        // public DeviceBrand DeviceBrand { get; set; }
    }
}
