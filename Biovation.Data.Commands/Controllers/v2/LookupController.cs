﻿using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    public class LookupController : Controller
    {
        private readonly LookupRepository _lookupRepository;


        public LookupController(LookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

    }
}