﻿using DataAccessLayerCore.Attributes;
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
    }
}