using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
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