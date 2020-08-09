using System;
using DataAccessLayer.Attributes;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Biovation.CommonClasses.Models
{
    public class TimeZoneDetail
    {
        [Id]
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
    }
}
