using Biovation.Domain;
using System.Collections.Generic;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi
{
    public class VirdiServer
    {
        internal readonly UCSAPI UcsApi;

        private readonly Dictionary<uint, DeviceBasicInfo> _onlineDevices;


        public VirdiServer(UCSAPI ucsApi, Dictionary<uint, DeviceBasicInfo> onlineDevices)
        {
            UcsApi = ucsApi;
            _onlineDevices = onlineDevices;
        }

        public void StopServer()
        {
            UcsApi.ServerStop();
        }

        public Dictionary<uint, DeviceBasicInfo> GetOnlineDevices()
        {
            return _onlineDevices;
        }


    }
}
