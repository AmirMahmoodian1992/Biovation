using System;
using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class BrokerMessageData<T>
    {
        [Id]
        public string Id { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public List<T> Data { get; set; }

        public int EventId { get; set; }

        public string SourceName { get; set; }

        public string SourceDatabaseName { get; set; }
    }
}
