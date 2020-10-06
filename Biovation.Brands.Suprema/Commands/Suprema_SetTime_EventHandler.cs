using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaSetTime : ICommand
    {
        private int TimeToSet { get; }

        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        public SupremaSetTime(int timeToSet, Dictionary<uint, Device> devices)
        {
            OnlineDevices = devices;
            TimeToSet = timeToSet;
        }

        public object Execute()
        {
            //var timeToSet = Convert.ToInt32(items.First());

            var tasksDevice = new List<Task>();

            foreach (var device in OnlineDevices)
            {
                tasksDevice.Add(Task.Run(() =>
                {
                    device.Value.SetTime(TimeToSet);
                }));
            }

            Task.WaitAll(tasksDevice.ToArray());

            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get online devices command";
        }

        public string GetDescription()
        {
            return " Get suprema online devices command";
        }
    }
}
