using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Newtonsoft.Json;

namespace Biovation.Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class BlackListController : ControllerBase
    {
        private readonly RestClient _restClient;
        private readonly BlackListService _blackListService;
        private readonly string _kasraAdminToken;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        private readonly DeviceService _deviceService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskService _taskService;

        public BlackListController(RestClient restClient, BlackListService blackListService, BiovationConfigurationManager biovationConfigurationManager, DeviceService deviceService, TaskStatuses taskStatuses, TaskTypes taskTypes, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService)
        {
            _restClient = restClient;
            _blackListService = blackListService;
            _biovationConfigurationManager = biovationConfigurationManager;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;
            _deviceService = deviceService;

            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskService = taskService;
        }

        [HttpPost]
        [Route("CreateBlackList")]
        public Task<List<ResultViewModel>> CreateBlackList([FromBody] List<BlackList> blackLists)
        {

            return Task.Run(() =>
            {
                try
                {
                    var creatorUser = HttpContext.GetUser();
                    var resultsBlackLists = _blackListService.CreateBlackList(blackLists, token: _kasraAdminToken);

                    Task.Run(async () =>
                    {
                        var successResult = new List<BlackList>();
                        foreach (var blackList in resultsBlackLists)
                        {
                            if (blackList.Validate == 1)
                            {
                                successResult.Add(_blackListService.GetBlacklist((int)blackList.Id, token: _kasraAdminToken).Result
                                    .FirstOrDefault());
                            }
                        }

                        try
                        {

                            var groupByList = successResult.GroupBy(x => new { x.Device.Brand.Name }).ToList();

                            foreach (var list in groupByList)
                            {
                                var blackListItem = list.FirstOrDefault();
                                var brandName = blackListItem?.Device?.Brand?.Name;
                                if (brandName == null) continue;

                                var task = new TaskInfo
                                {
                                    CreatedAt = DateTimeOffset.Now,
                                    CreatedBy = creatorUser,
                                    TaskType = _taskTypes.SendBlackList,
                                    Priority = _taskPriorities.Medium,
                                    DeviceBrand = blackListItem.Device?.Brand,
                                    TaskItems = new List<TaskItem>(),
                                    DueDate = DateTime.Today
                                };

                                foreach (var blacklist in blackLists)
                                {
                                    var devices = _deviceService.GetDevices(code: blacklist.Device.Code, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
                                    if (devices is null)
                                        continue;

                                    var deviceId = devices.DeviceId;
                                    task.TaskItems.Add(new TaskItem
                                    {
                                        Status = _taskStatuses.Queued,
                                        TaskItemType = _taskItemTypes.SendBlackList,
                                        Priority = _taskPriorities.Medium,
                                        DeviceId = deviceId,
                                        Data = JsonConvert.SerializeObject(new { BlackListId = blacklist.Id, UserId = blacklist.User.Code }),
                                        IsParallelRestricted = true,
                                        IsScheduled = false,
                                        OrderIndex = 1
                                    });

                                    _taskService.InsertTask(task);
                                    await _taskService.ProcessQueue(blackListItem.Device?.Brand).ConfigureAwait(false);

                                }

                                var restRequest =
                                    new RestRequest($"/{brandName}/{brandName}BlackList/SendBlackLisDevice",
                                        Method.POST);
                                restRequest.AddJsonBody(list);
                                restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                                var restResult = await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

                                //result.Add(restResult.Data);
                            }

                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    });


                    return resultsBlackLists;

                }
                catch (Exception)
                {
                    return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = "error" } };
                }
            });

        }



        [HttpGet]
        [Route("GetBlackList")]
        public Task<List<BlackList>> GetBlackList(int id = default, int userid = default, int deviceId = default, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default)
        {
            return Task.Run(() => _blackListService.GetBlacklist(id, userid, deviceId, startDate, endDate, isDeleted, token: _kasraAdminToken));
        }

        [HttpPost]
        [Route("ChangeBlackList")]
        public Task<ResultViewModel> ChangeBlackList([FromBody] BlackList blackList)
        {
            return Task.Run(() =>
            {
                var creatorUser = HttpContext.GetUser();
                var result = _blackListService.ChangeBlackList(blackList);

                Task.Run(async () =>
            {
                try
                {
                    var successBlackList = new List<BlackList>();
                    if (result.Validate == 1)
                    {
                        successBlackList = _blackListService.GetBlacklist((int)result.Id, token: _kasraAdminToken).Result;
                    }

                    var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                    if (brand?.Name != null)
                    {
                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.SendBlackList,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = brand,
                            TaskItems = new List<TaskItem>(),
                            DueDate = DateTime.Today
                        };

                        foreach (var blacklist in successBlackList)
                        {
                            var devices = _deviceService.GetDevices(code: blacklist.Device.Code, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
                            if (devices is null)
                                continue;

                            var deviceId = devices.DeviceId;
                            task.TaskItems.Add(new TaskItem
                            {
                                Status = _taskStatuses.Queued,
                                TaskItemType = _taskItemTypes.SendBlackList,
                                Priority = _taskPriorities.Medium,
                                DeviceId = deviceId,
                                Data = JsonConvert.SerializeObject(new { BlackListId = blacklist.Id, UserId = blacklist.User.Code }),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1
                            });

                            _taskService.InsertTask(task);
                            await _taskService.ProcessQueue(brand).ConfigureAwait(false);

                        }
                        var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                            Method.POST);
                        restRequest.AddJsonBody(successBlackList);
                        restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                        var restResult = await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            });

                return result;

            });
        }
        [HttpPost]
        [Route("DeleteBlackList")]
        public Task<ResultViewModel> DeleteBlackList(int id)
        {
            return Task.Run(() =>
            {
                var creatorUser = HttpContext.GetUser();
                var result = _blackListService.DeleteBlackList(id, token: _kasraAdminToken);

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlacklist((int)result.Id, isDeleted: true, token: _kasraAdminToken).Result;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand != null)
                        {
                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = _taskTypes.SendBlackList,
                                Priority = _taskPriorities.Medium,
                                DeviceBrand = brand,
                                TaskItems = new List<TaskItem>(),
                                DueDate = DateTime.Today
                            };

                            foreach (var blacklist in successBlackList)
                            {
                                var devices = _deviceService.GetDevices(code: blacklist.Device.Code, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
                                if (devices is null)
                                    continue;

                                var deviceId = devices.DeviceId;
                                task.TaskItems.Add(new TaskItem
                                {
                                    Status = _taskStatuses.Queued,
                                    TaskItemType = _taskItemTypes.SendBlackList,
                                    Priority = _taskPriorities.Medium,
                                    DeviceId = deviceId,
                                    Data = JsonConvert.SerializeObject(new { BlackListId = blacklist.Id, UserId = blacklist.User.Code }),
                                    IsParallelRestricted = true,
                                    IsScheduled = false,
                                    OrderIndex = 1
                                });

                                _taskService.InsertTask(task);
                                await _taskService.ProcessQueue(brand).ConfigureAwait(false);

                            }
                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);
                            restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                            var restResult = await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                });

                return result;

            });
        }


    }
}
