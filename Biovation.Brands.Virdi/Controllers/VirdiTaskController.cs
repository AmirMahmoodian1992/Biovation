﻿using System;
using System.Threading.Tasks;
using Biovation.Brands.Virdi.Manager;
using Biovation.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.Virdi.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[Controller]")]
    public class VirdiTaskController : ControllerBase
    {
        private readonly TaskManager _taskManager;

        public VirdiTaskController(TaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[Action]")]
        public async Task<ResultViewModel> RunProcessQueue(int deviceId = default)
        {
            try
            {
                await _taskManager.ProcessQueue(deviceId).ConfigureAwait(false);
                return new ResultViewModel { Success = true };
            }
            catch (Exception exception)
            {
                return new ResultViewModel { Success = false, Message = exception.ToString() };
            }

        }
    }
}
