using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public static class TaskPriorities
    {
        public const string LowestCode = "13001";
        public const string LowCode = "13002";
        public const string MediumCode = "13003";
        public const string HighCode = "13004";
        public const string ImmediateCode = "13005";

        public static Lookup Lowest = Lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, LowestCode));
        public static Lookup Low = Lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, LowCode));
        public static Lookup Medium = Lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, MediumCode));
        public static Lookup High = Lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, HighCode));
        public static Lookup Immediate = Lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, ImmediateCode));
    }
}
