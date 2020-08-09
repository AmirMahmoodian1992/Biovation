using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Models
{
    public class SystemInfo
    {
        public static string Name;
        public static string Version;

        public  string MinmumPatchVersionRequired = "9.2.6";
        public  List<ModuleInfo> Modules { get; set; }
      

    }
}