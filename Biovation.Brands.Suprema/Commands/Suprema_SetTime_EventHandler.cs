using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaSetTime : ICommand
    {
        private int TimeToSet { get; }

        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;

        public SupremaSetTime(int timeToSet, Dictionary<uint, Device> onlineDevices)
        {
            TimeToSet = timeToSet;
            _onlineDevices = onlineDevices;
        }

        public object Execute()
        {
            //var timeToSet = Convert.ToInt32(items.First());

            var tasksDevice = new List<Task>();

            foreach (var device in _onlineDevices)
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
