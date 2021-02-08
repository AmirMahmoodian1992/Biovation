using System;
using System.Threading.Tasks;
using Biovation.Brands.EOS.Manager;
using Biovation.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.EOS.Controllers
{
    [ApiController]
    [Route("biovation/api/[controller]")]
    public class EosTaskController : ControllerBase
    {
        private readonly TaskManager _taskManager;

        public EosTaskController(TaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[Action]")]
        public Task<ResultViewModel> RunProcessQueue(int deviceId = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    _taskManager.ProcessQueue(deviceId);
                    return new ResultViewModel { Success = true };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Success = false, Message = exception.ToString() };
                }
            });
        }
    }
}
