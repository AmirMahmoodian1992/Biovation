using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Service;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using EosClocks;
using RestSharp;
using System.Collections.Generic;
using Biovation.CommonClasses.Manager;

namespace Biovation.Brands.EOS.Devices
{
    /// <summary>
    /// ساخت و بازگردانی یک نمونه ساعت، با توجه به نوع مورد نیاز
    /// </summary>
    public class DeviceFactory
    {
        private readonly EosLogService _eosLogService;

        private readonly LogEvents _logEvents;
        private readonly TaskTypes _taskTypes;
        private readonly RestClient _restClient;
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;
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

        public DeviceFactory(EosLogService eosLogService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, FaceTemplateTypes faceTemplateTypes, UserCardService userCardService, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, RestClient restClient, TaskManager taskManager, Dictionary<uint, Device> onlineDevices, TaskService taskService, UserService userService, DeviceService deviceService, AccessGroupService accessGroupService, FaceTemplateService faceTemplateService, BiovationConfigurationManager biovationConfigurationManager, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands, MatchingTypes matchingTypes)
        {
            _logEvents = logEvents;
            _restClient = restClient;
            _taskManager = taskManager;
            _onlineDevices = onlineDevices;
            _taskService = taskService;
            _userService = userService;
            _deviceService = deviceService;
            _accessGroupService = accessGroupService;
            _faceTemplateService = faceTemplateService;
            _biovationConfigurationManager = biovationConfigurationManager;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _deviceBrands = deviceBrands;
            _matchingTypes = matchingTypes;
            _logSubEvents = logSubEvents;
            _eosLogService = eosLogService;
            _eosCodeMappings = eosCodeMappings;
            _fingerTemplateTypes = fingerTemplateTypes;
            _userCardService = userCardService;
            _faceTemplateTypes = faceTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
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
                        return new SupremaBaseDevice(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings, _biometricTemplateManager, _fingerTemplateTypes, _taskManager, _restClient, _onlineDevices);
                    }

                case StFace110:
                case StFace710:
                    {
                        return new HanvonBase(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings, _faceTemplateTypes, _userCardService, _taskManager, _restClient, _onlineDevices);
                    }

                case StShineL:
                    {
                        return new StShineDevice(ProtocolType.Hdlc, device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings, _biometricTemplateManager, _fingerTemplateTypes, _taskManager, _restClient, _onlineDevices);
                    }

                case StShineM:
                    {
                        return new StShineDevice(ProtocolType.Zk, device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings, _biometricTemplateManager, _fingerTemplateTypes, _taskManager, _restClient, _onlineDevices);
                    }

                case StFace120:
                case StFace130:
                case StFace160:
                case StEco210:
                case StP220:
                    {
                        return new ZkBaseDevice(device, _eosLogService,_eosCodeMappings,_taskService, _userService, _deviceService, _accessGroupService, _userCardService, _faceTemplateService, _restClient, _onlineDevices, _biovationConfigurationManager, _logEvents, _logSubEvents, _taskTypes, _taskPriorities, _taskStatuses, _taskItemTypes, _deviceBrands, _taskManager, _matchingTypes, _biometricTemplateManager, _fingerTemplateTypes, _faceTemplateTypes);
                    }

                default:
                    return new Device(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings);
            }
        }
    }
}
