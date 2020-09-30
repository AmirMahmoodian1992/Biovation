using DataAccessLayerCore.Attributes;
using System;
using System.Collections.Generic;

namespace Biovation.Domain
{
    public class TaskInfo
    {
        [Id]
        public int Id { get; set; }
        [OneToOne]
        public Lookup Priority { get; set; }
        [OneToOne]
        public Lookup TaskType { get; set; }
        [OneToOne]
        public Lookup Status { get; set; }
        [OneToOne]
        public User CreatedBy { get; set; }
        /*[OneToOne]
        public DeviceBrand DeviceBrand { get; set; }*/
        public DateTimeOffset CreatedAt { get; set; }
        [OneToMany]
        public List<TaskItem> TaskItems { get; set; }
        [OneToOne]
        public Lookup DeviceBrand { get; set; }

        public int CurrentIndex { get; set; }
        public int TotalCount { get; set; }

        [OneToOne]
        public User UpdatedBy { get; set; }
        /*[OneToOne]
        public DeviceBrand DeviceBrand { get; set; }*/
        public DateTimeOffset UpdatedAt { get; set; }

        public string SchedulingPattern { get; set; }

        public DateTimeOffset QueuedAt { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public TaskInfo Parent { get; set; }
    }
}