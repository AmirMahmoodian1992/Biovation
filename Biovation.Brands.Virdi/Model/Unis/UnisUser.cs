using System;
using System.Globalization;
using DataAccessLayerCore.Attributes;

namespace Biovation.Brands.Virdi.Model.Unis
{
    public class UnisUser
    {
        [Id]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string StartDateString
        {
            get => StartDate.ToString("yyyyMMddHHmmss");
            set => StartDate = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }
        public DateTime StartDate { get; set; }
        public string EndDateString
        {
            get => EndDate.ToString("yyyyMMddHHmmss");
            set => EndDate = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }
        public DateTime EndDate { get; set; }
    }
}
