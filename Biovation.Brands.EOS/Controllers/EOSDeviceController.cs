using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.EOS.Commands;
using Biovation.Brands.EOS.Devices;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biovation.Brands.EOS.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class EosDeviceController : Controller
    {
        private readonly EosServer _eosServer;
        private readonly DeviceService _deviceService;
        private readonly UserService _userService;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly CommandFactory _commandFactory;


        public EosDeviceController(DeviceService deviceService, Dictionary<uint, Device> onlineDevices, EosServer eosServer, CommandFactory commandFactory, UserService userService)
        {
            _eosServer = eosServer;
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _userService = userService;
            _commandFactory = commandFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            foreach (var onlineDevice in _onlineDevices)
            {
                if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                {
                    onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.EosCode)?.FirstOrDefault()?.Name;
                }
                onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
            }

            return onlineDevices;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            if (device.Active)
            {
                _eosServer.ConnectToDevice(device);
            }

            else
            {
                _eosServer.DisconnectFromDevice(device);
            }

            return new ResultViewModel { Validate = 0, Id = device.DeviceId };
        }


        [HttpPost]
        [Authorize]
        public ResultViewModel DeleteUserFromDevice(uint code, [FromBody]Newtonsoft.Json.Linq.JArray userId, bool updateServerSideIdentification = false)
        {
            var result = new List<ResultViewModel>();
            var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());
            foreach (var id in userIds)
            {
                var deleteUser = _commandFactory.Factory(CommandType.DeleteUserFromTerminal,
                    new List<object> { code, id });
                var deleteresult = deleteUser.Execute();
                //result.Add(new ResultViewModel { Id = id, Validate = (ResultViewModel)boolResult ? 1 : 0, Message = "" });
                result.Add((ResultViewModel)deleteresult ?? new ResultViewModel { Id = id, Validate = 0, Message = "" });

                //task.TaskItems.Add(new TaskItem
                //{
                //    Status = TaskStatuses.Queued,
                //    TaskItemType = TaskItemTypes.DeleteUserFromTerminal,
                //    Priority = TaskPriorities.Medium,
                //    DueDate = DateTime.Today,
                //    DeviceId = device.DeviceId,
                //    Data = JsonConvert.SerializeObject(new { userId = id }),
                //    IsParallelRestricted = true,
                //    IsScheduled = false,
                //    OrderIndex = 1,

                //});
            }

            //_taskService.InsertTask(task).Wait();
            //BioStarServer.ProcessQueue();

            //var result = new ResultViewModel { Validate = 1, Message = "Removing User queued" };

            if (result.Any(x => x.Validate == 0))
            {
                return new ResultViewModel { Id = userId.Count, Validate = 0, Message = "failed" };
            }

            return new ResultViewModel { Id = userId.Count, Validate = 1, Message = "success" };

            //return result;

        }


        [HttpPost]
        [Authorize]
        public Task<List<ResultViewModel>> RetrieveUserFromDevice(uint code, [FromBody] List<int> userIds)
        {

            return Task.Run(() =>
            {
                try
                {
                    var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();

                    var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
                    var deviceId = devices.DeviceId;

                    foreach (var id in userIds)
                    {
                        var getUser = _commandFactory.Factory(CommandType.GetUser,
                new List<object> { code, id });
                        var getUserresult = getUser.Execute();
                    }

                    return new List<ResultViewModel>
                        {new ResultViewModel {Validate = 1, Message = "Retrieving users queued"}};
                }

                catch (Exception exception)
                {
                    return new List<ResultViewModel>
                    {new ResultViewModel { Validate = 0, Message = exception.ToString() }};
                }
            });

        }

    }
}
