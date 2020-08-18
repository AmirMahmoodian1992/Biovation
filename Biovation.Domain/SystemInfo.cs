using System.Collections.Generic;

namespace Biovation.Domain
{
    public class SystemInfo
    {
        public static string Name;
        public static string Version;

        public  string MinmumPatchVersionRequired = "9.2.6";
        public  List<ModuleInfo> Modules { get; set; }
      

    }
}