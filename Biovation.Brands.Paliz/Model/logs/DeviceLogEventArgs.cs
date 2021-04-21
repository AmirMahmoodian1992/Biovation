using PalizTiara.Api.Models;

namespace Biovation.Brands.Paliz.Model.logs
{
    public class DeviceLogEventArgs
    {
        public DeviceLogEventArgs()
        {

        }
        public DeviceLogEventArgs(DeviceLogModel deviceLogModel, bool result)
        {

        }

        public bool Result { get; set; }
        public DeviceLogModel DeviceLogModel { get; set; }
    }
}
