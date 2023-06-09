﻿using Biovation.Brands.PW.Manager;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Serilog;

namespace Biovation.Brands.PW.Devices
{
    public class DeviceFactory
    {
        private readonly ILogger _logger;
        private readonly LogEvents _logEvents;
        private readonly LogService _logService;
        private readonly TaskService _taskService;
        private readonly LogSubEvents _logSubEvents;
        private readonly DeviceBrands _deviceBrands;
        private readonly PwCodeMappings _pwCodeMappings;

        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public DeviceFactory(LogEvents logEvents, LogSubEvents logSubEvents, PwCodeMappings pwCodeMappings,
            BiovationConfigurationManager biovationConfigurationManager, LogService logService, ILogger logger,
            DeviceBrands deviceBrands, TaskService taskService)
        {
            _logger = logger;
            _deviceBrands = deviceBrands;
            _taskService = taskService;
            _logEvents = logEvents;
            _logService = logService;
            _logSubEvents = logSubEvents;
            _pwCodeMappings = pwCodeMappings;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        /// <param name="device">اطلاعات کامل دستگاه</param>
        /// <returns>Device object</returns>
        public Device Factory(DeviceBasicInfo device)
        {
            switch (device.Model.Id)
            {
                default:
                    return new Device(device, _biovationConfigurationManager, _logEvents, _logSubEvents,
                        _pwCodeMappings, _logService, _deviceBrands, _taskService, _logger);
            }
        }
    }
}
