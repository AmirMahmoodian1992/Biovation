using System.Collections.Generic;

namespace Biovation.Domain
{
    public class SystemInfo
    {
        public static string Name;
        public static string Version;

        public  string MinimumPatchVersionRequired = "9.2.6";
        public  List<ServiceInfo> Services { get; set; }
    }
}