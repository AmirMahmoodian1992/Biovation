using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Gateway.Controllers.v1
{
    [Route("biovation/api/[controller]")]
    public class LookupController : Controller
    {

        private readonly LookupService _lookupService;

        public LookupController(LookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        [Route("GetLookups")]
        public Task<List<Lookup>> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default)
        {
            return Task.Run(() => _lookupService.GetLookups(code, name, lookupCategoryId, codePrefix));
        }
    }
}