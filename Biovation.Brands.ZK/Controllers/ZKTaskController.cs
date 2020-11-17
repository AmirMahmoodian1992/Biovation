using System;
using System.Threading.Tasks;
using Biovation.Brands.ZK.Manager;
using Biovation.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[Controller]")]
    public class ZkTaskController : ControllerBase
    {
        private readonly TaskManager _taskManager;

        public ZkTaskController(TaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[Action]")]
        public Task<ResultViewModel> RunProcessQueue()
        {
            return Task.Run(() =>
            {
                try
                {
                    _taskManager.ProcessQueue();
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
