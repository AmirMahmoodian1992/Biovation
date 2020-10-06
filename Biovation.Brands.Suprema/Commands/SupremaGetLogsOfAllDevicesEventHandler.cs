using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biovation.Brands.Suprema.Devices;
using Biovation.SocketHandler;

namespace Biovation.Brands.Suprema.EventHandlers
{
    public class SupremaGetLogsOfAllDevicesEventHandler : EventDispatcher
    {
        /// <summary>
        /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
        /// </summary>
        /// <seealso cref="EventDispatcher" />
        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        /// <param name="itemsList">Unique user id on database</param>
        /// <param name="devices">All connected devices</param>
        public override object Dispatch(List<object> itemsList, Dictionary<uint, Device> devices)
        {
            //var deviceId = Convert.ToUInt32(itemsList[0]);
            //var startDate = itemsList[1].ToString();
            //var endDate = itemsList[2].ToString();

            foreach (var device in devices)
            {
                var refDate = new DateTime(1970, 1, 1).Ticks / 10000000;
                var startDateTicks = Convert.ToInt32(Convert.ToInt64(DateTime.Today.AddDays(-1).Ticks) / 10000000 - refDate);
                var endDateTicks = Convert.ToInt32(Convert.ToInt64(DateTime.Today.AddDays(1).Ticks) / 10000000 - refDate);


                devices[device.Value.GetDeviceInfo().DeviceId].ReadLogOfPeriod(startDateTicks, endDateTicks);
                Console.WriteLine("Device: {0} is discharged.", device.Value.GetDeviceInfo().DeviceId);
            }

            return null;
        }

        public override object Dispatch(List<object> itemsList, Dictionary<uint, Device> devices, ClientConnection senderSocket)
        {
            foreach (var device in devices)
            {
                var refDate = new DateTime(1970, 1, 1).Ticks / 10000000;
                var startDateTicks = Convert.ToInt32(Convert.ToInt64(DateTime.Today.AddDays(-1).Ticks) / 10000000 - refDate);
                var endDateTicks = Convert.ToInt32(Convert.ToInt64(DateTime.Today.AddDays(1).Ticks) / 10000000 - refDate);


                devices[device.Value.GetDeviceInfo().DeviceId].ReadLogOfPeriod(startDateTicks, endDateTicks);
                Console.WriteLine("Device: {0} is discharged.", device.Value.GetDeviceInfo().DeviceId);
            }

            return null;
        }
    }
}
