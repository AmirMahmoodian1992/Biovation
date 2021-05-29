using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Newtonsoft.Json;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class BlackListController : ControllerBase
    {

        private readonly BlackListService _blackListService;
        private readonly RestClient _restClient;
        private readonly DeviceService _deviceService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskService _taskService;

        public BlackListController(BlackListService blackListService, RestClient restClient, DeviceService deviceService, TaskStatuses taskStatuses, TaskTypes taskTypes, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService)
        {
            _blackListService = blackListService;
            _restClient = restClient;
            _deviceService = deviceService;

            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskService = taskService;
        }

        // TODO - Need more  consideration.
        [HttpPost]
        public Task<List<ResultViewModel>> CreateBlackList([FromBody] List<BlackList> blackLists)
        {
            var token = HttpContext.Items["Token"] as string;
            return Task.Run(() =>
            {
                try
                {
                    var creatorUser = HttpContext.GetUser();
                    var resultsBlackLists = blackLists.Select(blackList => _blackListService.CreateBlackList(blackList, token)).ToList();

                    Task.Run(async () =>
                    {
                        var successResult = new List<BlackList>();
                        foreach (var blackList in resultsBlackLists)
                        {
                            if (blackList.Validate == 1)
                            {
                                successResult.Add(
                                    (_blackListService.GetBlacklist(id: (int) blackList.Id, token: token)).Data.Data
                                    .Find(l => l.Id == blackList.Id));

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
                                    Status = _taskStatuses.Queued,
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
                                    var devices =(await _deviceService.GetDevices(code: blacklist.Device.Code, brandId: DeviceBrands.VirdiCode)).Data?.Data?.FirstOrDefault();
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

                                    await _taskService.InsertTask(task);
                                    await _taskService.ProcessQueue(blackListItem.Device?.Brand).ConfigureAwait(false);

                                }
                                var restRequest =
                                    new RestRequest($"/{brandName}/{brandName}BlackList/SendBlackLisDevice",
                                        Method.POST);
                                restRequest.AddJsonBody(list);
                                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                                {
                                    restRequest.AddHeader("Authorization", HttpContext?.Request?.Headers["Authorization"].FirstOrDefault() ?? string.Empty);
                                }
                                await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

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
        [Route("{id}")]
        public Task<ResultViewModel<PagingResult<BlackList>>> GetBlackList([FromRoute] int id = default, int userid = default, int deviceId = default, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return Task.Run(() => _blackListService.GetBlacklist(id, userid, deviceId, startDate, endDate, isDeleted, token: token));
        }

        // TODO - Need more consideration.
        [HttpPut]
        public Task<ResultViewModel> ChangeBlackList([FromBody] BlackList blackList)
        {
            var creatorUser = HttpContext.GetUser();
            var token = HttpContext.Items["Token"] as string;
            return Task.Run(() =>
            {

                //var restRequest = new RestRequest($"Queries/v2/blackList", Method.PUT);
                //restRequest.AddJsonBody("blackList", blackList.ToString() ?? string.Empty);
                //var result = (_restClient.ExecuteAsync<ResultViewModel>(restRequest)).Result.Data;
                var result = _blackListService.ChangeBlackList(blackList, token: token);

                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlacklist(id: (int)result.Id, token: token).Data.Data;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand?.Name != null)
                        {
                            var task = new TaskInfo
                            {
                                Status = _taskStatuses.Queued,
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
                                var devices = (await _deviceService.GetDevices(code: blacklist.Device.Code, brandId: DeviceBrands.VirdiCode)).Data?.Data?.FirstOrDefault();
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

                                await _taskService.InsertTask(task);
                                await _taskService.ProcessQueue(brand).ConfigureAwait(false);

                            }
                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);
                            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                            {
                                restRequest.AddHeader("Authorization", HttpContext?.Request?.Headers["Authorization"].FirstOrDefault() ?? String.Empty);
                            }
                            await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

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

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteBlackList([FromRoute] int id)
        {
            var creatorUser = HttpContext.GetUser();
            var token = HttpContext.Items["Token"] as string;
            return Task.Run(() =>
            {

                //var restRequest = new RestRequest($"Queries/v2/blackList", Method.DELETE);
                //restRequest.AddQueryParameter("id", id.ToString());
                //var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                //var result = (requestResult.Result.Data);
                var result = _blackListService.DeleteBlackList(id, token: token);


                Task.Run(async () =>
                {
                    try
                    {
                        var successBlackList = new List<BlackList>();
                        if (result.Validate == 1)
                        {
                            successBlackList = _blackListService.GetBlacklist(id: (int)result.Id, isDeleted: true, token: token).Data.Data;
                        }

                        var brand = successBlackList?.FirstOrDefault()?.Device.Brand;

                        if (brand != null)
                        {
                            var task = new TaskInfo
                            {
                                Status = _taskStatuses.Queued,
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
                                var devices = (await _deviceService.GetDevices(code: blacklist.Device.Code, brandId: DeviceBrands.VirdiCode)).Data?.Data?.FirstOrDefault();
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

                                await _taskService.InsertTask(task);
                                await _taskService.ProcessQueue(brand).ConfigureAwait(false);

                            }

                            var restRequest = new RestRequest($"/{brand.Name}/{brand.Name}BlackList/SendBlackLisDevice",
                                Method.POST);
                            restRequest.AddJsonBody(successBlackList);
                            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                            {
                                restRequest.AddHeader("Authorization", HttpContext?.Request?.Headers["Authorization"].FirstOrDefault() ?? String.Empty);
                            }
                            await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

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
