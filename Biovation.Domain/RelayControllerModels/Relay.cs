using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RelayControllerModels
{
    public class Relay
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public int NodeNumber { get; set; }
        //public Tuple<Enum, DateTime> LastState { get; set; }
        [OneToOne]
        public RelayHub RelayHub { get; set; }
        [OneToOne]
        public Entrance Entrance { get; set; }
        [OneToMany]
        public List<Scheduling> Schedulings { get; set; }
        public string Description { get; set; }
    }
}
