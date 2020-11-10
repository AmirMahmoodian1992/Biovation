using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.ZK.Manager;
using Biovation.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.ZK.Controllers
{
    public class ZKTaskController
    {
        private readonly TaskManager _taskManager;

        public ZKTaskController(TaskManager taskManager)
        {
            _taskManager = taskManager;
        }


        [HttpGet]
        [AllowAnonymous]
        public Task<ResultViewModel> ManualActivationProcessQueue()
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
