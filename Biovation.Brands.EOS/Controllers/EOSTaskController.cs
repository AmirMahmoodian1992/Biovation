﻿using Biovation.Brands.EOS.Manager;
using Biovation.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
