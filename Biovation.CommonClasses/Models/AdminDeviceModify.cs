using System;
using System.Collections.Generic;

namespace Biovation.CommonClasses.Models
{
    public class AdminDeviceModify
    {

        public int PersonId { get; set; }
        public List<AdminDevice> Devices { get; set; }

    }

}
public class AdminDevice
{
    public Int64 GroupDeviceId { get; set; }
    public int TypeId { get; set; }
}
