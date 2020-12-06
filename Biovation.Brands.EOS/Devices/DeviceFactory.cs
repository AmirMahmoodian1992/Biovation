using Biovation.Brands.EOS.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using EosClocks;
using RestSharp;
using System.Collections.Generic;
using Serilog;
using Biovation.CommonClasses.Manager;

namespace Biovation.Brands.EOS.Devices
{
    /// <summary>
    /// ساخت و بازگردانی یک نمونه ساعت، با توجه به نوع مورد نیاز
    /// </summary>
    public class DeviceFactory
    {
        private readonly ILogger _logger;
        private readonly LogEvents _logEvents;
        private readonly TaskTypes _taskTypes;
        private readonly RestClient _restClient;
        private readonly LogService _logService;
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly LogSubEvents _logSubEvents;
        private readonly DeviceService _deviceService;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly EosCodeMappings _eosCodeMappings;
        private readonly UserCardService _userCardService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly AccessGroupService _accessGroupService;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;

        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public const int StPro = 2001;
        public const int StProPlus = 2002;
        public const int StShineL = 2003;
        public const int StShineM = 2004;
        public const int StFace110 = 2005;
        public const int StFace120 = 2006;
        public const int StFace130 = 2007;
        public const int StFace160 = 2008;
        public const int StFace710 = 2009;
        public const int StEco210 = 2010;
        public const int StP220 = 2011;

        public DeviceFactory(LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, FaceTemplateTypes faceTemplateTypes, UserCardService userCardService, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, RestClient restClient, TaskManager taskManager, Dictionary<uint, Device> onlineDevices, TaskService taskService, UserService userService, DeviceService deviceService, AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, BiovationConfigurationManager biovationConfigurationManager, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands, ILogger logger, LogService logService)
        {
            _logger = logger;
            _taskTypes = taskTypes;
            _logEvents = logEvents;
            _restClient = restClient;
            _logService = logService;
            _taskManager = taskManager;
            _taskService = taskService;
            _userService = userService;
            _deviceBrands = deviceBrands;
            _logSubEvents = logSubEvents;
            _taskStatuses = taskStatuses;
            _onlineDevices = onlineDevices;
            _deviceService = deviceService;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _eosCodeMappings = eosCodeMappings;
            _userCardService = userCardService;
            _faceTemplateTypes = faceTemplateTypes;
            _accessGroupService = accessGroupService;
            _faceTemplateService = faceTemplateService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
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
            switch (device.ModelId)
            {
                case StPro:
                case StProPlus:
                    {
                        return new SupremaBaseDevice(device, _logService, _logEvents, _logSubEvents, _eosCodeMappings, _biometricTemplateManager, _fingerTemplateTypes, _taskManager, _restClient, _onlineDevices);
                    }

                case StFace110:
                case StFace710:
                    {
                        return new HanvonBase(device, _logService, _logEvents, _logSubEvents, _eosCodeMappings, _faceTemplateTypes, _taskManager, _restClient, _onlineDevices);
                    }

                case StShineL:
                    {
                        return new StShineDevice(ProtocolType.Suprema, device, _logService, _logEvents, _logSubEvents, _eosCodeMappings, _biometricTemplateManager, _fingerTemplateTypes, _taskManager, _restClient, _onlineDevices, _logger);
                    }

                case StShineM:
                    {
                        return new StShineDevice(ProtocolType.Zk, device, _logService, _logEvents, _logSubEvents, _eosCodeMappings, _biometricTemplateManager, _fingerTemplateTypes, _taskManager, _restClient, _onlineDevices, _logger);
                    }

                case StFace120:
                case StFace130:
                case StFace160:
                case StEco210:
                case StP220:
                    {
                        return new ZkBaseDevice(device, _logService,_eosCodeMappings,_taskService, _userService, _deviceService, _accessGroupService, _userCardService, _faceTemplateService, _restClient, _onlineDevices, _biovationConfigurationManager, _logEvents, _logSubEvents, _taskTypes, _taskPriorities, _taskStatuses, _taskItemTypes, _deviceBrands, _taskManager, _biometricTemplateManager, _fingerTemplateTypes, _faceTemplateTypes);
                    }

                default:
                    return new Device(device, _logEvents, _logSubEvents, _eosCodeMappings);
            }
        }
    }
}
