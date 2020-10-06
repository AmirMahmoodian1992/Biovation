using Biovation.Brands.Suprema.Devices.Suprema_Version_1;
using Biovation.Brands.Suprema.Model;

namespace Biovation.Brands.Suprema.Devices
{
    /// <summary>
    /// ساخت و بازگردانی یک نمونه ساعت، با توجه به نوع مورد نیاز
    /// </summary>
    public class DeviceFactory
    {
        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        /// <param name="device">اطلاعات کامل دستگاه</param>
        /// <param name="clientConnection"></param>
        /// <returns>Device object</returns>
        public static Device Factory(SupremaDeviceModel device/*, ClientConnection clientConnection = null*/)
        {
            switch (device.DeviceTypeId)
            {
                //case BSSDK.BS_DEVICE_BIOMINI_CLIENT:
                //    {
                //        return new BiominiClient(device, clientConnection);
                //    }
                case BSSDK.BS_DEVICE_FSTATION:
                    {
                        return new FaceStation(device);
                    }

                case BSSDK.BS_DEVICE_BIOSTATION:
                    {
                        return new BioStation(device);
                    }

                case BSSDK.BS_DEVICE_BIOSTATION2:
                    {
                        return new BioStationT2(device);
                    }

                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    {
                        return new OtherDevices(device);
                    }
                default:
                    return null;
            }
        }
    }
}
