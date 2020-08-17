using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Gateway.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class LookupController : Controller
    {

        private readonly LookupService _lookupService;

        public LookupController(LookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        public Task<IActionResult> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default)
        {
            throw null;
        }
    }
}