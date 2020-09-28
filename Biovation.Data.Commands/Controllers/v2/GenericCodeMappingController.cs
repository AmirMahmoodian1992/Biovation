﻿using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/commands/v2/[controller]")]
    public class GenericCodeMappingController : Controller
    {
        private readonly GenericCodeMappingRepository _genericCodeMappingRepository;

        public GenericCodeMappingController(GenericCodeMappingRepository genericCodeMappingRepository)
        {
            _genericCodeMappingRepository = genericCodeMappingRepository;
        }

    }
}
