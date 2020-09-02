using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;


namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/commands/v2/[controller]")]
    public class LogController : Controller
    {

        private readonly LogRepository _logRepository;


        public LogController(LogRepository logRepository)
        {
            _logRepository = logRepository;
        }

    }
}