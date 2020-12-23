using System.Collections.Generic;

namespace Biovation.Domain.RelayControllerModels
{
    public class Relay
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NodeNumber { get; set; }
        //public Tuple<Enum, DateTime> LastState { get; set; }
        public RelayHub Hub { get; set; }
        public Entrance Entrance { get; set; }
        public List<Scheduling> Schedulings { get; set; }
        public string Description { get; set; }
    }
}
