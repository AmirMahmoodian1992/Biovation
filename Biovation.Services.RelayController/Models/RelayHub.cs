using System;
using System.Collections.Generic;
using Biovation.Domain;

namespace Biovation.Services.RelayController.Models
{
    public class RelayHub
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int Capacity { get; set; }
        //public DeviceModel RelayHubModel { get; set; }
        //public DeviceModel RelayHubModel { get; set; }
        public string RelayHubModel { get; set; }
        public string Description { get; set; }

        public static implicit operator RelayHub(Entrance v)
        {
            throw new NotImplementedException();
        }
    }
}