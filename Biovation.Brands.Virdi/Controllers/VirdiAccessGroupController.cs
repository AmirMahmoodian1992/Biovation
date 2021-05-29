using Biovation.CommonClasses;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
//using DeviceBrands = Biovation.Constants.DeviceBrands;

namespace Biovation.Brands.Virdi.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiAccessGroupController : ControllerBase
    {
        private readonly VirdiServer _virdiServer;

        public VirdiAccessGroupController(VirdiServer virdiServer)
        {
            _virdiServer = virdiServer;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> SendAccessGroupToAllDevices([FromBody] int accessGroupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return new ResultViewModel {Validate = 1, Message = "Sending users queued"};
                }
                catch (Exception exception)
                {
                    return new ResultViewModel {Validate = 0, Message = exception.ToString()};
                }
            });
        }

        /*   public ResultViewModel SendAccessGroupToAllDevices([FromBody]int accessGroupId)
           {
               var devices = _deviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.VirdiCode);
   
               foreach (var device in devices)
               {
                   var sendAccessGroupCommand = CommandFactory.Factory(CommandType.SendAccessGroupToDevice,
                   new List<object> { device.DeviceId, accessGroupId });
   
                   sendAccessGroupCommand.Execute();
               }
   
               return new ResultViewModel { Validate = 0 };
           }*/

        [HttpGet]
        [Authorize]
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            return new ResultViewModel { Validate = 1 , Message = code.ToString() };
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyAccessGroup(string accessGroup)
        {
            try
            {
                _virdiServer.LoadFingerTemplates().ConfigureAwait(false);
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }
    }
}
