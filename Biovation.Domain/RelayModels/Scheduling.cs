using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RelayModels
{
    public class Scheduling
    {

        [Id] public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        [OneToOne]
        public Lookup Mode { get; set; }
    }
}