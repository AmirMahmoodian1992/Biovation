using System;
using Biovation.Domain.DataMappers;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class Gadget
    {
        [Id]
        public long Id { get; set; }

        [DataMapper(Mapper = typeof(ToUIntMapper))]
        public uint Code { get; set; }
        public string Name { get; set; }
        [OneToOne]
        public Connection ConnectionInfo { get; set; }
        [OneToOne]
        public Lookup Brand { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// ورژن سخت افزار دستگاه
        /// </summary>
        public string HardwareVersion { get; set; }

        /// <summary>
        /// سریال دستگاه
        /// </summary>
        public string SerialNumber { get; set; }
    }
}