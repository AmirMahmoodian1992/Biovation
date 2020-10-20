﻿using Biovation.Domain;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات گزارش
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class SupremaLog : Log
    {

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public ushort nIsLog { get; set; }
        public uint Reserved { get; set; }
        public override uint DateTimeTicks
        {
            get => nDateTime;
            set
            {
                nDateTime = value;
                if (LogDateTime == default || LogDateTime == DateTime.MinValue || LogDateTime == DateTime.MaxValue)
                {
                    LogDateTime = new DateTime(1970, 1, 1, new GregorianCalendar()).AddTicks((long)value * 10000000);
                }
            }
        }

    }
}
