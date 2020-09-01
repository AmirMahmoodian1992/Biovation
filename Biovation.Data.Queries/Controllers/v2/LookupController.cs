﻿using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/queries/v2/[controller]")]
    public class LookupController : Controller
    {
        private readonly LookupRepository _lookupRepository;


        public LookupController(LookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }
        [HttpGet]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default)
        {
            throw null;
        }
    }
}