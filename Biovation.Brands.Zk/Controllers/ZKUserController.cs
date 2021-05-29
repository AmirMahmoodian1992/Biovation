using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class ZkUserController : ControllerBase
    {
        private readonly DeviceService _deviceService;

        public ZkUserController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        [Authorize]
        public async Task<List<ResultViewModel>> SendUserToDevice(uint code, string userId)
        {
            return await Task.Run(() =>
            {
                var listResult = new List<ResultViewModel>();
                try
                {
                    try
                    {
                        //var userIds = JsonConvert.DeserializeObject<long[]>(userId);
                        var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                        if (devices != null)
                        {
                            listResult.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log($" --> SendUserToDevice Code: {code}  {e}");
                        listResult.Add(new ResultViewModel { Message = e.ToString(), Validate = 0 });

                    }
                    return listResult;
                }
                catch (Exception)
                {
                    listResult.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                    return listResult;
                }
            });

        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendUserToAllDevices([FromBody] User user)
        {
            return await Task.Run(() =>
                {
                    return new ResultViewModel { Id = user.Id, Validate = 1 };
                });
        }

        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> DeleteUserFromAllTerminal(int[] userCodes)
        {
            return await Task.Run(() =>
            {
                return userCodes.Select(code=> new ResultViewModel { Id = code, Validate = 1 }).ToList() ;
            });
        }
    }
}