using Biovation.Domain;
using System.Linq;

namespace Biovation.Constants
{
    public class TaskPriorities
    {
        public const string LowestCode = "13001";
        public const string LowCode = "13002";
        public const string MediumCode = "13003";
        public const string HighCode = "13004";
        public const string ImmediateCode = "13005";

        public TaskPriorities(Lookups lookups)
        {
            Lowest = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, LowestCode));
            Low = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, LowCode));
            Medium = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, MediumCode));
            High = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, HighCode));
            Immediate = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, ImmediateCode));
        }

        public Lookup Lowest;
        public Lookup Low;
        public Lookup Medium;
        public Lookup High;
        public Lookup Immediate;
    }
}
