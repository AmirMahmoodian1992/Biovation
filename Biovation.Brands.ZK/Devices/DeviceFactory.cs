using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using RestSharp;
using System.Collections.Generic;
using Serilog;

namespace Biovation.Brands.ZK.Devices
{
    public class DeviceFactory
    {
        private readonly ILogger _logger;
       
        private readonly RestClient _restClient;
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly UserCardService _userCardService;
        private readonly FaceTemplateService _faceTemplateService;

        private readonly LogEvents _logEvents;
        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly DeviceBrands _deviceBrands;
        private readonly MatchingTypes _matchingTypes;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly ZkCodeMappings _zkCodeMappings;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        private readonly LogService _logService;
        private readonly FingerTemplateService _fingerTemplateService;

        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        public DeviceFactory(LogEvents logEvents, LogService logService, UserService userService, TaskService taskService, MatchingTypes matchingTypes, DeviceService deviceService, AccessGroupService accessGroupService, Dictionary<uint, Device> onlineDevices, FingerTemplateService fingerTemplateService, UserCardService userCardService, FaceTemplateService faceTemplateService, RestClient restClient, ZkCodeMappings zkCodeMappings, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, FaceTemplateTypes faceTemplateTypes, BiovationConfigurationManager biovationConfigurationManager, ILogger logger)
        {
            _logEvents = logEvents;
            _logService = logService;
            _userService = userService;
            _taskService = taskService;
            _matchingTypes = matchingTypes;
            _deviceService = deviceService;
            _accessGroupService = accessGroupService;
            _onlineDevices = onlineDevices;
            _fingerTemplateService = fingerTemplateService;
            _userCardService = userCardService;
            _faceTemplateService = faceTemplateService;
            _restClient = restClient;
            _zkCodeMappings = zkCodeMappings;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _deviceBrands = deviceBrands;
            _biometricTemplateManager = biometricTemplateManager;
            _fingerTemplateTypes = fingerTemplateTypes;
            _faceTemplateTypes = faceTemplateTypes;
            _biovationConfigurationManager = biovationConfigurationManager;
            _logger = logger;
        }

        public Device Factory(DeviceBasicInfo device)
        {
            switch (device.Model.Id)
            {
                case DeviceModels.IFace202:
                    {
                        return new IFace(device, _taskService, _userService, _deviceService, _logService, _accessGroupService, _fingerTemplateService, _userCardService, _faceTemplateService, _restClient, _onlineDevices, _biovationConfigurationManager, _logEvents, _zkCodeMappings, _taskTypes, _taskPriorities, _taskStatuses, _taskItemTypes, _deviceBrands, _matchingTypes, _biometricTemplateManager, _fingerTemplateTypes, _faceTemplateTypes, _logger);
                    }

                case DeviceModels.BlackWhite:
                {
                    return new BlackWhite(device, _taskService, _userService, _deviceService, _logService, _accessGroupService, _fingerTemplateService, _userCardService, _faceTemplateService, _restClient, _onlineDevices, _biovationConfigurationManager, _logEvents, _zkCodeMappings, _taskTypes, _taskPriorities, _taskStatuses, _taskItemTypes, _deviceBrands, _matchingTypes, _biometricTemplateManager, _fingerTemplateTypes, _faceTemplateTypes, _logger);
                }

                default:

                    return new Device(device, _taskService, _userService, _deviceService, _logService, _accessGroupService, _fingerTemplateService, _userCardService, _faceTemplateService, _restClient, _onlineDevices, _biovationConfigurationManager, _logEvents, _zkCodeMappings, _taskTypes, _taskPriorities, _taskStatuses, _taskItemTypes, _deviceBrands, _matchingTypes, _biometricTemplateManager, _fingerTemplateTypes, _faceTemplateTypes, _logger);
            }
        }
    }
}
